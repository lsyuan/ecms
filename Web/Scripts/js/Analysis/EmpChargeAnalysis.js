var analysisChart; //直方图表对象
$(function () {
    InitCtrl();
});
//
function InitCtrl() {
    //开始分析 event
    $("#btnAnalysis").click(function () {
        Analysis();
    });
    //渲染直方图
    analysisChart = new Highcharts.Chart({
        chart: {
            renderTo: 'empanalysisContent',
            type: 'column',
            margin: [50, 50, 100, 80]
        },
        title: {
            text: '职工收费统计'
        },
        credits: { enabled: false },
        xAxis: {
            categories: [],
            title: { text: '职工(姓名)' },
            labels: {
                rotation: 90,
                y:20,
                style: {
                    fontSize: '13px',
                    fontFamily: 'Verdana, sans-serif'
                }
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: '收费金额(￥)'
            }
        },
        legend: {
            enabled: false
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.point.name + '</b>' +
                        '收费总金额: ' + Highcharts.numberFormat(this.y, 2) + '￥';
            }
        },
        series: [{
            name: 'Population',
            data: [],//数据格式[{name:"路人甲",y:5}, {name:"路人乙",y:10}, {name:"酱油丁",y:20}],
            dataLabels: {
                enabled: true,
                rotation: -90,
                color: '#FFFFFF',
                align: 'right',
                x: 4,
                y: 10,
                style: {
                    fontSize: '13px',
                    fontFamily: 'Verdana, sans-serif'
                }
            }
        }]
    });
}
//开始分析处理
function Analysis() { 
    var startTime = $("#beginDate").datebox("getValue");
    var endTime = $("#endDate").datebox("getValue");
    if (startTime == "" || endTime == "") {
        $.messager.alert('提示:', '请指定统计时间段!'); 
        return;
    }
    if (startTime > endTime) {
        $.messager.alert('提示:', '起始时间不能大于终止时间!');   
        return;
    }
    $.messager.progress({ text: '数据统计中。。。' }); //等待进度条
    analysisChart.setTitle({ text: '职工收费统计(<b>' + startTime + '</b><b>至' + endTime + '</b>)' });
    $.ajax({
        url: "/Analysis/AnalysisEmpCharge",
        data: { beginDate: startTime, endDate: endTime },
        cache: false,
        success: function (data) {
            $.messager.progress("close"); //关闭进度条
            if (data != null) {
                analysisChart.series[0].setData(data);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $.messager.show("系统提示", "数据加载失败！");
        }
    });
}
