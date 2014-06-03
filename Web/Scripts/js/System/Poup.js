var currentPoupID = "";
var flag = 0;
var PoupName = "";
$(function () {
    ShowType();
    LoadTree();
});
//加载tree
function LoadTree()
{
    $("#myDataTable").treeTable({
        checkbox: true,
        treeColumn: 0,
        key: 'ID',
        parentKey: 'PID',
        onSelected: function (node) {
            PoupName = node[0].innerText;
            currentPoupID = $("#myDataTable").ttKey();
        },
        bind: {
            templateID: '#CompanyList',
            url: "/Poup/GetPoupList",
            ajaxPara: null,
            ajaxLoad: true,
            ajaxError: function (e) {
                ErrorInfo(e);
            },
            ajaxSuccess: function () {
                GetGroupVote();//加载完成事件
            }
        }
    });
}

function AddOrModify(AOM) {

	flag = AOM;
	if (flag == 1) {
		$.ajax({
			url: "/Poup/GetPoup",
			data: "id=" + currentPoupID.replace('\'', ''),
			success: function (data) {
				$("#formAddNewRow").JsonData(data);
			},
			error: function () {
				ShowMessage("获取菜单信息失败！");
				return;
			}
		});
	}
	// 新增
	else {
		$("#formAddNewRow").JsonData(null);
		// 获取当前节点的PID，如果不为""则不能添加子菜单，当前只支持2级菜单
		var PID = $("#myDataTable").ttSelectedItem().ttParentKey();
		if (PID != "") {
			return;
		}
	}
	$("#formAddNewRow").dialog({

		height: 180,
		modal: true
	});
}
function AddNewPoup() {

	/// add new dept
	$("#formAddNewRow").dialog('close');
	if (flag != 1) {
		if (currentPoupID == "") {
			ShowMessage("请先选择菜单");
			return;
		}
		var poupName = $("#TB_PoupName").val();
		if (poupName == "" || poupName == null) {
			ShowMessage("请输入菜单名称");
			return;
		}
		$.ajax({
			url: "/Poup/AddPoup",
			data: "PID=" + currentPoupID + "&Name=" + poupName + "&Path=" + $("#TB_Path").val(),
			success: function (data) {
				ShowMessage("添加成功！");
				$("#myDataTable").ttSelectedItem().ttAddNodes(data);
			},
			error: function (jqXHR, textStatus, errorThrown) {
				ShowMessage("添加失败！");
			}
		});
	}
	///update dept message
	else {
		$.ajax({
			url: "/Poup/ModifyPoup",
			type: "GET",
			scriptCharset: "jsonp",
			data: "ID=" + $("#HFD_POUP_ID").val() + "&Name=" + $("#TB_PoupName").val() + "&Path=" + $("#TB_Path").val(),
			success: function (data) {
				ShowMessage("更新成功！");
				$("#myDataTable").ttSelectedItem().ttModify(data);
			},
			error: function (jqXHR, textStatus, errorThrown) {
				ShowMessage("更新失败！");
			}
		});
	}
}
function Cancel() {
	$("#formAddNewRow").dialog('close');
}
function Delete() {
	var PName = $("#myDataTable");
	if (currentPoupID == "") {
		return;
	}
	$("#msg").html("警告：删除 <strong>" + PoupName + "</strong> 将删除所有的子菜单，确定删除吗？");
	$("#dialog-confirm").dialog({
		resizable: false,
		height: 140,
		modal: true,
		buttons: {
			"确定": function () {
				$.ajax({
					url: "/Poup/DeletePoup",
					data: "ID=" + currentPoupID.replace('\'', ''),
					success: function (data) {
						ShowMessage("删除成功！");
						currentPoupID = "";
						$("#myDataTable").ttSelectedItem().ttRemove();
					},
					error: function () {
						ShowMessage("删除失败！");
					}
				});
			},
			"取消": function () {
				$(this).dialog("close");
			}
		}
	});
}
function Modify() {
	$.ajax({
		url: "/Dept/Delete",
		data: "ID=" + currentPoupID.replace('\'', ''),
		success: function (data) {
			ShowMessage("更新成功！");
			$("#myDataTable").ttSelectedItem().ttModify(data);
		},
		error: function () {
			ShowMessage("删除失败！");
		}
	});
}
//处理显示方式
function ShowType() {
    if (location.href.indexOf("?") == -1) {//隐藏选择框
        $(".chkVote").hide();
    }
    else {
        $(".Bar").hide();
        treeNodeCheck();
    }
}

//treeNode的父节点check处理
function treeNodeCheck() {
    $(".chkVote").live("click", function () {
        var $parentChk = $(this);
        var $chkObj = $(this).parent().parent();
        if ($chkObj.attr("pid") == "") {//当前为父节点 
            var pId = $(this).val();
            $("#myDataTable>tbody>tr[pid=" + pId + "]").each(function () {
                if ($parentChk.attr("checked")) {
                    $(this).find("input[type=checkbox]").attr("checked", "checked");
                }
                else {
                    $(this).find("input[type=checkbox]").removeAttr("checked");
                }
            });
        }
    });
}
//js获取url参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

//获取url参数中指定角色的权限(指定角色权限时使用)
function GetGroupVote() {
    var GroupID = getQueryString("GroupID"); 
    if(GroupID!=null||GroupID!=""){
        var url = "/System/GetAllVoteByGroupID";
        var parames = { "GroupID": GroupID };
        $.post(url, parames, function (data) {
            if (data != null) {
                for (var i = 0; i < data.poupID.length; i++) {
                    $("input[value=" + data.poupID[i] + "]").attr("checked", "checked");
                }
            }
        }, "json");
    }

}

