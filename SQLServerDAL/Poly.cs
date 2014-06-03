using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using System.Collections.Generic;
namespace Ajax.DAL
{
	/// <summary>
	/// 数据访问类:Poly
	/// </summary>
	public partial class PolyDAL
	{
		public PolyDAL()
		{ }
		#region  Method

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.Exist<Poly>(ID);
			}
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Poly model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<Poly>(model);
			}
		}
		/// <summary>
		/// 新增多个策略
		/// </summary>
		/// <param name="polys">策略数组</param>
		/// <param name="delItemID">缴费项编号</param>
		public void SavePolys(List<Poly> polys, string delItemID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				//删除旧记录
				db.BeginTransaction();
				if (!string.IsNullOrEmpty(delItemID))
				{
					string strSqlDel = "delete from T_Poly where ItemID=@ItemID";
					Dictionary<string, object> paramDel = new Dictionary<string, object>();
					paramDel.Add("@ItemID", delItemID);
					db.ExecuteNonQuery(strSqlDel, paramDel);
				}
				foreach (Poly p in polys)
				{
					db.Insert<Poly>(p);
				}
				db.Commit();
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Poly model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Update<Poly>(model);
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
				return db.DeleteByID<Poly>(ID);
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from T_Poly ");
			strSql.Append(" where ID in (" + IDlist + ")  ");

			using (DBHelper db = DBHelper.Create())
			{
				return db.ExecuteNonQuery(strSql.ToString()) > 0;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Poly GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<Poly>(ID);
			}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Poly> GetList(string strWhere)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<Poly>(strWhere);
			}
		}

		/// <summary>
		/// 获取指定缴费项的缴费策略
		/// </summary>
		/// <param name="ItemID">缴费项</param>
		/// <returns></returns>
		public List<Poly> GetPolyListByItemID(string ItemID)
		{
			List<Poly> polyList = new List<Poly>();
			string strSql = "select ID,UnitPrice,LowerBound,HignerBound from T_Poly where ItemID=@ItemID";
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> paramList = new Dictionary<string, object>();
				paramList.Add("ItemID", ItemID);
				//polyList=db.GetList<Poly>(" and ItemID=@ItemID ", paramList, "", "");
				using (System.Data.Common.DbDataReader ddr = db.ExecuteReader(strSql, paramList))
				{
					while (ddr != null && ddr.Read())
					{
						Poly p = new Poly();
						p.ID = ddr["ID"].ToString();
						p.UnitPrice = Convert.ToDecimal(ddr["UnitPrice"]);
						p.LowerBound = Convert.ToInt32(ddr["LowerBound"]);
						p.HignerBound = Convert.ToInt32(ddr["HignerBound"]);
						polyList.Add(p);
					}
				}
			}
			return polyList;
		}
		/// <summary>
		/// 获取指定缴费项的缴费策略
		/// </summary>
		/// <param name="ItemID">缴费项ID</param>
		/// <returns></returns>
		public List<dynamic> GetPolicyListByItemID(string ItemID)
		{
			string strSql = "select ID,UnitPrice,LowerBound,HignerBound from T_Poly where ItemID=@ItemID";
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> paramList = new Dictionary<string, object>();
				paramList.Add("@ItemID", ItemID);
				return db.GetDynaminObjectList(strSql, paramList);
			}
		}
		public decimal GetUnitPrice(string itemID, decimal count)
		{
			return 0;
		}
		#endregion  Method
	}
}

