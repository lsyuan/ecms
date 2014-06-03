using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
	/// <summary>
	/// 操作用户
	/// </summary>
	public partial class OperatorRule
	{

		private readonly Ajax.DAL.OperatorDAL dal = new Ajax.DAL.OperatorDAL();
		/// <summary>
		/// 构造函数
		/// </summary>
		public OperatorRule()
		{

		}
		#region  Method
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			return dal.Exists(ID);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		/// <param name="model">新增账号实体类</param> 
		public void Add(Ajax.Model.Operator model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.Operator model)
		{
			return dal.Update(model);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(string ID)
		{
			return dal.Delete(ID);
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Ajax.Model.Operator GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.Operator GetModelByCache(string ID)
		{

			string CacheKey = "OperatorModel-" + ID;
			object objModel = Ajax.Common.DataCache.GetCache(CacheKey);
			if (objModel == null)
			{
				try
				{
					objModel = dal.GetModel(ID);
					if (objModel != null)
					{
						int ModelCache = Ajax.Common.ConfigHelper.GetConfigInt("ModelCache");
						Ajax.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
					}
				}
				catch { }
			}
			return (Ajax.Model.Operator)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Ajax.Model.Operator> GetModelList(string strWhere)
		{
			return dal.GetList(strWhere);
		} 
		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="pwd"></param>
		/// <returns></returns>
		public List<dynamic> Login(string userName, string pwd)
		{
			List<dynamic> userList;
			try
			{
				userList = dal.Login(userName, pwd);
			}
			catch
			{
				return null;
			}
			return userList;
		}
		/// <summary>
		/// 获取登录用户权限列表
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public List<dynamic> GetLoginUserVoteList(string userID)
		{
			return dal.GetLoginVoteList(userID);
		}

		/// <summary>
		/// 修改密码
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="oldPwd">旧密码</param>
		/// <param name="newPwd">新密码</param>
		/// <returns></returns>
		public bool ChangePwd(string userID, string oldPwd, string newPwd)
		{
			return dal.ChangePwd(userID, oldPwd, newPwd);
		}
		/// <summary>
		/// 系统用户管理json
		/// </summary>
		/// <param name="param"></param>
		/// <param name="oper"></param>
		/// <param name="emp"></param>
		/// <param name="itemCount"></param>
		/// <returns></returns>
		public List<object> SearchOperator(EasyUIGridParamModel param, Operator oper, Employee emp, out int itemCount)
		{
			return dal.SearchOperator(param, oper, emp, out itemCount);
		}
		/// <summary>
		/// 获取Operator用于JsonData
		/// </summary>
		/// <param name="operatorID"></param>
		/// <returns></returns>
		public object GetSingelOperator(string operatorID)
		{
			return dal.GetSingelOperator(operatorID);
		}
		/// <summary>
		/// 批量删除操作员
		/// </summary>
		/// <param name="id">操作员ID</param>
		/// <returns></returns>
		public bool DeleteOperator(string[] ids)
		{
			return dal.DeleteOperator(ids);
		}
		/// <summary>
		/// 禁用操作员
		/// </summary>
		/// <param name="ids">操作员ID</param>
		/// <returns></returns>
		public bool OperatorDisable(string[] ids)
		{
			bool flag = true;
			foreach (string id in ids)
			{
				if (string.IsNullOrEmpty(id))
				{
					continue;
				}
				if (!dal.DisableOperator(id))
				{
					flag = false;
				}
			}
			return flag;
		}

		/// <summary>
		/// 指定所属的权限组
		/// </summary>
		/// <param name="empID"></param>
		/// <param name="groupID"></param>
		/// <returns></returns>
		public bool GrantGroupVote(string empID, string groupID)
		{
			if (string.IsNullOrEmpty(empID) || string.IsNullOrEmpty(groupID))
			{
				return false;
			}
			return dal.GrantGroupVote(empID, groupID);
		}
		/// <summary>
		/// 根据名字查询操作用户
		/// </summary>
		/// <param name="OperatorName"></param>
		/// <returns></returns>
		public List<Operator> GetOperatorByName(string OperatorName)
		{
			if (string.IsNullOrEmpty(OperatorName))
			{
				return null;
			}
			return dal.GetOperatorByName(OperatorName);
		}

		#endregion  Method
	}
}

