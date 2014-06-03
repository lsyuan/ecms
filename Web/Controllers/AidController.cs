using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajax.BLL;
using Ajax.Model;
using Ajax.Common;
using Web.Common;

namespace Web.Controllers
{
	/// <summary>
	/// 辅助功能
	/// </summary>
	public class AidController : Controller
	{
		#region view页面
		/// <summary>
		/// view 其他缴费
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.其他缴费)]
		public ActionResult AnotherCharge()
		{
			return View();
		}

		#endregion
		/// <summary>
		/// 其他缴费一览json
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.其他缴费, AccessEnums.Read)]
		public ActionResult AnotherChargeSearch(EasyUIGridParamModel param, AnotherCharge aCharge)
		{
			int itemCount = 0;
			List<dynamic> chargeList = new AnotherChargeRule().AnotherChargeSearch(param, aCharge, out itemCount);
			var showList = from anotherCharge in chargeList
						   select new
						   {
							   ID = anotherCharge.ID,
							   CustomerName = anotherCharge.CUSTOMERNAME,
							   Money = anotherCharge.MONEY,
							   ActMoney = anotherCharge.ACTMONEY,
							   Remark = anotherCharge.REMARK,
							   ChargeDate = TimeParser.FormatDateTime(anotherCharge.CHARGEDATE),
							   OperatorName = anotherCharge.OPERATORNAME,
							   Detail = "查看详细"
						   };
			return Json(new { total = chargeList.Count, rows = showList }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 新增其他收费
		/// </summary>
		/// <param name="aCharge"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.其他缴费, AccessEnums.Add)]
		public ActionResult AddAnotherCharge(AnotherCharge aCharge)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				aCharge.ID = Guid.NewGuid().ToString("N");
				aCharge.OperatorID = MyTicket.CurrentTicket.UserID;
				aCharge.ChargeDate = DateTime.Now;
				new AnotherChargeRule().Add(aCharge);
				result.Success = true;
				result.Message = "收费成功。";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "收费失败：" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 加载其他缴费详细
		/// </summary>
		/// <param name="chargeID">缴费记录ID</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.其他缴费, AccessEnums.Read)]
		public ActionResult LoadOtherChargeDetail(string chargeID)
		{
			dynamic aCharge = new AnotherChargeRule().GetModelByID(chargeID);
			return Json(aCharge, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 缴费审核操作
		/// </summary>
		/// <param name="guids">缴费记录ID，逗号分隔</param>
		/// <param name="isPass">是否通过</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.其他缴费, AccessEnums.Update)]
		public ActionResult ChargeAudit(string guids, bool isPass)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				result.Success = new AnotherChargeRule().Aduit(guids, isPass);
				result.Message = result.Success ? "审核成功。" : "审核操作失败。";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "审核失败：" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		#region WriteOff

		public ActionResult WriteOff()
		{
			return View();
		}
		#endregion

		#region CourtDeterminaton
		public ActionResult CourtDeterminaton()
		{
			return View();
		}
		#endregion
	}
}
