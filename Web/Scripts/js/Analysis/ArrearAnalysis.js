$(function () {
	InitCtrl();
});

var analysisChart;
function InitCtrl() {
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
		tooltip: {
			formatter: function () {
				return '<b>' + this.x + '</b>' +
                        '欠费总金额: ￥' + Highcharts.numberFormat(this.y, 2);
			}
		},
		series: [{
			name: 'Population',
			data: [],
			dataLabels: {
				enabled: true
			}
		}]
	});
}
//开始统计
function BeginStastics() {
	var areaList = [];
	$.post("/Analysis/FeeStastics", { areaID: "" }, function (jsonData) {
		var areaFeeList = [];
		for (var index = 0; index < jsonData.areaList.length; index++) {
			var feeCount = 0;
			$.each(jsonData.stasticsList, function (i) {
				if (jsonData.stasticsList[i].areaName == jsonData.areaList[index]) {
					feeCount += parseFloat(jsonData.stasticsList[i].fee);
				}
			});
			areaFeeList.push(feeCount);
		}
		analysisChart.series[0].setData(areaFeeList)
		analysisChart.xAxis[0].setCategories(jsonData.areaList);
	}, "json");
}