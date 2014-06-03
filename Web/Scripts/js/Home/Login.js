$(document).ready(function () {
    RefreshValidateCode();
    $("#TB_UserName").focus();
    //登录页面回车键
    $("input[type=text]").keyup(function (e) {
        if (e.keyCode == 13) {
            GoLogin();
        }
    });
    $("input[type=password]").keyup(function (e) {
        if (e.keyCode == 13) {
            GoLogin();
        }
    });

    $(document).keyup(function (e) {
        if (e.keyCode == 13) {
            $("input").blur();
            GoLogin();
        }
    });
    //登录
    $("#btnLogin").click(function () {
        GoLogin();
    });
    //登录角色选择对话框
    $("#dgSelectLoginGroup").dialog({
        title: "选择一个登录角色",
        width: 300,
        height: 200,
        closable: false,
        closed: true,
        cache: false,
        modal: true
    });
    //登录用户选择
    $("#userSelect a").live("click", function () {
        var GroupID = $(this).attr("GID");
        //$.ajax({
        //    url: "/Home/SetLoginGroup",
        //    type: "POST",
        //    data: { "groupID": GroupID },
        //    success: function (data) { }
        //});
        location.href = "/Home/SetLoginGroup?groupID=" + GroupID;
    });
});
//重新获取验证码
function RefreshValidateCode(obj) {
    $("#valiCode").attr("src", "/Home/GetValidateCode?" + Date());
}
//登录操作处理
function GoLogin() {
    var userName = $("#TB_UserName").val();
    var pwd = $("#TB_Pwd").val();
    var validateCode = $("#TB_ValidateCode").val();
    if (!InputCheck("loginForm")) {
        return;
    }
    if (userName.indexOf('\'') > 0 || pwd.indexOf('\'') > 0 || validateCode.indexOf('\'') > 0) {
        $.messager.alert('提示：', "包含特殊字符,请确认!");
        return;
    }
    $.ajax({
        url: "/Home/GoLogin",
        type: "POST",
        data: { "userName": userName, "pwd": pwd, "validateCode": validateCode },
        success: function (data) {
            if (data.Success != undefined) {
                if (data.Success) {
                    location.href = data.Url;
                }
                else {
                    $.messager.alert('提示：', data.Message);
                    $("#valiCode").attr("src", "/Home/GetValidateCode?" + Date());
                }
            }
            else {//登录角色选择
                var strHtml = "";
                $.each(data, function (i) {
                    strHtml += '<li><a href="javascript:void(0)" GID="' + data[i].GroupID + '">' + data[i].GroupName + '</a></li>';
                });
                $("#userSelect").html(strHtml);
                $("#dgSelectLoginGroup").dialog("open");
            }
        },
        error: function () {
            $.messager.alert('Error：', "数据访问失败!");
            return;
        }
    });
}