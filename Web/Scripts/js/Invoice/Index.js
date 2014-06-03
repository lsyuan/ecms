/*view of Invoice Mgr*/
$(function () {
    InitCtrl();
});

function InitCtrl() {
    //初始化新增弹出窗
    var buttons = [{ iconCls: 'icon-save',
        text: "保存",
        handler: function () { SaveAddOrEdit(); }
    }];
    InitDialog("dgAddorEdit", "新增票据：", 350, 200, null, buttons);
    //grid
    var toolbar = [{
        iconCls: 'icon-add',
        text: "票据登记",
        handler: function () {
            $("#ID").val("");
            OpenDialog();
        }
    }, "-",
        {
            iconCls: 'icon-edit',
            text: "票据编辑",
            handler: function () {
                var chkRow = $('#gridInvoice').datagrid("getChecked")[0]; //选中行
                if (chkRow) {
                    $("#dgAddorEdit").JsonData(chkRow);
                    if (chkRow.CurrentCode == "") {
                        OpenDialog();
                    } else {
                        $.messager.alert('提示：', "不能修改已经开始使用的发票！");
                    }
                }
            }
        }];
    InitGridTable('gridInvoice', "票据登记管理", '/Invoice/InvoiceSearch', toolbar, { InvoiceType: $("#dpInvoiceTypeSearch").val() }, false);
    //加载票据分类
    $.post("/Invoice/InvoiceAllTypeJson", {}, function (data) {
        if (data != null) {
            $.each(data, function (i) {
                var strHtml = "<option value=" + data[i].id + ">" + data[i].text + "</option>";
                $("#dpInvoiceTypeSearch").append(strHtml);
                $("#dpInvoiceType").append(strHtml);
            });
        }
    });
    //查询按钮事件
    $("#btnSearch").click(function () {
        $("#gridInvoice").datagrid("reload", { InvoiceType: $("#dpInvoiceTypeSearch").val() });
    });
}
//打开对话框
function OpenDialog() {
    if ($("#ID").val() == "") {
        $("#dgAddorEdit").dialog("setTitle", "新增票据:");
    }
    else {
        $("#dgAddorEdit").dialog("setTitle", "编辑票据:");
    }
    $("#dgAddorEdit").dialog("open");
}
//新注册or更新票据
function SaveAddOrEdit() {
    if (!InputCheck("dgAddorEdit")) {
        return;
    }
    if ($("#dpInvoiceType").val() == null) {
        $.messager.alert('提示：', "票据类型不能为空！");
        return;
    }
    var strUrl = "/Invoice/AddInvoice"
    if ($("#ID").val() != "") {
        strUrl = "/Invoice/ModifyInvoice";
    }
    var parames = $("#dgAddorEdit").JsonData();
    $.ajax({
        url: strUrl,
        data: parames,
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data != null) {
                $.messager.alert('提示：', data.Message);
                if (data.Success) {
                    $("#gridInvoice").datagrid("reload");
                }
            }
            $("#dgAddorEdit").dialog("close");
        },
        error: function () {
            $.messager.alert('提示：', "数据访问失败！");
        }
    });
}