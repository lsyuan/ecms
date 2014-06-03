/*票据分类管理*/
$(function () {
    InitControl();
});
//初始化
function InitControl() {
    //dialog
    //初始化新增弹出窗
    var buttons = [{ iconCls: 'icon-save',
        text: "保存",
        handler: function () { SaveAddOrEdit(); }
    }];
    InitDialog("dgAddorEdit", "新增票据分类：", 400, 200, null, buttons);
    //grid
    var toolbar = [{
        iconCls: 'icon-add',
        text: "添加",
        handler: function () {
            $("#ID").val("");
            OpenDialog();
        }
    }, "-",
        {
            iconCls: 'icon-edit',
            text: "编辑",
            handler: function () {
                var chkRow = $('#gridInvoiceType').datagrid("getChecked")[0]; //选中行
                if (chkRow) {
                    $("#dgAddorEdit").JsonData(chkRow);
                    $("#txtStepValue").numberbox("setValue", chkRow.StepValue); 
                    OpenDialog();
                }
            }
        }, "-",
        {
            iconCls: 'icon-cancel',
            text: "删除",
            handler: function () {
                var chkRow = $('#gridInvoiceType').datagrid("getChecked")[0]; //选中行
                if (chkRow) {
                    $.messager.confirm('系统提示', '确定要删除该分类?', function (r) {
                        if (r) { DelInvoiceType(chkRow.ID); }
                    });
                }
            }
        }];
        InitGridTable('gridInvoiceType', "票据分类一览", '/Invoice/TypeSearch', toolbar, {}, false);
}
//打开对话框
function OpenDialog() {
    if ($("#ID").val() == "") {
        $("#dgAddorEdit").dialog("setTitle", "新增票据分类:");
        $("#dgAddorEdit").find("input[type=text]").val("");
    }
    else {
        $("#dgAddorEdit").dialog("setTitle", "编辑票据分类:");
    }
    $("#dgAddorEdit").dialog("open");
}
//保存新增or编辑
function SaveAddOrEdit() {
    if (!InputCheck("dgAddorEdit")) {
        return;
    }
    var strUrl = "/Invoice/AddInvoiceType";
    if ($("#ID").val()!= "") {
        strUrl = "/Invoice/EditInvoiceType";
    }
    var parames = $("#dgAddorEdit").JsonData();
    parames.SetpValue=$("#txtStepValue").numberbox("getValue");
    $.ajax({
        url: strUrl,
        data: parames,
        dataType: 'json',
        cache:false,
        success: function (data) {
            if (data != null) {
                $.messager.alert('提示：', data.Message);
            }
            if (data.Success) {
                $("#gridInvoiceType").datagrid("reload");
            }
            $("#dgAddorEdit").dialog("close");
        },
        error: function () {
            $.messager.alert('提示：', "数据访问失败！");
        }
    });
}
//删除分类
function DelInvoiceType(TypeID) {
    if (TypeID == ""||TypeID==undefined) return;
    var parames = {TypeID:TypeID};
    $.ajax({
        url: "/Invoice/DeleteInvoiceType",
        data: parames,
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data != null) {
                $.messager.alert('提示：', data.Message);
            }
            if (data.Success) {
                $("#gridInvoiceType").datagrid("reload");
            }
        } 
    });
}