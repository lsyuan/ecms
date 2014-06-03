using System;
using System.Text;
using Ajax.Model;
using Ajax.DBUtility;
using System.Collections.Generic;
using Ajax.Common;
namespace Ajax.DAL
{
	/// <summary>
	/// 数据访问类:AnotherCharge
	/// </summary>
	public partial class AnotherChargeDAL
	{
		public AnotherChargeDAL()
		{ }
		#region  Method

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				AnotherCharge anotherCharge = db.GetById<AnotherCharge>(ID);
				return anotherCharge != null && anotherCharge.ID != null;
			}
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(AnotherCharge model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<AnotherCharge>(model);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(AnotherCharge model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Update<AnotherCharge>(model);
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
				return db.DeleteByID<AnotherCharge>(ID);
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			using (DBHelper db = DBHelper.Create())
			{
				StringBuilder strSql = new StringBuilder();
				strSql.Append("delete from T_AnotherCharge ");
				strSql.Append(" where ID in (" + IDlist + ")  ");
				return db.ExecuteNonQuery(strSql.ToString()) > 0;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public AnotherCharge GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<AnotherCharge>(ID);
			}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<AnotherCharge> GetList(string strWhere)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<AnotherCharge>(strWhere, null, "ChargeDate desc", "");
			}
		}

		/// <summary>
		/// 获取其他缴费的一览json
		/// </summary>
		/// <param name="param"></param>
		/// <param name="aCharge"></param>
		/// <param name="itemCount"></param>
		/// <returns></returns>
		public List<dynamic> AnotherChargeSearch(EasyUIGridParamModel param, AnotherCharge aCharge, out int itemCount)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append(@"SELECT ac.ID, ac.CustomerName, ac.Money, ac.ActMoney, ac.ChargeDate, ac.Remark, 
								   te.Name AS OperatorName
							FROM   T_AnotherCharge ac
								   LEFT JOIN T_operator o
										ON  o.ID = ac.OperatorID
								   LEFT JOIN T_EMPLOYEE te
										ON  o.EmployeeID = te.ID
							WHERE  1 = 1 ");
			Dictionary<string, object> paramList = new Dictionary<string, object>();

			if (aCharge.Status != -1)
			{
				strSql.Append("and ac.status=@status ");
				paramList.Add("Status", aCharge.Status);
			}
			using (DBHelper db = DBHelper.Create())
			{
				itemCount = db.GetCount(strSql.ToString(), paramList);
				int pageIndex = Convert.ToInt32(param.page) - 1;
				int pageSize = Convert.ToInt32(param.rows);
				return db.GetDynaminObjectList(strSql.ToString(), pageIndex, pageSize, "ID", paramList);
			}
		}

		/// <summary>
		/// 得到一个对象实体
		/// <param name="ID"></param>
		/// </summary>
		public dynamic GetModelByID(string ID)
		{
			Dictionary<string, object> paramList = new Dictionary<string, object>();
			using (DBHelper db = DBHelper.Create())
			{
				StringBuilder strSql = new StringBuilder();
				strSql.Append(@"select ac.ID,ac.CustomerName,ac.Money,ac.ActMoney,ac.ChargeDate,ac.Remark, 
								te.Name as OperatorName 
								from T_AnotherCharge ac ;
								left join T_operator o on o.ID=ac.OperatorID 
								left join T_EMPLOYEE te on o.EmployeeID=te.ID 
								where ac.ID=@ID");
				paramList.Add("@ID", ID);
				return db.GetSingelDynaminObject(strSql.ToString(), paramList);
			}
		}


		public bool Aduit(string guids, bool isPass)
		{
			string[] guidList = guids.TrimEnd(',').Split(',');
			Dictionary<string, object> param = new Dictionary<string, object>();
			string sql = string.Format("update T_AnotherCharge set status={0} where ID = @ID", isPass ? 1 : 2);
			bool flag = false;
			using (DBHelper db = DBHelper.Create())
			{
				db.BeginTransaction();
				for (int i = 0; i < guidList.Length; i++)
				{
					if (!string.IsNullOrEmpty(guidList[i]))
					{
						param.Clear();
						param.Add("ID", guidList[i]);
						db.ExecuteNonQuery(sql, param);
					}
				}
				db.Commit();
				flag = true;
			}
			return flag;
		}

		#endregion  Method
	}
}

