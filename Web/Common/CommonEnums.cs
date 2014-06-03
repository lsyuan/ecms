﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
	/// <summary>
	/// 系统菜单枚举
	/// </summary>
	/// <remarks>
	/// eg:AddNewCustomer -- 新增客户 -- 3002 菜单节点Value
	/// </remarks>
	public enum PoupEnums
	{
		/// <summary>
		/// 新增客户
		/// </summary>
		新增客户 = 3002,
		账单查询 = 5004,
		临时收费 = 2006,
		职工收费统计 = 4006,
		客户信息查询 = 3003,
		账号管理 = 9006,
		协议管理 = 2007,
		年终结算 = 5002,
		客户审批 = 3004,
		法院执行 = 6003,
		区域收费统计 = 4005,
		区域设置 = 9007,
		公告通知 = 1005,
		计费单位维护 = 9010,
		客户类型管理 = 9004,
		收费项分类管理 = 9009,
		参数设置 = 9005,
		收费审批 = 2005,
		欠费提醒 = 1007,
		报表分析 = 4002,
		以物顶账 = 2004,
		收费项目管理 = 9003,
		综合查询 = 4003,
		销账申请 = 6002,
		子类管理 = 6004,
		期初欠费 = 5003,
		密码修改 = 9008,
		欠费统计 = 4004,
		客户管理 = 3001,
		统计分析 = 4001,
		日常管理 = 1001,
		新发公告 = 1002,
		示例页面 = 1003,
		内部管理 = 7001,
		职员管理 = 7002,
        部门管理 = 7003,
        登陆日志管理 = 7004,
		系统设置 = 9001,
		菜单管理 = 9002,
		业务办理 = 2001,
		账务管理 = 5001,
		辅助功能 = 6001,
		其他缴费 = 6005,
		客户缴费 = 2003,
		主页 = 1000,
		角色管理 = 9011
	}

	/// <summary>
	/// 数据访问权限枚举
	/// </summary>
	[Flags] 
	public enum AccessEnums
	{ 
		Add = 1,
		Delete = 2,
		Read = 4,
		Update = 8
	}

}