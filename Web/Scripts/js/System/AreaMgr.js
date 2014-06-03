/*view of area*/
$(function () {
	InitControl();
});

function InitControl() {
	//初始化添加窗口
	var addButtons = [{
		iconCls: 'icon-save',
		text: "保存",
		handler: function () { AddorEditNode(); }
	}];
	InitDialog("dgAddOrEdit", "新增地区：", 400, 200, null, addButtons, function () { });
	//新增button
	$("#btnAdd").click(function () {
		var chkNode = $('#areaTreeGrid').treegrid("getSelected");
		if (chkNode) {
			$("#txtName").val(chkNode.Area_Name);
			$("#txtPID").val(chkNode.Identifier);
		}
		$("#txtAreaID").val("");
		$("#txtManager").combobox("setValue", "");
		$("#txtAreaName").val("");
		$("#dgAddOrEdit table tr:first").show();
		$("#dgAddOrEdit").dialog("setTitle", "新增地区:");
		$("#dgAddOrEdit").dialog("open");
	});
	//修改button
	$("#btnModify").click(function () {
		$("#dgAddOrEdit table tr:first").hide();
		var chkNode = GetChkID();
		if (chkNode) {
			$("#txtAreaID").val(chkNode.Identifier);
			$("#txtAreaName").val(chkNode.Area_Name);
			$("#txtEmpID").val(chkNode.Manager);
			$("#txtManager").combobox("setValue", chkNode.ManagerName); 
			$("#dgAddOrEdit").dialog("setTitle", "编辑地区:");
			$("#dgAddOrEdit").dialog("open");
		}
	});
	//删除button
	$("#btnDelete").click(function () {
		var chkNode = GetChkID();
		if (chkNode) {
			$.messager.confirm('系统提示', '确认要删除该节点及其所有子节点?', function (r) {
				if (r) {
					$.post("/System/DeleteArea", { ID: chkNode.Identifier }, function (data) {
						RefreashTree();
					});
				}
			});
		}
	});
	//负责人输入自动完成
	InputAutoComplete("txtManager", "/Employee/GetEmployeesByName", function () {
		var empID = $("#txtManager").combobox("getValue");
		$("#txtEmpID").val(empID); 
	});
}
//获取选中节点的ID
function GetChkID() {
	var chkNode = $('#areaTreeGrid').treegrid("getSelected");
	if (chkNode != undefined && chkNode != null) {
		return chkNode;
	}
	else {
		$.messager.alert('提示：', "未选中任何行！");
		return false;
	}
}
//重新加载树
function RefreashTree() {
	$('#areaTreeGrid').treegrid("reload");
}

//新增节点
function AddorEditNode() {
	if (!InputCheck("dgAddOrEdit")) {
		return;
	}
	var strUrl = $("#txtAreaID").val()==""? "/System/AddArea":"/System/ModifyArea";
	$.ajax({
		url: strUrl,
		data: $("#dgAddOrEdit").JsonData(),
		success: function (data) {
			if (data.Success != undefined) {
				$.messager.alert("系统提示", data.Message);
				if (data.Success) {
					RefreashTree();
				}
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			$.messager.alert("系统提示", "添加失败！");
		}
	});
	$("#dgAddOrEdit").JsonData(null);
	$("#dgAddOrEdit").dialog("close");
}