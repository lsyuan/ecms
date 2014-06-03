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
	/// 数据访问类:SysParameter
	/// </summary>
	public partial class SysParameterDAL
	{
		public SysParameterDAL()
		{ }
		#region  Method

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.Exist<SysParameter>(ID);
			}
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(SysParameter model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<SysParameter>(model);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(SysParameter model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Update<SysParameter>(model);
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
				return db.DeleteByID<SysParameter>(ID);
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from T_SysParameter ");
			strSql.Append(" where ID in (" + IDlist + ")  ");
			using (DBHelper db = DBHelper.Create())
			{
				return db.ExecuteNonQuery(strSql.ToString()) > 0;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public SysParameter GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<SysParameter>(ID);
			}
		}


		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<SysParameter> GetList(string strWhere)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<SysParameter>("", null, null, null);
			}
		}

		/// <summary>
		/// 更新系统参数
		/// </summary>
		/// <param name="list">系统参数集合</param>
		/// <returns></returns>
		public bool UpdateSysParameter(List<SysParameter> list)
		{
			if (list == null)
			{
				return true;
			}
			using (DBHelper db = DBHelper.Create())
			{
				db.BeginTransaction();
				foreach (SysParameter item in list)
				{
					db.Update<SysParameter>(item);
				}
				db.Commit();
			}
			return true;
		}

		/// <summary>
		/// 获取系统某个参数的值
		/// </summary>
		/// <param name="parameterName">参数名</param>
		/// <returns></returns>
		public object GetSysParameterValue(string parameterName)
		{
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> param = new Dictionary<string, object>();
				param.Add("Name", parameterName);
				SysParameter parameter = db.GetModel<SysParameter>(" and Name=@Name", param);
				if (parameter != null)
				{
					return parameter.Value;
				}
				else
				{
					return 0;
				}
			}
		}
		#endregion  Method
	}
}

