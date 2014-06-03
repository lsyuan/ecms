/*角色管理页面*/
$(function () {
    InitControl();
});
//初始化操作
function InitControl() {
    //新增or编辑角色dialog
    var addButtonsDgAdd = [{
        iconCls: 'icon-save',
        text: "保存",
        handler: function () { AddOrEditGroup(); }
    }, {
        iconCls: 'icon-cancel',
        text: "取消",
        handler: function () { $("#dgAddOrEditGroup").dialog("close"); $("#dgAddOrEditGroup").JsonData(null); }
    }];
    InitDialog("dgAddOrEditGroup", "新增客户类型：", 300, 200, null, addButtonsDgAdd, function () {
        RefreashTable();
    });
    //初始化表格控件
    var toolbar = [{
        iconCls: 'icon-add',
        text: "新增角色",
        handler: function () {
            $("#dgAddOrEditGroup").attr("tag", "add");
            $("#dgAddOrEditGroup").dialog("open");
        }
    }, "-",
        {
            iconCls: 'icon-edit',
            text: "编辑",
            handler: function () {
                var chkRow = $('#GroupDataTable').datagrid("getChecked"); //选中行
                if (chkRow[0]) {
                    $("#dgAddOrEditGroup").attr("tag", "edit");
                    $("#ID").val(chkRow[0].ID);
                    $("#GroupName").val(chkRow[0].NAME);
                    $("#dgAddOrEditGroup").dialog("setTitle", "编辑角色:");
                    $("#dgAddOrEditGroup").dialog("open");
                }
            }
        }, "-",
        {
            iconCls: 'icon-cancel',
            text: "删除",
            handler: function () { DeleteGroup(); }
        }];
    var queryParams = {
        Name: $('#txtName').val()
    };
    var voteToolbar = [{
        iconCls: 'icon-save',
        text: "保存权限设置",
        handler: function () { SaveVoteSetting(); }
    }];
    InitGridTable('#GroupDataTable', "系统角色列表", '/System/SearchGroup', toolbar, queryParams, true);
    var groupGrid = $("#GroupDataTable").datagrid("options");
    groupGrid.onLoadSuccess = function (data) {
        RenderDetailBtn(data.rows);
    };
    groupGrid.onClickCell = function (rowIndex, field, value) {
        if (field == "DETAIL") {
            var gridData = $("#GroupDataTable").datagrid("getData").rows;
            LoadGroupVote(gridData[rowIndex].ID, gridData[rowIndex].NAME);
        }
    };
    //权限列表
    $('#VoteDataTable').datagrid({
        title: "权限列表",
        pageSize: 10,
        pagination: false,
        singleSelect: true,
        striped: true,
        fitColumns: true,
        url: "/Home/GetMenuGridJson",
        toolbar: voteToolbar,
        queryParams: null
    });
    var voteGrid = $("#VoteDataTable").datagrid("options");
    voteGrid.onClickCell = function (rowIndex, field, value) {
        if (field.indexOf("Vote") != -1) {
            $("#VoteDataTable").datagrid("selectRow", rowIndex);
        }
    };
    //btton event
    $("#btnSearch").click(function () {
        RefreashTable();
    });
}
//绘制查看详细按钮
function RenderDetailBtn(objData) {
    $.each(objData, function (i) {
        $('#GroupDataTable').datagrid('updateRow', {
            index: i,
            row: {
                ID: objData[i].ID,
                NAME: objData[i].NAME,
                DETAIL: "<a href='#' class='web_button' > 权限设置</a>"
            }
        });
    });
}
//重新加载数据
function RefreashTable() {
    $('#GroupDataTable').datagrid("reload", {
        Name: $('#txtName').val()
    });
    $("#dgAddOrEditGroup").JsonData(null);
}
//新增or更新
function AddOrEditGroup() {
    if (!InputCheck("frmAddOrEditGroup")) {
        return;
    }
    var strUrl = ($("#dgAddOrEditGroup").attr("tag") == "add") ? "/System/AddGroup" : "/System/ModifyGroup";
    var params = $("#dgAddOrEditGroup").JsonData();
    $.ajax({
        url: strUrl,
        data: params,
        dataType: 'json',
        success: function (result) {
            if (result.Success != undefined) {
                $.messager.alert("提示：", result.Message);
                $("#dgAddOrEditGroup").dialog("close");
                RefreashTable();
            }
        },
        error: function (hx) {
            $("#dgAddOrEditGroup").dialog("close");
            $.messager.alert("error：", "数据访问失败！");
            return;
        }
    });
}
//删除角色
function DeleteGroup() {
    var chkRow = $('#GroupDataTable').datagrid("getChecked"); //选中行
    if (chkRow[0]) {
        $.messager.confirm('系统提示', '确定要删除选中的角色?', function (r) {
            if (!r) return;
            var guid = chkRow[0].ID;
            $.ajax({
                url: "/System/DeleteGroup",
                data: { "guids": guid },
                dataType: 'json',
                success: function (data) {
                    if (data.Success != undefined) {
                        $("#dgAddOrEditGroup").dialog("close");
                        $.messager.alert("提示：", data.Message);
                        RefreashTable();
                    }
                },
                error: function () {
                    $.messager.alert("error：", "数据访问失败！");
                    return;
                }
            });
        });
    }
}
//加载角色权限列表
function LoadGroupVote(groupID, groupName) {
    $(".panel-title").eq(1).html("'" + groupName + "'的权限:");
    $.ajax({
        url: "/System/GetGroupVoteByID",
        data: { groupID: groupID },
        dataType: 'json',
        type: "post",
        success: function (data) {
            if (data != undefined) {
                $(".datagrid-btable:last").find("input[type=checkbox]").removeAttr("checked");
                var gridData = $("#VoteDataTable").datagrid("getData");
                $.each(data, function (i) {
                    var poupID = data[i].ID;
                    var resultArray = CheckIndex(parseInt(data[i].VOTETYPE));
                    var rowCheckCtrl = $("#" + poupID).parent().parent().parent().find("input[type=checkbox]");
                    $.each(resultArray, function (i) {
                        var checkValue = resultArray[i];
                        $.each(rowCheckCtrl, function (i) {
                            if ($(this).val() == checkValue) {
                                $(this).attr("checked", "checked");
                            }
                        });
                    });
                });
            }
        },
        error: function () {
            $.messager.alert("error：", "数据访问失败！");
            return;
        }
    });
}
//保存权限设置
function SaveVoteSetting() {
    var groupInfo = $("#GroupDataTable").datagrid("getSelected");
    if (groupInfo == null) {
        $.messager.alert("提示：", "未选中任何角色！");
        return;
    }
    var objString = "{";
    var index = 0;
    $(".datagrid-btable:last").find("tr").each(function (i) {
        var VoteCount = 0;
        if ($(this).find("input[type=checkbox]").length == 4) {
            $(this).find("input[type=checkbox]:checked").each(function (i) {
                VoteCount += parseInt($(this).val());
            });
            if (VoteCount > 0) {
                var poupID = $("#VoteDataTable").datagrid("getRows")[i].ID;
                objString += "'[" + index + "].PoupID':'" + poupID + "',";
                objString += "'[" + index + "].VoteType':'" + VoteCount + "',";
                index++;
            }
        }
    });
    if (objString != "{") {
        objString = objString.substr(0, objString.length - 1);
    }
    objString += "}";
    var parames = strToJson(objString);
    extend(parames, { groupID: groupInfo.ID }); //json合并
    $.ajax({
        url: "/System/SaveGroupVoteSetting",
        data: parames,
        dataType: 'json',
        type: "post",
        success: function (data) {
            if (data != undefined) {
                $.messager.alert("提示：", data.Message);
            }
        },
        error: function () {
            $.messager.alert("error：", "权限保存失败！");
            return;
        }
    });
}
//根据权限值，解析要选中的checkbox
function CheckIndex(voteType) {
    var returnArray = new Array();
    if (voteType == 1 || voteType == 2 || voteType == 4 || voteType == 8) {
        returnArray.push(voteType);
    }
    else {
        switch (voteType) {
            case 3:
                returnArray = [1, 2];
                break;
            case 5:
                returnArray = [1, 4];
                break;
            case 6:
                returnArray = [2, 4];
                break;
            case 9:
                returnArray = [1, 8];
                break;
            case 10:
                returnArray = [2, 8];
                break;
            case 12:
                returnArray = [4, 8];
                break;
            case 7:
                returnArray = [1, 2, 4];
                break;
            case 11:
                returnArray = [1, 2, 8];
                break;
            case 14:
                returnArray = [2, 4, 8];
                break;
            case 15:
                returnArray = [1, 2, 4, 8];
                break;
        }
    }
    return returnArray;
}