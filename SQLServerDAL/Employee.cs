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
	/// 数据访问类:Employee
	/// </summary>
	public partial class EmployeeDAL
	{
		public EmployeeDAL()
		{ }
		#region  Method


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Employee model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<Employee>(model);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Employee model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Update<Employee>(model);
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
				return db.DeleteByID<Employee>(ID);
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.BeginTransaction();
				foreach (string id in IDlist.Split(','))
				{
					db.DeleteByID<Employee>(id);
				}
				db.Commit();
				return true;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Employee GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<Employee>(ID);
			}
		}
		/// <summary>
		/// 获取职员查询列表
		/// </summary>
		/// <param name="param"></param>
		/// <param name="emp"></param>
		/// <param name="itemCount"></param>
		/// <returns></returns>
		public List<object> GetSearchJson(EasyUIGridParamModel param, Employee emp, out int itemCount)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append(@"select te.ID,te.Name,te.PY,BirthDate,Sex,DeptID,Job,'' as CardType,te.ICCardID as CardID,Address,te.OfficePhone,td.Name as DEPTNAME,WorkTime,FireTime,te.Status 
							FROM T_Employee te 
							LEFT JOIN T_Dept td ON td.ID = te.DeptID 
							where 1=1 and te.Status!=2 ");
			StringBuilder strSqlCount = new StringBuilder();
			strSqlCount.Append(@"select count(0) from t_employee te where 1=1 ");
			Dictionary<string, object> paramm = new Dictionary<string, object>();

			if (!string.IsNullOrEmpty(emp.Name))
			{
				strSql.Append(" and te.Name like @Name ");
				strSqlCount.Append(" and te.Name like @Name ");
				paramm.Add("Name", "%" + emp.Name + "%");
				strSql.Append(" and te.PY like @PY ");
				strSqlCount.Append(" and te.Name like @Name ");
				paramm.Add("PY", "%" + Pinyin.GetPinyin(emp.Name) + "%");
			}
			int pageIndex = Convert.ToInt32(param.page) - 1;
			int pageSize = Convert.ToInt32(param.rows);
			using (DBHelper db = DBHelper.Create())
			{
				string sql = strSql.ToString();
				itemCount = db.GetCount(strSqlCount.ToString(), paramm);
				return db.GetDynaminObjectList(sql, pageIndex, pageSize, "Name", paramm);
			}
		}

		/// <summary>
		/// 获取分页数据
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public List<object> GetDynamicPagingList(string sql, string[] paramName, string[] value, string orderBy, int pageIndex, int pageNumber)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(sql, pageIndex, pageNumber, orderBy, paramName, value);
			}
		}
		/// <summary>
		/// 获取某个职员信息
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public object GetEmployee(string ID)
		{
			string sql = @"SELECT te.*,
								   te.ICCardID AS CardType,
								   td.Name AS DeptName
							FROM   T_Employee te 
								   LEFT JOIN T_Dept td
										ON  td.ID = te.DeptID
							WHERE  te.ID = @ID ";
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> param = new Dictionary<string, object>();
				param.Add("ID", ID);
				return db.GetSingelDynaminObject(sql, param);
			}
		}
		/// <summary>
		/// 查询职员信息
		/// </summary>
		/// <param name="filterStr">字符过滤</param>
		/// <returns></returns>
		public List<dynamic> QueryEmployee(string filterStr)
		{
			string sql = @"SELECT emp.id,
								   emp.Name,
								   emp.DeptID,
								   td.Name as DEPTNAME
							FROM   t_employee emp
								   LEFT JOIN T_Dept td
										ON  td.ID = emp.DeptID
							WHERE  emp.status=1 and emp.name LIKE @name OR emp.py LIKE @PY";
			Dictionary<string, object> param = new Dictionary<string, object>();
			param.Add("NAME", "%" + filterStr + "%");
			param.Add("PY", "%" + Pinyin.GetPinyin(filterStr) + "%");
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(sql, param);
			}
		}
		/// <summary>
		/// 职员缴费分析
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public List<dynamic> EmpChargeAnalysis(string startDate, string endDate)
		{
			string strSql = string.Format(@"select e.ID,e.name,
            isnull(sum(c.Money),0)+isnull(sum(tc.Money),0)+isnull(sum(ac.ActMoney),0) as  feeCount from T_Employee e
            left join T_Operator o on o.employeeId=e.ID
            left join T_charge c on o.ID=c.OperatorID and c.CreateDate>='{0}' and c.CreateDate<='{1}'
            left join T_TempCharge tc on o.ID=tc.OperatorID and tc.CreateTime>='{0}' and tc.CreateTime<='{1}'
            left join T_AnotherCharge ac on o.ID=ac.OperatorID and ac.ChargeDate>='{0}' and ac.ChargeDate<='{1}'
            group by e.ID,e.name", startDate, endDate + " 23:59:59");
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(strSql, null);
			}
		}
		#endregion  Method

	}
}

