using System;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using Ajax.Common;
using System.Collections.Generic;
using Ajax.BLL;
using Ajax.Model;
using Web.Common;

namespace Web.Controllers
{
	public class AccountController : Controller
	{

		#region 年终结算

		public ActionResult YearBalance()
		{
			return View();
		}
		/// <summary>
		/// 获取年终总结一览数据
		/// </summary>
		/// <param name="param"></param>
		/// <param name="yeArrear"></param>
		/// <returns></returns>
		public ActionResult YearBalanceSearch(EasyUIGridParamModel param, YearEndArrear yeArrear)
		{
			int itemCount = 0;
			var YeArrearList = new YearEndArrearRule().Search(param, yeArrear, out itemCount);
			var showList = from ya in YeArrearList
						   select new
						   {
							   ID = ya.ID,
							   Name = ya.NAME,
							   Money = ya.MONEY,
							   Status = GetYearEndArrearStatus(ya.STATUS),
							   Year = ya.YEAR,
							   ChargeDate = ya.CHARGEDATE
						   };
			return Json(new { total = itemCount, rows = showList }, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 标记为坏账
		/// </summary>
		/// <param name="strID"></param>
		/// <returns></returns>
		public ActionResult MarkBadFee(string strID)
		{
			AjaxResult result = new AjaxResult();
			result.Success = new YearEndArrearRule().UpdateStatus(strID, "2");
			result.Message = result.Success ? "标记成功。" : "标记失败！";
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 执行年终结算
		/// </summary>
		/// <returns></returns>
		public ActionResult CaculateYearFee(string strYear)
		{
			AjaxResult result = new AjaxResult();
			if (string.IsNullOrEmpty(strYear))
			{
				strYear = DateTime.Now.AddYears(-1).Year.ToString();
			}
			try
			{
				new YearEndArrearRule().CaculateYearFee(strYear);
				result.Success = true;
				result.Message = "年终结算执行完成。";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 期初欠费
		/// <summary>
		/// view page
		/// </summary>
		/// <returns></returns>
		public ActionResult NewArrear()
		{
			return View();
		}
		/// <summary>
		/// 新增期初欠费记录
		/// </summary>
		/// <param name="fMoney"></param>
		/// <returns></returns>
		public ActionResult AddNewArrear(FirstMoney fMoney)
		{
			AjaxResult result = new AjaxResult();
			fMoney.ID = Guid.NewGuid().ToString("N");
			fMoney.Status = 0;
			try
			{
				new FirstMoneyRule().Add(fMoney);
				result.Success = true;
				result.Message = "期初欠费添加成功";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 获取期初欠费一览数据
		/// </summary>
		/// <param name="param"></param>
		/// <param name="customerID"></param>
		/// <returns></returns>
		public ActionResult NewArrearSearch(EasyUIGridParamModel param, FirstMoney fMoney)
		{
			int itemCount = 0;
			List<dynamic> newArrearList = new FirstMoneyRule().Search(param, fMoney, out itemCount);
			var showList = from newArrear in newArrearList
						   select new
						   {
							   ID = newArrear.ID,
							   Name = newArrear.NAME,
							   Year = newArrear.YEAR,
							   Money = newArrear.MONEY,
							   Status = GetFirstMoneyStatus(newArrear.STATUS),
							   ChargeDate = newArrear.CHARGEDATE
						   };
			return Json(new { total = itemCount, rows = showList, footer = new List<dynamic>() { new { Money = showList.Sum(t => Convert.ToInt32(Convert.ToDecimal(t.Money)).ToString("0.00")), Name = "合计" } } }, JsonRequestBehavior.AllowGet);

		}
		/// <summary>
		/// 缴费
		/// </summary>
		/// <returns></returns>
		public ActionResult Charge(string customerID)
		{
			AjaxResult result = new AjaxResult();
			result.Success = new FirstMoneyRule().UpdateStatus(customerID, 1);
			result.Message = result.Success ? "缴费成功，等待审核。" : "缴费失败！";
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 期初欠费审核
		/// </summary>
		/// <param name="guids"></param>
		/// <param name="isPass"></param>
		/// <returns></returns>
		public ActionResult ChargeAudit(string guids, bool isPass)
		{
			AjaxResult result = new AjaxResult();
			result.Success = new FirstMoneyRule().ChargeAudit(guids, isPass);
			result.Message = result.Success ? "审核成功。" : "审核失败！";
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 期初欠费记录删除
		/// </summary>
		/// <param name="customerID"></param>
		/// <returns></returns>
		public ActionResult Delete(string customerID)
		{
			AjaxResult result = new AjaxResult();
			result.Success = new FirstMoneyRule().UpdateStatus(customerID, 3);
			result.Message = result.Success ? "期初欠费记录已成功删除。" : "欠费记录删除失败！";
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 账单查询

		public ActionResult BillQuery()
		{
			return View();
		}

		#endregion

		#region private method
		/// <summary>
		/// 获取期初欠费记录状态文本描述
		/// </summary>
		/// <param name="strStatus"></param>
		/// <returns></returns>
		private string GetFirstMoneyStatus(string strStatus)
		{
			string strResult = strStatus;
			switch (strStatus)
			{
				case "0":
					strResult = "未缴费";
					break;
				case "1":
					strResult = "待审核";
					break;
				case "2":
					strResult = "已缴费";
					break;
				case "3":
					strResult = "已删除";
					break;
			}
			return strResult;
		}
		/// <summary>
		/// 获取年终欠费记录状态
		/// </summary>
		/// <param name="strStatus"></param>
		/// <returns></returns>
		private string GetYearEndArrearStatus(string strStatus)
		{
			string strResult = strStatus;
			switch (strStatus)
			{
				case "0":
					strResult = "欠费未缴纳";
					break;
				case "1":
					strResult = "欠费已缴纳";
					break;
				case "2":
					strResult = "呆坏账";
					break;
			}
			return strResult;
		}
		#endregion
	}
}
