/*message view*/
$(function () {
    InitControl();
});
function InitControl() {
    $("#btnSearch").click(function () {
        $('#dgMsgs').datagrid("load", {
            Status: $("#dpStatus").val()
        });
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
    //数据表格控件
    var toolbar = [{
        iconCls: 'icon-add',
        text: "新发公告",
        handler: function () { location.href = "/Normal/AddMessage"; }
    }, '-',
               {
                   iconCls: 'icon-cancel',
                   text: "删除",
                   handler: function () { DelMsg(); }
               }
    ];
    var queryParams = { Status: $("#dpStatus").val() };
    InitGridTable('dgMsgs', "公告列表", '/Normal/Search', toolbar, queryParams, true);
    var grid = $('#dgMsgs').datagrid("options");
    grid.onClickCell = function (rowIndex, field, value) {
        if (field == "VIEW" || field == "TITLE")//单击查看列
        {
            var objRows = $('#dgMsgs').datagrid('getRows');
            var guid = objRows[rowIndex].ID;
            var title = objRows[rowIndex].TITLE;
            ShowMsgContent(guid, title);
        }
    };
}
//显示公告内容
function ShowMsgContent(guid, title) {
    $.post("/Normal/GetMsgDetail", { msgID: guid }, function (data) {
        $("#msgTitle").html(title);
        $("#msgContent").html(data);
        $("#dgMsgDetail").dialog('open');
    });
}
//删除公告
function DelMsg() {
    var chkRow = $('#dgMsgs').datagrid("getChecked");//选中行
    if (chkRow != null && chkRow.length > 0) {
        $.messager.confirm('系统提示', '确定要删除选中的公告?', function (r) {
            if (r) {
                var msgIDs = "";
                for (var i = 0; i < chkRow.length; i++) {
                    msgIDs += chkRow[i].ID + ",";
                }
                $.post("/Normal/DeleteMsg", { msgIDs: msgIDs }, function (data) {
                    $('#dgMsgs').datagrid('load');
                });
            }
        });
    }
    else {
        $.messager.alert('提示：', "请选择要删除的公告！");
    }
}

function HidePanel() {
    $("#dgMsgDetail").dialog('close');
}