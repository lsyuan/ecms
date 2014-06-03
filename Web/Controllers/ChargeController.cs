using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ajax.BLL;
using Ajax.Common;
using Ajax.Model;
using Web.Common;

namespace Web.Controllers
{
	/// <summary>
	/// 缴费信息
	/// </summary>
	public class ChargeController : Controller
	{

		#region 临时缴费
		/// <summary>
		/// 临时收费view
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.临时收费)]
		public ActionResult TempCharge()
		{
			return View();
		}
		/// <summary>
		/// 临时缴费记录表
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.临时收费, AccessEnums.Read)]
		public ActionResult Search(EasyUIGridParamModel param, TempCharge tCharge, TempChargeDetail tChargeDetail)
		{
			int itemCount = 0;
			List<dynamic> chargeList = new Ajax.BLL.TempChargeRule().Search(param, tCharge, out itemCount);
			var showList = from tempCharge in chargeList
						   select new
						   {
							   ID = tempCharge.ID,
							   CUSTOMERNAME = tempCharge.CUSTOMERNAME,
							   CHARGENAME = tempCharge.CHARGENAME,
							   MONEY = tempCharge.MONEY,
							   CREATETIME = tempCharge.CREATETIME,
							   NAME = tempCharge.NAME
						   };
			return Json(new { total = itemCount, rows = showList }, JsonRequestBehavior.AllowGet);

		}
		/// <summary>
		/// 新增临时收费记录
		/// <param name="tCharge"></param>
		/// <param name="tChargeDetail"></param>
		/// </summary>
		[AccessFilter(PoupEnums.临时收费, AccessEnums.Add)]
		public ActionResult TempChargeAdd(TempCharge tCharge, List<TempChargeDetail> tChargeDetail)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				tCharge.ID = Guid.NewGuid().ToString("N");
				tCharge.OperatorID = MyTicket.CurrentTicket.UserID;
				new TempChargeRule().AddTempCharge(tCharge, tChargeDetail);
				result.Success = true;
				result.Message = "收费成功";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 是否周期性缴费
		/// </summary>
		/// <param name="isRegular"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项目管理, AccessEnums.Read)]
		public ActionResult ChargeItem(string isRegular)
		{
			List<dynamic> chargeList = new ChargeItemRule().GetChargeItemForCheckBox(isRegular);
			var showList = from charge in chargeList
						   select new
						   {
							   ID = charge.ID,
							   NAME = charge.NAME,
							   PRICE = charge.UNITPRICE
						   };
			return Json(showList, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 根据周期性收费缴费项json
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项目管理, AccessEnums.Read)]
		public ActionResult SelectChargeItemByType()
		{
			List<dynamic> list = new ChargeItemRule().SelectChargeItemByType();
			ViewBag.ChargeItemList=list;
			return PartialView();
		}
		#endregion

		#region 客户缴费
		/// <summary>
		/// 客户缴费
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户缴费, AccessEnums.Read)]
		public ActionResult CustomerCharge()
		{
			return View();
		}

		/// <summary>
		/// 获取
		/// </summary>
		/// <param name="customerID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项目管理, AccessEnums.Read)]
		public ActionResult SearchChargeItem(string customerID)
		{
			List<dynamic> chargeItemList = new ChargeItemRule().SearchChargeItem(customerID);
			if (chargeItemList != null)
			{
				var showList = from chargeItems in chargeItemList
							   select new
							   {
								   Name = chargeItems.NAME,
								   Price = chargeItems.UNITPRICE,
								   Count = chargeItems.COUNT,
								   AgreeMentMoney = chargeItems.AGREEMENTMONEY,
								   ItemCount = 0
							   };
				return Json(new { total = chargeItemList.Count, rows = showList }, JsonRequestBehavior.AllowGet);
			}
			return Json(new { total = 0, rows = "" }, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 获取指定客户的所有子客户及其缴费信息
		/// </summary>
		/// <param name="customerID">父客户编号</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户信息查询, AccessEnums.Read)]
		public ActionResult SearchCustomerChildrenList(string customerID)
		{
			List<dynamic> childrenChargeItems = new ChargeItemRule().GetCustomerChildrenInfo(customerID);
			if (childrenChargeItems != null)
			{
				var showList = from childrenChargeItem in childrenChargeItems
							   select new
							   {
								   Name = childrenChargeItem.NAME,
								   AreaName = childrenChargeItem.AREANAME,
								   Address = childrenChargeItem.ADDRESS,
								   ChargeName = childrenChargeItem.CHARGENAME,
								   Price = childrenChargeItem.UNITPRICE,
								   Count = childrenChargeItem.COUNT,
								   ItemCount = 0
							   };
				return Json(new { total = childrenChargeItems.Count, rows = showList }, JsonRequestBehavior.AllowGet);
			}
			return Json(new { total = 0, rows = "" }, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 计算客户总费用
		/// </summary>
		/// <param name="customerId">客户id</param>
		/// <returns></returns>
		private ActionResult CaculateCustomerTotalFee(string customerId)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				result.Success = true;
				result.Message = new ChargeRule().CaculateCustomerFee(customerId).ToString("n2");
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "计费出错：" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 保存客户缴费记录
		/// </summary>
		/// <param name="customerId">客户ID</param>
		/// <param name="chargeMonth">缴费月份</param>
		/// <param name="chargeMoney">缴费金额</param>
		/// <returns>
		/// Msg:缴费成功
		/// Sucess:缴费状态，true/false
		/// </returns> 
		[AccessFilter(PoupEnums.客户缴费, AccessEnums.Add)]
		public ActionResult SaveFeeRecord(string customerId, int chargeMonth, decimal chargeMoney)
		{
			if (string.IsNullOrEmpty(customerId) || chargeMonth < 1 || chargeMoney < 0)
			{
				return Json(false, JsonRequestBehavior.AllowGet);
			}
			string msg = new ChargeRule().Charge(customerId, chargeMonth, chargeMoney, MyTicket.CurrentTicket.UserID);
			return Json(msg, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 客户缴费审核一览查询
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户缴费, AccessEnums.Read)]
		public ActionResult CustomerChargeConfirmSearch(EasyUIGridParamModel param)
		{
			int count = 0;
			List<dynamic> chargeList = new ChargeRule().ChargeSearch("", param, out count);
			var showList = from charge in chargeList
						   select new
						   {
							   ID = charge.ID,
							   CustomerName = charge.CUSTOMERNAME,
							   Money = charge.MONEY,
							   CreateTime = charge.CREATEDATE,
							   OperName = charge.OPERATORNAME,
							   Detail = "查看详情"
						   };
			return Json(new { total = count, rows = showList }, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 缴费审核
		/// <summary>
		/// 缴费审核
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户缴费, AccessEnums.Read)]
		public ActionResult Confirm()
		{
			return View();
		}

		/// <summary>
		/// 临时缴费审核查询
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户缴费, AccessEnums.Read)]
		public ActionResult TempChargeConfirmSearch(EasyUIGridParamModel param)
		{
			int itemCount = 0;
			TempCharge tc = new TempCharge();
			tc.Status = 0;
			List<dynamic> tempCharges = new TempChargeRule().Search(param, tc, out itemCount);
			var showList = from tempCharge in tempCharges
						   select new
						   {
							   ID = tempCharge.ID,
							   CustomerName = tempCharge.CUSTOMERNAME,
							   ChargeName = tempCharge.CHARGENAME,
							   Money = tempCharge.MONEY,
							   CreateTime = tempCharge.CREATETIME,
							   OperName = tempCharge.NAME,
							   Detail = "查看详细"
						   };
			return Json(new { total = tempCharges.Count, rows = showList }, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 加载临时缴费详细
		/// </summary>
		/// <param name="chargeID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户缴费, AccessEnums.Read)]
		public ActionResult LoadTempChargeDetail(string chargeID)
		{
			List<dynamic> tempChargeDetals = new TempChargeDetailRule().GetTempleChargeDetailByID(chargeID);
			return Json(tempChargeDetals, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 临时缴费审核操作
		/// </summary>
		/// <param name="guids">临时缴费记录ID</param>
		/// <param name="isPass">是否通过</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.临时收费, AccessEnums.Update)]
		public ActionResult TempChargeAudit(string guids, bool isPass)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				result.Success = new TempChargeRule().TempChargeAudit(guids, isPass);
				result.Message = result.Success ? "审核成功。" : "审核操作失败。";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "审核失败：" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}


		/// <summary>
		/// 缴费审核操作
		/// </summary>
		/// <param name="guids">缴费记录ID，逗号分隔</param>
		/// <param name="isPass">是否通过</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户缴费, AccessEnums.Update)]
		public ActionResult ChargeAudit(string guids, bool isPass)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				result.Success = new ChargeRule().ChargeAudit(guids, isPass);
				result.Message = result.Success ? "审核成功。" : "审核操作失败。";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "审核失败：" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region 缴费策略
		/// <summary>
		/// 获取指定缴费项的分级缴费策略
		/// </summary>
		/// <param name="chargeItemID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项目管理, AccessEnums.Read)]
		public ActionResult LoadChargePolicy(string chargeItemID)
		{
			List<dynamic> policyList = new PolyRule().GetPolicyListByItemID(chargeItemID);
			var showList = from policy in policyList
						   select new
						   {
							   ID = policy.ID,
							   UNITPRICE = policy.UNITPRICE,
							   LOWERBOUND = policy.LOWERBOUND,
							   HIGNERBOUND = policy.HIGNERBOUND == int.MaxValue.ToString() ? "以上" : policy.HIGNERBOUND
						   };
			return Json(showList, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 保存缴费项的分级策略
		/// </summary>
		/// <param name="ployList">分级策略列表</param>
		/// <param name="itemID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项目管理, AccessEnums.Update)]
		public ActionResult SaveChargePolicy(List<Poly> ployList)
		{
			AjaxResult result = new AjaxResult();
			if (ployList == null && ployList.Count == 0)
			{
				result.Success = false;
				result.Message = "策略保存失败：分级策略为空";
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			string delItemID = ployList[0].ItemID;
			try
			{
				if (ployList == null)
				{
					ployList = new List<Poly>();
				}
				foreach (Poly p in ployList)
				{
					p.ID = Guid.NewGuid().ToString("N");
					if (p.HignerBound == -1)
					{
						p.HignerBound = int.MaxValue;
					}
				}
				new PolyRule().SavePolys(ployList, delItemID);
				result.Success = true;
				result.Message = "策略保存成功。";
			}
			catch (Exception ex)
			{
				result.Success = true;
				result.Message = "策略保存失败：" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 缴费历史记录
		/// <summary>
		/// 获取指定客户的所有子客户及其缴费信息
		/// </summary>
		/// <param name="customerID">父客户编号</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户缴费, AccessEnums.Read)]
		public ActionResult ChargeSearch(string customerID)
		{
			if (string.IsNullOrEmpty(customerID))
			{
				return Json(new { total = 0, rows = "" }, JsonRequestBehavior.AllowGet);
			}
			List<dynamic> historyChargeList = new ChargeRule().ChargeSearch(customerID);
			if (historyChargeList != null)
			{
				return Json(new { total = historyChargeList.Count, rows = historyChargeList }, JsonRequestBehavior.AllowGet);
			}
			return Json(new { total = 0, rows = "" }, JsonRequestBehavior.AllowGet);
		}
		#endregion

	}
}
