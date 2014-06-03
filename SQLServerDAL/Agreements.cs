using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using System.Collections.Generic;
using Ajax.Common;
using System.Data.Common;
namespace Ajax.DAL
{
	/// <summary>
	/// 数据访问类:Agreements
	/// </summary>
	public partial class AgreementsDAL
	{
		public AgreementsDAL()
		{ }
		#region  Method

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				Agreements agreements = db.GetById<Agreements>(ID);
				return agreements.ID != string.Empty;
			}
		}
		/// <summary>
		/// 判断协议已经存在
		/// </summary>
		/// <param name="agreeeID">协议编号</param>
		/// <param name="customerID">客户编号</param>
		/// <returns></returns>
		public bool Exists(string agreeeID, string customerID)
		{
			string strSql = @"select count(0) from  T_Agreements where (ID=@ID or customerID=@customerID)  and datediff(day,enddate,getdate())<=0";
			Dictionary<string, object> param = new Dictionary<string, object>();
			param.Add("ID", agreeeID);
			param.Add("customerID", customerID);
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetCount(strSql, param) > 0;
			}
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Agreements model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<Agreements>(model);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Agreements model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Update<Agreements>(model);
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
				return db.DeleteByID<Agreements>(ID);
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from T_Agreements ");
			strSql.Append(" where ID in (" + IDlist + ")  ");
			using (DBHelper db = DBHelper.Create())
			{
				return db.ExecuteNonQuery(strSql.ToString()) > 0;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Agreements GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<Agreements>(ID);
			}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Agreements> GetList(string strWhere)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<Agreements>(strWhere, null, "", null);
			}
		}

		/// <summary>
		/// 获取协议过期提醒
		/// <param name="top">显示行数</param>
		/// </summary>
		public List<dynamic> GetTipContracts(int top)
		{
			string strSql = string.Format(@"SELECT top 10 c.name,a.EndDate  FROM T_Agreements a
                                            left join T_Customer c on c.ID=a.customerID
                                            where a.status=1 and datediff(day,getdate(),a.endDate)<=10 ", top);
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(strSql, null);
			}
		}

		/// <summary>
		/// 获取收费协议json
		/// </summary>
		/// <param name="param"></param>
		/// <param name="agree"></param>
		/// <param name="itemCount"></param>
		/// <returns></returns>
		public List<dynamic> SearchContrast(EasyUIGridParamModel param, Agreements agree, Customer c, out int itemCount)
		{
			List<SqlParameter> paramList = new List<SqlParameter>();
			StringBuilder StrSql = new StringBuilder();
			StrSql.Append(@"select a.ID,c.ID as customerID,c.Code,c.Name,a.Money,a.Status, 
                            a.begindate,a.enddate,a.createtime,a.checktime,a.Code as agreementCode
                            from  T_Agreements a  
                            left join T_Customer c on c.ID=a.CustomerID  
                            where 1=1 ");
			Dictionary<string, object> paramDic = new Dictionary<string, object>();
			if (!string.IsNullOrEmpty(agree.ID))
			{
				StrSql.Append("and a.ID=@ID ");
				paramDic.Add("ID", agree.ID);
			}
			if (agree.BeginDate.ToString() != "0001/1/1 0:00:00" && agree.EndDate.ToString() != "0001/1/1 0:00:00")
			{
				StrSql.Append("and a.begindate>=@beginDate and a.enddate<=@endDate ");
				paramDic.Add("beginDate", agree.BeginDate);
				paramDic.Add("endDate", agree.EndDate);
			}
			if (agree.Status != -1)
			{
				StrSql.Append("and a.status=@status ");
				paramDic.Add("status", agree.Status);
			}
			if (!string.IsNullOrEmpty(agree.CustomerID))
			{
				StrSql.Append("and c.ID=@customerID ");
				paramDic.Add("customerID", agree.CustomerID);
			}
			if (!string.IsNullOrEmpty(c.Name))
			{
				StrSql.Append("and c.Name like @name ");
				paramDic.Add("name", string.Format("%{0}%", c.Name));
			}

			int pageIndex = Convert.ToInt32(param.page) - 1;
			int pageSize = Convert.ToInt32(param.rows);
			using (DBHelper db = DBHelper.Create())
			{
				itemCount = db.GetCount(string.Format(DBHelper.StrGetCountSql, StrSql), paramDic);
				return db.GetDynaminObjectList(StrSql.ToString(), pageIndex, pageSize, null, paramDic);
			}
		}
		/// <summary>
		/// 批量更改协议状态
		/// </summary>
		/// <param name="guidList">主键列表</param>
		/// <param name="status">状态</param>
		public void UpdateAgreeStatus(List<string> guidList, StatusEnum.AgreeStatusEnum status)
		{
			StringBuilder strSql = new StringBuilder("update T_Agreements set status='" + status.ToString("D") + "' where ID in(");
			for (int i = 0; i < guidList.Count; i++)
			{
				strSql.AppendFormat(i == guidList.Count - 1 ? "'{0}'" : "'{0}',", guidList[i]);
			}
			strSql.Append(")");
			using (DBHelper db = DBHelper.Create())
			{
				db.ExecuteNonQuery(strSql.ToString());
			}
		}
		/// <summary>
		/// 更改所有未审核协议为已审批
		/// </summary>
		public void ApprovalAll()
		{
			string strSql = "update T_Agreements set status='1' where status='0'";
			using (DBHelper db = DBHelper.Create())
			{
				db.ExecuteNonQuery(strSql);
			}
		}
		/// <summary>
		/// 获取指定用户的ID
		/// </summary>
		/// <param name="customerID"></param>
		public object GetAgreementObjectByCustomerID(string customerID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> dic = new Dictionary<string, object>();
				dic.Add("CustomerID", customerID);
				string sql = @" SELECT ta.ID, ta.CustomerID, ta.Code  AS aCode, ta.[Money], ta.[Status], ta.BeginDate, 
									ta.EndDate, ta.OperatorID, ta.CheckOperatorID, ta.CreateTime, ta.CheckTime, 
									ta.Remark
								FROM   T_Agreements ta where 1=1  and status=1 and customerID=@customerID";
				return db.GetSingelDynaminObject(sql, dic);
			}
		}
		/// <summary>
		/// 获取指定用户的ID
		/// </summary>
		/// <param name="customerID"></param>
		public Agreements GetAgreementByCustomerID(string customerID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> dic = new Dictionary<string, object>();
				dic.Add("CustomerID", customerID);
				return db.GetModel<Agreements>(" and status=1 and customerID=@customerID", dic);
			}
		}
        /// <summary>
        /// 获取剩余未缴纳的协议金额
        /// </summary>
        /// <param name="agreementID"></param>
        /// <returns></returns>
        public decimal GetLastAgreeFee(string agreementID)
        {
            string strSql = @"select
                            (select money from T_agreements a where a.ID='{0}' )
                            -(select sum(Money) from T_charge c where c.agreementID='{0}') ";
            using (DBHelper db = DBHelper.Create())
            {
                object feeObj= db.ExecuteScalar(string.Format(strSql,agreementID), null);
                decimal fee=Convert.ToDecimal(feeObj);
                return fee > 0 ? fee : 0;
            }
        }
		#endregion  Method
	}
}

