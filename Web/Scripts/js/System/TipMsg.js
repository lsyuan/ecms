$(function () {
	//大小自适应
	AutoHeight();
	$(document).resize(function () {
		AutoHeight();
	});
	//公告详细内容弹出框
	$("#dgMsgDetail").dialog({
		title: '公告详细：',
		width: 600,
		height: 400,
		resizable: true,
		closed: true,
		cache: false,
		modal: true
	});
	LoadMsg(); //公告
	LoadContract(); //协议到期提醒
	LoadArrearList(); //欠费列表
	$('.window-mask').width('98%');
	//event bind
	$("#MsgList").on("click", "li", function (e) {
		var targetObj = $(this).children(":first");
		ShowMsgContent(targetObj.attr('id'), targetObj.attr('title'));
	});
});

//高度自适应
function AutoHeight() {
	var height = $(document).height() / 2 - 23;
	$(".cornerPanel").height(height);
	$(".cornerPanel .content").height(height - 25);
}

//加载公告信息
function LoadMsg() {
	$.ajax({
		url: "/System/LoadSystemMsg",
		data: {},
		dataType: 'json',
		cache: false,
		success: function (data) {
			if (data != undefined) {
				var strHtml = "";
				$.each(data, function (i) {
					data[i].CREATEDATE = FormatTimeString(data[i].CREATEDATE);
					strHtml += template.render('MsgListModel', data[i]);
				});
				$("#MsgList").html(strHtml);
			}
		},
		error: function () {
			$.messager.alert("error：", "数据加载失败！");
			return null;
		}
	});
}
//加载协议过期信息
function LoadContract() {
	$.ajax({
		url: "/System/LoadTipContracts",
		data: {},
		dataType: 'json',
		cache: false,
		success: function (data) {
			if (data != undefined) {
				var strHtml = "";
				$.each(data, function (i) {
					data[i].ENDDATE = FormatTimeString(data[i].ENDDATE);
					strHtml += template.render('ContractListModel', data[i]);
				});
				$("#ContractList").html(strHtml);
			}
		}
	});
}
//加载欠费用户
function LoadArrearList() {
	$.post("/Analysis/GetArrearList", {}, function (jsonData) {
		var strHtml = "";
		$.each(jsonData, function (i) {
			if (i > 7) return;
			strHtml += template.render('ArrearListModel', jsonData[i]);
		});
		$("#ArrearList").html(strHtml);
	}, "json");
}
//显示公告内容
function ShowMsgContent(guid, title) {
	$.post("/Normal/GetMsgDetail", { msgID: guid }, function (data) {
		$("#msgTitle").html(title);
		$("#msgContent").html(data);
		$("#dgMsgDetail").dialog('open');
	});
}
function HidePanel() {
	$("#dgMsgDetail").dialog('close');
}