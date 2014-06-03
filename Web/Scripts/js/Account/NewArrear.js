/*期初欠费*/
$(function () {
    InitCtrl();
});
//初始化页面及控件
function InitCtrl() {
    //客户信息自动完成
    InputAutoComplete("txtCustomerName", "/Customer/GetCustomerListByID", function () { });
    InputAutoComplete("txtCustomer", "/Customer/GetCustomerListByID", function () { });
    //gridFirstMoney
    var toolbar = [
        {
            iconCls: 'icon-add',
            text: "新增期初欠费",
            handler: function () {
                $("#ID").val("");
                $("#dgAddorEdit").dialog("open");
            }
        }, "-",
        {
            iconCls: 'icon-fee',
            text: "完成缴费",
            handler: function () {
                var chkRow = $('#gridFirstMoney').datagrid("getChecked")[0]; //选中行
                if (chkRow) {
                    Charge(chkRow);
                }
                else {
                    $.messager.alert("提示：", "请选择要操作的记录。");
                }
            }
        }, "-",
        {
            iconCls: 'icon-cancel',
            text: "删除",
            handler: function () {
                var chkRow = $('#gridFirstMoney').datagrid("getChecked")[0]; //选中行
                if (chkRow) {
                    Delete(chkRow);
                }
                else {
                    $.messager.alert("提示：", "请选择要操作的记录。");
                }
            }
        }, "-",
        {
            iconCls: 'icon-back',
            text: "作废",
            handler: function () {
                var chkRow = $('#gridFirstMoney').datagrid("getChecked")[0]; //选中行
                if (chkRow) {
                    Pull(chkRow);
                }
                else {
                    $.messager.alert("提示：", "请选择要操作的记录。");
                }
            }
        }];
    InitGridTable('gridFirstMoney', "期初欠费管理", '/Account/NewArrearSearch', toolbar,
        { customerID: $("#txtCustomer").combobox("getValue"), Status: $("#dpStatus").val() }, true, 15, { showFooter: true, rownumbers: true });
    //addOrEdit Dialog
    var addDialogButtons = [{
        iconCls: 'icon-save',
        text: "保存",
        handler: function () { SaveAddOrEdit(); }
    }];
    InitDialog("dgAddorEdit", "新增期初欠费：", 400, 200, null, addDialogButtons, function () {
        $('#txtCustomerName').combobox("setValue", "");
        $("#txtMoney").numberbox("setValue", "");
    });
    //查询button
    $("#btnSearch").click(function () {
        GridReload();
    });
    $("#btnAddNew").click(function () {
        $("#ID").val("");
        $("#dgAddorEdit").dialog("open");
    });
    $("#dpYear").val(new Date().getFullYear());
}

//重新加载一览数据
function GridReload() {
    $("#gridFirstMoney").datagrid("load", { customerID: $("#txtCustomer").combobox("getValue"), Status: $("#dpStatus").val() });
}
//保存
function SaveAddOrEdit() {
    if (!InputCheck("dgAddorEdit")) {
        return;
    }
    var params = {};
    params.CustomerID = $('#txtCustomerName').combobox("getValue");
    params.Money = $("#txtMoney").numberbox("getValue");
    params.Year = $("#dpYear").val();
    $.ajax({
        url: "/Account/AddNewArrear",
        data: params,
        type: 'post',
        dataType: 'json',
        success: function (data) {
            if (data.Success != undefined) {
                $.messager.alert("提示：", data.Message);
                if (data.Success) {
                    GridReload();
                }
            }
            $("#dgAddorEdit").dialog("close");
        }
    });
}
//缴费
function Charge(chkRow) {
    if (chkRow.Status != "未缴费") {
        $.messager.alert("提示：", "状态为'" + chkRow.Status + "'不能再次缴费!");
        return;
    }
    $.post("/Account/Charge", { customerID: chkRow.ID }, function (data) {
        if (data.Success != undefined) {
            $.messager.alert("提示：", data.Message);
            if (data.Success) {
                GridReload();
            }
        }
    });
}
//删除
function Delete(chkRow) {
    if (chkRow.Status == "已删除") {
        $.messager.alert("提示：", "状态已经为‘删除’!");
        return;
    }
    $.messager.confirm('系统提示', '确定要删除该记录?', function (r) {
        if (r) {
            $.post("/Account/Delete", { customerID: chkRow.ID }, function (data) {
                if (data.Success != undefined) {
                    $.messager.alert("提示：", data.Message);
                    if (data.Success) {
                        GridReload();
                    }
                }
            });
        }
    });
}
//作废
function Pull(chkRow) {
    if (chkRow.Status == "已作废") {
        $.messager.alert("提示：", "状态已经为‘作废’!");
        return;
    }
    if (chkRow.Status != "已缴费") {
        $.messager.alert("提示：", "如要作废缴费，该缴费记录的状态应为‘已缴费’!");
        return;
    }
    $.messager.confirm('系统提示', '确定要作废该记录并回退客户费用吗?', function (r) {
        if (r) {
            $.post("/Account/Pull", { customerID: chkRow.ID }, function (data) {
                if (data.Success != undefined) {
                    $.messager.alert("提示：", data.Message);
                    if (data.Success) {
                        GridReload();
                    }
                }
            });
        }
    });
}

function hideButton() {
    var selectVal = $("#dpStatus :selected").val();
    $('div.datagrid-toolbar a').show();
    $('div.datagrid-toolbar div').show();
    $('div.datagrid-toolbar a').eq(3).hide();
    $('div.datagrid-toolbar div').eq(3).hide();
    if (selectVal == 3) {
        $('div.datagrid-toolbar a').eq(2).hide();
        $('div.datagrid-toolbar div').eq(2).hide();
        $('div.datagrid-toolbar a').eq(3).hide();
        $('div.datagrid-toolbar div').eq(3).hide();
        $('div.datagrid-toolbar a').eq(1).hide();
        $('div.datagrid-toolbar div').eq(1).hide();
    }
    else if (selectVal == 4) {
        $('div.datagrid-toolbar a').eq(3).hide();
        $('div.datagrid-toolbar div').eq(3).hide();
        $('div.datagrid-toolbar a').eq(2).hide();
        $('div.datagrid-toolbar div').eq(2).hide();
        $('div.datagrid-toolbar a').eq(1).hide();
        $('div.datagrid-toolbar div').eq(1).hide();
    }
    else if (selectVal == 2) {
        $('div.datagrid-toolbar a').eq(1).hide();
        $('div.datagrid-toolbar div').eq(1).hide();
        $('div.datagrid-toolbar a').eq(2).hide();
        $('div.datagrid-toolbar div').eq(2).hide();
        $('div.datagrid-toolbar a').eq(3).show();
        $('div.datagrid-toolbar div').eq(3).show();
    }
    else if (selectVal == 1) {
        $('div.datagrid-toolbar a').eq(1).hide();
        $('div.datagrid-toolbar div').eq(1).hide();
    }
    GridReload();
}