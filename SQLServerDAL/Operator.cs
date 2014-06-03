using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using Ajax.Common;
using System.Collections.Generic;
using System.Collections;
using System.Data.Common;
namespace Ajax.DAL
{
	/// <summary>
	/// 数据访问类:Operator
	/// </summary>
	public partial class OperatorDAL
	{
		public OperatorDAL()
		{ }
		#region  Method

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.Exist<Operator>(ID);
			}
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Operator model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<Operator>(model);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Operator model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Update<Operator>(model);
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
				Operator curr_operator = db.GetById<Operator>(ID);
				// 超级管理员不允许删除
				if (curr_operator.IsAdmin == 1)
				{
					return false;
				}
				curr_operator.Status = 3;
				db.Update<Operator>(curr_operator);
				return true;
			}
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Operator GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<Operator>(ID);
			}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Operator> GetList(string strWhere)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<Operator>(strWhere);
			}
		}

		/// <summary>
		/// 用户登录，返回用户身份凭证Object对象集合，选择登录角色
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="Pwd"></param>
		/// <returns></returns>
		public List<dynamic> Login(string userName, string Pwd)
		{
			string strEmpID = string.Empty;
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select employeeID from T_Operator where Name =@Name and pwd = @Pwd and status=1");
			Dictionary<string, object> param = new Dictionary<string, object>();
			param.Add("Name", userName);
			param.Add("Pwd", DESEncrypt.Encrypt(Pwd));
			using (DBHelper db = DBHelper.Create())
			{
				using (DbDataReader sdr = db.ExecuteReader(strSql.ToString(), param))
				{
					while (sdr != null && sdr.Read())
					{
						strEmpID = sdr["employeeID"].ToString();
					}
				}
				if (string.IsNullOrEmpty(strEmpID))
				{
					throw new Exception("用户名或者密码错误。");
				}
				else
				{
					strSql.Clear();
					strSql.Append("select o.ID,o.name as operName,o.isadmin,o.groupID,g.Name as groupName,e.ID as empID, ");
					strSql.Append("e.Name as empName,d.ID as deptID,d.Name as deptName ");
					strSql.Append("from T_Operator o ");
					strSql.Append("inner join T_Group g on o.groupID=g.ID ");
					strSql.Append("left join T_Employee e on e.ID=o.employeeID ");
					strSql.Append("left join T_Dept d on d.ID=e.deptID ");
					strSql.Append("where o.status=1 ");
					strSql.Append("and o.employeeID=@empID");
					param.Clear();
					param.Add("empID", strEmpID);
					return db.GetDynaminObjectList(strSql.ToString(), param);
				}
			}
		}
		/// <summary>
		/// 获取登录用户的权限列表
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public List<dynamic> GetLoginVoteList(string userID)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select p.path from T_Operator o  ");
			strSql.Append("left join T_Group_Vote gv on gv.groupID=o.groupID ");
			strSql.Append("left join T_Poup p on p.ID=gv.PoupID ");
			strSql.Append("where o.ID=@userID");
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> param = new Dictionary<string, object>();
				param.Add("UserID", userID);
				return db.GetDynaminObjectList(strSql.ToString(), param);
			}
		}

		/// <summary>
		/// 修改密码
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="oldPwd">旧密码</param>
		/// <param name="newPwd">新密码</param>
		/// <returns></returns>
		public bool ChangePwd(string userId, string oldPwd, string newPwd)
		{
			using (DBHelper db = DBHelper.Create())
			{
				Operator myoperator = db.GetById<Operator>(userId);
				if (myoperator.Pwd != DESEncrypt.Encrypt(oldPwd))
				{
					return false;
				}
				myoperator.Pwd = DESEncrypt.Encrypt(newPwd);
				db.Update<Operator>(myoperator);
				return true;
			}
		}
		/// <summary>
		/// 系统用户管理json
		/// </summary>
		/// <param name="param"></param>
		/// <param name="oper"></param>
		/// <param name="emp"></param>
		/// <param name="itemCount"></param>
		/// <returns></returns>
		public List<dynamic> SearchOperator(EasyUIGridParamModel param, Operator oper, Employee emp, out int itemCount)
		{
			List<SqlParameter> paramList = new List<SqlParameter>();
			StringBuilder strSql = new StringBuilder();
			strSql.Append("SELECT oper.ID, oper.EmployeeID AS empid,oper.status ,");
			strSql.Append("case oper.[Status] WHEN 1 THEN '启用' WHEN 0 THEN '<font color=''Gray''>已禁用</font>' else '<font color=''red''>已删除</font>' end AS STATUSNAME, ");
			strSql.Append("oper.Name, case oper.IsAdmin WHEN 1 THEN '是' ELSE '否' end as isAdmin,oper.createdate,oper.groupid,");
			strSql.Append("emp.name AS empname,G.NAME AS GROUPNAME ");
			strSql.Append("FROM T_Operator oper ");
			strSql.Append("LEFT JOIN T_Employee emp ON emp.ID = oper.EmployeeID ");
			strSql.Append("LEFT JOIN T_GROUP G ON G.ID=OPER.GROUPID ");
			strSql.Append("where 1=1  and oper.status <3");
			Dictionary<string, object> paramss = new Dictionary<string, object>();

			if (!string.IsNullOrEmpty(oper.Name))
			{
				strSql.Append("and (oper.name like @name or emp.name like @name) ");
				paramss.Add("name", string.Format("%{0}%", oper.Name));
			}
			int pageIndex = Convert.ToInt32(param.page) - 1;
			int pageSize = Convert.ToInt32(param.rows);
			using (DBHelper db = DBHelper.Create())
			{
				itemCount = db.GetCount(strSql.ToString(), paramss);
				return db.GetDynaminObjectList(strSql.ToString(), pageIndex, pageSize, "Status", paramss);
			}
		}
		/// <summary>
		/// 获取Operator用于jsondata
		/// </summary>
		/// <param name="operatorID"></param>
		/// <returns></returns>
		public object GetSingelOperator(string operatorID)
		{
			string sql = @"SELECT to1.ID, to1.EmployeeID AS EMPLOYEEID,to1.status , 
                        to1.Name,  te.name AS EMPLOYEENAME,
                            case to1.[Status] WHEN 1 THEN '启用' WHEN 2 THEN '已禁用' else '<font color=''red''>已删除</font>' end AS STATUSNAME, case to1.IsAdmin WHEN 1 THEN '是' ELSE '否' end as isAdmin,to1.CreateDate,to1.groupid
                                FROM T_Operator to1 LEFT JOIN T_Employee te
							ON te.ID = to1.EmployeeID
							where to1.id = @ID";
			Dictionary<string, object> param = new Dictionary<string, object>();
			param.Add("ID", operatorID);
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetSingelDynaminObject(sql, param);
			}
		}

		/// <summary>
		/// 删除操作员
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public bool DeleteOperator(string[] ids)
		{
			List<CommandInfo> commandList = new List<CommandInfo>();
			using (DBHelper db = DBHelper.Create())
			{
				foreach (string id in ids)
				{
					CommandInfo command = new CommandInfo();
					command.CommandText = "update T_Operator set status = 3 where id = @id";
					command.Parameters = new DbParameter[] { 
						db.CreateParameter("id",id)
					};
					commandList.Add(command);
				}
				return db.ExecuteNonQuery(commandList) > 0;
			}
		}
		/// <summary>
		/// 禁用操作员
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool DisableOperator(string id)
		{
			using (DBHelper db = DBHelper.Create())
			{
				Operator opr = new Operator();
				opr = db.GetById<Operator>(id);
				// 管理员不能被禁用
				if (opr.IsAdmin == 1)
				{
					return false;
				}
				// 禁用启用切换
				if (opr.Status == 2)
				{
					opr.Status = 1;
				}
				else
				{
					opr.Status = 2;
				}
				// 已删除的点击禁用、启用时默认启用
				if (opr.Status == 3)
				{
					opr.Status = 1;
				}
				db.Update<Operator>(opr);
				return true;
			}
		}

		/// <summary>
		/// 指定用户所属的角色
		/// </summary>
		/// <param name="empID"></param>
		/// <param name="groupID"></param>
		/// <returns></returns>
		public bool GrantGroupVote(string empID, string groupID)
		{
			int flag = 0;
			string strSql = "update T_Operator set groupID=@groupID where ID=@empID";
			SqlParameter pGroupID = new SqlParameter("@groupID", SqlDbType.VarChar);
			SqlParameter pEmpID = new SqlParameter("@empID", SqlDbType.VarChar);
			pGroupID.Value = groupID;
			pEmpID.Value = empID;
			Dictionary<string, object> param = new Dictionary<string, object>();
			param.Add("groupID", groupID);
			param.Add("empID", empID);
			using (DBHelper db = DBHelper.Create())
			{
				return db.ExecuteNonQuery(strSql, param) > 0;
			}
		}


		/// <summary>
		/// 根据名字查询操作用户
		/// </summary>
		/// <param name="OperatorName"></param>
		/// <returns></returns>
		public List<Operator> GetOperatorByName(string OperatorName)
		{
			Dictionary<string, object> paramDic = new Dictionary<string, object>();
			string strWhereSql = " and (Name like @Name or py like @Name)";
			paramDic.Add("Name", string.Format("%{0}%", OperatorName));
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<Operator>(strWhereSql, paramDic, "", "");
			}
		}

		#endregion  Method
	}
}

