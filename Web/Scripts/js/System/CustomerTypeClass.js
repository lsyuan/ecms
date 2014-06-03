//view of customerTypeClass
$(function () {
    InitControl();
});

//初始化控件
function InitControl() {
    //初始化弹出框
    //新增客户类型dialog
    var addButtonsDgAdd = [{
        iconCls: 'icon-save',
        text: "保存",
        handler: function () {
            AddOrEditCustomerType();
        }
    }, {
        iconCls: 'icon-cancel',
        text: "取消",
        handler: function () { $("#dgAddNew").dialog("close"); }
    }];
    InitDialog("dgAddNew", "新增客户类型：", 300, 200, null, addButtonsDgAdd, function () {
        $("#CIID").val("");
        $("#Name").val("");
        RefreashTable();
    });
    //分配缴费项dialog
    var addButtonsDgModify = [{
        iconCls: 'icon-save',
        text: "保存",
        handler: function () { SaveChargeItems(); }
    }, {
        iconCls: 'icon-cancel',
        text: "取消",
        handler: function () { $("#dgModifyChargeItem").dialog("close"); }
    }];
    InitDialog("dgModifyChargeItem", "分配缴费项：", 600, 400, null, addButtonsDgModify, function () {
        RefreashTable();
    });
    //初始化表格控件
    var toolbar = [{
        iconCls: 'icon-add',
        text: "添加",
        handler: function () { $("#dgAddNew").dialog("open"); }
    }, "-",
        {
            iconCls: 'icon-edit',
            text: "编辑",
            handler: function () {
                var chkRow = $('#CustomerTypeDataTable').datagrid("getChecked"); //选中行
                if (chkRow[0]) {
                    $("#CIID").val(chkRow[0].ID);
                    $("#Name").val(chkRow[0].NAME);
                    $("#dgAddNew").dialog("setTitle", "编辑客户类型:");
                    $("#dgAddNew").dialog("open");
                }
            }
        }, "-",
        {
            iconCls: 'icon-cancel',
            text: "删除",
            handler: function () { BtnDelete(); }
        }, "-",
        {
            iconCls: 'icon-help',
            text: "缴费项分配",
            handler: function () {
                var chkRow = $('#CustomerTypeDataTable').datagrid("getChecked"); //选中行
                if (chkRow[0]) {
                    LoadCurrentChargeItem(chkRow[0].ID);
                    $("#dgModifyChargeItem").dialog("open");
                }
            }
        }];
    var queryParams = {
        Name: $('#txtCustomerTypeName').val()
    };
    InitGridTable('#CustomerTypeDataTable', "客户类型列表", '/System/GetAllCustomerType', toolbar, queryParams, true);
    //注册button
    $("#btnSearch").click(function () {
        RefreashTable();
    });
}
//重新加载数据
function RefreashTable() {
    $('#CustomerTypeDataTable').datagrid("reload", {
        Name: $('#txtCustomerTypeName').val()
    });
}
//新增or编辑客户类型
function AddOrEditCustomerType() {
    if (!InputCheck("#frmAddNewCustomerType")) {
        return;
    }
    var params = $("#dgAddNew").JsonData();
    var strUrl = $("#CIID").val() == "" ? "/System/AddCustomerType" : "/System/UpdateCustomerType";
    $.ajax({
        url: strUrl,
        data: params,
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data != undefined) {
                $.messager.alert("提示：", data.Message);
                $("#dgAddNew").dialog("close");
                RefreashTable();
            }
        },
        error: function () {
            $.messager.alert("error：", "新增失败！");
            return;
        }
    });
    $("#dgAddNew").dialog("close");
}

function BtnDelete() {
    var chkRow = $('#CustomerTypeDataTable').datagrid("getChecked"); //选中行
    if (chkRow[0]) {
        $.messager.confirm('系统提示', '确定要删除选中的客户类型?', function (r) {
            if (!r) return;
            var guid = chkRow[0].ID;
            $.ajax({ 
                cache: false,
                url: "/System/DeleteCustomerType",
                data: { "ID": guid },
                success: function (data) {
                    $.messager.alert("提示：", "删除成功！");
                    RefreashTable();
                },
                error: function () {
                    $.messager.alert("提示：", "删除失败！");
                }
            });
        });
    }
    else {
        $.messager.alert("提示：", "请选中要删除的行！");
    }
}

//保存配置好的收费项
function SaveChargeItems() {
    var strChargeItemIDs = "";
    $("input[name=chkChargeItem]:checked").each(function () {
        strChargeItemIDs += $(this).val() + ";";
    });
    var chkRow = $('#CustomerTypeDataTable').datagrid("getChecked"); //选中行
    $.ajax({
        url: "/System/ModifyTypeToItem",
        cache: false,
        data: { "customerTypeID": chkRow[0].ID, "chargeItemArray": strChargeItemIDs },
        success: function (data) {
            if (data.Success != undefined) {
                $("#dgModifyChargeItem").dialog("close");
                RefreashTable();
                $.messager.alert("提示：", data.Message);
            }
        },
        error: function (data) {
            $.messager.alert("error：", "数据访问失败。");
        }
    });
}
//加载当前选中项的缴费项
function LoadCurrentChargeItem(guid) {
    AjaxCall("/System/GetMyChargeItem", { ID: guid }, function (data) {
        if (data) {
            $("input[name=chkChargeItem]").removeAttr("checked");
            for (var i = 0; i < data.length; i++) {
                $("#" + data[i].ITEMID).attr("checked", "checked");
            }
        }
    }, function (data) {
        $.messager.alert("error：", "数据加载失败。");
    });
}
