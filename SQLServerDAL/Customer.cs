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
	/// 数据访问类:Customer
	/// </summary>
	public partial class CustomerDAL
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public CustomerDAL()
		{ }

		#region  Method

		/// <summary>
		/// 增加一条数据
		/// <param name="model"></param>
		/// </summary>
		public void Add(Customer model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<Customer>(model);
			}
		}
		/// <summary>
		/// 新增客户重载
		/// <param name="model"></param>
		/// <param name="ccItemLists"></param>
		/// </summary>
		public void Add(Customer model, List<CustomerChargeItem> ccItemLists)
		{

			using (DBHelper db = DBHelper.Create())
			{
				db.BeginTransaction();
				db.Insert<Customer>(model);
				foreach (CustomerChargeItem item in ccItemLists)
				{
					db.Insert<CustomerChargeItem>(item);
				}
				db.Commit();
			}


		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Customer model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Update<Customer>(model);
				return true;
			}
		}
		/// <summary>
		/// 更新客户信息重载
		/// </summary>
		/// <param name="model">客户基本信息</param>
		/// <param name="ccItemLists">客户对应缴费项</param>
		/// <returns></returns>
		public bool Update(Customer model, List<CustomerChargeItem> ccItemLists)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.BeginTransaction();
				db.Update<Customer>(model);
				string sql = "delete from T_CustomerChargeItem where customerID=@ID";
				Dictionary<string, object> paramList = new Dictionary<string, object>();
				paramList.Add("ID", model.ID);
				db.ExecuteNonQuery(sql, CommandType.Text, paramList);
				foreach (CustomerChargeItem item in ccItemLists)
				{
					db.Insert<CustomerChargeItem>(item);
				}
				db.Commit();
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
				Customer customer = db.GetById<Customer>(ID);
				customer.Status = 3;
				db.Update<Customer>(customer);
				return true;
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
				Customer customer = new Customer();
				foreach (string item in IDlist.TrimEnd(',').Split(','))
				{
					customer = db.GetById<Customer>(item);
					customer.Status = 2;
					db.Update<Customer>(customer);
				}
				db.Commit();
				return true;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Customer GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<Customer>(ID);
			}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Customer> GetList(string strWhere)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<Customer>(strWhere);
			}
		}


		#endregion  Method

		#region 客户新增界面
		/// <summary>
		/// 客户查询
		/// </summary>
		/// <param name="pageModel">分页参数</param>
		/// <param name="customer">查询实体</param>
		/// <param name="itemCount">查询结果总数</param>
		/// <returns></returns>
		public List<dynamic> Search(EasyUIGridParamModel pageModel, Customer customer, out int itemCount)
		{
			Dictionary<string, object> param = new Dictionary<string, object>();
			StringBuilder strSql = new StringBuilder();
			strSql.Append(@"select tc.ID,tc.Name,tct.Name as customerType,tc.createTime,tc.Status, 
                                Contactor,Address,a.ID as agreeID 
                                FROM T_Customer tc  
                                left join T_CustomerType tct on tc.TypeID=tct.ID  
                                left join T_Agreements a on a.CustomerID=tc.ID and a.status=1 
                                where 1=1 ");
			//前台三个查询条件
			if (!string.IsNullOrEmpty(customer.Name))
			{
				string str = string.Format("%{0}%", customer.Name.ToUpper());
				strSql.Append(" and (tc.Name like @name or tc.PY like @PY)");
				param.Add("name", str);
				param.Add("PY", str);
			}
			if (customer.Status != -1)
			{
				strSql.Append(" and tc.status=@status ");
				param.Add("status", customer.Status);
			}
			if (!string.IsNullOrEmpty(customer.AreaID))
			{
				strSql.Append(" and tc.areaID=@areaID ");
				param.Add("status", customer.AreaID);
			}
			//分页信息
			int pageIndex = Convert.ToInt32(pageModel.page) - 1;
			int pageSize = Convert.ToInt32(pageModel.rows);

			using (DBHelper db = DBHelper.Create())
			{
				string sql = strSql.ToString();
				itemCount = db.GetCount(string.Format(DBHelper.StrGetCountSql, sql), param);
				return db.GetDynaminObjectList(sql, pageIndex, pageSize, "ID", param);
			}
		}

		/// <summary>
		/// 客户明细查询
		/// </summary>
		/// <param name="customerID">客户编号</param>
		/// <returns></returns>
		public object CustomerDetail(string customerID)
		{
			string sql = @"select c.*,area.Name as areaName,pc.name as parentName,a.ID as agreementID,a.beginDate,a.EndDate,a.Money,opr.name as operatorName,opr2.name as managerName,tct.name as TYPENAME
							from t_customer c  
							left join t_customer pc on pc.id=c.pid  
							left join t_area area on area.id=c.areaid 
							left join T_Agreements a on a.customerID=c.ID and a.status=1  
							left join T_Employee opr on opr.id = c.OperatorID 
							left join T_Employee  opr2 on opr2.id = c.ManagerID 
							LEFT JOIN T_CustomerType tct ON tct.ID = c.TypeID
							where c.ID=@customerID";
			Dictionary<string, object> param = new Dictionary<string, object>();
			param.Add("customerID", customerID);
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetSingelDynaminObject(sql, param);
			}
		}
		/// <summary>
		/// 客户状态审批
		/// </summary>
		/// <param name="customerIdList">客户集合ID</param>
		/// <param name="status">审核结果1通过，4未通过</param>
		/// <returns></returns>
		public bool Audit(List<string> customerIdList, int status)
		{
			string sql = "update T_Customer set status=@status where ID = @ID";
			string updateBeginChargeDate = "update T_Customer set beginchargeDate = CONVERT(date,GETDATE(),101) where ID = @ID and beginchargeDate is null";
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			using (DBHelper db = DBHelper.Create())
			{
				db.BeginTransaction();
				for (int i = 0; i < customerIdList.Count; i++)
				{
					dictionary.Add("status", status);
					dictionary.Add("ID", customerIdList[i]);
					int x = db.ExecuteNonQuery(sql, dictionary);
					if (x < 1)
					{
						db.RollBack();
						return false;
					}
					x = db.ExecuteNonQuery(updateBeginChargeDate, dictionary);
					dictionary.Clear();
				}
				db.Commit();
				return true;
			}
		}
		/// <summary>
		/// 启用/禁用 
		/// </summary>
		/// <param name="customerIDList"></param>
		/// <param name="status"></param>
		public void SetEnabled(List<string> customerIDList, int status)
		{
			string sql = @"update T_Customer set status=@status where ID=@id";
			using (DBHelper db = DBHelper.Create())
			{
				db.BeginTransaction();
				Dictionary<string, object> param = new Dictionary<string, object>();
				param.Add("status", status);
				foreach (var item in customerIDList)
				{
					param.Add("ID", item);
					db.ExecuteNonQuery(sql, param);
					param.Remove("ID");
				}
				db.Commit();
			}
		}
		#endregion

		/// <summary>
		/// 模糊查询
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="queryType">
		/// 客户查询类型
		/// 1：只查询子客户，2只查询父客户，为空时查询全部
		/// </param>
		/// <returns></returns>
		public List<Ajax.Model.Customer> GetCustomerList(string filter, string queryType)
		{
			List<Customer> cList = new List<Customer>();
			string namePY = Pinyin.GetPinyin(filter);
			string isParent = string.Empty;
			if (string.IsNullOrEmpty(queryType))
			{

			}
			else if (queryType.Equals("2"))
			{
				isParent = "and Agent = 1";
			}
			else if (queryType.Equals("1"))
			{
				isParent = "and Agent = 0";
			}
			string strSql = "select ID,Name from  T_Customer where (Name like @Name or py like @PY ) and status = 1 {0}";
			strSql = string.Format(strSql, isParent);
			Dictionary<string, object> param = new Dictionary<string, object>();
			param.Add("Name", "%" + filter + "%");
			param.Add("PY", "%" + namePY + "%");
			using (DBHelper db = DBHelper.Create())
			{
				using (DbDataReader sdr = db.ExecuteReader(strSql, param))
				{
					while (sdr != null && sdr.Read())
					{
						Customer customer = new Customer();
						customer.ID = sdr["ID"].ToString();
						customer.Name = sdr["Name"].ToString();
						cList.Add(customer);
					}
				}
			}
			return cList;
		}

		/// <summary>
		/// 根据客户类型获取对应缴费项
		/// </summary>
		/// <param name="customerTypeID"></param>
		/// <returns></returns>
		public List<dynamic> GetChargeItemByCustomerTypeID(string customerTypeID)
		{
			string strSql = @"select citem.*,unit1.Name+'.'+unit2.Name as Unit,0 as Count,0 as AgreementMoney from T_ChargeItem cItem   
                               inner join  T_TypeToItem t on t.itemID=cItem.ID  
                               left join t_unit unit1 on unit1.id=cItem.UnitID1  
                               left join t_unit unit2 on unit2.id=cItem.UnitID2  
                               where TypeID=@typeID";
			Dictionary<string, object> param = new Dictionary<string, object>();
			param.Add("typeID", customerTypeID);
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(strSql.ToString(), param);
			}
		}
		/// <summary>
		/// 根据客户guid获取对应缴费项
		/// </summary>
		/// <param name="customerID"></param>
		/// <returns></returns>
		public List<dynamic> GetChargeItemByCustomerID(string customerID)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select cItem.*,unit1.Name+'.'+unit2.Name as Unit,ccItem.Count,ccItem.AgreementMoney from   T_CustomerChargeItem ccItem ");
			strSql.Append("left join T_ChargeItem cItem on cItem.ID=ccItem.ItemID ");
			strSql.Append("left join t_unit unit1 on unit1.id=cItem.UnitID1 ");
			strSql.Append("left join t_unit unit2 on unit2.id=cItem.UnitID2 ");
			strSql.Append("where ccItem.customerID=@customerID");
			Dictionary<string, object> param = new Dictionary<string, object>();
			param.Add("customerID", customerID);
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(strSql.ToString(), param);
			}
		}

		/// <summary>
		/// 客户自动完成
		/// </summary>
		/// <param name="q"></param>
		/// <returns></returns>
		public List<Customer> QueryCustomer(string q)
		{
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> paramList = new Dictionary<string, object>();
				paramList.Add("Name", string.Format("%{0}%", q));
				paramList.Add("PY", string.Format("%{0}%", q.ToUpper()));
				return db.GetList<Customer>(" and (Name like @name or PY like @py)", paramList, "len(name)", "");
			}
		}

		/// <summary>
		/// 获取指定客户的子客户
		/// </summary>
		/// <param name="customerID"></param>
		/// <returns></returns>
		public List<Customer> GetChildrenCustomer(string customerID)
		{
			List<Customer> customerList = new List<Customer>();
			customerList = GetList(string.Format(" and PID='{0}' and status =1", customerID));
			return customerList;
			//return db.GetList<Customer>(" and PID=@customerID ", paramList, "", "");
		}
	}
}

