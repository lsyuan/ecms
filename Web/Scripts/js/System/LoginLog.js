$(function () {
    InitCtrl();
});
//
function InitCtrl() {
    //初始化表格控件
    var toolbar = [
        {
            iconCls: 'icon-cancel',
            text: "全部清空",
            handler: function () { BtnDelete(); }
        }];
    InitGridTable('#dgLoginLog', "客户类型列表", '/System/SearchLoginLog', toolbar, null, true, 10);
    //自动完成
    InputAutoComplete("OperatorID", "/System/GetOperatorByName", null);
    //btton event 
    $("#btnSearch").click(function () {
        $('#dgLoginLog').datagrid('load', {
            OperatorID: $('#OperatorID').combobox("getValue"),
            startDate: $("#startDate").datebox("getValue"),
            endDate: $("#endDate").datebox("getValue")
        });
    });
}

//日志清空
function BtnDelete() {
    $.messager.confirm('系统提示', '确定清空所有登录日志?', function (r) {
        if (r) {
            $.post("/System/DeleteAllLoginLog", {}, function (data) {
                if (data.Success != undefined) {
                    $('#dgLoginLog').datagrid('reload');
                }
            });
        }
    });

}