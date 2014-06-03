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
	/// 数据访问类:ChargeDetail
	/// </summary>
	public partial class ChargeDetailDAL
	{
		public ChargeDetailDAL()
		{ }
		#region  Method
		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(ChargeDetail model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<ChargeDetail>(model);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(ChargeDetail model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Update<ChargeDetail>(model);
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
				return db.DeleteByID<ChargeDetail>(ID);
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from T_ChargeDetail ");
			strSql.Append(" where ID in (" + IDlist + ")  ");
			using (DBHelper db = DBHelper.Create())
			{
				return db.ExecuteNonQuery(strSql.ToString()) > 0;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public ChargeDetail GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<ChargeDetail>(ID);
			}

		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<ChargeDetail> GetList(string strWhere)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<ChargeDetail>(strWhere);
			}

		}  

		#endregion  Method
	}
}

