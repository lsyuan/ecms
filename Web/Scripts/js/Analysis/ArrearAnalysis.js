$(function () {
    InitCtrl();
});

function InitCtrl() {
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
    //searchbox文本区点击处理
    $(".searchbox-text").click(function () {
        $(this).next().eq(0).find(".searchbox-button").trigger("click")
    });
    var toolbar = [{
        iconCls: 'icon-add',
        text: "打印催收表",
        handler: function () {
            print();
        }
    }];

    var queryParams = { areaID: $("#txtAreaID").val() };
    InitGridTable('#ArrearDataTable', "", '/Analysis/GetArrearList', toolbar, queryParams, true);

}

function Query() {
    var queryParams = { areaID: $("#txtAreaID").val() };
    $("#ArrearDataTable").datagrid('reload', queryParams);
}

//#region 打印
var LODOP;
function print() {
    CreateOneFormPage();
}
function CreateOneFormPage() {
    var a = $("#ArrearDataTable").datagrid("getSelected");
    if (a == null && a == undefined) {
        return;
    }
    var data = new Object();
    data.Name = a.NAME;
    data.BeginChargeDate = a.BEGINCHARGEDATE;
    $("#Div_SingleArrearMsg").JsonData(data);
    LODOP = getLodop(document.getElementById('LODOP'), document.getElementById('LODOP_EM'));
    LODOP.PRINT_INIT("欠费列表");
    LODOP.SET_PRINT_STYLE("FontSize", 18);
    LODOP.SET_PRINT_STYLE("Bold", 1);
    LODOP.SET_PRINT_STYLE("Alignment", 2);
    LODOP.SET_PRINT_PAGESIZE(1, '210mm', '297mm', 'A4');
    //LODOP.ADD_PRINT_TEXT(50, 231, 260, 39, "欠费列表");
    LODOP.ADD_PRINT_HTM(50, 50, 700, 800, document.getElementById("Div_SingleArrearMsg").innerHTML);
    LODOP.PREVIEW();
};
//#endregion打印

