using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajax.BLL;
using Web.Common;
using Ajax.Model;
using Ajax.Common;

namespace Web.Controllers
{
	public class PrintController : Controller
	{
		#region 打印催缴单
		/// <summary>
		/// 欠费催缴单打印
		/// </summary>
		/// <returns></returns>
		public ActionResult ArrearTipPrint()
		{
			return View();
		}
		/// <summary>
		/// 获取欠费详细信息
		/// </summary>
		/// <param name="customerIDList"></param>
		/// <returns></returns>
		public JsonResult GetArrearInfo(string customerIDs)
		{
			CustomerRule cusRule = new CustomerRule();
			AgreementsRule agreeRule = new AgreementsRule();
			ChargeRule chRule = new ChargeRule();
			ChargeItemRule chargeItemRule = new ChargeItemRule();

			List<object> arrearList = new List<object>();
			string[] customerIDArray = customerIDs.Split(',');
			foreach (string customerID in customerIDArray)
			{
				// 客户信息
				var customer = cusRule.CustomerDetail(customerID);
				// 协议信息
				var agreements = agreeRule.GetAgreementObjectByCustomerID(customerID);
				decimal totalFee = chRule.CaculateCustomerFee(customerID);
				//缴费总月份
				Ajax.Model.Customer c = new CustomerRule().GetModel(customerID);
				int monthCount = chRule.GetMonthCount(Convert.ToDateTime(c.BeginChargeDate));
				// 缴费项目信息
				List<dynamic> chargeItemList = new ChargeItemRule().SearchChargeItem(customerID);
				var chargeItem = from chargeItems in chargeItemList
								 select new
								 {
									 Name = chargeItems.NAME,
									 Price = chargeItemRule.GetPriceByItemID(chargeItems.ID, Convert.ToDecimal(chargeItems.COUNT), customerID),
									 Count = chargeItems.COUNT,
									 AgreeMentMoney = chargeItems.AGREEMENTMONEY,
									 ItemCount = monthCount * CaculateItemCount(chargeItems.AGREEMENTMONEY, Convert.ToString(chargeItemRule.GetPriceByItemID(chargeItems.ID, Convert.ToDecimal(chargeItems.COUNT), customerID)), chargeItems.COUNT)
								 };
				//子客户费用信息
				List<Ajax.Model.Customer> customerChildrenList = new CustomerRule().GetChildrenCustomer(customerID);
				var childrenCustomer = from childC in customerChildrenList
									   select new
									   {
										   Name = childC.Name,
										   Fee = chRule.CaculateCustomerFee(childC.ID, false)
									   };

				arrearList.Add(new { customer, agreements, monthCount, totalFee, chargeItem, childrenCustomer });
			}
			return Json(arrearList, JsonRequestBehavior.AllowGet);
		}
		#endregion
		#region private
		/// <summary>
		/// 单项小计
		/// </summary>
		/// <returns></returns>
		private decimal CaculateItemCount(string AgreeMentMoney, string Price, string Count)
		{
			decimal AMondy = Convert.ToDecimal(AgreeMentMoney);
			if (AMondy != 0)
			{
				return AMondy;
			}
			else
			{
				return Convert.ToDecimal(Price) * Convert.ToDecimal(Count);
			}
		}
		#endregion
	}
}
