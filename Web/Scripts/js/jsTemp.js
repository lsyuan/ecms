//view of customerTypeClass
$(function () {
    InitControl();
    InitChargeItems();
    MustWrite("name", "客户类型名称必填");
});
//初始化控件
function InitControl() {
    //初始化弹出框
    //新增客户类型dialog
    var addButtonsDgAdd = [{ iconCls: 'icon-add',
        text: "保存",
        handler: function () { }
    }, { iconCls: 'icon-cancel',
        text: "取消",
        handler: function () { $("#").dialog("close"); }
    }];
    InitDialog("", "新增客户类型：", 300, 200, null, addButtonsDgAdd, function () {
        RefreashTable();
    });
    //分配缴费项dialog
    var addButtonsDgModify = [{ iconCls: 'icon-add',
        text: "保存",
        handler: function () {  }
    }, { iconCls: 'icon-cancel',
        text: "取消",
        handler: function () { $("#dgModifyChargeItem").dialog("close"); }
    }];
    InitDialog("", "分配缴费项：", 450, 300, null, addButtonsDgModify, function () {
        RefreashTable();
    });
    //初始化表格控件
    var toolbar = [{
        iconCls: 'icon-add',
        text: "添加",
        handler: function () { $("#").dialog("open"); }
    }, "-",
        {
            iconCls: 'icon-edit',
            text: "编辑",
            handler: function () {
                var chkRow = $('#').datagrid("getChecked"); //选中行
                if (chkRow[0]) {
                  
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
            handler: function () { $("#dgModifyChargeItem").dialog("open"); }
        }
        ];
    var queryParams = {
        Name: $('#').val()
    };
    InitGridTable('#', "客户类型列表", '/System/GetAllCustomerType', toolbar, queryParams, true);
    //注册button
    $("#btnSearch").click(function () {
        RefreashTable();
    });
}
//重新加载数据
function RefreashTable() {
    $('#').datagrid("reload", {
        Name: $('#').val()
    });
}
//新增客户类型
function AddNewCustomerType() {
    if (!$("#").form('validate')) {
        return;
    }
    var params = $("#").JsonData();
    $.ajax({
        url: "/System/AddCustomerType",
        data: params,
        dataType: 'json',
        success: function (data) {
            if (data.state != "1") {
                $.messager.alert("提示：", "新增成功！");
            }
            else {
                $.messager.alert("error：", "新增失败！"); //data.ErrorMsg
            }
        },
        error: function () {
            $.messager.alert("error：", "新增失败！");
            return;
        }
    });
}
//编辑客户类型
function EditCustomerType() {
    var result = $("#").JsonData();
    $.ajax({
        url: "/System/UpdateCustomerType",
        data: result,
        dataType: 'json',
        success: function (data) {
            Cancel();
            if (data[0].state != "1") {
                $.messager.alert("系统提示：", "保存成功.");
            }
            else {
                $.messager.alert("error:", data.ErrorMsg);
            }
        },
        error: function () {
            $.messager.alert("error:", "保存失败！");
            return;
        }
    });
}

//删除职员
function BtnDelete() {
    var chkRow = $('#').datagrid("getChecked"); //选中行
    if (chkRow[0]) {
        $.messager.confirm('系统提示', '确定要删除选中的客户类型?', function (r) {
            if (!r) return;
            var guid = chkRow[0].ID;
            $.ajax({
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
//初始化收费项
function InitChargeItems() {
    $.ajax({
        url: "/System/GetChargeItemForCheckBox",
        data: "isRegular=1",
        success: function (data) {
            if (data && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var strHtml = "<input type='checkbox' name='CHK_ChargeItem' value='" + data[i].ID + "'/>" + data[i].NAME + "&nbsp;";
                    $("#p_chargeItemContainer").append(strHtml);
                }
            }
            else {
                $("#p_chargeItemContainer").html("<font style='color:red'>系统尚未设置任何收费项目</font>");
            }
        },
        error: function (data) {
            $.messager.alert("error：", "数据加载失败！");
        }
    });
}
//保存配置好的收费项
function SaveChargeItems() {
    var strValue = "";
    $("input[name=CHK_ChargeItem]:checked").each(function () {
        strValue += $(this).val() + ",";
    });
    if (strValue == "") {
        $("#dgModifyChargeItem").dialog("close");
        return;
    }
    $.ajax({ url: "/System/ModifyTypeToItem",
        data: "customerTypeID=" + selectData[0] + "&chargeItemArray=" + strValue,
        success: function (data) {
            if (data) {
                $.messager.alert("提示：", "修改成功。");
            }
            else {
                $.messager.alert("提示：", "修改失败。");
            }
        },
        error: function (data) {
            $.messager.alert("error：", "修改失败。");
        }
    })
}
