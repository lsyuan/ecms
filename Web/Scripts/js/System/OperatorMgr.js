/*view of 用户管理*/
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
        handler: function () { AddOrEditOperator(); }
    }];
    InitDialog("dgAddorEditOperator", "新增系统用户：", 400, 280, null, addButtonsDgAdd, function () {
        //RefreashTable();
    });
    //初始化表格控件
    var toolbar = [{
        iconCls: 'icon-add',
        text: "添加",
        handler: function () {
            $("#ID").val("");
            $("#CreateDate").val("");
            $("#dgAddorEditOperator").dialog("open");
        }
    }, "-",
        {
            iconCls: 'icon-edit',
            text: "编辑",
            handler: function () {
                var chkRow = $('#OperatorDataTable').datagrid("getChecked")[0]; //选中行
                if (chkRow) {
                    $("#ID").val(chkRow.ID);
                    LoadOperatorInfo(chkRow.ID);
                    //$("#EMPLOYEENAME").combobox("disable");
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
            text: "启用/禁用",
            handler: function () { SetEnable(); }
        }];
    var queryParams = {
        Name: $('#txtName').val()
    };
    InitGridTable('#OperatorDataTable', "系统用户列表", '/System/SearchOperator', toolbar, queryParams, true);
    //职工名称自动完成
    InputAutoComplete("EMPLOYEENAME", "/Employee/GetEmployeesByName", function () {
        var selectValue = $('#EMPLOYEENAME').combobox("getValue");
        $("#EmployeeID").val(selectValue);
    });
}
//重新加载数据
function RefreashTable() {
    $('#OperatorDataTable').datagrid("reload", {
        Name: $('#txtName').val()
    });
}

//保存or编辑登录账户
function AddOrEditOperator() {
    //必填验证
    if (!InputCheck("dgAddorEditOperator")) {
        return;
    }
    //正确性验证
    if ($("#txtPWD").val() != $("#txtPWDconfirm").val()) {
        $.messager.alert('提示：', "密码不一致！");
        return;
    }
    var parames = $("#dgAddorEditOperator").JsonData();
    parames.EMPNAME = $("#EMPLOYEENAME").combobox("textbox").val(); //获取特殊字段值
    var strUrl = $("#ID").val() == "" ? "/System/AddOperator" : "/System/ModifyOperator";
    $.ajax({
        url: strUrl,
        cache: false,
        data: parames,
        success: function (data) {
            if (data != undefined) {
                $("#dgAddorEditOperator").dialog("close");
                $("#dgAddorEditOperator").JsonData(null);
                $("#EMPLOYEENAME").combobox("setValue", "");
                $.messager.alert("系统提示", data.Message);
            }
            RefreashTable();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $.messager.alert("系统提示", "添加失败！");
        }
    });
}

//加载操作员信息
function LoadOperatorInfo(operId) {
    $.ajax({
        url: "/System/GetSingelOperatorInfo",
        cache: false,
        type: "POST",
        data: { Id: operId },
        success: function (data) {
            if (data != undefined) {
                $("#dgAddorEditOperator").dialog("setTitle", "编辑用户信息:");
                $("#dgAddorEditOperator").dialog("open");
                $("#dgAddorEditOperator").JsonData(data); //特殊字段赋值
                $("[dn=PWD]").val("123");
                $("#EMPLOYEENAME").combobox("setValue", data.EMPLOYEENAME);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $.messager.alert("系统提示", "数据加载失败！");
        }
    });
}
//账户删除
function BtnDelete() {
    var chkRow = $('#OperatorDataTable').datagrid("getChecked"); //选中行
    if (chkRow && chkRow.length > 0) {
        var strDelIDs = "";
        for (var i = 0; i < chkRow.length; i++) {
            strDelIDs += chkRow[i].ID + ",";
        }
        $.messager.confirm('系统提示', '确定要删除选中的系统用户?', function (r) {
            if (!r) return;
            $.ajax({
                url: "/System/DeleteOperator",
                type: "POST",
                cache: false,
                data: { IDs: strDelIDs },
                success: function (data) {
                    if (data != undefined) {
                        $.messager.alert("系统提示", data.Message);
                        RefreashTable();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $.messager.alert("系统提示", "数据加载失败！");
                }
            });
        });
    }
}

//账户的禁用
function SetEnable() {
    var chkRow = $('#OperatorDataTable').datagrid("getChecked"); //选中行
    if (chkRow && chkRow.length > 0) {
        var strDelIDs = "";
        for (var i = 0; i < chkRow.length; i++) {
            strDelIDs += chkRow[i].ID + ",";
        }
        $.messager.confirm('系统提示', '确定要禁用选中的系统用户?', function (r) {
            if (!r) return;
            $.ajax({
                url: "/System/OperatorDisable",
                cache: false,
                type: "POST",
                data: { IDs: strDelIDs },
                success: function (data) {
                    if (data != undefined) {
                        $.messager.alert("系统提示", data.Message);
                        RefreashTable();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $.messager.alert("系统提示", "数据加载失败！");
                }
            });
        });
    }
}

function Clear() {
    $("#dgAddorEditOperator").JsonData(null);
    $("#EMPLOYEENAME").combobox("setValue", '');
}