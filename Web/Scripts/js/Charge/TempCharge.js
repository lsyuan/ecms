/*tempCharge view*/
$(function () {
	InitControl();
});
//初始化控件
function InitControl() {
	var buttons = [
        {
        	iconCls: 'icon-save',
        	minimizable: false,
        	text: "保存",
        	handler: function () {
        		if (!InputCheck("#frmAddTempCharge")) {//未通过验证
        			return;
        		}
        		//正确性验证
        		var needPay = parseFloat($("#txtGiveChange").val()); //应缴
        		var actFee = parseFloat($("#txtFee").val()); //实缴
        		if (needPay != actFee) {
        			$.messager.confirm('警告：', '实缴和应缴金额不一致，是否继续提交?', function (r) {
        				if (r) {
        					SaveTempCharge();
        				}
        			});
        		}
        		else {
        			SaveTempCharge();
        		}
        	}
        },
        {
        	iconCls: 'icon-cancel',
        	minimizable: false,
        	text: "取消",
        	handler: function () {
        		$("#dgAddTempCharge").dialog("close");
        	}
        }
    ];
	InitDialog("dgAddTempCharge", "临时收费：", 700, 400, null, buttons);
	//数据表格控件
	var toolbar = [{
		iconCls: 'icon-add',
		text: "收费",
		handler: function () { $("#dgAddTempCharge").dialog("open"); }
	}];
	var queryParams = {
		CustomerName: $('#CustomerNameSearch').val(),
		CreateTime: $("#CreateTimeSearch").val()
	};
	InitGridTable('TempChargeDataTable', "临时缴费记录表", '/Charge/Search', toolbar, queryParams);
	//button Event
	//查询
	$("#btnSearch").click(function () {
		RefreashTable()
	});
	$("#btnAddChargeItem").click(function () {
		AddChargeItem();
	});
	//取消收费项
	$(".btnCancel").live("click", function () {
		$(this).parent().parent().remove();
		CalculateFee();
	});
	//加载下拉列表
	$.ajax({
		url: "/Charge/ChargeItem?isRegular=False",
		data: null,
		success: function (data) {
			if (data != undefined) {
				var strHtml = "";
				$.each(data, function (i) {
					var strText = data[i].NAME + "(" + data[i].PRICE + "元)";
					strHtml += "<option value=" + data[i].ID + " price='" + data[i].PRICE + "'>" + strText + "</option>";
				});
				$("#chargeItem").html(strHtml);
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			$.messager.show("系统提示", "数据加载失败！");
		}
	});
	//数量文本框离开自动计算
	$(".validatebox-text").live("change", function () {
		CalculateFee();
	});
	$("select[name=chargeItem]").live("change", function () {
		CalculateFee();
	})
}
//
function RefreashTable() {
	$('#TempChargeDataTable').datagrid("reload", {
		CustomerName: $('#CustomerNameSearch').val(),
		CreateTime: $('#CreateTimeSearch').datebox('getValue')
	});
}

//保存临时收费项
function SaveTempCharge() {
	//数据提交保存
	var params = $("#dgAddTempCharge").JsonData();
	params.RealChargeMoney = $("#txtFee").numberbox('getValue');
	var chargeItems = GetChargeItemJsonData(); //缴费项
	$.ajax({
		url: "/Charge/TempChargeAdd",
		type: "POST",
		data: extend({}, [params, chargeItems]),
		dataType: 'json',
		success: function (data) {
			if (data.Success != undefined) {
				$.messager.alert("提示：", data.Message);
				$("#dgAddTempCharge").dialog("close");
				if (data.Success) {
					RefreashTable();
					//清空输入项目
					var $ChargeItems = $(".layoutTable").eq(1);
					$ChargeItems.find(".btnCancel").trigger("click");
					$ChargeItems.find("input[type=text]").val("");
					$("#Remark").val("");
				}
			}
		},
		error: function () {
			$.messager.alert("error：", "数据访问失败！");
			return;
		}
	});
}

//新增本次收费的收费项
var strModelHtml = "<tr>";
strModelHtml += "<td style='width:20%;text-align:right;'><label>收费项：</label></td>";
strModelHtml += "<td  style='width:30%'>";
strModelHtml += "<select name='chargeItem' />";
strModelHtml += "</td>";
strModelHtml += "<td style='width:18%;text-align:right;'><label style='text-align:right;'>缴费数量：</label></td>";
strModelHtml += "<td><input type='text' name='Money' vRequired='t'/>";
strModelHtml += "<img class='btnCancel' src='../../Content/images/EasyUIImages/cancel.png' alt='删除' title='取消收费项'/></td>";
strModelHtml += "</tr>";
function AddChargeItem() {
	var $ChargeItems = $(".layoutTable").eq(1);
	if ($ChargeItems.find("tr").length > 5) {
		$.messager.alert("提示：", "最多只能添加5项收费！");
		return;
	}
	$ChargeItems.append(strModelHtml);
	var strHtml = $("#chargeItem").html();
	$ChargeItems.find("select[name=chargeItem]:last").append(strHtml);
	$ChargeItems.find("input[name=Money]:last").numberbox({ min: 0, precision: 2 });
}
//获取缴费项json
function GetChargeItemJsonData() {
	var objString = "{";
	$tableDetails = $(".layoutTable").eq(1);
	var rowCount = $tableDetails.find("tr").length;
	for (var i = 1; i < rowCount; i++) {
		$objRow = $tableDetails.find("tr").eq(i);
		objString += "'[" + (i - 1) + "].ItemID':'" + $objRow.find("select").val() + "',";
		objString += "'[" + (i - 1) + "].Count':'" + $objRow.find("input[name=Money]").val() + "'";
		if (i != rowCount - 1) {
			objString += ",";
		}
	}
	objString += "}";
	return strToJson(objString);
}
//本次应缴费用结算
function CalculateFee() {
	var objRows = $(".layoutTable").eq(1).find("tr");
	var totalCount = 0;
	$.each(objRows, function (i) {
		if (i != 0) {
			var price = objRows.eq(i).find("select").find("option:selected").attr("price");
			var itemCount = parseFloat(objRows.eq(i).find(".validatebox-text").val());
			totalCount += price * (itemCount.toFixed(2));
		}
	});
	if (!isNaN(totalCount)) {
	    $("#txtGiveChange").val(totalCount.toFixed(2));
	}
}
