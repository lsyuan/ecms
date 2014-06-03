$(function () {
    InitControl();
});

function InitControl() {
    //初始化新增、编辑窗口
    var addButtons = [{
        iconCls: 'icon-save',
        text: "保存",
        handler: function () { AddOrEditUnit(); }
    }];
    InitDialog("dgAddOrEdit", "新增计费单位：", 400, 200, null, addButtons, function () {
        RefreashTable();
    });
    //初始化表格控件
    var toolbar = [{
        iconCls: 'icon-add',
        text: "添加",
        handler: function () {
            $("#CIID").val("");
            $("#dgAddOrEdit").find("input[type=text]").val("");
            $("#dgAddOrEdit").dialog("setTitle", "新增计费单位:");
            $("#dgAddOrEdit").dialog("open");
        }
    }, "-",
        {
            iconCls: 'icon-edit',
            text: "编辑",
            handler: function () {
                var chkRow = $('#dgFeeUnit').datagrid("getChecked"); //选中行
                if (chkRow[0]) {
                    $.ajax({
                        url: "/System/GetSingelUnit",
                        data: { ID: chkRow[0].ID },
                        dataType: 'json',
                        success: function (data) {
                            $("#dgAddOrEdit").JsonData(data);
                            ControlTimeValue();
                        },
                        error: function () {

                        }
                    });
                    $("#dgAddOrEdit").dialog("setTitle", "编辑计费单位:");
                    $("#dgAddOrEdit").dialog("open");
                }
                else {
                    $.messager.alert("提示", "请选择要编辑的单位！");
                }
            }
        }, "-",
        {
            iconCls: 'icon-cancel',
            text: "删除",
            handler: function () { DeleteUnit(); }
        }];
    var queryParams = {
        Name: $('#txtName').val(),
        Status: 1
    };
    InitGridTable('#dgFeeUnit', "计费单位列表", '/System/UnitSearch', toolbar, queryParams, true);
    $("#btnSearch").click(function () {
        RefreashTable();
    });
    // 权重只能为数字
    $('#TimeValue').numberbox({
        min: 0, // 计时时可为0
        precision: 0,
        max: 12  // 最大为12个月即一年
    });
}
//刷新
function RefreashTable() {
    var queryParams = { Name: $('#txtName').val() };
    queryParams.Status = 1;
    $('#dgFeeUnit').datagrid("reload", queryParams);
}
//新增or编辑
function AddOrEditUnit() {
    if (!InputCheck("#frmAddOrEditUnit")) {
        return;
    }
    ControlTimeValue();
    var result = $("#dgAddOrEdit").JsonData();
    var strUrl = $("#CIID").val() == "" ? "/System/AddUnit" : "/System/ModifyUnit";
    $.ajax({
        url: strUrl,
        data: result,
        dataType: 'json',
        success: function (result) {
            if (result != undefined) {
                $("#dgAddOrEdit").dialog("close");
                $.messager.alert("提示", "操作成功");
            }
        },
        error: function () {
            $.messager.alert("error", "数据访问失败！");
            return;
        }
    });
}
//删除
function DeleteUnit() {
    var chkRow = $('#dgFeeUnit').datagrid("getChecked"); //选中行
    if (chkRow[0]) {
        $.messager.confirm('系统提示', '确定要删除选中的计费单位?', function (r) {
            if (!r) return;
            var id = chkRow[0].ID;
            $.ajax({
                url: "/System/DeleteUnit",
                data: { "id": id },
                success: function (data) {
                    if (data.Success != undefined) {
                        $.messager.alert("提示", data.Message);
                        RefreashTable();
                    }
                },
                error: function () {
                    $.messager.alert("error", "数据访问失败！");
                    return;
                }
            });
        });
    }
}

// 计时时不能输入权重
function ControlTimeValue() {
    if ($("#SELECT_LEVEL :selected").val() == "1") {
        $("#TimeValue").attr("disabled", "disabled");
        $("#TimeValue").val(0);
    }
    else {
        $("#TimeValue").attr("disabled", false);
    }
}