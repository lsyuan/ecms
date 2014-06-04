using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Ajax.DBUtility;
using Ajax.Model;
using Ajax.Common;

namespace Ajax.DAL
{
	/// <summary>
	/// 数据访问类:Charge
	/// </summary>
	public partial class ChargeDAL
	{
		public ChargeDAL()
		{ }
		#region  Method

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				Charge charge = db.GetById<Charge>(ID);
				return charge.ID != string.Empty;
			}
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Charge model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<Charge>(model);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Charge model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<Charge>(model);
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
				return db.DeleteByID<Charge>(ID);
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
				strSql.Append("delete from T_Charge ");
				strSql.Append(" where ID in (" + IDlist + ")  ");
				return db.ExecuteNonQuery(IDlist) > 0;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Charge GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<Charge>(ID);
			}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Charge> GetList(string strWhere)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<Charge>(strWhere, null, null, null);
			}
		}

		/// <summary>
		/// 获取缴费信息表json
		/// </summary>
		/// <param name="customerID">客户ID</param>
		/// <returns></returns>
		public List<dynamic> ChargeSearch(string customerID, EasyUIGridParamModel param, out int itemCount)
		{
			StringBuilder strSql = new StringBuilder();
			string sql = @"SELECT tc.ID,
							   tc.CustomerID,
							   tc.[Money],
							   tc.CreateDate,
							   tc.OperatorID,
							   CASE tc.IsAgreementCharge
									WHEN 0 THEN '否'
									WHEN 1 THEN '是'
							   END       AS IsAgreementCharge,
							   tc.BeginDate,
							   tc.EndDate,
							   CASE tc.[Status]
									WHEN 0 THEN '未审核'
									WHEN 1 THEN '已审核'
									WHEN 2 THEN '已退费'
									WHEN 3 THEN '无效'
							   END       AS STATUS,
							   te.Name  AS OperatorName,
							   ta.Code,
							   tc2.Name  AS customerName
						FROM   T_Charge tc
							   INNER JOIN T_Customer tc2
									ON  tc2.ID = tc.CustomerID
							   INNER JOIN T_Operator to1
									ON  to1.ID = tc.OperatorID
							   INNER JOIN T_Employee te ON te.id = to1.EmployeeID
							   LEFT JOIN T_Agreements ta
									ON  tc.AgreementID = ta.ID 
								where 1=1 and (tc.status = 0) {0}";
			string sqlCount = @"select count(0) FROM   T_Charge tc
							   INNER JOIN T_Customer tc2
									ON  tc2.ID = tc.CustomerID
							   INNER JOIN T_Operator to1
									ON  to1.ID = tc.OperatorID
							   INNER JOIN T_Employee te ON te.id = to1.EmployeeID
							   LEFT JOIN T_Agreements ta
									ON  tc.AgreementID = ta.ID 
								where 1=1 and (tc.status = 0 ) {0}";
			using (DBHelper db = DBHelper.Create())
			{
				StringBuilder strWhere = new StringBuilder();
				Dictionary<string, object> paramss = new Dictionary<string, object>();
				if (!string.IsNullOrEmpty(customerID))
				{
					strWhere.Append(" and tc.CustomerID = @CUSTOMERID ");
					paramss.Add("CustomerID", customerID);
				}

				itemCount = db.GetCount(string.Format(sqlCount, strWhere.ToString()), paramss);
				return db.GetDynaminObjectList(string.Format(sql, strWhere.ToString()), Convert.ToInt32(param.page) - 1, Convert.ToInt32(param.rows), "", paramss);
			}
		}

		/// <summary>
		/// 获取缴费信息表json
		/// </summary>
		/// <param name="customerID">客户ID</param>
		/// <returns></returns>
		public List<dynamic> ChargeSearch(string customerID)
		{
			StringBuilder strSql = new StringBuilder();
			string sql = @"SELECT tc.ID,
							   tc.CustomerID,
							   tc.[Money],
							   tc.CreateDate,
							   tc.OperatorID,
							   CASE tc.IsAgreementCharge
									WHEN 0 THEN '否'
									WHEN 1 THEN '是'
							   END       AS IsAgreementCharge,
							   tc.BeginDate,
							   tc.EndDate,
							   CASE tc.[Status]
									WHEN 0 THEN '未审核'
									WHEN 1 THEN '已审核'
									WHEN 2 THEN '已退费'
									WHEN 3 THEN '无效'
							   END       AS STATUS,
							   te.Name  AS OperatorName,
							   ta.Code,
							   tc2.Name  AS customerName
						FROM   T_Charge tc
							   INNER JOIN T_Customer tc2
									ON  tc2.ID = tc.CustomerID
							   INNER JOIN T_Operator to1
									ON  to1.ID = tc.OperatorID
							   INNER JOIN T_Employee te ON te.id = to1.EmployeeID
							   LEFT JOIN T_Agreements ta
									ON  tc.AgreementID = ta.ID 
								where 1=1 and (tc.status = 0 or tc.status = 1) {0}";
			using (DBHelper db = DBHelper.Create())
			{
				StringBuilder strWhere = new StringBuilder();
				Dictionary<string, object> paramss = new Dictionary<string, object>();
				if (!string.IsNullOrEmpty(customerID))
				{
					strWhere.Append(" and tc.CustomerID = @CUSTOMERID ");
					paramss.Add("CustomerID", customerID);
				}

				return db.GetDynaminObjectList(string.Format(sql, strWhere.ToString()), paramss);
			}
		}
		/// <summary>
		/// 获取缴费信息表json
		/// </summary>
		/// <param name="customerID">客户ID</param>
		/// <returns></returns>
		public List<dynamic> CustomerChargeConfirmSearch()
		{
			string sql = @"SELECT tc.ID,
							   tc.CustomerID,
							   tc.[Money],
							   tc.CreateDate,
							   tc.OperatorID, 
							   CASE tc.[Status]
									WHEN 0 THEN '未审核'
									WHEN 1 THEN '已审核'
									WHEN 2 THEN '已退费'
									WHEN 3 THEN '无效'
							   END       AS STATUS,
							   te.Name  AS OperatorName,
							   ta.Code,
							   tc2.Name  AS customerName
						FROM   T_Charge tc
							   INNER JOIN T_Customer tc2
									ON  tc2.ID = tc.CustomerID
							   INNER JOIN T_Operator to1
									ON  to1.ID = tc.OperatorID
							   INNER JOIN T_Employee te ON te.id = to1.EmployeeID
							   LEFT JOIN T_Agreements ta
									ON  tc.AgreementID = ta.ID 
								where  1=1 and PID !=null"; // PID != null 限制不查询子客户的缴费
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(sql, null);
			}
		}
		/// <summary>
		/// 协议用户欠费统计
		/// </summary>
		/// <param name="param"></param>
		/// <param name="pageCount"></param>
		/// <returns></returns>
		public List<dynamic> AnalysisAgreeArrear(EasyUIGridParamModel param, out int pageCount)
		{
			string strSql = @"";
			int pageIndex = Convert.ToInt32(param.page) - 1;
			int pageSize = Convert.ToInt32(param.rows);

			using (DBHelper db = DBHelper.Create())
			{
				pageCount = db.GetCount(string.Format(DBHelper.StrGetCountSql, strSql), null);
				return db.GetDynaminObjectList(strSql, null);
			}
		}
		/// <summary>
		/// 非协议用户欠费统计
		/// </summary>
		/// <param name="param"></param>
		/// <param name="pageCount"></param>
		/// <returns></returns>
		public List<dynamic> AnalysisNoAgreeArrear(EasyUIGridParamModel param, out int pageCount)
		{
			string strSql = @"SELECT tc.ID,tc.Name,isnull(ta.[Money],0) agreeMoney,
                            isnull(sum(tcharge.[Money]),0) feeMoney
                            FROM T_Customer tc
                            INNER JOIN T_Agreements ta ON tc.id=ta.CustomerID AND ta.[Status]=1
                            LEFT JOIN T_Charge tcharge ON tcharge.CustomerID=tc.ID
                            WHERE tc.[Status]=1 AND ta.[Status]=1
                            GROUP BY tc.ID,tc.Name,ta.[Money] 
                            HAVING isnull(sum(tcharge.[Money]),0)<isnull(ta.[Money],0)";
			int pageIndex = Convert.ToInt32(param.page) - 1;
			int pageSize = Convert.ToInt32(param.rows);

			using (DBHelper db = DBHelper.Create())
			{
				pageCount = db.GetCount(string.Format(DBHelper.StrGetCountSql, strSql), null);
				return db.GetDynaminObjectList(strSql, null);
			}
		}

		/// <summary>
		/// 按月份缴费
		/// </summary>
		/// <param name="charge">缴费主表</param>
		/// <param name="chargeDetail">缴费明细</param>
		/// <param name="ChargeMonth">缴费时长（月）</param>
		/// <returns></returns>
		public bool ChargeByMonth(Charge charge, ChargeDetail[] chargeDetail, int ChargeMonth)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.BeginTransaction();
				// 插入主表
				db.Insert<Charge>(charge);

				// 插入明细
				db.InsertBatch<ChargeDetail>(chargeDetail);

				// 如果缴费是自动审批，缴费后更新客户的开始缴费时间
				if (charge.Status == 1)
				{
					string sql = "update t_customer set beginChargeDate = DATEADD(MONTH,@MONTH,beginChargeDate) where ID = @customerID or PID = @customerID";
					Dictionary<string, object> param = new Dictionary<string, object>();
					param.Add("MONTH", ChargeMonth);
					param.Add("customerID", charge.CustomerID);
					int i = db.ExecuteNonQuery(sql, param);
					if (i < 1)
					{
						db.RollBack();
						return false;
					}
				}

				db.Commit();
			}
			return true;
		}
		/// <summary>
		/// 按协议缴费
		/// </summary>
		/// <param name="charge">缴费主表</param>
		/// <param name="agreement">协议信息</param>
		/// <returns></returns>
		public string ChargeByAgreement(Charge charge, Agreements agreement)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.BeginTransaction();
				decimal agreementMoney = agreement.Money;
				#region 更新协议缴费状态
				string sql = "select sum(money) from charge where AgreementID = @AgreementID and (status = 1 or status = 0)";
				Dictionary<string, object> dic = new Dictionary<string, object>();
				dic.Add("AgreementID", agreement.ID);

				object o = db.ExcuteScular(sql, dic);

				decimal chargedSumMoney = 0m;

				decimal.TryParse((o ?? 0).ToString(), out chargedSumMoney);

				// 如果协议总金额大于等于协议金额，则更新协议状态为缴讫
				if (chargedSumMoney == agreement.Money)
				{
					agreement.Status = 2;
					db.Update<Agreements>(agreement);
				}
				else if ((chargedSumMoney + charge.Money) > agreement.Money)
				{
					db.RollBack();
					return "本期缴费金额加上历史缴费金额大于本协议缴费总金额，不能缴费，请调整缴费金额";
				}
				#endregion
				// 插入主表
				db.Insert<Charge>(charge);

				db.Commit();

				return "缴费成功";
			}
		}
		/// <summary>
		/// 缴费审核操作
		/// </summary>
		/// <param name="guidList">缴费记录ID</param>
		/// <param name="isPass">是否通过</param>
		/// <returns></returns>
		public bool ChargeAudit(List<string> guidList, bool isPass)
		{
			Dictionary<string, object> param = new Dictionary<string, object>();
			string sql = string.Format("update T_Charge set status={0} where ID = @ID", isPass ? 1 : 2);
			bool flag = false;
			using (DBHelper db = DBHelper.Create())
			{
				db.BeginTransaction();
				for (int i = 0; i < guidList.Count; i++)
				{
					if (!string.IsNullOrEmpty(guidList[i]))
					{
						param.Clear();
						param.Add("ID", guidList[i]);
						#region update agreement status
						#endregion
						Charge charge = db.GetById<Charge>(guidList[i]);
						db.ExecuteNonQuery(sql, param);
						if (charge != null && charge.IsAgreementCharge != 0)
						{
							// 查询该协议下面总的缴费金额
							string agreementTotalmoney = @"select sum(Money) from T_Charge where AgreementID=@AgreementID and status = 1";
							Dictionary<string, object> agreementsParam = new Dictionary<string, object>();
							agreementsParam.Add("AgreementID", charge.AgreementID);
							int total = Convert.ToInt32(db.ExcuteScular(agreementTotalmoney, agreementsParam));
							Agreements agreement = db.GetById<Agreements>(charge.AgreementID);
							// 如果该协议已经缴纳完费用，更新协议状态和客户开始缴费时间。
							if (total >= agreement.Money)
							{
								string updateAgreement = @"update T_Agreements set status = 3 where id=@id";
								agreementsParam = new Dictionary<string, object>();
								agreementsParam.Add("id", charge.AgreementID);
								if (db.ExecuteNonQuery(updateAgreement, agreementsParam) > 0)
								{
									// 更新客户开始缴费时间为协议的结束时间
									string updateCustomer = "update t_customer set BeginChargeDate = @BeginChargeDate where id=@id";
									Dictionary<string, object> customerParam = new Dictionary<string, object>();
									customerParam.Add("BeginChargeDate", agreement.EndDate);
									customerParam.Add("id", charge.CustomerID);
									if (db.ExecuteNonQuery(updateCustomer, customerParam) < 1)
									{
										db.RollBack();
									}
								}
								else
								{
									db.RollBack();
								}
							}
						}
						else
						{
							db.ExecuteNonQuery(sql, param);
						}
					}
				}
				db.Commit();
				flag = true;
			}
			return flag;
		}
		#endregion  Method

		#region 欠费分析
		/// <summary>
		/// 获取欠费统计信息
		/// </summary>
		/// <param name="areaID">区域ID</param>
		/// <returns></returns>
		public List<dynamic> GetArrearList(string areaID, string time)
		{
			StringBuilder whereStr1 = new StringBuilder();
			whereStr1.Append("tc.BeginChargeDate < GETDATE()");
			if (!string.IsNullOrEmpty(time))
			{
				string[] aa = time.Split(',');
				if (aa == null || aa.Length != 2)
				{
					return new List<dynamic>();
				}
				int litter = 0;
				int.TryParse(aa[0], out litter);
				int bigger = 0;
				int.TryParse(aa[1], out bigger);
				if (litter == -1 || bigger == -1)
				{
					return new List<dynamic>();
				}
				whereStr1.Clear().AppendFormat(" (tc.BeginChargeDate >= dateadd(m,{0},GETDATE()) and tc.BeginChargeDate >= dateadd(m,{1},GETDATE()))", litter * -1, bigger * -1);
				//whereStr2.Clear().AppendFormat(" (tc.BeginChargeDate >= dateadd(m,{0},GETDATE()) and tc.BeginChargeDate >= dateadd(m,{1},GETDATE()))", litter * -1, bigger * -1);
			}
			string sql = @"SELECT tc.ID, tc.Name, tc.Code, tc.Contactor, tc.Phone, tc.MobilePhone, tc.[Address], 
									tc.BeginChargeDate, tct.Name AS typeName, ta.Name AS areaName
							FROM   T_Customer tc
									INNER JOIN T_CustomerType tct
										ON  tct.ID = tc.TypeID
									LEFT JOIN T_Area ta
										ON  ta.ID = tc.AreaID
							WHERE  {0}
									AND tc.[Status] = 1
									AND NOT EXISTS (
											SELECT *
											FROM   T_Agreements ta
											WHERE  ta.CustomerID = tc.ID
													AND ta.[Status] = 1
										)  
									And TC.AREAID LIKE @AREAID
							UNION ALL
							SELECT tc.ID, tc.Name, tc.Code, tc.Contactor, tc.Phone, tc.MobilePhone, tc.[Address], 
									tc.BeginChargeDate, tct.Name AS typeName, ta.Name AS areaName
							FROM   T_Customer tc
									INNER JOIN T_CustomerType tct
										ON  tct.ID = tc.TypeID
									LEFT JOIN T_Area ta
										ON  ta.ID = tc.AreaID
							WHERE  tc.[Status] = 1
									AND EXISTS(
											SELECT *
											FROM   T_Agreements ta
											WHERE  ta.CustomerID = tc.ID
													AND ta.[Status] = 1
													AND ta.BeginDate < GETDATE()
													AND ta.EndDate > GETDATE()
									)
									And TC.AREAID LIKE @AREAID";
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> param = new Dictionary<string, object>();
				param.Add("AreaID", string.Format("%{0}%", areaID));
				return db.GetDynaminObjectList(string.Format(sql, whereStr1.ToString()), param);
			}
		}
		#endregion
	}
}

