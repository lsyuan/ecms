$(function () {
    $.ajax({
        url: "/System/GetAllSysParameter",
        data: null,
        dataType: 'json',
        cache: false,
        success: function (data) {
            $.each(data, function (i) {
                $(".layoutTable tr").eq(i).JsonData(data[i]);
            });
        },
        error: function (result) {
            $.messager.alert('提示：', result.responseText, 'error');
        }
    });
});

function Submit() {
    var datas = new Array();
    var objString = "{";
    var rowsObj = $("[dd]");
    $.each(rowsObj,function (i) {
        var aa = rowsObj.eq(i).JsonData();
        objString += "'[" + i + "].ID':'" + aa.ID + "',";
        objString += "'[" + i + "].Name':'" + aa.Name + "',";
        objString += "'[" + i + "].ValidateGroup':'" + aa.ValidateGroup + "',";
        objString += "'[" + i + "].Value':'" + aa.Value + "',";
    });
    objString = objString.substr(0, objString.length - 1);
    objString += "}";
    var params = strToJson(objString);
    $.ajax({
        url: "/System/SaveAllSysParameter",
        data: params,
        success: function (data) {
            if (data.Success != undefined) {
                $.messager.alert('提示：', data.Message, 'info');
            }
        },
        error: function (result) {
            $.messager.alert('提示：', "数据访问失败", 'error');
        }
    });
}

function checkInput(obj) {
    if (event.keyCode == 48 || event.keyCode == 96) {
        event.returnValue = false;
        $(obj).val("0");
    }
    if (event.keyCode == 49 || event.keyCode == 97) {
        event.returnValue = false;
        $(obj).val("1");
    }
    if (event.keyCode != 48 && event.keyCode != 49 && event.keyCode != 96 && event.keyCode != 97) {
        event.returnValue = false;
    }
}