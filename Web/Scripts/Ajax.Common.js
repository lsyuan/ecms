$(function () {
    ResetAlign(); 
    $(":text,[autocomplete]").focus(function () {
        this.select();
    }); 
});
/************************************************************************/
/* 设置class=layoutTable td的align属性                                                                     */
/************************************************************************/
function ResetAlign() {
    $('td :eq(1)', $('.layoutTable')).css('text-align', 'right');
    alert();
}

/************************************************************************/
/*显示网页消息                                                            */
/************************************************************************/
function ShowMessage(title, msg) {
    if (!$("#msgDIV")) {
        var newMsg = document.createElement("div");

        newMsg.setAttribute("z-index", "9999");
        newMsg.innerHTML = "<div id = \"msgDIV\" title=页面消息>" + msg + "</div>";
        document.body.appendChild(newMsg);
        $("#msgDIV").css({ zIndex: 99999 });
    }
    $("#msgDIV").dialog({
        modal: true,
        buttons: {
            Ok: function () {
                $(this).dialog("close");
                $("#msgDIV").remove();
            }
        }
    });
}
function HideMsg() {
    $("#effect").remove();
}

function hideMenu() {
    $("#LayOutSide").toggleClass('LayOutSide1');
    $("#LayOutMain").toggleClass('LayOutMain1');
    if ($("#LayOutSide").prop('class') == 'LayOutSide') {
        $("#menuDIV").show();
    }
    else {
        $("#menuDIV").hide();
    }
}
/*
*	禁用网页右键
*/
function disableRightClick(e) {
    if (!document.rightClickDisabled) {
        if (document.layers) {
            document.captureEvents(Event.MOUSEDOWN);
            document.onmousedown = disableRightClick;
        }
        else document.oncontextmenu = disableRightClick;
        return document.rightClickDisabled = true;
    }
    if (document.layers || (document.getElementById && !document.all)) {
        if (e.which == 2 || e.which == 3) {
            return false;
        }
    }
    else {
        return false;
    }
}

/*
*	文本框边框样式
*/
(function ($) {
    $.addTextBox = function (t, p) {
        p = $.extend({
            width: 174
        }, p);
        var span = $.trim($(t).attr("label"));
        if (span != "") {
            $(t).before("<div class='span'>" + span + "</div>");
        }
        $(t).wrap("<div class='aq_box'><div class='aq_box_wrap'></div></div>")
            .parents(".aq_box")
            .siblings().removeClass("hover").end()
            .hover(function () {
                $(this).toggleClass("hover");
            }, function () {
                $(this).toggleClass("hover");
            })
            .find(".textbox").width(p.width);
    };
    var docloaded = false;
    $(document).ready(function () {
        docloaded = true
    });
    $.fn.textbox = function (p) {
        return this.each(function () {
            if (!docloaded) {
                var t = this;
                $(document).ready(function () {
                    $.addTextBox(t, p);
                });
            } else {
                $.addTextBox(this, p);
            }
        });
    };
})(jQuery);

function GetJsonValue(data) {
    var result = [];
    $(data).each(function (i, n) {
        result.push(n);
    });
    GlobalData("Area", [{ "id": 123, "name": 城南区 }]);
    var a = GlobalData("Area");
    return result;
}

/* 设置下拉列表的值
*	
*/
function SetOptionValue(obj, data, Isclear) {
    if (Isclear != undefined || Isclear == true) {
        for (i = $(obj).length; i >= 0; i--) {
            $(obj).children().remove();
        }
    }
    for (i = 0; i < data.length; i++) {
        var a = "<option value=" + data[i].ID + ">" + data[i].NAME + "</option>";
        $(obj).append(a);
    }
}

/*只输入数字
*	
*/
function myKeyDown() {
    var k = window.event.keyCode;
    if (k == 47) {
        window.event.returnValue = false;
    }
    if ((k == 46) || (k == 8) || (k == 189) || (k == 109) || (k == 190) || (k == 110) || (k >= 48 && k <= 57) || (k >= 96 && k <= 105) || (k >= 37 && k <= 40))
    { }
    else if (k == 13) {
        window.event.keyCode = 9;
    }
    else {
        window.event.returnValue = false;
    }
}

/*客户选择框自动完成  
*/
function CustomerAutoFill(id) {

    $(id).autocomplete("/Customer/QueryCustomer", {
        minChars: 0,
        width: 310,
        matchContains: true,
        autoFill: false,
        formatItem: function (row, i, max) {
            return row.NAME;
        },
        formatMatch: function (row, i, max) {
            return row.NAME;
        },
        formatResult: function (row) {
            return row.NAME;
        }
    });

}
function InitArea(id, spanID, holderID) {
    $("#" + spanID).bind('click', function () {
        $("#" + spanID).BuddyBox("#" + holderID);
        $("#" + spanID).pbShow();
    });
    $.ajax({
        url: "/System/GetAreaTree",
        data: null,
        success: function (data) {
            var array = new Array();
            $(data).each(function (i, x) {
                var temp = new Object();
                temp.id = this.ID == "" ? null : this.ID;
                temp.pid = this.PID;
                temp.text = this.TEXT;
                temp.expanded = this.EXPANDED;
                array.push(temp);
            });

            $("#" + id).omTree({
                dataSource: array,
                simpleDataModel: true,
                onSelect: function (nodeData, event) {
                    $("#" + spanID).attr("AREAID", nodeData.id);
                    $("[dn='AREAID']").val(nodeData.id);
                    $("#" + spanID).text(nodeData.text);
                    $("#" + spanID).val(nodeData.text);
                    $("#" + spanID).pbHide();
                }
            });
        },
        error: function () {
        }
    });
}


function AjaxCall(url, data, success, error, cache) {
    // 默认不缓存
    if (cache == undefined || cache == null) {
        cache = false;
    }
    $.ajax({
        cache: cache,
        url: url,
        data: data,
        success: success,
        error: error
    });
}
//#endregion
