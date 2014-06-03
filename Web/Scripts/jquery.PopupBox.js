/*==================================================================================================
*	$(...).PopupBox:			弹出框 
*/
(function ($) {

    /*
	*	内部对象关联机制：
	*	
	*	弹出的层与 this 通过id关联： pbdiv_【this ID】,如：
	*	this id: testDiv1， 
	*	则弹出层的ID为: pbdiv_testDiv1
	*/

    //缺省参数
    $.PopupBox = {
        defaults: { width: 200, height: 300, left: 100, top: 100, onBeforeShow: null, buddy: false, targetID: null, autoHide: true, zIndex: 999999 }
    };

    //构造函数
    $.fn.PopupBox = function (opts) {
        var T = this;
        var divID = "pbdiv_" + T[0].id;
        var options = $.extend({}, $.PopupBox.defaults, opts);
        if (T.data('options') != null) {
            options = T.data('options');
        }
        else {
            T.data('options', options);
            if (options.targetID != null) {
                var tt = $(options.targetID);
                if (tt.css('width') != "auto") { options.width = parseInt(tt.css('width')) + 2; }
                if (tt.css('height') != "auto") { options.height = parseInt(tt.css('height')) + 2; }
            }
            else if (options.buddy == false) {
                if (T.css('width') != "auto") { options.width = parseInt(T.css('width')); }
                if (T.css('height') != "auto") { options.height = parseInt(T.css('height')); }
            }
        }
        if (!T.hasClass('pb_popup_class')) {
            T.addClass('pb_popup_class');
            if ($("#" + divID).length == 0) {
                var div = $("<div class='bgBoxStyle' id='" + divID + "' style='z-index:" + options.zIndex + ";display:none;background-color:#d0d0d0;position:absolute;'></div>");

                $(document.body).append(div);
                if (options.buddy == false) {
                    this.show();
                    div.append(this);
                }
                else if (options.autoHide == true) {
                    $(document).click(function (event) {
                        var target = event.srcElement || event.target;
                        var popRootDiv = $(target).parents('#' + divID);

                        if (popRootDiv.length == 0 && target.id != T[0].id) {
                            if (div.css('display') != 'none') { T.pbHide(); }
                        }
                    });
                }
                div.css("left", options.left + 'px')
				   .css("top", options.top + 'px')
				   .css("height", options.height + 'px')
				   .css('width', options.width + 'px');
            }
        }
        $('#' + divID).width(options.width).height(options.height);
        if (options.targetID != null) {
            var tt = $(options.targetID);
            $('#' + divID).append(tt);
            tt.show();
        }
        return T;
    };

    //取得Box(JQuery)对象
    $.fn.pbGetPopDiv = function () {
        return $("#pbdiv_" + this[0].id);
    };

    //显示Box
    $.fn.pbShow = function () {
        var options = this.data('options');

        if (options.onBeforeShow != null) { options.onBeforeShow(); }
        var div = $("#pbdiv_" + this[0].id);


        var divh = div.height();
        var divw = div.width();
        var Ttop = this.offset().top;
        var Th = this.outerHeight();
        var Twidth = this.offset().left;
        var Tw = this.outerWidth();
        var h = document.documentElement.clientHeight;
        var w = document.documentElement.clientWidth;
        if (options.onCenter) {
            div.css("top", ((h - divh) / 2) + "px");
            div.css("left", ((w - divw) / 2) + 'px');
        }
        else if (options.buddy) {
            div.css("left", this.offset().left + 'px');
            //如果下边可以完整显示 或则 上边不能完整显示，则默认显示下边
            if (((h - divh - Ttop - Th) >= 0) || (Ttop < divh)) { div.css("top", (Ttop + Th) + 'px'); }
            else { div.css("top", (Ttop - divh) + 'px'); }

            if (((w - divw - Twidth) >= 0) || (Twidth < divw)) { div.css("left", (Twidth) + 'px'); }
            else { div.css("left", (Twidth + Tw - divw) + 'px'); }
        }

        div.show();
        return this;
    };

    //隐藏Box
    $.fn.pbHide = function () {
        $("#pbdiv_" + this[0].id).hide();
        return this;
    };

    $.fn.BuddyBox = function (targetID, opts) {
        var options = $.extend({}, $.PopupBox.defaults, opts, { buddy: true, targetID: targetID });
        this.PopupBox(options);
    };



    /*==================================================================================================
	*	$(...).TreePopup:			基于PopupBox的封装, 弹出框的内容是一个自定义数据源的 treeTable
	*/
    //缺省参数
    $.TreePopup = {
        defaults: {
            width: 280,
            height: 320,
            OnSelected: null, // 函数原型：OnSelected( model info , sender)
            buddy: true,
            ajaxPara: null,
            autoHide: true
        }
    };

    //构造函数
    //{opts: featureName , dataModel , treeColumn ,txtField, rootParentKey, url,ajaxLoad}
    $.fn.TreePopup = function (opts) {
        var options = $.extend({}, $.TreePopup.defaults, opts);
        var infoDivId = "__TREE_POPUP_" + options.featureName + "_DIV__";
        var featureClass = '__PB_TREE_POPUP_' + options.featureName + '_CLASS__';
        var dataName = "INFO_" + options.featureName;
        var treeTableID = "TABLE_" + infoDivId;

        //初始化第一次弹出
        if (!this.hasClass(featureClass)) {
            var T = this;
            //设置data中的featureName
            T.data("FEATURE_NAME", options.featureName);

            //弹出PopupBox，设置PopupBox样式，并将treeTable放置到PopupBox中
            var beforeShow = function (sender, options) {
                return function () {
                    $("#" + treeTableID).data("ExtData", { "PB_ContactElementID": sender[0].id });
                    sender.pbGetPopDiv().append($("#" + infoDivId));
                    if (sender.data(dataName) != null) { $("#" + treeTableID).ttSetSelectItem(T.data(dataName).ID); }

                    $("#" + treeTableID).data('options').onSelected = function (node, extData) {
                        if (options.OnSelected != null) {
                            var sender = $("#" + extData.PB_ContactElementID);
                            var info = {};
                            for (var v in options.dataModel) {
                                info[v] = node.attr("CPR_" + options.featureName + "_" + v);
                            }
                            //用户自定义格式化数据
                            if (options.dataFormat != null) {
                                info = options.dataFormat(info, node);
                            }
                            options.OnSelected(info, sender);
                            sender.pbHide();
                        }
                    };
                };
            };
            options.onBeforeShow = new beforeShow(T, options);
            T.PopupBox(options);
            T.pbGetPopDiv().css("backgroundColor", "#FFFFFF").css("border", "1px solid #CCCCCC");
            var infoDiv = $("#" + infoDivId);
            if (infoDiv.length == 0) {
                infoDiv = $("<div id=" + infoDivId + " class='region_popup_class' style='position:absolute;left:0px;top:0px;right:0px;bottom:0px;overflow:auto'><table style='width:100%' id='" + treeTableID + "'></table></div>");
                T.pbGetPopDiv().append(infoDiv);
            }
        }

        //绑定 Tree 数据
        if ((!this.hasClass(featureClass)) || options.ReBindTree == true) {
            var rt = "";
            for (var v in options.dataModel) { rt += $.format(" {0}='{{1}}' ", "CPR_" + options.featureName + "_" + v, v); }
            rt = $.format("<tr {0}><td>{{1}}<td></tr>", rt, options.txtField);
            $("#" + treeTableID).treeTable({
                treeColumn: options.treeColumn,
                key: "ID",
                parentKey: "ParentID",
                onSelected: function (node) { },
                bind: {
                    rowTemplate: rt,
                    rootParentKey: options.rootParentKey,
                    url: options.url,
                    ajaxLoad: options.ajaxLoad,
                    ajaxPara: options.ajaxPara
                }
            });
        }

        //设置初始化完成标志
        this.addClass(featureClass);
        return T;
    };

    $.fn.TreeInfo = function (ftName) {
        return this.data("INFO_" + ((ftName == null) ? this.data("FEATURE_NAME") : ftName));
    };

})(jQuery);