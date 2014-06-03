$(document).ready(function () {
    $("#BT_Commit").live('click', function () {
        if ($("#TB_Origin").val() == "" || $("#TB_NewPwd").val() == "" || $("#TB_NewConfirm").val() == "" || $("#TB_Origin").val() == undefined || $("#TB_NewPwd").val() == undefined || $("#TB_NewConfirm").val() == undefined) {
            $.messager.alert('提示：', "请输入完整内容");
            Clear();
            return;
        }
        var oldPwd = $("#TB_Origin").val();
        var newPwd = $("#TB_NewPwd").val();
        var newPwdConfirm = $("#TB_NewConfirm").val();
        if (newPwd != newPwdConfirm) {
            $.messager.alert('提示：', "两次输入的密码必须相同！");
            Clear();
            return;
        }
        $.ajax({
            url: "/System/ChangePwd",
            data: "old=" + oldPwd + "&newPwd=" + newPwd,
            success: function (data) {
                if (data) {
                    $.messager.alert('提示：', "密码修改成功");
                }
                else {
                    $.messager.alert('提示：', "密码修改失败");
                }
                Clear();
            },
            error: function () {
                $.messager.alert('提示：', "密码修改失败。");
                Clear();
            }
        });
    });
    $("#BT_Clear").live('click', function () {
        Clear();
    });

});
function Clear() {
	$("input[type=password]").val("");
}
function ShowMsg() {
	$("#DIV_Msg").dialog({
		modal: true,
		buttons: {
			"确定": function () {
				$(this).dialog("close");
			}
		}
	});
}