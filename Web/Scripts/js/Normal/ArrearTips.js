$(document).ready(function () {
	//选择统计区域弹出框
	var buttons = [
        {
        	iconCls: 'icon-ok',
        	minimizable: false,
        	text: "确定",
        	handler: function () {
        		var chkObjs = $("#areaTreeGrid").treegrid("getSelected");
        		if (chkObjs != null && chkObjs.Identifier != "") {
        			$("#txtAreaID").val(chkObjs.Identifier);
        			$("#txtAreaName").searchbox('setValue', chkObjs.Area_Name);
        		}
        		$("#dArea").dialog("close");
        	}
        }
    ];
	InitDialog("dArea", "选择统计区域：", 650, 400, null, buttons);
	//所属地区
	$('#txtAreaName').searchbox({
		searcher: function (value, name) {
			if (value != "") {
				$("#areaTreeGrid").treegrid("select", value);
			}
			$("#dArea").dialog("open");
		},
		validtype: "selectValueRequired",
		prompt: ''
	});

	var toolbar = [{
		iconCls: 'icon-add',
		text: "打印催收表",
		handler: function () {
			ArrearTipPrint();
		}
	}];
	var queryParams = { areaID: $("#txtAreaID").val(), time: $("#arrearTime :selected").val() };
	InitGridTable('#ArrearDataTable', "欠费用户列表", '/Analysis/GetArrearList', toolbar, queryParams, false);
});
function GoSearch() {
	var areaID = $("#txtAreaName").searchbox("getValue") ? $("#txtAreaID").val() : "";
	var queryParams = { areaID: areaID, time: $("#arrearTime :selected").val() };
	$("#ArrearDataTable").datagrid('reload', queryParams);
}
//打印催缴通知单
function ArrearTipPrint() {
	var chkRow = $('#ArrearDataTable').datagrid("getChecked"); //选中行
	if (chkRow.length == 0) {
		$.messager.alert("系统提示:", "请选择要操作的数据！");
	}
	else {
		var customerArray = [];
		$.each(chkRow, function (i) {
			customerArray.push(chkRow[i].ID);
		});
		$.post("/Print/GetArrearInfo", { customerIDs: customerArray.join(',') }, function (data) {
			if (data != null || data.length < 1) {
				var LODOP = getLodop(document.getElementById('LODOP'), document.getElementById('LODOP_EM'));
				LODOP.PRINT_INITA(10, 10, 754, 453, "催缴通知单打印");
				$.each(data, function (i) {
					var strHtml = template.render('ArrearTipModel', data[i]);
					LODOP.ADD_PRINT_HTM(30, 70, 600, 500, strHtml);
					if (i != data.length - 1) {
						LODOP.NewPage();
					}
				});
				LODOP.PREVIEW();
			}
			else {
				$.messager.alert("系统提示:", "欠费信息加载失败！");
			}
		});
	}
}
  
   