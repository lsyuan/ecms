/*customer audit view*/
$(function () {
    InitControlConfirm();
});
//初始化控件
function InitControlConfirm() {
    //数据表格控件
    var toolbar = [{
        iconCls: 'icon-ok',
        text: "通过审批",
        handler: function () { Audit(undefined, 1); }
    }, {
        iconCls: 'icon-cancel',
        text: "拒绝审批",
        handler: function () { Audit(undefined, 4); }
    }];
    var queryParams = { status: "0" };
    InitGridTable('dgCustomers', "待审批客户列表", '/Customer/Search', toolbar, queryParams, false);
    //渲染查看详细的button
    var grid = $('#dgCustomers').datagrid("options");
    grid.onLoadSuccess = function (data) { RenderDetailBtn(data); };
    grid.onClickCell = function (rowIndex, field, value) {
        if (field == "CreateTime") {
            var rowsObj = $('#dgCustomers').datagrid('getRows');
            LoadSingleCustomer(rowsObj[rowIndex].ID);
            $("#dgEdit").dialog("open");
        }
    };
    //查看详细对话框
    var buttons = [
        {
            iconCls: 'icon-ok',
            minimizable: false,
            text: "通过审核",
            handler: function () { Audit($("#txtID").val(), 1); }
        }, {
            iconCls: 'icon-cancle',
            text: "不通过审批",
            handler: function () { Audit($("#txtID").val(), 4); }
        }
    ];
    InitDialog("dgEdit", "客户详细信息：", 650, 400, null, buttons);
}
//审批
function Audit(customerID, status) {
    var chkRow = $('#dgCustomers').datagrid("getChecked"); //选中行
    if (chkRow != null && chkRow.length > 0 || customerID != undefined) {
        var strID = "";
        for (var i = 0; i < chkRow.length; i++) {
            strID += chkRow[i].ID + ",";
        }
        if (customerID != undefined) {
            strID = customerID;
        }
        $.messager.confirm('系统提示', '确定选中的客户都通过了人工审核?', function (r) {
            if (r) {
                $.post("/Customer/Audit", { customerID: strID, status: status }, function (data) {
                    if (data != null) {
                        $.messager.alert('提示：', data.Message);
                    }
                    $('#dgCustomers').datagrid('load');
                });
                $("#dgEdit").dialog("close");
            }
        });
    }
    else {
        $.messager.alert('提示：', "请选择一行进数据进行相应操作！");
    }
}
//生成详细查看的按钮
function RenderDetailBtn(gridData) {
    for (var i = 0; i < gridData.rows.length; i++) {
        $('#dgCustomers').datagrid('updateRow', {
            index: i,
            row: {
                ID: gridData.rows[i].ID,
                Name: gridData.rows[i].Name,
                TypeID: gridData.rows[i].TypeID,
                Status: gridData.rows[i].Status,
                Contactor: gridData.rows[i].Contactor,
                Address: gridData.rows[i].Address,
                CreateTime: '<label style="cursor:pointer">查看详细<label/>'
            }
        });
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
                //控件赋值
                $("#txtAreaID").val(data.AREAID);
                $("#ManagerID").val(data.MANAGERID);
                $("#txtAreaName").searchbox('setValue', data.AREANAME);
                if (data.PID == undefined || data.PID == "") {
                    $("input[name=Agent]").eq(1).attr("checked", "checked").change();
                }
                else {
                    $("#txtParentID").val(data.PID);
                    $('#txtParentName').searchbox("setValue", data.PARENTNAME);
                }
                LoadChargeItem(data.TYPEID);
                //$("#dpCustomerType").change();
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
                    //if (parseFloat(data[i].AgreementMoney) > 0)//协议金额
                    //{
                    //    strCount = data[i].AgreementMoney;
                    //}
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
        if ($tdObjs.eq(4).text().indexOf("能") == -1) {//非协议缴费
            price = parseFloat($tdObjs.eq(1).text()) * parseFloat($tdObjs.eq(5).find("input").val());
        } else { //协议缴费
            price = parseFloat($(".ItemCount").eq(i).numberbox('getValue'));
        }
        $tdObjs.eq(6).text(price.toFixed(2)); //精度处理
        feeCount += price;
    });
    $("#lblFeeCount").text(feeCount.toFixed(2));
}
