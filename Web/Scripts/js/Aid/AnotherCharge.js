$(function () {
    InitControl();
});
//init
function InitControl() {
    //客户缴费表
    var otherChargeToolbar = [{
        iconCls: 'icon-add',
        text: "收费",
        handler: function () { $("#dgOtherCharge").dialog("open"); }
    }, {
        iconCls: 'icon-delete',
        text: "删除",
        handler: deleteAnotherCharge
    }, {
        iconCls: 'icon-delete',
        text: "回退",
        handler: backAnotherCharge
    }];
    InitGridTable('gridOtherCharge', "其他缴费一览", '/Aid/AnotherChargeSearch?Status=-1', otherChargeToolbar, null, false);
    //收费dialog
    var addButtonsDgAdd = [{
        iconCls: 'icon-cancel',
        text: "取消",
        handler: function () { $("#dgOtherCharge").dialog("close"); }
    }, {
        iconCls: 'icon-save',
        text: "保存",
        handler: function () { SaveOtherCharge(); }
    }];
    InitDialog("dgOtherCharge", "新增其他收费：", 530, 200, null, addButtonsDgAdd, cleardata);
}
// delete another charge record
function deleteAnotherCharge() {

}
// back another charge record
function backAnotherCharge() {

}
function cleardata() {
    $("#dgOtherCharge").JsonData(null);
}
//保存其他收费
function SaveOtherCharge() {
    if (!InputCheck("dgOtherCharge")) {
        return;
    }
    var parames = $("#dgOtherCharge").JsonData();
    $.ajax({
        url: "/Aid/AddAnotherCharge",
        data: parames,
        type: 'post',
        dataType: 'json',
        success: function (data) {
            if (data.Success != undefined) {
                $.messager.alert("提示：", data.Message);
                $("#dgOtherCharge").dialog("close");
                $("#gridOtherCharge").datagrid('reload', null);
            }
        }
    });
}