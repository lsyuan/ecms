/*公共脚本*/
$(function () {
    $(".combo-text").attr("readonly", "readonly"); //统一禁用所有combox的编辑功能，也可以单独使用editable:false
    ResetAlign();
    //统一处理：日期控件文本框点击显示日历
    $(".combo-text").click(function () {
        $(this).next().find(".combo-arrow").trigger("click");
    });
    //统一处理下拉列表select控件自动绑定
    OptionBindAuto();
    //渲染圆角Div
    InitCornerDiv();
    //内容页高度自适应
    PageAutoHight();
    $.ajaxSetup({
        cache: false
    });
});

//初始化地区选择弹出框
function InitAreaSelect() {
    $("#dArea").dialog({
        title: '选择所属地区：',
        width: 400,
        height: 250,
        closed: true,
        cache: false,
        modal: true
    });
}

//构建弹出窗
function InitDialog(ID, title, width, height, tools, buttons, fun, extension) {
    var option = {
        title: title,
        width: width,
        height: height,
        closed: true,
        cache: false,
        modal: true,
        collapsible: false,
        buttons: buttons,
        tools: tools,
        onClose: fun
    };
    option = $.extend(option, extension, true);
    $("#" + ID.replace("#", "")).dialog(option);
}

/********************表单必填验证 ****************************/
//输入非空检查（formID：表单的ID），在需要验证的input上添加vRequired=‘t’
function InputCheck(formID) {
    var flag = true;
    formID = formID.replace("#", "");
    var requireObjs = $("[vRequired]", $("#" + formID));
    for (var i = 0; i < requireObjs.length; i++) {
        var $inputObj = null;
        if ($(requireObjs[i]).is(":hidden")) {
            $inputObj = $(requireObjs[i]).next().find("input:first");
        }
        else {
            $inputObj = $(requireObjs[i]);
        }
        if ($inputObj.val() == "" && $inputObj != undefined) {
            $inputObj.css("border", "1px solid red"); //红色高亮提示
            flag = false;
        }
        else {
            $inputObj.css("border", "1px solid #5A81C4"); //填写后样式还原
        }
    }
    return flag;
}
/********************数据列表 ****************************/
//初始化数据列表
function InitGridTable(ID, title, url, toolbar, queryParam, singleSelect, pagesize, extension) {
    var option = {
        title: title,
        pageSize: pagesize ? pagesize : 15,
        striped: true,
        pagination: true, 
        singleSelect: singleSelect == null ? true : singleSelect,
        fitColumns: true,
        url: url,
        toolbar: toolbar,
        queryParams: queryParam,
        onLoadSuccess: function () {
            if (pagesize > 25) {
                $('#' + ID.replace("#", "")).css("height", "450");
            }
        }
    };
    option = $.extend(option, extension, true);
    $('#' + ID.replace("#", "")).datagrid(option);
}
/******************* 设置class=layoutTable td的align属性  ***************/
function ResetAlign() {
    $('label', $('.layoutTable')).filter(".name").css('float', 'right');
}
function GlobalData(key, value) {
    var topWin = window.top;
    if (topWin != window) { return topWin.GlobalData(key, value); }
    return $("#" + gid).data(key, value);
}

/************************下拉类表自动绑定***********************************/
// 设置下拉列表的值
function SetOptionValue(obj, data, Isclear) {
    obj = "#" + obj.replace("#", "");
    if (Isclear != undefined || Isclear == true) {
        $(obj).children().remove();
    }
    var strHtml = "";
    for (var i = 0; i < data.length; i++) {
        if (data[i].ID != undefined) {
            strHtml += "<option value=" + data[i].ID + ">" + data[i].NAME + "</option>";
        }
        else {
            strHtml += "<option value=" + data[i].id + ">" + data[i].text + "</option>";
        }
    }
    $(obj).append(strHtml);
}
//select下拉自动绑定（例：加属性JsonUrl=‘/System/XXXX’）
function OptionBindAuto() {
    $("[JsonUrl]").each(function () {
        var strUrl = $(this).attr("JsonUrl");
        var ctrlID = $(this).attr("ID");
        $.ajax({
            url: strUrl,
            data: null,
            cache:false,
            username: ctrlID, //用于记录控件ID
            success: function (data) {
                SetOptionValue(this.username, data);
                $("#" + this.username).change();//加载完成自动触发选中事件
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $.messager.show("系统提示", "数据加载失败！");
            }
        });
    });
}
/************************Json数据操作***********************************/
//字符串转换为json对象
function strToJson(str) {
    var json = eval('(' + str + ')');
    return json;
}
//json数据的合并
function extend(des, src, override) {
    if (src instanceof Array) {
        for (var i = 0, len = src.length; i < len; i++)
            extend(des, src[i], override);
    }
    for (var i in src) {
        if (override || !(i in des)) {
            des[i] = src[i];
        }
    }
    return des;
}
//格式化时间字符串
function FormatTimeString(timeString) {
    if (timeString == "" || timeString == undefined) {
        return "";
    }
    return timeString.replace(/ \d{2}:\d{2}:\d{2}/g, '');
}

/********************文本框自动完成(服务器端格式：{id:'',text:''})**********************/
//ctrlID:文本框控件ID,url：请求地址, func：选中后处理函数
function InputAutoComplete(ctrlID, strUrl, func) {
    ctrlID = ctrlID.replace("#", "");
    $("#" + ctrlID).combobox({
        hasDownArrow: false,
        valueField: 'id',
        width: 160,
        mode: 'remote', //远程动态获取
        url: strUrl,
        textField: 'text',
        delay: 200,
        onHidePanel: function () {
            if (func != null)
                func();
        }
    });
    //是否正确输入的验证
    $("#" + ctrlID).combobox("textbox").blur(function () {
        var inputText = $("#" + ctrlID).combobox("textbox").val();
        var dataSource = $("#" + ctrlID).combobox("getData");
        var flag = false;
        $.each(dataSource, function (i) {
            if (dataSource[i].text == inputText) {
                flag = true;
                return;
            }
        });
        if (!flag) {
        	$("#" + ctrlID).combobox("setValue", "");
        }
    });
}
/********************圆角div(设置class=cornerPanel)**********************/
function InitCornerDiv() {
    if ($(".cornerPanel").length == 0) return;
    settings = {
        tl: { radius: 5 },
        tr: { radius: 5 },
        bl: { radius: 5 },
        br: { radius: 5 },
        antiAlias: true,
        autoPad: false
    }
    var myBoxObject = new curvyCorners(settings, "cornerPanel");
    myBoxObject.applyCornersToAll();
}
/********************内容页高度自适应**********************/
function PageAutoHight() {
    //页面高度自适应
    $contentDiv = $("#contentDiv");
    if ($contentDiv.length != 0) {
        var h = $(document).height() - 64;
        $contentDiv.height(h);
    }
}
function trim(txt) {
    var tmp = "";
    var item_length = txt.length;
    var item_length_minus_1 = txt.length - 1;
    for (index = 0; index < item_length; index++) {
        if (txt.charAt(index) != ' ') {
            tmp += txt.charAt(index);
        } else {
            if (tmp.length > 0) {
                if (txt.charAt(index + 1) != ' ' && index != item_length_minus_1) {
                    tmp += txt.charAt(index);
                }
            }
        }
    }
    return tmp;
}