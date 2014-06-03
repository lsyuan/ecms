$(function () {
    InitControl();
});
//
function InitControl() {
    //新增缴费项dialog
    var addButtonsDg = [{
        iconCls: 'icon-save',
        text: "保存",
        handler: function () { AddOrEditChargeItem(); }
    }];
    InitDialog("dgChargeItemAddOrEdit", "新增缴费项：", 700, 240, null, addButtonsDg, function () {
        $("#dgChargeItemAddOrEdit").JsonData(null);
    });
    //设置收费策略dialog
    var addButtonsDgPolicy = [{
        iconCls: 'icon-save',
        text: "保存",
        handler: function () { SavePolicy(); }
    }];
    InitDialog("dgPolicy", "设置收费策略：", 500, 240, null, addButtonsDgPolicy, function () {
        //$("#dgPolicy").JsonData(null);
    });
    //初始化表格控件
    var toolbar = [{
        iconCls: 'icon-add',
        text: "新增",
        handler: function () {
            $("#ID").val("");
            $("#dgChargeItemAddOrEdit").dialog("setTitle", "新增缴费项:");
            $("#dgChargeItemAddOrEdit").dialog("open");
        }
    }, "-",
        {
            iconCls: 'icon-edit',
            text: "编辑",
            handler: function () {
                var chkRow = $('#ChargeItemDataTable').datagrid("getChecked"); //选中行
                if (chkRow[0]) {
                    var chkObj = LoadSingleRow(chkRow[0].ID);
                    $("#dgChargeItemAddOrEdit").dialog("setTitle", "编辑缴费项:");
                    $("#dgChargeItemAddOrEdit").dialog("open");
                }
            }
        }, "-",
        {
            iconCls: 'icon-setting',
            text: "分级收费策略",
            handler: function () {
                var chkRow = $('#ChargeItemDataTable').datagrid("getChecked"); //选中行
                if (chkRow[0]) {
                    LoadChargePolicy(chkRow[0].ID);
                    $("#dgPolicy").dialog("open");
                }
            }
        }, "-",
        {
            iconCls: 'icon-cancel',
            text: "删除",
            handler: function () { DeleteChargeItem(); }
        }];
    var queryParams = {
        Name: ""
    };
    InitGridTable('ChargeItemDataTable', "系统缴费项目列表", '/System/SearchChargeItem', toolbar, queryParams, true, 20);
    //button Event
    $("#btnSearch").click(function () {
        RefreashTable();
    });
    $("#btnAddPolicy").click(function () {//新增缴费策略
        AddPolicyRow();
    });
    $(".btnDelPolicy").live("click", function () {//删除缴费策略
        var strHtml = "<img class='btnDelPolicy' src='../../Content/images/EasyUIImages/cancel.png' alt='删除' title='删除'>";
        $(this).parent().parent().remove();
        var $lastRowObj = $("#policyTable tbody tr:last");
        $lastRowObj.find("td:last").html(strHtml);
        var newBegin = parseFloat($lastRowObj.find("td").eq(2));
        if (!isNaN(newBegin)) {
            $("#txtBegin").val(newBegin);
        }
        else {
            $("#txtBegin").val("0");
        }
    });
    //是否周期性缴费
    $("#SelIsRegular").change(function () {
        var $selUnitObj = $("#SelUnitID2");
        if ($(this).val() == "0") {
            $selUnitObj.removeAttr("disabled");
        }
        else {
            $selUnitObj.children().each(function () {
                if ($(this).text().indexOf("月") != -1) {
                    $(this).attr("selected", "selected");
                    return;
                }
            });
            $selUnitObj.attr("disabled", "disabled");
        }
    });
}
//重新加载数据
function RefreashTable() {
    $('#ChargeItemDataTable').datagrid("reload", {
        Name: $('#txtSearchName').val()//,
        //UnitPrice: $("#txtUnitPrice").val()
    });
}
//加载编辑行数据
function LoadSingleRow(guid) {
    $.ajax({
        url: "/System/GetChargeItem",
        data: { ID: guid },
        dataType: 'json',
        cache: false,
        success: function (data) {
            $("#frmChargeItem").JsonData(data);
            $("#UnitPrice").numberbox("setValue", data.UNITPRICE);
            $("#SelIsRegular").trigger("change");
        },
        error: function () {
            $.messager.alert("error：", "数据加载失败！");
            return null;
        }
    });
}
//加载缴费策略
function LoadChargePolicy(guid) {
    $.ajax({
        url: "/Charge/LoadChargePolicy",
        data: { chargeItemID: guid },
        dataType: 'json',
        cache: false,
        success: function (data) {
            $("#policyTable tbody tr").remove();
            $("#txtBegin").val("0");
            $.each(data, function (i) {
                AddPolicyRow(data[i]);
                var lastHignBound = parseFloat(data[i].HIGNERBOUND);
                if (!isNaN(lastHignBound)) {
                    $("#txtBegin").val(lastHignBound);
                }
            });
        },
        error: function () {
            $.messager.alert("error：", "数据加载失败！");
            return null;
        }
    });
}

//新增or编辑
function AddOrEditChargeItem() {
    if (!InputCheck("#frmChargeItem")) {//输入验证
        return;
    }
    var strUrl = $("#ID").val() == "" ? "/System/AddChargeItem" : "/System/ModifyChargeItem";
    var params = $("#frmChargeItem").JsonData();
    params.UnitPrice = $("#UnitPrice").numberbox("getValue");
    //combbbox控件另外获取值
    $.ajax({
        url: strUrl,
        data: params,
        dataType: 'json',
        success: function (data) {
            if (data.Success != undefined) {
                $.messager.alert("提示：", data.Message);
                $("#dgChargeItemAddOrEdit").dialog("close");
                $("#dgChargeItemAddOrEdit").JsonData(null);
                RefreashTable();
            }
        },
        error: function () {
            $("#dgChargeItemAddOrEdit").dialog("close");
            $.messager.alert("error：", "数据访问失败！");
            $("#dgChargeItemAddOrEdit").JsonData(null);
            return;
        }
    });
}
//删除收费项
function DeleteChargeItem() {
    var chkRow = $('#ChargeItemDataTable').datagrid("getChecked"); //选中行
    if (chkRow[0]) {
        $.messager.confirm('系统提示', '确定要删除选中的收费项?', function (r) {
            if (!r) return;
            $.ajax({
                url: "/System/DeleteChargeItem",
                data: { ID: chkRow[0].ID },
                dataType: 'json',
                cache: false,
                success: function (data) {
                    if (data.Success != undefined) {
                        $.messager.alert("提示：", data.Message);
                        RefreashTable();
                    }
                },
                error: function () {
                    $.messager.alert("error：", "数据访问失败！");
                    return;
                }
            });
        });
    }
}
//新增缴费策略
function AddPolicyRow(rowObj) {
    if (rowObj == undefined && !InputCheck("dgPolicy")) {
        return;
    }
    var begin = (rowObj == undefined) ? parseFloat($("#txtBegin").val()) : parseFloat(rowObj.LOWERBOUND);
    var end = (rowObj == undefined) ? parseFloat($("#txtEnd").numberbox("getValue")) : parseFloat(rowObj.HIGNERBOUND);
    var price = (rowObj == undefined) ? parseFloat($("#txtPrice").numberbox("getValue")) : parseFloat(rowObj.UNITPRICE);
    var strHtml = "<tr><td align='center'>" + begin + "</td>";
    strHtml += "<td align='center'>" + (isNaN(end) ? "以上" : end) + "</td><td align='center'>" + price + "</td>";
    strHtml += "<td align='center'><img class='btnDelPolicy' src='../../Content/images/EasyUIImages/cancel.png' alt='删除' title='删除'></td></tr>";
    //添加前判断
    if (!isNaN(end) && begin >= end) {
        $.messager.alert("提示：", "上限必须大于下限！", "info");
        return;
    }
    var $policyRowsObj = $("#policyTable tbody tr");
    var lineCount = $policyRowsObj.length;
    if (lineCount == 0) {
        if (begin > 0) {
            $.messager.alert("提示：", "只能从0开始", "info");
            return;
        }
    }
    else {
        var lastEnd = parseFloat($policyRowsObj.eq(lineCount - 1).find("td").eq(1).text());
        if (isNaN(lastEnd)) {
            return;
        }
    }
    for (var i = 0; i < lineCount; i++) {
        if (i < lineCount) {
            $policyRowsObj.eq(i).find(".btnDelPolicy").remove();
        }
    }
    $("#policyTable").append(strHtml);
    if (!isNaN(end)) {
        $("#txtBegin").val(end);
    }
    $("#txtEnd").numberbox("setValue", "");
    $("#txtPrice").numberbox("setValue", "");
}
//保存收费协议
function SavePolicy() {
    var chkRow = $('#ChargeItemDataTable').datagrid("getChecked"); //选中行
    var strJson = "{";
    $("#policyTable tbody tr").each(function (i) {
        strJson += "'[" + i + "].ItemID':'" + chkRow[0].ID + "',";
        strJson += "'[" + i + "].UnitPrice':'" + $(this).children().eq(2).text() + "',";
        strJson += "'[" + i + "].LowerBound':'" + $(this).children().eq(0).text() + "',";
        var hignBound = parseFloat($(this).children().eq(1).text());
        if (isNaN(hignBound)) {
            hignBound = -1; //-1表示‘以上’
        }
        strJson += "'[" + i + "].HignerBound':'" + hignBound + "',";
    });
    strJson = strJson.substring(0, strJson.length - 1);
    strJson += "}";
    var parames = strToJson(strJson);
    $.ajax({
        url: "/Charge/SaveChargePolicy",
        data: parames,
        success: function (data) {
            if (data.Success != undefined) {
                $.messager.alert("提示：", data.Message);
            }
        },
        error: function () {
            $.messager.alert("error：", "数据访问失败！");
            return;
        }
    });
}