var analysisChart;
$(function () {
    InitCtrl();
});
//
function InitCtrl() {
    //选择统计区域弹出框
    var buttons = [
        { iconCls: 'icon-ok',
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
    //searchbox文本区点击处理
    $(".searchbox-text").click(function () {
        $(this).next().eq(0).find(".searchbox-button").trigger("click")
    });
    //开始分析 event
    $("#btnAnalysis").click(function () {
        AreaChargeAnalysis();
    });
    //渲染直方图
    analysisChart = new Highcharts.Chart({
        chart: {
            renderTo: 'areaAnalysisContent',
            type: 'column',
            margin: [50, 50, 100, 80]
        },
        title: {
            text: '地区收费统计'
        },
        credits: { enabled: false },
        xAxis: {
            categories: [],
            title: { text: '地区(名称)' },
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
            data: [], //数据格式[{name:"路人甲",y:5}, {name:"路人乙",y:10}, {name:"酱油丁",y:20}],
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
//开始地区收费统计分析
function AreaChargeAnalysis() {
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

    var parentNode = null;
    if ($("#txtAreaID").val()!="") {
    	parentNode=$('#areaTreeGrid').treegrid("getParent", $("#txtAreaID").val());
    }
    var parentID = parentNode != null ? parentNode.Identifier : $("#txtAreaID").val();
    $.ajax({
        url: "/Analysis/AnalysisAreaCharge",
        data: { pID: parentID, beginDate: startTime, endDate: endTime },
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

//#region 打印
var LODOP;
function print() {
    CreateOneFormPage();
    LODOP.PREVIEW();
}
function CreateOneFormPage() {
    LODOP = getLodop(document.getElementById('LODOP'), document.getElementById('LODOP_EM'));
    LODOP.PRINT_INIT("区域收费统计");
    LODOP.SET_PRINT_STYLE("FontSize", 18);
    LODOP.SET_PRINT_STYLE("Bold", 1);
    LODOP.ADD_PRINT_TEXT(50, 231, 260, 39, "区域收费统计");
    LODOP.ADD_PRINT_HTM(88, 200, 350, 600, document.getElementById("areaAnalysisContent").innerHTML);
};
//#endregion