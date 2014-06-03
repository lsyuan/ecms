using System;
using System.Collections.Generic;
using System.Web;
using Ajax.Model;
using Web.Provider;

namespace Web.Common
{
	/// <summary>
	/// 登录身份模型
	/// </summary>
	public sealed class Ticket
	{
		/// <summary>
		/// 当前用户ID
		/// </summary>
		public string UserID { get; set; }
		/// <summary>
		/// 当前用户所属区域
		/// </summary>
		public string AreaID { get; set; }
		/// <summary>
		/// 当前用户用户名
		/// </summary>
		public string UserName { get; set; }
		/// <summary>
		/// 是否是超级管理员
		/// </summary>
		public bool IsAdmin { get; set; }
		/// <summary>
		/// 当前操作员的职工ID
		/// </summary>
		public string EmployeeID { get; set; }
		/// <summary>
		/// 当前操作员的职员姓名
		/// </summary>
		public string EmployeeName { get; set; }
		/// <summary>
		/// 当前用户所属的操作用户组
		/// </summary>
		public string GroupID { get; set; }
		/// <summary>
		/// 部门名称
		/// </summary>
		public string GroupName { get; set; }
		/// <summary>
		/// 当前操作员所属的部门ID
		/// </summary>
		public string DeptID { get; set; }
		/// <summary>
		/// 当前操作员所属的部门名称
		/// </summary>
		public string DeptName { get; set; }
		/// <summary>
		/// 权限集合
		/// </summary>
		public Dictionary<string, int> VoteDic { get; set; }
		/// <summary>
		/// 当前操作员
		/// </summary>
		//public Operator CurrentOperator { get; set; }

		public Ticket()
		{

		}
		/// <summary>
		/// 实例化当前登录用户的登录凭证
		/// </summary>
		/// <param name="userID">操作用户ID</param>
		/// <param name="areaID">区域ID</param>
		/// <param name="userName">操作用户登录名称</param> 
		/// <param name="employeeID">职员ID</param>
		/// <param name="employeeName">职员名称</param>
		/// <param name="groupID">权限组ID</param>
		/// <param name="deptID">部门ID</param>
		/// <param name="deptName">部门名称</param> 
		public Ticket(string userID, string areaID, string userName, bool isAdmin, string employeeID, string employeeName, string groupID, string deptID, string deptName, Operator opr)
		{
			UserID = userID;
			UserName = userName;
			AreaID = areaID;
			IsAdmin = isAdmin;
			//VoteList = voteList;
			EmployeeID = employeeID;
			employeeName = EmployeeName;
			GroupID = groupID;
			DeptID = deptID;
			DeptName = deptName;
			//this.CurrentOperator = opr;
		}
	}
	/// <summary>
	/// 登录身份验证
	/// </summary>
	public class MyTicket
	{
		static Ticket currentTicket = new Ticket();
		/// <summary>
		/// 当前登录用户身份验证对象
		/// </summary>
		public static Ticket CurrentTicket
		{
			get
			{
				if (HttpContext.Current.User.Identity.IsAuthenticated)
				{
					// The user is authenticated. Return the user from the forms auth ticket.
					return ((LoginPrincipal)(HttpContext.Current.User)).User;
				}
				else if (HttpContext.Current.Items.Contains("User"))
				{
					// The user is not authenticated, but has successfully logged in.
					return (Ticket)HttpContext.Current.Items["User"];
				}
				else
				{
					return null;
				}
				//// 如果userID为空，返回null，禁止访问
				//if (currentTicket != null && string.IsNullOrEmpty(currentTicket.UserID))
				//{
				//	return null;
				//}
				//return currentTicket;
			}
			set { currentTicket = value; }
		}
	}
}