//view of ContrastMgr
$(function () {
    InitControl();
});
//初始化控件
function InitControl() {
    //初始化表格控件
    var toolbar = [{
        iconCls: 'icon-add',
        text: "新增协议",
        handler: function () {
            $("#dgAddAgree").attr("tag", "add");
            $("#ID").numberbox("enable");
            $("#dgAddAgree").dialog("setTitle", "新增协议:");
            $("#dgAddAgree").dialog("open");
        }
    }, "-",
        {
            iconCls: 'icon-edit',
            text: "编辑",
            handler: function () {
                var chkRow = $('#AgreeDataTable').datagrid("getChecked"); //选中行
                if (chkRow[0]) {
                    if (chkRow[0].STATUS != "未审核") {
                        $.messager.alert("提示：", "只能修改未审核的协议！");
                        return;
                    }
                    $("#dgAddAgree").attr("tag", "edit");
                    $("#ID").numberbox("disable");
                    $("#ID").numberbox("setValue", chkRow[0].ID);
                    var statusValue = chkRow[0].STATUS.replace("未审核", "0").replace("已审核", "1");
                    $("#Status").attr("disabled", "disabled");
                    $("#Status").val(statusValue);
                    $("#customerID").val(chkRow[0].CUSTOMERID);
                    $("#customerSelect").combobox("setValue", chkRow[0].CUSTOMERID);
                    $("#Money").val(chkRow[0].MONEY);
                    $('#BeginDate').datebox('setValue', FormatTimeString(chkRow[0].BEGINDATE));
                    $('#EndDate').datebox('setValue', FormatTimeString(chkRow[0].ENDDATE));
                    $("#dgAddAgree").dialog("setTitle", "编辑客户协议:");
                    $("#dgAddAgree").dialog("open");
                }
            }
        }, "-",
        {
            iconCls: 'icon-ok',
            text: "通过审批",
            handler: function () { Agree(); }
        }, "-",
        {
            iconCls: 'icon-checkAll',
            text: "全部通过",
            handler: function () { Agree("all"); }
        }, "-",
        {
            iconCls: 'icon-cancel',
            text: "拒绝协议",
            handler: function () { RefuseAgree(); }
        }];
    var queryParams = $("#Bar").JsonData();
    InitGridTable('#AgreeDataTable', "缴费协议表", '/Business/SearchAgree', toolbar, queryParams, false);
    //新增协议dialog
    var addButtonsDgAdd = [{
        iconCls: 'icon-save',
        text: "保存",
        handler: function () { AddOrEditAgree(); }
    }, {
        iconCls: 'icon-cancel',
        text: "取消",
        handler: function () { $("#dgAddAgree").dialog("close"); }
    }];
    InitDialog("dgAddAgree", "新增协议：", 600, 300, null, addButtonsDgAdd, function () { });
    //注册button
    $("#btnSearch").click(function () {
        RefreashTable();
    });
    //自动完成
    InputAutoComplete("#customerSelect", "/Customer/GetCustomerListByID?IsParent=2", function () {
        var selectValue = $('#customerSelect').combobox("getValue");
        $("#CustomerID").val(selectValue);
    });
}
//重新加载数据
function RefreashTable() {
    var params = $("#toolbar").JsonData(); //搜索条件
    $('#AgreeDataTable').datagrid("reload", params);
}
//新增协议
function AddOrEditAgree() {
    if (!InputCheck("#dgAddAgree")) {
        return;
    }
    //正确性验证
    var startTime = $("#BeginDate").datebox("getValue");
    var endTime = $("#EndDate").datebox("getValue");
    if (startTime > endTime) {
        $.messager.alert("提示：", "协议起始时间不能大于结束时间！");
        return;
    }
    if ($("#customerID").val() == "") {
        $.messager.alert("提示：", "用户输入不正确！");
        return;
    }
    var strUrl = ($("#dgAddAgree").attr("tag") == "add") ? "/Business/AddAgree" : "/Business/ModifyAgree";
    var params = $("#dgAddAgree").JsonData();
    params.BeginDate = $('#BeginDate').datebox('getValue'); //时间控件需要手动获取
    params.EndDate = $('#EndDate').datebox('getValue');
    $.ajax({
        url: strUrl,
        data: params,
        dataType: 'json',
        success: function (data) {
            if (data.Success != undefined) {
                $.messager.alert("提示：", data.Message);
                if (data.Success) {
                    $("#dgAddAgree").JsonData(null);
                    $("#CustomerID").val("");
                    $("#customerSelect").combobox("setValue", '');
                    $("#BeginDate").datebox('setValue', '');
                    $("#EndDate").datebox('setValue', '');
                    $("#dgAddAgree").dialog("close");
                    RefreashTable();
                }
            }
        },
        error: function () {
            $.messager.alert("error：", "数据访问失败！");
            return;
        }
    });
}
//拒绝协议
function RefuseAgree() {
    var chkRow = $('#AgreeDataTable').datagrid("getChecked"); //选中行
    if (chkRow[0]) {
        $.messager.confirm('系统提示', '确定要拒绝选中的协议?', function (r) {
            if (!r) return;
            var guids = "";
            for (var i = 0; i < chkRow.length; i++) {
                guids += chkRow[i].ID + ";";
            }
            $.ajax({
                url: "/Business/RefuseAgree",
                data: { "guids": guids },
                success: function (data) {
                    if (data.Success != undefined) {
                        $.messager.alert("提示：", data.Message);
                        if (data.Success) {
                            RefreashTable();
                        }
                    }
                },
                error: function () {
                    $.messager.alert("提示：", "数据访问失败！");
                }
            });
        });
    }
    else {
        $.messager.alert("提示：", "请选中要拒绝的协议！");
    }
}

//通过审核协议
function Agree(type) {
    if (type == "all") {//全部同意
        $.messager.confirm('系统提示', '确定要将所有‘未审核’协议改为‘已审核’?', function (r) {
            if (!r) return;
            $.ajax({
                url: "/Business/ApprovalAll",
                data: { "guids": "" },
                success: function (data) {
                    if (data.Success != undefined) {
                        RefreashTable();
                    }
                },
                error: function () {
                    $.messager.alert("提示：", "数据访问失败！");
                }
            });
        });
    }
    else { //同意选中
        var chkRow = $('#AgreeDataTable').datagrid("getChecked"); //选中行
        if (chkRow[0]) {
            $.messager.confirm('系统提示', '确定通过选中的所有协议?', function (r) {
                if (!r) return;
                //通过协议
                var guids = "";
                for (var i = 0; i < chkRow.length; i++) {
                    if (chkRow[i].STATUS == "未审核") {//只能处理未审核
                        guids += chkRow[i].ID + ";";
                    }
                }
                if (guids == "") {
                    $.messager.alert("提示：", "只能审核‘未审核’的协议！");
                    return;
                }
                $.ajax({
                    url: "/Business/ApprovalAgree",
                    data: { "guids": guids },
                    success: function (data) {
                        if (data.Success != undefined) {
                            $.messager.alert("提示：", data.Message);
                            if (data.Success) {
                                RefreashTable();
                            }
                        }
                    },
                    error: function () {
                        $.messager.alert("提示：", "数据访问失败！");
                    }
                });
            });
        }
    }
}

function hideButton() {
    var selectVal = $("#dpStatus :selected").val();
    $('div.datagrid-toolbar a').show();
    $('div.datagrid-toolbar div').show();
    if (selectVal != 0) {
        $('div.datagrid-toolbar a').hide();
        $('div.datagrid-toolbar div').hide();
        $('div.datagrid-toolbar a').eq(0).show();
        $('div.datagrid-toolbar div').eq(0).show();
    }
}