$(function () {
	InitCtrl();
});

var analysisChart;
function InitCtrl() {
	//初始化弹出窗
	$('#dgCustomerList').dialog({
		title: '欠费用户列表',
		width: 500,
		height: 300,
		closed: true,
		cache: false,
		modal: true
	});
	//统计图表
	analysisChart = new Highcharts.Chart({
		chart: {
			renderTo: 'FeeStastics',
			type: 'column'
		},
		title: {
			text: '<b>地区欠费统计</b>'
		},
		credits: { enabled: false },
		xAxis: {
			categories: [],
			title: { text: '地区' },
			labels: {
				rotation: 90,
				y: 20,
				style: {
					fontSize: '13px',
					fontFamily: 'Verdana, sans-serif'
				}
			}
		},
		yAxis: {
			min: 0,
			title: {
				text: '欠费金额(￥)'
			}
		},
		legend: {
			enabled: false
		},
		plotOptions: {
			column: {
				cursor: 'pointer',
				events: {
					click: function (event) {
						var title = "'" + event.point.category + "'的欠费用户：";
						$('#dgCustomerList').dialog("open").dialog("setTitle", title);
						LoadArrearCustomerList(event.point.x);
					}
				}
			}
		},
		tooltip: {
			formatter: function () {
				return '<b>' + this.x + '</b>' +
                        '欠费总金额: ￥' + Highcharts.numberFormat(this.y, 2) + "<br />点击查看欠费用户";
			}
		},
		series: [{
			name: '欠费金额',
			data: [],
			dataLabels: {
				enabled: true
			}
		}]
	});
}
//开始统计
var stasticsData;
function BeginStastics() {
	$.post("/Analysis/FeeStastics", { areaID: "" }, function (jsonData) {
		stasticsData = jsonData;
		var areaFeeList = [];
		var areaList = [];
		for (var index = 0; index < jsonData.areaList.length; index++) {
			var feeCount = 0;
			var areaObj = jsonData.areaList[index];
			areaList.push(areaObj.Name);
			$.each(jsonData.stasticsList, function (i) {
				if (jsonData.stasticsList[i].areaID == areaObj.ID ||
				areaObj.childrenIDs.indexOf(jsonData.stasticsList[i].areaID) != -1) {
					feeCount += parseFloat(jsonData.stasticsList[i].fee);
				}
			});
			areaFeeList.push(feeCount);
		}
		analysisChart.series[0].setData(areaFeeList)
		analysisChart.xAxis[0].setCategories(areaList);
	}, "json");
}
//加载欠费用户列表
function LoadArrearCustomerList(areaIndex) {
	var customerList = [];
	var areaObj = stasticsData.areaList[areaIndex];
	$.each(stasticsData.stasticsList, function (i) {
		if (stasticsData.stasticsList[i].areaID == areaObj.ID ||
				areaObj.childrenIDs.indexOf(stasticsData.stasticsList[i].areaID) != -1) {
			customerList.push(stasticsData.stasticsList[i]);
		}
	});
	$('#ArrearDataTable').datagrid('loadData', customerList);
}