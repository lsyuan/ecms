/*view of employee*/
$(function () {
	InitControl();
});
//初始化控件
function InitControl() {
	//初始化数据表格控件
	var toolbar = [{
		iconCls: 'icon-add',
		text: "添加",
		handler: function () {
			$("#EID").val("");
			$("#dgAdd").find("input[type=text]").val("");
			$("#dgAdd").dialog("setTitle", "新增职员信息:");
			$("#dgAdd").dialog("open");
		}
	}, "-",
                 {
                 	iconCls: 'icon-edit',
                 	text: "编辑",
                 	handler: function () { BtnEdit(); }
                 }, "-",
                 {
                 	iconCls: 'icon-cancel',
                 	text: "删除",
                 	handler: function () { BtnDelete(); }
                 }
        ];
	var queryParams = {
		Name: $('#txtEmployeeName').val()
	};
	InitGridTable('gridEmployee', "职员信息列表", '/Employee/Search', toolbar, queryParams, true);
	var grid = $('#gridEmployee').datagrid("options");
	//grid.onDblClickRow = function (rowIndex, field, value) { BtnEdit(rowIndex, field, value); };
	//初始化新增弹出窗
	var buttons = [{ iconCls: 'icon-save',
		text: "保存",
		handler: function () { BtnAddorEdit(); }
	}];
	InitDialog("dgAdd", "新增职员信息：", 550, 320, null, buttons, function () {
		$('#gridEmployee').datagrid("reload");
	});
	//查询按钮
	$("#btnQuery").click(function () {
		$('#gridEmployee').datagrid("load", { Name: $("#txtEmployeeName").val() });
	});
	//部门选择框
	var deptButtons = [{ iconCls: 'icon-ok',
		text: "选择",
		handler: function () {
			var chkObjs = $("#deptTreeGrid").treegrid("getSelected");
			if (chkObjs != null && chkObjs.Identifier != "") {
				$("#txtDeptID").val(chkObjs.Identifier); //隐藏控件
				$("#DeptName").searchbox('setValue', chkObjs.Dept_Name);
				$("#dgDept").dialog("close");
			}
		}
	}];
	InitDialog("dgDept", "选择所属部门：", 450, 250, null, deptButtons);
	$('#DeptName').searchbox({
		searcher: function (value, name) {
			if (value != "") {
				$("#deptTreeGrid").treegrid("select", $("#txtDeptID").val())
			}
			$("#dgDept").dialog("open");
		}
	});
	//searchbox文本区点击处理
	$(".searchbox-text").click(function () {
		$(this).next().find(".searchbox-button").trigger("click")
	});
}
//新增or更新职员 
function BtnAddorEdit() {
	if (!InputCheck("dgAdd")) {
		return;
	}
	var parames = $("#dgAdd").JsonData();
	//获取特殊控件值
	parames.BirthDate = $("#BirthDate").datebox("getValue");
	parames.WorkTime = $("#WorkTime").datebox("getValue");
	parames.FireTime = $("#FireTime").datebox("getValue");
	var strUrl = "/Employee/AddEmployee";
	if ($("#EID").val() == "") {//新增
		strUrl = "/Employee/AddEmployee";
	}
	else { //编辑
		strUrl = "/Employee/ModifyEmployee";
	}
	$.ajax({
		url: strUrl,
		data: parames,
		dataType: 'json',
		success: function (data) {
			if (data != null) {
				$.messager.alert('提示：', data.Message);
			}
			$("#dgAdd").dialog("close");
		},
		error: function () {
			$.messager.alert('提示：', "数据访问失败！");
			$("#dgAdd").dialog("close"); ;
		}
	});
}
//编辑职员信息
function BtnEdit() {
	var chkRow = $('#gridEmployee').datagrid("getChecked")[0]; //选中行
	if (chkRow) {
		$("#EID").val(chkRow.ID);
		$.ajax({
			url: "/Employee/GetEmployee?R="+Math.random(),
			data: { "empID": chkRow.ID },
			success: function (data) {
				$("#dgAdd").JsonData(data);
				//特殊控件赋值
				$("#txtDeptID").val(data.DEPTID); //隐藏控件DeptName
				$("#DeptName").searchbox('setValue', data.DEPTNAME);
				$("#dgAdd").dialog("setTitle", "用户信息编辑:");
				$("#BirthDate").datebox("setValue", FormatTimeString(data.BIRTHDATE));
				$("#WorkTime").datebox("setValue", FormatTimeString(data.WORKTIME));
				$("#FireTime").datebox("setValue", FormatTimeString(data.FIRETIME));
				var Dialog = $("#dgAdd").dialog("open");
			},
			error: function () {
				$.messager.alert("error：", "获取职员信息失败！");
				return;
			}
		});
	}
	else {
		$.messager.alert("系统提示:", "未选中任何行！");
	}
}
//删除职员
function BtnDelete() {
	var chkRow = $('#gridEmployee').datagrid("getChecked"); //选中行
	if (chkRow[0]) {
		$.messager.confirm('系统提示', '确定要删除该客户?', function (r) {
			if (r) {
				var guids = "";
				for (var i = 0; i < chkRow.length; i++) {
					guids += chkRow[i].ID + ",";
				}
				$.ajax({
					url: "/Employee/DeleteEmployee?r=" + Math.random(),
					data: { "guids": guids },
					success: function (data) {
						if (data) {
							//$.messager.alert("系统提示:", "删除成功！");
							$('#gridEmployee').datagrid("reload");
						}
						else {
							$.messager.alert("系统提示:", "删除失败！");
						}
					},
					error: function () {
						$.messager.alert("系统提示:", "删除失败！");
						return;
					}
				});
			}
		});
	}
	else {
		$.messager.alert("系统提示:", "未选中任何行！");
	}
}