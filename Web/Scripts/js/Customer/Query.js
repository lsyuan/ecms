/*customer query view*/
$(function () {
    InitControlQuery();
});
//初始化控件
function InitControlQuery() {
    //地区选择
    var buttons = [
        {
            iconCls: 'icon-ok',
            minimizable: false,
            text: "确定",
            handler: function () {
                var chkObjs = $("#areaTreeGrid").treegrid("getSelected");
                if (chkObjs != null && chkObjs.Identifier != "") {
                    if ($("#dArea").data("ctrlID") == "txtAreaIDSearch") {
                        $("#txtAreaIDSearch").data("areaID", chkObjs.Identifier);
                        $("#txtAreaIDSearch").searchbox('setValue', chkObjs.Area_Name);
                    }
                    else {
                        $("#txtAreaName").data("areaID", chkObjs.Identifier);
                        $("#txtAreaName").searchbox('setValue', chkObjs.Area_Name);
                    }
                }
                $("#dArea").dialog("close");
            }
        }
    ];

    //客户编辑窗口
    var editButtons = [{
        iconCls: 'icon-save',
        text: "保存",
        handler: function () { SaveCustomer(); }
    }];
    //编辑客户对话框
    InitDialog("dgEdit", "编辑客户信息", 650, 400, null, editButtons);

    //查询按钮 txtName
    $("#btnSearch").click(function () {
        BtnSearch();
    });
    //数据表格控件
    var toolbar = [{
        iconCls: 'icon-add',
        text: "启用",
        handler: function () { BtnEnabled("1"); }
    }, "-", {
        iconCls: 'icon-remove',
        text: "禁用",
        handler: function () { BtnEnabled("2"); }
    }, "-",
        {
            iconCls: 'icon-edit',
            text: "编辑",
            handler: function () {
                var chkRow = $('#gridCustomers').datagrid("getChecked")[0]; //选中行
                if (chkRow != undefined) {
                    $("#txtID").val(chkRow.ID);
                    $('#dgEdit').dialog("open");
                    LoadSingleCustomer(chkRow.ID);
                }
                else {
                    $.messager.alert('提示：', "请选择一行进数据进行相应操作！");
                }
            }
        }, "-",
        {
            iconCls: 'icon-cancel',
            text: "删除",
            handler: function () { BtnDelete(); }
        }];
    var queryParams = {
        name: $('#txtName').val(),
        status: $("#dpStatus").val(),
        areaID: $("#txtAreaIDSearch").data("areaID")
    };
    InitDialog("dArea", "选择所属地区", 600, 400, null, buttons);
    InitGridTable('gridCustomers', "客户信息列表", '/Customer/Search', toolbar, queryParams, true);
    //地区查询选择框
    $('#txtAreaIDSearch').searchbox({//查询
        searcher: function (value, name) {
            if (value != "") {
                $("#areaTreeGrid").treegrid("select", value)
            }
            $("#dArea").data("ctrlID", "txtAreaIDSearch");
            $("#dArea").dialog("open");
        },
        prompt: '选择要查询的地区'
    });
    //所属地区
    $('#txtAreaName').searchbox({
        searcher: function (value, name) {
            if (value != "") {
                $("#areaTreeGrid").treegrid("select", value);
            }
            $("#dArea").data("ctrlID", "txtAreaName")
            $("#dArea").dialog("open");
        },
        validtype: "selectValueRequired",
        prompt: ''
    });
    //负责人自动完成
    InputAutoComplete("txtManager", "/Employee/GetEmployeesByName", function () {
        var selectValue = $('#txtManager').combobox("getValue");
        $('#txtManager').data("managerID", selectValue);
    });
    InputAutoComplete("txtCustomerName", "/Customer/GetCustomerListByID", function () {
        var selectValue = $('#txtCustomerName').combobox("getValue");
        $("#CustomerID").val(selectValue);
    });
    //父客户选择
    InputAutoComplete("#txtParentName", "/Customer/GetCustomerListByID?IsParent=2", function () {
        var parentID = $("#txtParentName").combobox("getValue");
        if ($("#txtID").val() != parentID) {//父客户不能选择自己
            $("#txtParentName").data("parentID", parentID);
        }
        else {
            $("#txtParentName").combobox("setValue", "");
        }
    });
    //代缴权限(有代缴权限的用户不能有父级用户)
    $("#dpAgent").change(function () {
        if ($(this).val() == "0") {//无
            SetParentCtrlEnable(true);
        }
        else {
            SetParentCtrlEnable(false);
        }
    });
    //客户类型切换
    $("#dpCustomerType").change(function () {
        var strTypeID = $(this).val();
        LoadChargeItem(strTypeID, "");
    });
}
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
//查询
function BtnSearch() {
    $('#gridCustomers').datagrid('load', {
        Name: $('#txtName').val(),
        Status: $("#dpStatus").val(),
        AreaID: $("#txtAreaIDSearch").data("areaID")
    });
}

//启用/禁用
function BtnEnabled(status) {
    var chkRow = $('#gridCustomers').datagrid("getChecked"); //选中行
    if (chkRow != null && chkRow.length > 0) {
        var customerIDs = "";
        for (var i = 0; i < chkRow.length; i++) {
            customerIDs += chkRow[i].ID + ";";
        }
        var strUrl = "/Customer/SetEnabled";
        $.ajax({
            url: strUrl,
            data: { customerIDs: customerIDs, status: status },
            type: 'post',
            dataType: 'json',
            success: function (data) {
                if (data != null) {
                    $.messager.alert('提示：', data.Message);
                    BtnSearch();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $.messager.alert("error：", XMLHttpRequest + ":" + textStatus + errorThrown);
                return;
            }
        });
    }
    else {
        $.messager.alert('提示：', "请选择一行进数据进行相应操作！");
    }
}

//删除
function BtnDelete() {
    var chkRow = $('#gridCustomers').datagrid("getChecked")[0]; //选中行
    if (chkRow != null && chkRow.ID != "") {
        $.messager.confirm('系统提示', '确定要删除该客户?', function (r) {
            if (r) {
                $.post("/Customer/CustomerDelete", { customerID: chkRow.ID }, function (data) {
                    $('#gridCustomers').datagrid("reload");
                });
            }
        });
    }
    else {
        $.messager.alert('提示：', "请选择一行进数据进行相应操作！");
    }
}
//编辑客户详细信息
function LoadSingleCustomer(customerID) {
    var strUrl = "/Customer/CustomerDetailOnly";
    $.ajax({
        url: strUrl,
        data: { customerID: customerID },
        type: 'post',
        dataType: 'json',
        success: function (data) {
            if (data.ID != undefined) {
                $("#dgEdit").JsonData(data);
                $('#txtManager').combobox("setValue", data.MANAGERNAME);
                $("#txtManager").data("managerID", data.MANAGERID);
                //控件赋值
                $("#txtAreaName").data("areaID", data.AREAID);
                $("#txtAreaName").searchbox('setValue', data.AREANAME);
                if (data.AGENT == "0") {//无
                    SetParentCtrlEnable(true);
                    $("#txtParentName").data("parentID", data.PID);
                    $('#txtParentName').combobox("setValue", data.PARENTNAME);
                }
                else {
                    SetParentCtrlEnable(false);
                }
                LoadChargeItem(data.TYPEID);
                $("#lblAreaName").text(data.AREANAME); //用于审批页面，详细查看赋值 
            }
            else {
                $.messager.alert("提示：", "该客户的信息可能已被删除");
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("error：", XMLHttpRequest + ":" + textStatus + errorThrown);
            return;
        }
    });
}
//保存客户信息
function SaveCustomer() {
    if (!InputCheck("dgEdit")) {
        return;
    }
    if ($("#dpAgent").val() == "1" && $("#txtParentName").data("parentID") == null) {
        $.messager.alert("提示：", data.Message);
        return;
    }
    //以下为客户信息的提交，暂时隐藏
    var strUrl = $("#txtID").val() == "" ? "/Customer/AddCustomer" : "/Customer/CustomerModify";
    var params = $("#dgEdit").JsonData();
    params.ManagerID = $("#txtManager").data("managerID");
    //需要手动获取的控件
    params.AreaID = $('#txtAreaName').data("areaID"); //所在地区
    params.PID = $("#txtParentName").data("parentID"); //父级单位所在地区
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
                $("#dpCustomerType").val("");
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
//加载缴费项
function LoadChargeItem(customerTypeID, strCustomerID) {
    $.ajax({
        url: "/Customer/GetChargeItem",
        data: { customerTypeID: customerTypeID, customerID: strCustomerID != undefined ? strCustomerID : $("#txtID").val() },
        type: 'post',
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data != undefined) {
                $("#tableChargeItem>tbody>tr").remove(); //移除旧行
                for (var i = 0; i < data.length; i++) {
                    var isRegularFee = (data[i].IsRegular == "0" || data[i].IsRegular == "False") ? "否" : "是"; //周期性缴费
                    var isAgreeMent = (data[i].IsAgreeMent == "0" || data[i].IsAgreeMent == "False") ? "否" : "能"; //协议缴费
                    var strCount = isNaN(parseFloat(data[i].Count)) ? "0" : data[i].Count; //数量/协议金额
                    var strHtml = "<tr><td><input type='hidden' class='chargeItemID' value='" + data[i].ID + "'/>" + data[i].Name + "</td>";
                    strHtml += "<td>" + data[i].UnitPrice + "</td><td>" + data[i].UnitID1 + "</td><td>" + isRegularFee + "</td>";
                    strHtml += "<td>" + isAgreeMent + "</td>";
                    strHtml += "<td><input type='text' class='ItemCount' value='" + strCount + "'/></td>";
                    strHtml += "<td>0</td><td><input type='text' class='AgreementMoney' value='" + data[i].AgreementMoney + "'/></td>";
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

function hideButton() {
    var selectVal = $("#dpStatus :selected").val();
    if (selectVal != 3) {
        //隐藏第一个按钮
        $('div.datagrid-toolbar a').show();
        //隐藏第一条分隔线
        $('div.datagrid-toolbar div').show();
    }
    else {
        //隐藏第一个按钮
        $('div.datagrid-toolbar a').eq(3).hide();
        //隐藏第一条分隔线
        $('div.datagrid-toolbar div').eq(3).hide();

    }
}