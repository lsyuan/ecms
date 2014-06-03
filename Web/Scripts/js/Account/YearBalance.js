$(function () {
    InitCtrl();
});

function InitCtrl() {
    //年份
    var curDate = new Date();
    var year = curDate.getFullYear() - 1;
    for (var i=0; i<5; i++) {
        $("#dpYear").append("<option value='"+year+"'>"+year+"</option>");
        year--;
    }
    //查询
    $("#btnSearch").click(function () {
        GridReload();
    });
    //执行结算
    $("#btnYearFee").click(function () {
        $("#btnYearFee").attr("disabled","disabled");
        $.post("/Account/CaculateYearFee", { strYear: $("#dpYear").val() }, function (data) {
            $("#btnYearFee").removeAttr("disabled");
            if (data.Success != undefined) {
                $.messager.alert("提示：", data.Message);
                if (data.Success) {
                    GridReload();
                }
            }
        });
    });
    //客户信息自动完成
    InputAutoComplete("txtCustomer", "/Customer/GetCustomerListByID", function () { });
    //gridYearBalance
    var toolbar = [{
        iconCls: 'icon-checkAll',
        text: "标记为坏账",
        handler: function () {
            var chkRow = $('#gridYearBalance').datagrid("getChecked")[0]; //选中行
            if (chkRow) {
                MarkBadFee(chkRow);
            }
            else {
                $.messager.alert("提示：", "请选择要操作的记录。");
            }
        }
    }];
    InitGridTable('gridYearBalance', "年终欠费管理", '/Account/YearBalanceSearch', toolbar, { customerID: $("#txtCustomer").combobox("getValue"), Year: $("#dpYear").val() }, true);
}
//刷新
function GridReload() {
    $("#gridYearBalance").datagrid("load", { CustomerID: $("#txtCustomer").combobox("getValue"), Year: $("#dpYear").val() });
}
//标记为坏账
function MarkBadFee(chkRow) {
    if (chkRow.Status != "欠费未缴纳") {
        $.messager.alert("提示：", "状态已经为‘" + chkRow.Status + "’!");
        return;
    }
    $.messager.confirm('系统提示', '确定要标记为坏账?', function (r) {
        if (r) {
            $.post("/Account/MarkBadFee", { strID: chkRow.ID }, function (data) {
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