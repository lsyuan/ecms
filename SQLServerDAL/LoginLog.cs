using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using System.Collections.Generic;
using Ajax.Common;
namespace Ajax.DAL
{
	/// <summary>
	/// 数据访问类:LoginLog
	/// </summary>
	public partial class LoginLogDAL
	{
		public LoginLogDAL()
		{ }
		#region  Method

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(LoginLog model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<LoginLog>(model);
			}
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.DeleteByID<LoginLog>(ID);
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from T_LoginLog ");
			strSql.Append(" where ID in (" + IDlist + ")  ");
			using (DBHelper db = DBHelper.Create())
			{
				int rows = db.ExecuteNonQuery(strSql.ToString());
				if (rows > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}


		/// <summary>
		/// 日志查询
		/// <param name="log"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// </summary>
		public List<LoginLog> GetList(LoginLog log, DateTime startTime, DateTime endTime)
		{
			Dictionary<string, object> paramDic = new Dictionary<string, object>();
			using (DBHelper db = DBHelper.Create())
			{
				string strWhereSql = " ";
				if (!string.IsNullOrEmpty(log.OperatorID))
				{
					strWhereSql += " and OperatorID=@OperatorID";
					paramDic.Add("OperatorID", log.OperatorID);
				}
				if (startTime != DateTime.MinValue && endTime != DateTime.MinValue)
				{
					strWhereSql += string.Format(" and CreateTime between '{0}' and '{1}'", startTime, endTime);
				}
				return db.GetList<LoginLog>(strWhereSql, paramDic, "", "");
			}
		}
		/// <summary>
		/// 日志查询json
		/// <param name="param"></param>
		/// <param name="log"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// </summary>
		public List<dynamic> Search(EasyUIGridParamModel param, string OperatorID, DateTime startTime, DateTime endTime, out int itemCount)
		{
			List<LoginLog> logList = new List<LoginLog>();
			Dictionary<string, object> paramDic = new Dictionary<string, object>();
			using (DBHelper db = DBHelper.Create())
			{
				string strSql = @" SELECT  l.ID,o.Name OPeratorID,CreateTime,case Type when 0 then '退出登录' else '登录' end as type,te.name as oprname
                                    FROM   T_LoginLog l
                                    left join T_Operator o on o.ID=l.OperatorID
									inner join t_employee te on o.employeeid = te.id 
                                    WHERE     1=1";
				StringBuilder strSqlCount = new StringBuilder();
				strSqlCount.Append(@"select count(0) from t_loginlog tl where 1=1 ");
				if (!string.IsNullOrEmpty(OperatorID))
				{
					strSql += " and l.OperatorID=@OperatorID";
					strSqlCount.Append(" and tl.OperatorID=@OperatorID");
					paramDic.Add("OperatorID", OperatorID);
				}
				if (startTime != DateTime.MinValue)
				{
					strSql += " and l.CreateTime > @CreateTime ";
					strSqlCount.Append(" and tl.CreateTime >  @CreateTime ");
					paramDic.Add("CreateTime", startTime);
				}
				if (endTime != DateTime.MinValue)
				{
					endTime = endTime.AddDays(1);
					strSql += " and l.CreateTime < @endTime";
					strSqlCount.Append(" and tl.CreateTime < @endTime");
					paramDic.Add("endTime", endTime);
				}
				itemCount = db.GetCount(strSqlCount.ToString(), paramDic);
				int pageIndex = Convert.ToInt32(param.page) - 1;
				int pageSize = Convert.ToInt32(param.rows);
				return db.GetDynaminObjectList(strSql, pageIndex, pageSize, "ID", paramDic); ;
			}
		}
		/// <summary>
		/// 清楚日志
		/// </summary>
		public void DeleteAll()
		{
			string strSql = "delete from T_LoginLog";
			using (DBHelper db = DBHelper.Create())
			{
				db.ExecuteNonQuery(strSql);
			}
		}

		#endregion  Method
	}
}

