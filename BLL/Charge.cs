using System;
using System.Collections.Generic;
using System.Data;
using Ajax.Common;
using Ajax.Model;
using Ajax.Common;
namespace Ajax.BLL
{
	/// <summary>
	/// 缴费信息表
	/// </summary>
	public partial class ChargeRule
	{
		private readonly Ajax.DAL.ChargeDAL dal = new Ajax.DAL.ChargeDAL();
		#region  Method
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			return dal.Exists(ID);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Ajax.Model.Charge model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.Charge model)
		{
			return dal.Update(model);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(string ID)
		{

			return dal.Delete(ID);
		}
		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			return dal.DeleteList(IDlist);
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Ajax.Model.Charge GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.Charge GetModelByCache(string ID)
		{

			string CacheKey = "ChargeModel-" + ID;
			object objModel = Ajax.Common.DataCache.GetCache(CacheKey);
			if (objModel == null)
			{
				try
				{
					objModel = dal.GetModel(ID);
					if (objModel != null)
					{
						int ModelCache = Ajax.Common.ConfigHelper.GetConfigInt("ModelCache");
						Ajax.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
					}
				}
				catch { }
			}
			return (Ajax.Model.Charge)objModel;
		}
		/// <summary>
		/// 获取缴费信息表json
		/// </summary>
		/// <param name="customerID">客户ID</param>
		/// <param name="param">EasyUIGridParamModel query parameter</param>
		/// <param name="itemCount">count</param>
		/// <returns></returns>
		public List<dynamic> ChargeSearch(string customerID, Ajax.Common.EasyUIGridParamModel param, out int itemCount)
		{
			return dal.ChargeSearch(customerID, param, out itemCount);
		}
		/// <summary>
		/// 获取缴费信息表json
		/// </summary>
		/// <param name="customerID">客户ID</param>
		/// <param name="param">EasyUIGridParamModel query parameter</param>
		/// <param name="itemCount">count</param>
		/// <returns></returns>
		public List<dynamic> ChargeSearch(string customerID)
		{
			return dal.ChargeSearch(customerID);
		}
		/// <summary>
		/// 获取缴费信息表json
		/// </summary>
		/// <param name="customerID">客户ID</param>
		/// <returns></returns>
		public List<dynamic> CustomerChargeConfirmSearch()
		{
			return dal.CustomerChargeConfirmSearch();
		}

		/// <summary>
		/// 递归计算客户（默认包括子客户）的应缴费用
		/// </summary>
		/// <param name="customerID"></param>
		/// <param name="containChildCustomer">是否计继续算子客户应缴费用</param>
		/// <returns></returns>
		public decimal CaculateCustomerFee(string customerID, bool containChildCustomer = true)
		{
			decimal totalFee = 0;
			Customer c = new CustomerRule().GetModel(customerID);
			int monthCount = GetMonthCount(Convert.ToDateTime(c.BeginChargeDate));
			ChargeItemRule chargeItemRule = new ChargeItemRule();
			Agreements agree = new AgreementsRule().GetAgreementByCustomerID(customerID);
			if (agree != null)
			{
				totalFee = new AgreementsRule().GetLastAgreeFee(agree.ID);
				if (agree.EndDate > DateTime.Now)
				{
					monthCount = GetMonthCount(agree.EndDate);
					//客户缴费项
					List<CustomerChargeItem> chargeItems = new CustomerChargeItemRule().GetListBycustomerID(customerID);
					foreach (CustomerChargeItem chargeItem in chargeItems)
					{
						if (chargeItem.AgreementMoney > 0)
						{
							totalFee += chargeItem.AgreementMoney;
						}
						else
						{
							decimal itemPrice = chargeItemRule.GetPriceByItemID(chargeItem.ItemID, chargeItem.Count, customerID);
							totalFee += monthCount * chargeItem.Count * itemPrice;
						}
					}
				}
			}
			else//非协议用户&协议过期
			{
				//客户缴费项
				List<CustomerChargeItem> chargeItems = new CustomerChargeItemRule().GetListBycustomerID(customerID);
				foreach (CustomerChargeItem chargeItem in chargeItems)
				{
					if (chargeItem.AgreementMoney > 0)
					{
						totalFee += chargeItem.AgreementMoney * monthCount;
					}
					else
					{
						decimal itemPrice = chargeItemRule.GetPriceByItemID(chargeItem.ItemID, chargeItem.Count, customerID);
						totalFee += monthCount * chargeItem.Count * itemPrice;
					}
				}
				//递归所有子客户应缴金额
				if (containChildCustomer)
				{
					List<Customer> customerChildrenList = new CustomerRule().GetChildrenCustomer(customerID);
					if (customerChildrenList != null)
					{
						//子客户费用
						foreach (Customer childC in customerChildrenList)
						{
							totalFee += CaculateCustomerFee(childC.ID);
						}
					}
				}
			}
			return totalFee;
		}

		/// <summary>
		/// 缴费操作
		/// </summary>
		/// <param name="CustomerID">客户ID</param>
		/// <param name="ChargeMonth">缴费月数</param>
		/// <param name="ChargeMoney">缴费金额</param>
		/// <returns></returns>
		public string Charge(string CustomerID, int ChargeMonth, decimal ChargeMoney, string operatorID)
		{
			CustomerRule customerRule = new CustomerRule();
			Ajax.DAL.ChargeDAL chargeDAL = new DAL.ChargeDAL();
			Customer customer = customerRule.GetModel(CustomerID);
			SysParameterRule parameterRule = new SysParameterRule();
			dynamic result = new System.Dynamic.ExpandoObject();

			int status = 0;	// 缴费状态，0 不自动审批，1 自动审批
			decimal allMoney = 0m;

			status = Convert.ToInt32(parameterRule.GetSysParameterValue(Ajax.Common.CommonEnums.SysParameterEnums.ChargeAutoPass));

			if (customer == null)
			{
				result = "客户不存在";
				return result;
			}
			if (customer.Status != 1)
			{
				result = "客户状态非启用，不能缴费";
				return result;
			}
			Agreements agreement = new AgreementsRule().GetAgreementByCustomerID(CustomerID);

			// 定义缴费主表对象
			Ajax.Model.Charge charge = new Ajax.Model.Charge()
			{
				AgreementID = agreement == null ? null : agreement.ID,
				BeginDate = agreement == null ? customer.BeginChargeDate.Value : agreement.BeginDate,
				EndDate = agreement == null ? customer.BeginChargeDate.Value.AddMonths(ChargeMonth) : agreement.EndDate,
				CreateDate = DateTime.Now,
				CustomerID = CustomerID,
				ID = Guid.NewGuid().ToString("N"),
				IsAgreementCharge = agreement == null ? 0 : 1,
				Money = ChargeMoney,
				OperatorID = operatorID,
				Status = status
			};
			// 协议缴费，不计算缴费明细
			if (agreement != null)
			{
				string temp = chargeDAL.ChargeByAgreement(charge, agreement);
				if (!string.IsNullOrEmpty(temp))
				{
					result = temp;
				}
				else
				{
					result = "缴费成功";
				}
			}
			// 非协议缴费，计算缴费明细
			else
			{
				// 明细缴费
				List<Ajax.Model.ChargeDetail> chargeDetailList = new List<Ajax.Model.ChargeDetail>();		// 缴费明细
				List<CustomerChargeItem> myChargeItem = new List<CustomerChargeItem>();
				myChargeItem = new CustomerChargeItemRule().GetListBycustomerID(CustomerID);
				ChargeItemRule chargeitemRule = new ChargeItemRule();
				//	先添加本客户
				foreach (CustomerChargeItem item in myChargeItem)
				{
					Ajax.Model.ChargeDetail chargeDetail = new Ajax.Model.ChargeDetail()
					{
						ChargeID = charge.ID,
						CreateDate = DateTime.Now,
						ID = Guid.NewGuid().ToString("N"),
						ChargeItemID = item.ItemID,
						ItemMoney = chargeitemRule.GetPriceByItemID(item.ItemID, item.Count, CustomerID) * ChargeMonth,
						Month = ChargeMonth,
						Status = status
					};
					allMoney += chargeDetail.ItemMoney;
					chargeDetailList.Add(chargeDetail);
				}

				List<Customer> childCustomerList = customerRule.GetChildrenCustomer(CustomerID);
				foreach (Customer c in childCustomerList)
				{
					myChargeItem = new CustomerChargeItemRule().GetListBycustomerID(c.ID);
					foreach (CustomerChargeItem item in myChargeItem)
					{
						Ajax.Model.ChargeDetail chargeDetail = new Ajax.Model.ChargeDetail()
						{
							ChargeID = charge.ID,
							CreateDate = DateTime.Now,
							ID = Guid.NewGuid().ToString("N"),
							ChargeItemID = item.ItemID,
							ItemMoney = chargeitemRule.GetPriceByItemID(item.ItemID, item.Count, c.ID) * ChargeMonth,
							Month = ChargeMonth,
							Status = status
						};
						allMoney += chargeDetail.ItemMoney;
						chargeDetailList.Add(chargeDetail);
					}
				}
				charge.Money = allMoney;
				chargeDAL.ChargeByMonth(charge, chargeDetailList.ToArray(), ChargeMonth);
				result = "缴费成功";
			}

			return result;
		}

		/// <summary>
		/// 缴费审核操作
		/// </summary>
		/// <param name="guids"> 缴费记录ID</param>
		/// <param name="isPass">是否通过</param>
		/// <returns></returns>
		public bool ChargeAudit(string guids, bool isPass)
		{
			string[] guidArray = guids.TrimEnd(',').Split(',');
			return dal.ChargeAudit(new List<string>(guidArray), isPass);
		}
		#endregion  Method

		#region 欠费
		/// <summary>
		/// 获取欠费统计信息
		/// </summary>
		/// <param name="areaID">区域ID</param>
		/// <param name="time">欠费时长</param>
		/// <returns></returns>
		public List<dynamic> GetArrearList(string areaID, string time)
		{
			Ajax.DAL.ChargeDAL chargeDAL = new DAL.ChargeDAL();
			return chargeDAL.GetArrearList(areaID, time);
		}
		#endregion


		/// <summary>
		/// 获取计费的月数
		/// </summary>
		/// <param name="beginChargeDate">最后一次缴费时间</param>
		/// <returns></returns>
		public int GetMonthCount(DateTime beginChargeDate)
		{
			TimeSpan tspan = DateTime.Now - beginChargeDate;
			int monthCount = tspan.Days / 30;
			if (monthCount == 0 && beginChargeDate < DateTime.Now)
			{
				monthCount = 1;
			}
			return monthCount;
		}

	}
}

