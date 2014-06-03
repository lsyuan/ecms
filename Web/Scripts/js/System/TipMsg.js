$(function () {
    //大小自适应
    AutoHeight();
    $(document).resize(function () {
        AutoHeight();
    });
    //公告详细内容弹出框
    $("#dgMsgDetail").dialog({
        title: '公告详细：',
        width: 600,
        height: 400,
        resizable: true,
        closed: true,
        cache: false,
        modal: true
    });
    LoadMsg(); //公告
    LoadContract(); //协议到期提醒
    $('.window-mask').width('98%');
});

//高度自适应
function AutoHeight() {
    var height = $(document).height() / 2 - 23; 
    $(".cornerPanel").height(height);
    $(".cornerPanel .content").height(height - 25);
}

//加载公告信息
function LoadMsg() {
    $.ajax({
        url: "/System/LoadSystemMsg",
        data: {},
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data != undefined) {
                var strHtml = "";
                $.each(data, function (i) {
                    var title = data[i].TITLE;
                    var author = data[i].NAME;
                    var id = data[i].ID;
                    var createDate = FormatTimeString(data[i].CREATEDATE);
                    strHtml += "<li><a id='" + id + "' tit='" + title + "' IsMsg='true' href='#'>" + title + "</a><span  style='position:absolute;right:10px;'>" + createDate + "</span></li></ul>";
                });
                $("#MsgList").html(strHtml);
                $($("[IsMsg='true']")).each(function (i, x) {
                    $(this).bind('click', function () {
                        ShowMsgContent($(this).attr('id'), $(this).attr('tit'));
                    });
                });
            }
        },
        error: function () {
            $.messager.alert("error：", "数据加载失败！");
            return null;
        }
    });
}
//加载公告信息
function LoadContract() {
    $.ajax({
        url: "/System/LoadTipContracts",
        data: {},
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data != undefined) {
                var strHtml = "";
                $.each(data, function (i) {
                    strHtml += "<li>";
                    strHtml += "<a href='/Business/ContrastMgr'>" + data[i].NAME + "</a>";
                    strHtml += "<span style='position: absolute; right: 10px;'>到期时间：" + FormatTimeString(data[i].ENDDATE) + "</span>";
                    strHtml += "</li>";
                }); 
                $("#ContractList").html(strHtml);
            }
        }
    });
}

//显示公告内容
function ShowMsgContent(guid, title) { 
    $.post("/Normal/GetMsgDetail", { msgID: guid }, function (data) {
        $("#msgTitle").html(title);
        $("#msgContent").html(data);
        $("#dgMsgDetail").dialog('open');
    });
} 
function HidePanel() {
    $("#dgMsgDetail").dialog('close');
}