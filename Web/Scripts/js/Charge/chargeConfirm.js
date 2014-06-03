//view of Charge Confirm
$(function () {
    InitControl();
});
//init
function InitControl() {
    //详细弹出窗
    InitDialog("dgChargeDetail", "缴费详细", 430, 300, null, null);
    //客户缴费表
    var customerChargeToolbar = [{
        iconCls: 'icon-ok',
        text: "审核通过",
        handler: function () { Audit("gridCustomerCharge", true); }
    }, {
        iconCls: 'icon-cancel',
        text: "拒绝审批",
        handler: function () { Audit("gridCustomerCharge", false); }
    }];
    InitGridTable('gridCustomerCharge', "客户缴费审批", '/Charge/CustomerChargeConfirmSearch', customerChargeToolbar, null, false, 15);
    //临时缴费表
    var tempChargeToolbar = [{
        iconCls: 'icon-ok',
        text: "审核通过",
        handler: function () { Audit("gridTempCharge", true); }
    }, {
        iconCls: 'icon-cancel',
        text: "拒绝审批",
        handler: function () { Audit("gridTempCharge", false); }
    }];
    InitGridTable('gridTempCharge', "临时缴费审批", '/Charge/TempChargeConfirmSearch', tempChargeToolbar, null, false);
    //其他缴费表
    var otherChargeToolbar = [{
        iconCls: 'icon-ok',
        text: "审核通过",
        handler: function () { Audit("gridOtherCharge", true); }
    }, {
        iconCls: 'icon-cancel',
        text: "拒绝审批",
        handler: function () { Audit("gridOtherCharge", false); }
    }];
    InitGridTable('gridOtherCharge', "其他缴费审批", '/Aid/AnotherChargeSearch?Status=0', otherChargeToolbar, null, false);
    //期初欠费缴费审批
    var FirstMoneyChargeToolbar = [{
        iconCls: 'icon-ok',
        text: "审核通过",
        handler: function () { Audit("gridFirstMoneyCharge", true); }
    }, {
        iconCls: 'icon-cancel',
        text: "拒绝审批",
        handler: function () { Audit("gridFirstMoneyCharge", false); }
    }];
    InitGridTable('gridFirstMoneyCharge', "期初欠费缴费审批", '/Account/NewArrearSearch?Status=1', FirstMoneyChargeToolbar, null, false);


    //‘点击详细’事件绑定
    $("#gridCustomerCharge").datagrid("options").onClickCell = function (rowIndex, field, value) {
        if (field == "Detail") {
            var gridData = $("#gridCustomerCharge").datagrid("getData");
            LoadChargeDetail("1", gridData.rows[rowIndex].ID);
        }
    };
    $("#gridTempCharge").datagrid("options").onClickCell = function (rowIndex, field, value) {
        if (field == "Detail") {
            var gridData = $("#gridTempCharge").datagrid("getData");
            LoadChargeDetail("2", gridData.rows[rowIndex].ID);
        }
    };
    $("#gridOtherCharge").datagrid("options").onClickCell = function (rowIndex, field, value) {
        if (field == "Detail") {
            var gridData = $("#gridOtherCharge").datagrid("getData");
            LoadChargeDetail("3", gridData.rows[rowIndex].ID);
        }
    };
}
//加载详细缴费项
function LoadChargeDetail(chargeType, chargeID) {
    var strUrl = "";
    var ctrlID = "";
    var parames = { chargeID: chargeID };
    if (chargeType == "1") {//客户缴费
        strUrl = "/Charge/SearchCustomerChildrenList";
        ctrlID = "CustomerDetail";
    }
    else if (chargeType == "2") {//临时缴费
        strUrl = "/Charge/LoadTempChargeDetail";
        ctrlID = "TempDetail";
    }
    else {//其他缴费
        strUrl = "/Aid/LoadOtherChargeDetail";
        ctrlID = "OtherChargeDetail";
    }
    $.ajax({
        url: strUrl,
        data: parames,
        type: 'post',
        dataType: 'json',
        username: ctrlID,
        success: function (data) {
            if (data == undefined) {
                $.messager.alert("提示：", "暂无缴费详细记录");
                return;
            }
            $("#dgChargeDetail table").each(function () { $(this).hide(); }); //隐藏全部
            var strHtml = "";
            var strCtrlID = this.username;
            if (strCtrlID == "OtherChargeDetail") {
                $("#dgChargeDetail").dialog("open");
                data.CHARGEDATE = FormatTimeString(data.CHARGEDATE);
                $("#" + this.username).show().JsonData(data);
                return;
            }
            $.each(data, function (i) {
                strHtml += "<tr  bgcolor=\"#ffffff\">";
                if (strCtrlID == "CustomerDetail") {

                }
                else if (strCtrlID == "TempDetail") {
                    strHtml += "<td>" + data[i].CHARGENAME + "</td>";
                    strHtml += "<td>" + data[i].NAME + "</td>";
                    strHtml += "<td>" + data[i].COUNT + "</td>";
                    strHtml += "<td>" + data[i].MONEY + "</td>";
                    strHtml += "<td>" + FormatTimeString(data[i].CREATETIME) + "</td>";
                }
                strHtml += "</tr>";
            });
            $("#" + this.username).show().find("tbody").html(strHtml);
            $("#dgChargeDetail").dialog("open");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("error：", "数据访问失败");
            return;
        }
    });
}

//审核
function Audit(gridID, isPass) {
    var strUrl = "";
    var strGuids = "";
    var checkRows = $("#" + gridID).datagrid("getChecked");
    $.each(checkRows, function (i) {
        strGuids += checkRows[i].ID + ",";
    });
    if (strGuids == "") {
        $.messager.alert("提示：", "请选择要操作的数据！");
        return;
    }
    if (gridID == "gridCustomerCharge") {//客户交费审核
        strUrl = "/Charge/ChargeAudit";
    }
    else if (gridID == "gridTempCharge") {//临时交费审核
        strUrl = "/Charge/TempChargeAudit";
    }
    else if (gridID == "gridOtherCharge") { //其他交费审核
        strUrl = "/Aid/ChargeAudit";
    }
    else if (gridID == "gridFirstMoneyCharge") {//期初欠费缴费审核
        strUrl = "/Account/ChargeAudit";
    }
    var parames = { guids: strGuids, isPass: isPass };
    $.ajax({
        url: strUrl,
        data: parames,
        type: 'post',
        dataType: 'json',
        username: gridID,
        success: function (data) {
            if (data.Success != undefined) {
                $.messager.alert("提示：", data.Message);
                $("#" + this.username).datagrid("reload");
                alert();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("error：", XMLHttpRequest + ":" + textStatus + errorThrown);
            return;
        }
    });
}