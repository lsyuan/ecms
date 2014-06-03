using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.DBUtility;
using Ajax.Model;
using System.Collections.Generic;
using Ajax.Common;
namespace Ajax.DAL
{
	/// <summary>
	/// 数据访问类:Message
	/// </summary>
	public partial class MessageDAL
	{
		public MessageDAL()
		{ }
		#region  Method

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.Exist<Message>(ID);
			}
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Message model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<Message>(model);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Message model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Update<Message>(model);
				return true;
			}
		}


		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.DeleteByID<Message>(ID);
			}
		}
		/// <summary>
		/// 批量删除公告
		/// </summary>
		/// <param name="MsgIDList"></param>
		/// <returns></returns>
		public bool DeleteMul(List<string> MsgIDList)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.BeginTransaction();
				StringBuilder strDeleteOprMsgSql = new StringBuilder("delete from T_OperatorMsg where MsgID in(");
				for (int i = 0; i < MsgIDList.Count; i++)
				{
					strDeleteOprMsgSql.AppendFormat("'{0}'", MsgIDList[i]);
					strDeleteOprMsgSql.Append(i == MsgIDList.Count - 1 ? "" : ",");
				}
				strDeleteOprMsgSql.Append(")");
				int effectLine = db.ExecuteNonQuery(strDeleteOprMsgSql.ToString());

				StringBuilder strSql = new StringBuilder("delete from T_Message where ID in(");
				for (int i = 0; i < MsgIDList.Count; i++)
				{
					strSql.AppendFormat("'{0}'", MsgIDList[i]);
					strSql.Append(i == MsgIDList.Count - 1 ? "" : ",");
				}
				strSql.Append(")");
				db.ExecuteNonQuery(strSql.ToString());
				db.Commit();
				return true;
			}

		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Message GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<Message>(ID);
			}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Message> GetList(string strWhere)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<Message>(strWhere);
			}
		}
		/// <summary>
		/// 获取消息的json数据
		/// </summary>
		/// <param name="gridParam">分页信息</param>
		/// <param name="msg">记录查询条件的实体</param>
		/// <param name="itemCount">查询的总行书</param>
		/// <returns></returns>
		public List<object> GetSearchJson(EasyUIGridParamModel gridParam, int status, string userID, out int itemCount)
		{
			List<string> paramNames = new List<string>();
			List<string> paramValus = new List<string>();
			List<SqlParameter> paramList = new List<SqlParameter>();
			StringBuilder strSql = new StringBuilder();
			strSql.Append(@"SELECT m.ID, Title, CreateDate, (
									   CASE 
											WHEN (om.status = 0) THEN '未读'
											WHEN (om.status = 1) THEN '已读'
									   END
								   ) STATUS
							FROM   T_Message m
								   LEFT JOIN t_operatorMsg om
										ON  m.ID = om.MsgID
							WHERE  1 = 1 ");
			StringBuilder countSql = new StringBuilder(@"SELECT count(0)
														FROM   T_Message m
															   LEFT JOIN t_operatorMsg om
																	ON  m.ID = om.MsgID
														WHERE  1 = 1 ");
			Dictionary<string, object> param = new Dictionary<string, object>();

			//前台查询条件 
			if (status != -1)
			{
				strSql.Append(" and om.status=@status ");
				countSql.Append(" and om.status=@status ");
				param.Add("status", status);
			}
			if (!string.IsNullOrEmpty(userID))
			{
				strSql.Append(" and om.OperatorID=@OperatorID ");
				countSql.Append(" and om.OperatorID=@OperatorID ");
				param.Add("OperatorID", userID);
			}
			int pageIndex = Convert.ToInt32(gridParam.page) - 1;
			int pageSize = Convert.ToInt32(gridParam.rows);
			using (DBHelper db = DBHelper.Create())
			{
				string sql = strSql.ToString();
				itemCount = db.GetCount(countSql.ToString(), param);
				return db.GetDynaminObjectList(sql, pageIndex, pageSize, "ID", param);
			}
		}
		/// <summary>
		/// 获取最新消息/公告
		/// </summary>
		/// <param name="topCount">前N条</param>
		/// <param name="operatorID"></param>
		/// <returns></returns>
		public List<dynamic> GetLatestMsg(int topCount, string operatorID)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.AppendFormat("select top {0} m.title,o.Name,om.status,m.createDate,m.ID ", topCount);
			strSql.Append("from T_OperatorMsg om ");
			strSql.Append("left join T_Message m on m.ID=om.msgID ");
			strSql.Append("left join T_Operator o on o.ID=om.OperatorID ");
			strSql.AppendFormat("where om.OperatorID='{0}' ", operatorID);
			strSql.Append("order by m.createDate desc ");
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(strSql.ToString(), null);
			}
		}

		#endregion  Method
	}
}

