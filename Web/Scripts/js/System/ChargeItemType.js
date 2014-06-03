/*view of收费项分类管理*/
$(function () {
    InitControl();
});
//
function InitControl() {
    //新增or编辑角色dialog
    var addButtonsDgAdd = [{ iconCls: 'icon-save',
        text: "保存",
        handler: function () { AddOrEditChargeItemType(); $("#dgAddOrEditChargeItemType").JsonData(null); }
    }, { iconCls: 'icon-cancel',
        text: "取消",
        handler: function () { $("#dgAddOrEditChargeItemType").dialog("close"); $("#dgAddOrEditChargeItemType").JsonData(null); }
    }];
    InitDialog("dgAddOrEditChargeItemType", "新增缴费项分类：", 400, 250, null, addButtonsDgAdd, function () {
        RefreashTable();
    });
    //初始化表格控件
    var toolbar = [{
        iconCls: 'icon-add',
        text: "新增",
        handler: function () {
            $("#ID").val("");
            $("#dgAddOrEditChargeItemType").dialog("setTitle", "新增缴费项分类:");
            $("#dgAddOrEditChargeItemType").dialog("open");
        }
    }, "-",
        {
            iconCls: 'icon-edit',
            text: "编辑",
            handler: function () {
                var chkRow = $('#ChargeItemTypeDataTable').datagrid("getChecked"); //选中行
                if (chkRow[0]) {
                    $("#ID").val(chkRow[0].ID);
                    $("#ChargeItemTypeName").val(chkRow[0].NAME);
                    $("#dgAddOrEditChargeItemType").dialog("setTitle", "编辑缴费项分类:");
                    $("#dgAddOrEditChargeItemType").dialog("open");
                }
            }
        }, "-",
        {
            iconCls: 'icon-cancel',
            text: "删除",
            handler: function () { DeleteChargeItemType(); }
        }];
    var queryParams = {
        Name: $('#txtName').val()
    };
    InitGridTable('#ChargeItemTypeDataTable', "收费项分类列表", '/System/SearchChargeItemCategory', toolbar, queryParams, true);
    //button Event
    $("#btnSearch").click(function () {
        RefreashTable();
    });
}
//重新加载数据
function RefreashTable() {
    $('#ChargeItemTypeDataTable').datagrid("reload", {
        Name: $('#txtName').val()
    });
}
//新增分类
function AddOrEditChargeItemType() {
    if (!InputCheck("#frmAddOrEditChargeItemType")) {
        return;
    }
    var params = $("#dgAddOrEditChargeItemType").JsonData();
    var strUrl = ($("#ID").val() == "") ? "/System/ChargeItemCategoryAdd" : "/System/ModifyChargeItemCategory";
    $.ajax({
        url: strUrl,
        data: params,
        dataType: 'json',
        success: function (data) {
            if (data.Success != undefined) {
                $("#dgAddOrEditChargeItemType").dialog("close");
                $.messager.alert("提示：", data.Message);
                RefreashTable();
            }
        },
        error: function () {
            $.messager.alert("error：", "数据访问失败！");
            return;
        }
    });
}
//删除
function DeleteChargeItemType() {
    var chkRow = $('#ChargeItemTypeDataTable').datagrid("getChecked"); //选中行
    if (chkRow[0]) {
        $.messager.confirm('系统提示', '确定要删除选中的分类?', function (r) {
            if (!r) return;
            var guid = chkRow[0].ID;
            $.ajax({
                url: "/System/DeleteChargeItemCategory",
                data: {ID:guid},
                dataType: 'json',
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