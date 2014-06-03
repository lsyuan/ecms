/*customer add view*/
$(function () {
    InitControl();
});
//初始化下拉控件为只读
function InitControl() {
    //地区选择
    var buttons = [
        {
            iconCls: 'icon-ok',
            minimizable: false,
            text: "确定",
            handler: function () {
                var chkObjs = $("#areaTreeGrid").treegrid("getSelected");
                if (chkObjs != null && chkObjs.Identifier != "") {
                    $("#txtAreaID").val(chkObjs.Identifier);
                    $("#txtAreaName").searchbox('setValue', chkObjs.Area_Name);
                    $("#txtManager").val(chkObjs.Manager);
                    //客户查询页面
                    $("#txtAreaIDSearch").searchbox("setValue", chkObjs.Area_Name);
                }
                $("#dArea").dialog("close");
            }
        }
    ];
    InitDialog("dArea", "选择所属地区", 600, 400, null, buttons);
    //选择所属的父级客户
    InitDialog("dgParent", "选择所属的父级客户", 400, 250, null, null);
    //所属地区
    $('#txtAreaName').searchbox({
        searcher: function (value, name) {
            if (value != "") {
                $("#areaTreeGrid").treegrid("select", value);
            }
            strAreaCtrlID = "txtAreaName";
            $("#areaTreeGrid").focus();
            $("#dArea").dialog("open");
        },
        validtype: "selectValueRequired",
        prompt: ''
    });
    InputAutoComplete("#txtParentName", "/Customer/GetCustomerListByID?IsParent=2", function () {
        var parentID = $("#txtParentName").combobox("getValue");
        $("#txtParentID").val(parentID);
    });
    //searchbox文本区点击处理
    $(".searchbox-text").click(function () {
        $(this).next().eq(0).find(".searchbox-button").trigger("click")
    });
    //选中父级单位
    $("input[name=selectCustomer]").click(function () {
        $('#txtParentName').searchbox("setValue", $(this).parent().text());
        $('#txtParentID').val($(this).val());
        $("#dgParent").dialog("close");
    });
//    //代收权限切换
//    $("input[name=Agent]").change(function () {
//        if ($(this).val() == 1) {//有代收权限不指定父客户
//            $("#lblParent").hide();
//            $("#txtParentName").next(".searchbox").hide();
//        }
//        else {
//            $("#lblParent").show();
//            $("#txtParentName").next(".searchbox").show();
//        }
//    });
    //代缴权限(有代缴权限的用户不能有父级用户)
    $("#dpAgent").change(function () {
        if ($(this).val() == "0") {//无
            SetParentCtrlEnable(true);
        }
        else {
            SetParentCtrlEnable(false);
        }
    });
    //设置父客户控件的可用性
    function SetParentCtrlEnable(enabled) {
        if (enabled) {//启用
            $("#txtParentName").combobox("enable");
        }
        else {//禁用
            $("#txtParentName").data("parentID", "");
            $("#txtParentName").combobox("setValue", "");
            $("#txtParentName").combobox("disable");
        }
    }
    //保存按钮
    $("#btnSave").click(function () {
        SaveCustomer();
    });
    //负责人自动完成
    InputAutoComplete("txtManager", "/Employee/GetEmployeesByName", function () {
        var selectValue = $('#txtManager').combobox("getValue");
        $("#txtManagerID").val(selectValue);
    });
    //客户类型
    $("#dpCustomerType").change(function () {
        var strTypeID = $(this).val();
        LoadChargeItem(strTypeID);
    });
    $("#txtManager").combobox("textbox").focus(function () {
        $("#txtManager").combobox("textbox").val('');
        $("#txtManagerID").val('');
    });
    $("#txtParentName").combobox("textbox").focus(function () {
        $("#txtParentName").combobox("textbox").val('');
        $("#txtParentID").val('');
    });
    $('#BeginChargeDate').datebox({});
}


//加载缴费项
function LoadChargeItem(customerTypeID) {
    $.ajax({
        url: "/Customer/GetChargeItem",
        data: { customerTypeID: customerTypeID, customerID: $("#txtID").val() },
        type: 'post',
        dataType: 'json',
        success: function (data) {
            if (data != undefined) {
                $("#tableChargeItem>tbody>tr").remove(); //移除旧行
                for (var i = 0; i < data.length; i++) {
                    var isRegularFee = (data[i].IsRegular == "0" || data[i].IsRegular == "False") ? "否" : "是"; //周期性缴费
                    var isAgreeMent = (data[i].IsAgreeMent == "0" || data[i].IsAgreeMent == "False") ? "否" : "能"; //协议缴费
                    var strCount = isNaN(parseFloat(data[i].Count)) ? "0" : data[i].Count; //数量/协议金额
                    if (parseFloat(data[i].AgreementMoney) > 0)//协议金额
                    {
                        strCount = data[i].AgreementMoney;
                    }
                    var strHtml = "<tr><td><input type='hidden' class='chargeItemID' value='" + data[i].ID + "'/>" + data[i].Name + "</td>";
                    strHtml += "<td style='width:100px;text-align:right;'>" + data[i].UnitPrice + "</td><td style='width:140px;text-align:right;'>" + data[i].UnitID1 + "</td><td style='width:100px;'>" + isRegularFee + "</td>";
                    strHtml += "<td style='width:100px;'>" + isAgreeMent + "</td>";
                    strHtml += "<td style='text-align:right;width:100px'><input type='text' class='ItemCount' style='width:98%;text-align:right;' value='" + strCount + "'/></td>";
                    strHtml += "<td style='width:100px;text-align:right;'>0</td><td style='width:100px'><input type='text' class='AgreementMoney' style='width:98%;text-align:right;'  value='0'/></td>";
                    strHtml += "</tr>";
                    $("#tableChargeItem>tbody").append(strHtml);
                }
                $("#lblFeeItemCount").text(data.length); //缴费项总数
                InitNumberBox();
                CaculateFee();
            }
        }
    });
}
//初始化动态新增的数字文本框
function InitNumberBox() {
    $(".ItemCount").each(function () {
        $(this).numberbox({
            min: 0,
            precision: 2,
            disabled: $("#lblCustomerNum").length > 0 ? true : false, //查看详细页面使用
            onChange: function (newValue, oldValue) { //修改总数重新计费
                CaculateFee();
            }
        });
    });
    $(".AgreementMoney").each(function () {
        $(this).numberbox({
            min: 0,
            precision: 0,
            disabled: $("#lblCustomerNum").length > 0 ? true : false  //查看详细页面使用 
        });
    });
}
//费用计算
function CaculateFee() {
    var feeCount = 0; //一次性缴费总数
    $("#tableChargeItem>tbody>tr").each(function (i) {
        var $tdObjs = $(this).find("td");
        var price = 0;
        //if ($tdObjs.eq(4).text().indexOf("能") == -1) {//非协议缴费
        price = parseFloat($tdObjs.eq(1).text()) * parseFloat($tdObjs.eq(5).find("input").val());
        //} else { //协议缴费
        //price = parseFloat($(".ItemCount").eq(i).numberbox('getValue'));
        //}
        $tdObjs.eq(6).text(price.toFixed(2)); //精度处理
        feeCount += price;
    });
    $("#lblFeeCount").text(feeCount.toFixed(2));
}

//保存客户信息
function SaveCustomer() {
    if (!InputCheck("dgEdit")) {
        return;
    }
    //以下为客户信息的提交，暂时隐藏
    var strUrl = $("#txtID").val() == "" ? "/Customer/AddCustomer" : "/Customer/CustomerModify";
    var params = $("#dgEdit").JsonData();
    //需要手动获取的控件
    params.AreaID = $('#txtAreaID').val(); //所在地区
    params.PID = $('#txtParentID').val(); //父级单位所在地区
    params.TypeID = $("#dpCustomerType").val();
    params = extend({}, [params, GetChargeItemJsonData()]); //合并客户基本信息以及对应的缴费项
    $.ajax({
        url: strUrl,
        data: params,
        type: 'post',
        dataType: 'json',
        success: function (data) {
            if (data.Success != undefined) {
                $.messager.alert("提示：", data.Message);
                if (strUrl == "/Customer/CustomerModify") {//更新完成
                    $("#dgEdit").dialog("close");
                    BtnSearch();
                }
                $("#dgEdit").JsonData(null);
                $("#txtAreaName").val("");
                $('#txtManager').combobox("setValue", "");
                $('#txtParentName').combobox("setValue", "");
                $("#txtAreaName").searchbox('setValue', "");
                $("#txtManagerID,#txtParentID,#txtAreaID").val("");
                $("#dpCustomerType").val("");
                $("#tableChargeItem :text").val("");
                $(".ItemCount").each(function () {
                    $(this).numberbox('setValue', 0);
                });
                $('#BeginChargeDate').datebox('setValue', '');
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("error：", XMLHttpRequest + ":" + textStatus + errorThrown);
            return;
        }
    });
}

//获取缴费项json
function GetChargeItemJsonData() {
    var objString = "{";
    $("#tableChargeItem>tbody>tr").each(function (i) {
        objString += "'[" + i + "].ItemID':'" + $(this).find(".chargeItemID").val() + "',";
        objString += "'[" + i + "].Count':'" + $(".ItemCount").eq(i).numberbox('getValue') + "',";
        if ($(this).find("td").eq(4).text().indexOf("能") != -1) {
            objString += "'[" + i + "].AgreementMoney':'" + $('.AgreementMoney', $(this).find("td")).val() + "'";
        }
        else {
            objString += "'[" + i + "].AgreementMoney':'0'";
        }
        if (i != $("#tableChargeItem>tbody>tr").length - 1) {
            objString += ",";
        }
    });
    objString += "}";
    return strToJson(objString);
}