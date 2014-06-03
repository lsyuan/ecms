var CusotmerID = "";
//view of CustomerCharge
$(function () {
    InitControl();
    resize();
    $('#cc').layout();
    $(window).resize = resize;
});

function resize() {
    $("[t=1]").height($(window).height() - 190);
    $("#cc,#div_child,#div_history").css('min-height', ($(window).height() - 190));
}

//初始化控件
function InitControl() {
    //自动完成，动态获取
    InputAutoComplete("txtCustomerName", "/Customer/GetCustomerListByID?IsParent=2", function () {
        var selectValue = $('#txtCustomerName').combobox("getValue");
        $("#CustomerID").val(selectValue);
        CusotmerID = selectValue;
        LoadCustomerInfo(selectValue);
    });
    //初始化数据表
    $('#gridCharge').datagrid({
        singleSelect: true,
        fitColumns: true,
        url: "/Charge/SearchChargeItem",
        pagination: false
    });
    $('#gridChildrenMsg').datagrid({
        singleSelect: true,
        fitColumns: true,
        url: "/Customer/GetCustomerChildren",
        pagination: false
    });
    $('#gridHistoryCharge').datagrid({
        singleSelect: true,
        fitColumns: true,
        url: "/Charge/ChargeSearch",
        pagination: false
    });
}
//加载客户完整信息
function LoadCustomerInfo(customerID) {
    $.ajax({
        url: "/Customer/CustomerDetail",
        data: { customerID: customerID },
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.customer != undefined) {
                data.customer.BEGINDATE = FormatTimeString(data.customer.BEGINDATE);
                data.customer.ENDDATE = FormatTimeString(data.customer.ENDDATE);
                $("#span1").JsonData(data.customer);
                $("#frmCustomerInfo").JsonData(data.customer);
                $("#frmCustomerInfo1").JsonData(data.customer);
                if (data.customer.AGREEMENTID == "") {
                    $("#FeeMonthCount").removeAttr("disabled");
                    $("#FeeMoney").attr("disabled", "disabled");
                }
                else {
                    $("#FeeMonthCount").attr("disabled", "disabled");
                    $("#FeeMoney").removeAttr("disabled");
                }
            }
            else {
                $.messager.alert("error：", "没有找到客户信息。", "error");
                $("#frmCustomerInfo").JsonData(null);
                $("#frmCustomerInfo1").JsonData(null);
                $("#totalMoney").val("");
            }
            // 协议信息
            if (data.agreements != null) {
                //data.agreements.agreementBeginDate = FormatTimeString(data.agreements.agreementBeginDate);
                //data.agreements.agreementEndDate = FormatTimeString(data.agreements.agreementEndDate);
                //$("#frmCustomerInfo").JsonData(data.agreements);
                $("#ACODE").text(data.agreements.ACODE);
                $("#AgreeMoney").text(data.agreements.MONEY);
                $("#AgreeBegin").text(data.agreements.BEGINDATE);
                $("#AgreeEnd").text(data.agreements.ENDDATE);
            }
            // 子客户
            if (data.childCustomer != null) {
                $("#gridChildrenMsg").datagrid("loadData", data.childCustomer);
            }
            // 加载缴费历史
            if (data.chargeHistory != null) {
                $("#gridHistoryCharge").datagrid("loadData", data.chargeHistory);
            }
            // 加载客户缴费信息
            if (data.chargeItem != null) {
                $('#gridCharge').datagrid("loadData", data.chargeItem);
            }
        },
        error: function () {
            $.messager.alert("error：", "数据加载失败！");
            return;
        }
    });
    // 加载客户缴费信息
    //$('#gridCharge').datagrid("reload", { customerID: customerID });
    // 加载子客户信息
    //$('#gridChildrenMsg').datagrid("reload", { customerID: customerID });
    // 加载缴费历史
    //$('#gridHistoryCharge').datagrid("reload", { customerID: customerID });
}
//发送到服务器端计费
function CaculateFee(customerId) {
    $.messager.progress({ title: customerId });
    $.ajax({
        url: "/Charge/CaculateCustomerTotalFee",
        data: { customerID: customerId },
        type: 'POST',
        success: function (data) {
            $.messager.progress('close');
            if (data.Success) {
                $("#totalMoney").val(data.Message);
            }
            else {
                $.messager.alert("error：", data.Message, "error");
            }
        },
        error: function () {
            $.messager.alert("error：", "数据加载失败！");
            return;
        }
    });
}
//费用总计
function CalculateFee(gridID) {
}
///保存缴费记录
function SaveFeeRecord() {
    if ($("#FeeMoney").attr(":disabled") == "disabled" && !InputCheck("chargeInfo")) {
        return;
    }
    if ($("#FeeMoney").attr(":disabled") == "disabled" && $("#AgreeID").text() == "" && $("#FeeMoney").numberbox("getValue") != $("#totalMoney").val()) {
        $.messager.alert("提示：", "请输入正确的实缴金额!");
        return;
    }
    if (!confirm("确认缴费吗?")) {
        return;
    }
    $.ajax({
        url: "/Charge/SaveFeeRecord",
        data: { "CustomerID": CusotmerID, "ChargeMonth": $("#FeeMonthCount :selected").val(), "ChargeMoney": $("#FeeMoney").val() == "" ? 0 : $("#FeeMoney").val() },
        type: 'POST',
        success: function (data) {
            $.messager.alert("提示：", data, "info");
            $('#gridHistoryCharge').datagrid();
            $('#gridChildrenMsg').datagrid();
            $('#gridCharge').datagrid();
            $("#frmCustomerInfo").JsonData(null);
            $("#frmCustomerInfo1").JsonData(null);
            $('#txtCustomerName').combobox("setValue", "");
            $("#CustomerID").val("");
            $("#FeeMonthCount").val(1);
            $("#FeeMoney").val("");
        },
        error: function (xx) {
            $.messager.alert("error：", xx.responseText);
            return;
        }
    });
}