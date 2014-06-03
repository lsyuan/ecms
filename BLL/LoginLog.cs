using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
	/// <summary>
	/// 登录日志
	/// </summary>
	public partial class LoginLogRule
	{
		private readonly Ajax.DAL.LoginLogDAL dal = new Ajax.DAL.LoginLogDAL();
		public LoginLogRule()
		{ }
		#region  Method

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Ajax.Model.LoginLog model)
		{
			try
			{
				dal.Add(model);
			}
			catch { }
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(string ID)
		{

			return dal.Delete(ID);
		}
		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			return dal.DeleteList(IDlist);
		}

		/// <summary>
		/// 日志查询
		/// </summary>
		/// <param name="param"></param>
		/// <param name="OperatorID"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <param name="itemCount"></param>
		/// <returns></returns>
		public List<dynamic> Search(EasyUIGridParamModel param, string OperatorID, DateTime startTime, DateTime endTime, out int itemCount)
		{
			return dal.Search(param, OperatorID, startTime, endTime, out itemCount);
		}

		/// <summary>
		/// 清楚日志
		/// </summary>
		public void DeleteAll()
		{
			dal.DeleteAll();
		}

		#endregion  Method
	}
}

