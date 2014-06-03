using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajax.Model;
using Ajax.BLL;
using Ajax.Common;
using Web.Common;

namespace Web.Controllers
{
	public class BusinessController : Controller
	{
		//
		// GET: /Business/
		/// <summary>
		/// 协议管理
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.协议管理, AccessEnums.Add | AccessEnums.Update)]
		public ActionResult ContrastMgr()
		{
			return View();
		}

		/// <summary>
		/// 获取收费协议json
		/// </summary>
		/// <param name="param"></param>
		/// <param name="agree"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.协议管理)]
		public ActionResult SearchAgree(EasyUIGridParamModel param, Agreements agree, Ajax.Model.Customer c)
		{
			int itemCount = 0;
			List<dynamic> agreeList = new AgreementsRule().SearchAgree(param, agree, c, out itemCount);
			var showList = from a in agreeList
						   select new
						   {
							   ID = a.ID,
							   CONTRASTID = a.ID,
							   CUSTOMERID = a.CUSTOMERID,
							   CODE = a.CODE,
							   NAME = a.NAME,
							   MONEY = a.MONEY,
							   BEGINDATE = a.BEGINDATE,
							   AGREEMENTCODE = a.AGREEMENTCODE,
							   ENDDATE = TimeParser.FormatDateTime(a.ENDDATE),
							   STATUS = Convert.ToString(a.STATUS).Replace("0", "未审核").Replace("1", "已审核").Replace("2", "已删除")
						   };
			return Json(new { total = itemCount, rows = showList }, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 新增协议
		/// </summary>
		/// <param name="agree"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.协议管理, AccessEnums.Add)]
		public ActionResult AddAgree(Agreements agree)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				agree.CheckTime = DateTime.MaxValue;
				agree.ID = WebHelper.GetNewGuidUpper();
				agree.CheckOperatorID = MyTicket.CurrentTicket.UserID;
				agree.CreateTime = DateTime.Now;
				new AgreementsRule().Add(agree);
				result.Success = true;
				result.Message = "协议添加成功";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "协议添加失败:" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 编辑协议
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.协议管理, AccessEnums.Update)]
		public ActionResult ModifyAgree(Agreements a)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				result.Success = new AgreementsRule().Update(a);
				result.Message = "协议修改成功。";
			}
			catch
			{
				result.Success = false;
				result.Message = "协议修改失败。";
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 拒绝协议
		/// </summary>
		/// <param name="guids"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.协议管理, AccessEnums.Update)]
		public ActionResult RefuseAgree(string guids)
		{
			AjaxResult result = new AjaxResult();
			string[] guidArray = guids.Trim(';').Split(';');
			new AgreementsRule().RefuseAgree(new List<string>(guidArray), StatusEnum.AgreeStatusEnum.Delete);
			result.Success = true;
			result.Message = "保存成功。";
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 通过未审核的协议
		/// </summary>
		/// <param name="guids"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.协议管理, AccessEnums.Update)]
		public ActionResult ApprovalAgree(string guids)
		{
			AjaxResult result = new AjaxResult();
			string[] guidArray = guids.Trim(';').Split(';');
			new AgreementsRule().RefuseAgree(new List<string>(guidArray), StatusEnum.AgreeStatusEnum.Approval);
			result.Success = true;
			result.Message = "保存成功。";
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 通过所有未审核的协议
		/// </summary>
		/// <param name="guids"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.协议管理, AccessEnums.Update)]
		public ActionResult ApprovalAll()
		{
			AjaxResult result = new AjaxResult();
			new AgreementsRule().ApprovalAll();
			result.Success = true;
			result.Message = "保存成功。";
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		#region Arrive
		[AccessFilter(PoupEnums.以物顶账, AccessEnums.Read)]
		public ActionResult Arrive()
		{
			return View();
		}
		#endregion
	}
}
