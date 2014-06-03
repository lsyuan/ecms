var oTable;
var selectData;
var currentPositon = -1;
var selectedNode;
var tag = "Add";

/* Get the rows which are currently selected */
function fnGetSelected(oTableLocal) {
	return $('tr.row_selected');
}
$(document).ready(function () {
	var filterString = $("#TB_OperatorName").val();
	$("#myDataTable tbody tr").live('click', function (e) {
		$('tr.row_selected').removeClass('row_selected');
		$(this).addClass('row_selected');
		selectData = oTable.fnGetData(this);
		currentPositon = oTable.fnGetPosition(this);
		selectedNode = oTable.fnGetNodes(this);
	});
	oTable = $('#myDataTable').dataTable({
		"bProcessing": true,
		"bLengthChange": true,
		"bDestroy": true,
		"bJQueryUI": true,
		"bAutoWidth": false,
		"sPaginationType": "full_numbers",
		"sAjaxSource": 'Customer/GetAllCustomer',
		"aoColumns": [
                    { "sName": "ID", "sTitle": "", "sWidth": "0", "bSearchable": false, "bSortable": false, "bVisible": false },
			        { "sName": "NAME" },
			        { "sName": "TYPE", "bSearchable": false },
			        { "sName": "STATUS", "bSearchable": false },
			        { "sName": "BEGINCHARGEDATE", "bSearchable": false },
			        { "sName": "CONTACTOR", "bSearchable": false },
			        { "sName": "ADDRESS", "bSearchable": false }
					],
		"fnServerParams": function (aoData) {
			aoData.push({ "name": "filter", "value": filterString });
			aoData.push({ "name": "status", "value": $("#SEL_STATUS").val() });
		}
	});
	Init();
});

function Init() {
	CustomerAutoFill("#TB_CustomerName");
	$.ajax({
		url: "/System/GetChargeTypeList",
		success: function (data) {
			SetOptionValue("#SEL_CUSTOMER_TYPE", data, true);
		}
	});
	InitArea('mytree2', 'regionID', 'div');
	InitArea('mytree1', 'AreaID', 'div1');
}

function Query() {
	var filterString = $("#TB_CustomerName").val();
	oTable = $('#myDataTable').dataTable({
		"bProcessing": true,
		"bLengthChange": true,
		"bDestroy": true,
		"bJQueryUI": true,
		"bAutoWidth": false,
		"sPaginationType": "full_numbers",
		"sAjaxSource": 'GetAllCustomer',
		"aoColumns": [
                    { "sName": "ID", "sTitle": "", "sWidth": "0", "bSearchable": false, "bSortable": false, "bVisible": false },
			        { "sName": "NAME" },
			        { "sName": "TYPE", "bSearchable": false },
			        { "sName": "STATUS", "bSearchable": false },
			        { "sName": "BEGINCHARGEDATE", "bSearchable": false },
			        { "sName": "CONTACTOR", "bSearchable": false },
			        { "sName": "ADDRESS", "bSearchable": false }
					],
		"fnServerParams": function (aoData) {
			aoData.push({ "name": "filter", "value": filterString });
			aoData.push({ "name": "status", "value": $("#SEL_STATUS").val() });
		}
	});
}

/************************************************************************/
/*删除操作员                                                             */
/************************************************************************/
$('#BT_DeleteOperator').live('click', function () {
	var id = selectData[0];
	$.ajax({
		url: "/Customer/CustomerDelete",
		data: "id=" + id,
		success: function (data) {
			if (data) {
				var anSelected = fnGetSelected();
				$(anSelected).remove();
				oTable.fnDeleteRow(currentPositon);
				ShowMessage("删除成功！");
			}
			else {
				ShowMessage("删除失败！");
				return;
			}
		},
		error: function () {
			ShowMessage("删除失败！");
			return;
		}
	});
	//	$(anSelected).remove();
});

/************************************************************************/
/*增加操作员                                                             */
/************************************************************************/
$('#BT_AddOperator').live('click', function () {
	$("#formAddNewRow").dialog({
		width: 600,
		modal: true
	});
	Clear();
});

/************************************************************************/
/*编辑                                                                 
/************************************************************************/
$('#BT_EditOperator').live('click', function () {
	if (selectData != undefined) {
		tag = "Modify"; //	 置修改状态
		$("#formAddNewRow").dialog({
			width: 330,
			modal: true
		});
		/// 获取ID
		var id = selectData[0];
		/// 加载操作员数据
		$.ajax({
			url: "/Customer/GetSingelCustomer",
			data: "ID=" + id,
			success: function (data) {
				$("#formAddNewRow").JsonData(data);
			},
			error: function () {
				ShowMessage("获取操作员信息失败！");
				return;
			}
		});
	}
});

/************************************************************************/
/* 禁用操作员                                                                     */
/************************************************************************/

$("#BT_DisableOperator").live('click', function () {
	if (selectData != undefined) {
		/// 获取ID
		var id = selectData[0];
		$.ajax({
			url: "/System/OperatorDisable",
			data: "ID=" + id,
			success: function (data) {
				oTable.fnUpdate(data, currentPositon);
			},
			error: function () {
				ShowMessage("禁用失败！");
				return;
			}
		});
	}
});

function Submit() {
	// 新增操作员
	if (tag == "Add") {
		//		if (!MyValidate('formAddNewRow')) {
		//			ShowMessage("输入内容不完整");
		//		}
		var result = $("#formAddNewRow").JsonData();
		$.ajax({
			url: "/Customer/AddCustomer",
			data: result,
			dataType: 'json',
			success: function (data) {
				Cancel();
				ShowMessage('新增成功');
				oTable.fnAddData(data, currentPositon);
				Clear();
			},
			error: function () {
				ShowMessage("新增失败！");
				return;
			}
		});
	}
	// 修改操作员信息
	else {
		var result = $("#formAddNewRow").JsonData();
		$.ajax({
			url: "/Customer/CustomerModify",
			data: result,
			dataType: 'json',
			success: function (data) {
				Cancel();
				ShowMessage('保存成功');
				oTable.fnUpdate(data, currentPositon);
				Clear();
			},
			error: function () {
				ShowMessage("保存失败！");
				return;
			}
		});
	}
}
function Cancel() {
	$("#formAddNewRow").dialog('close');
}
function Clear() {
	$("#formAddNewRow").JsonData(null);

	selectedNode = undefined;
	selectData = undefined;
	currentPositon = -1;
	tag = "Add";
}