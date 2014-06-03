//Main
$(function () {
    InitMenu();
    $("#frmMain").attr("src", "/System/TipMsg");
});
//初始化菜单
function InitMenu() {
    var strUrl = "/Home/GetMenuJson";
    $.ajax({
        type: "post",
        url: strUrl,
        dataType: "json",
        error: function () { alert("数据加载失败。"); },
        success: function (menuObj) {
            CreateMenu(menuObj);
        }
    });
}
//创建菜单
function CreateMenu(menuObj) {
    for (var i = 0; i < menuObj.menus.length; i++) {
        var menu = menuObj.menus[i];
        //顶层菜单
        if (menu.pid == "") {
            var strHtml = "";
            for (var i_ = 0; i_ < menuObj.menus.length; i_++) {
                if (menu.menuId == menuObj.menus[i_].pid) {
                    strHtml += "<a class='childMenu1' href='" + menuObj.menus[i_].url + "' target='frmMain'><div class='childMenu'>" + menuObj.menus[i_].text + "</div></a>";
                }
            }
            $("#accordionDiv").accordion('add', {
                title: menu.text,
                content: strHtml,
                selected: false
            });
        }
    }
    //IE6下不启用动画效果
    if ($.browser.msie && $.browser.version == "6.0") {
        $('#accordionDiv').accordion({ animate: false });
    }
}
//退出登录
function loginOut() {
    location.href = "/Home/LoginOut";
}