/*view of dept*/
$(function () {
    InitControl();
});

function InitControl() {
    //初始化新增弹出窗
    var addButtons = [{
        iconCls: 'icon-add',
        text: "添加",
        handler: function () { AddNode(); }
    }];
    InitDialog("dgAdd", "新增部门：", 400, 200, null, addButtons, function () {
        RefreashTree();
    });
    //初始化编辑弹出窗
    var addButtons = [{
        iconCls: 'icon-save',
        text: "保存",
        handler: function () { EditNode(); }
    }];
    InitDialog("dgEdit", "编辑部门：", 400, 200, null, addButtons, function () {
        RefreashTree();
    });
    //新增按钮
    $("#btnAdd").click(function () {
        var chkNode = $('#deptTreeGrid').treegrid("getSelected");
        if (chkNode) {
            $("#txtPID").val(chkNode.Identifier);
            $("#txtParentDeptName").val(chkNode.Dept_Name);
            $("#txtDeptName").val("");
            $("#txtDeptName").focus();
        }
        else {
            $("#txtPID").val("");
            $("#txtParentDeptName").val("");
            $("#txtDeptName").val(""); 
        }
        $("#dgAdd").dialog("open");
    });
    //编辑按钮
    $("#btnModify").click(function () {
        var chkNode = GetChkID();
        if (chkNode) {
            $("#txtIDEdit").val(chkNode.Identifier);
            $("#txtDeptNameEdit").val(chkNode.Dept_Name);
            $("#txtDeptNameEdit").focus();
            $("#dgEdit").dialog("open");
        }
    });
    //删除按钮
    $("#btnDelete").click(function () {
        var chkNode = GetChkID();
        if (chkNode) {
            $.messager.confirm('系统提示', '确认要删除该节点及其所有子节点?', function (r) {
                if (r) {
                    $.ajax({
                        url: "/Dept/Delete",
                        data: { "ID": chkNode.Identifier },
                        cache: false,
                        success: function (data) {
                            $.messager.alert('提示：', "删除成功！");
                            RefreashTree();
                        },
                        error: function () {
                            $.messager.alert('提示：', "删除失败！");
                            RefreashTree();
                        }
                    });
                }
            });
        }
    });
}

//获取选中节点的ID
function GetChkID() {
    var chkNode = $('#deptTreeGrid').treegrid("getSelected");
    if (chkNode != undefined && chkNode != null) {
        return chkNode;
    }
    else {
        $.messager.alert('提示：', "未选中任何行！");
        return false;
    }
}

//新增部门节点
function AddNode() {
    if (!InputCheck("dgAdd")) {
        return;
    }
    $.ajax({
        url: "/Dept/AddDept",
        cache: false,
        data: { "PID": $("#txtPID").val(), "DeptName": $("#txtDeptName").val() },
        success: function (data) {
            $.messager.alert('提示：', "添加成功！");
            $("#dgAdd").dialog("close");
            RefreashTree();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $.messager.alert('提示：', "添加失败！");
            $("#dgAdd").dialog("close");
            RefreashTree();
        }
    });
}
//编辑部门节点
function EditNode() {
    if (!InputCheck("dgEdit")) {
        return;
    }
    $.ajax({
        url: "/Dept/Modify",
        cache: false,
        data: { "ID": $("#txtIDEdit").val(), "Name": $("#txtDeptNameEdit").val() },
        success: function (data) {
            $.messager.alert('提示：', "更新成功！");
            $("#txtIDEdit").val("");
            $("#txtDeptNameEdit").val("");
            $("#dgEdit").dialog("close");
            RefreashTree();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $.messager.alert('提示：', "更新失败！");
            $("#dgEdit").dialog("close");
            RefreashTree();
        }
    });
}
//重新加载tree
function RefreashTree() {
    $('#deptTreeGrid').treegrid("reload");
}