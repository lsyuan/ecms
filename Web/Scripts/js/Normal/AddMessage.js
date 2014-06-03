/*add message view*/
$(function () {
	InitControl();
	//页面提交
	$("#btnSubmit").click(function () {
		if (InputCheck("frmAddMsg")) {
			var msgData = $("#frmAddMsg").JsonData();
			msgData.Content = editor.html();
			$.ajax({
				url: "/Normal/AddMessageNew",
				data: msgData,
				dataType: 'json',
				success: function (data) {
					$("#frmAddMsg").JsonData(null);
					editor.html('');
					$("#txtAccept").val('');
					if (data != undefined) {
						$.messager.alert("提示：", data.Message);
					}
				},
				error: function (dd) {
					$.messager.alert("error：", "发布公告失败！");
					return null;
				}
			});
		}
	});
});
//初始化控件
function InitControl() {
    //打开接收人对话框
    $("#btnMsgTo").click(function () {
        $("#dgAcceptUser").dialog("open");
    });
    //公告接收人对话框
    $("#dgAcceptUser").dialog({
        title: '选择公告接收人：',
        width: 400,
        height: 200,
        closed: true,
        cache: false,
        modal: true,
        buttons: [{
            iconCls: "icon-ok",
            text: '完成',
            handler: function () { SaveChkAccept(); }
        }]
    });
    var values = "" //默认全部人
    $("input[name=chkAccept]").each(function () {
        values += $(this).val() + ",";
    });
    $("#txtAcceptIDs").val(values);
}
//确定选中的接收人
function SaveChkAccept() {
    var names = "";
    var values = "";
    $("input[name=chkAccept]:checked").each(function () {
        names += $(this).parent().text() + "; ";
        values += $(this).val() + ",";
    });
    $("#txtAccept").val(names.trim()); 
    $("#txtAcceptIDs").val(values);
    $("#dgAcceptUser").dialog("close");
}
