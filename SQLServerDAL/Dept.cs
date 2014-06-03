using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.DBUtility;
using Ajax.Model;
using System.Collections.Generic;
using System.Data.Common;
namespace Ajax.DAL
{
	/// <summary>
	/// 数据访问类:Dept
	/// </summary>
	public partial class DeptDAL
	{
		public DeptDAL()
		{ }
		#region  Method


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Dept model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<Dept>(model);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Dept model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Update<Dept>(model);
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
				db.DeleteByID<Dept>(ID);
				return true;
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from T_Dept ");
			strSql.Append(" where ID in (" + IDlist + ")  ");
			using (DBHelper db = DBHelper.Create())
			{
				return db.ExecuteNonQuery(strSql.ToString()) > 0;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Dept GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<Dept>(ID);
			}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Dept> GetList(string strWhere)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<Dept>(strWhere);
			}
		}


		#endregion  Method

		#region I_Dept 成员
		/// <summary>
		/// 获取部门动态对象
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="paramName"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public dynamic GetDeptDynamic(string sql, string[] paramName, string[] p)
		{
			Dictionary<string, object> param = new Dictionary<string, object>();
			if (p.Length != paramName.Length)
			{
				return null;
			}
			for (int i = 0; i < paramName.Length; i++)
			{
				param.Add(paramName[i], p[i]);
			}
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetSingelDynaminObject(sql, param);
			}
		}
		/// <summary>
		/// 获取部门动态对象集合
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="paramName"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public List<dynamic> GetDeptDynamicList(string sql, string[] paramName, string[] p)
		{
			Dictionary<string, object> param = new Dictionary<string, object>();
			if (p.Length != paramName.Length)
			{
				return null;
			}
			for (int i = 0; i < paramName.Length; i++)
			{
				param.Add(paramName[i], p[i]);
			}
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(sql, param);
			}
		}
		/// <summary>
		/// 获取部门动态对象集合
		/// </summary>
		/// <param name="strWhere"></param>
		/// <returns></returns>
		public List<Dept> GetListByModel(string strWhere)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select ID,PID,Code,Name,PY,Status,case status when 0 then '在用' else '停用' end as StatusName ");
			strSql.Append(" FROM T_Dept ");
			if (strWhere.Trim() != "")
			{
				strSql.Append(" where " + strWhere);
			}
			List<Dept> result = new List<Dept>();
			using (DBHelper db = DBHelper.Create())
			{
				using (DbDataReader dr = db.ExecuteReader(strSql.ToString()))
				{
					result.Add(new Dept()
					{
						ID = dr["ID"].ToString(),
						PID = dr["PID"].ToString(),
						Code = dr["Code"].ToString(),
						Name = dr["Name"].ToString(),
						Status = Convert.ToInt16(dr["Status"])
					});
				}
			}
			return result;
		}

		public System.Collections.Generic.List<Dept> GetListByModel(int Top, string strWhere, string filedOrder)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// 获取部门Code
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="paramName"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public string GetDeptCode(string sql, string[] paramName, string[] p)
		{
			Dictionary<string, object> param = new Dictionary<string, object>();
			if (p.Length != paramName.Length)
			{
				return null;
			}
			for (int i = 0; i < paramName.Length; i++)
			{
				param.Add(paramName[i], p[i]);
			}
			using (DBHelper db = DBHelper.Create())
			{
				object code = db.ExcuteScular(sql, param);
				return code != null ? code.ToString() : "";
			}
		}
		/// <summary>
		/// 更新部门信息
		/// </summary>
		/// <param name="ID">部门ID</param>
		/// <param name="Name">部门名称</param>
		/// <returns></returns>
		public bool Update(string ID, string Name)
		{
			string sql = "update T_DEPT set name =@Name where ID=@ID";
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> param = new Dictionary<string, object>();
				param.Add("Name", Name);
				param.Add("ID", ID);
				return db.ExecuteNonQuery(sql, param) > 0;
			}
		}
		#endregion
	}
}

