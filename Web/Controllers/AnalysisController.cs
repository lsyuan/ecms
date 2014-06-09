using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajax.BLL;
using Ajax.Model;
using Web.Common;

namespace Web.Controllers
{
	/// <summary>
	/// 统计分析
	/// </summary>
	public class AnalysisController : Controller
	{
		#region 职工收费统计
		/// <summary>
		/// 职工收费统计view页面
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.职工收费统计)]
		public ActionResult EmpChargeAnalysis()
		{
			return View();
		}
		/// <summary>
		/// 职工收费图表统计json请求
		/// </summary>
		/// <param name="beginDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.职工收费统计, AccessEnums.Read)]
		public JsonResult AnalysisEmpCharge(string beginDate, string endDate)
		{
			List<dynamic> mepChargeList = new EmployeeRule().EmpChargeAnalysis(beginDate, endDate);
			var showList = from empc in mepChargeList
						   select new
						   {
							   name = empc.NAME,
							   y = Convert.ToDouble(string.IsNullOrEmpty(empc.FEECOUNT) ? 0 : empc.FEECOUNT)
						   };
			return Json(showList, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 区域收费统计
		/// <summary>
		/// 区域收费统计view页面
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.区域收费统计)]
		public ActionResult AreaCharge()
		{
			return View();
		}
		/// <summary>
		/// 区域收费图表统计json请求
		/// </summary>
		/// <param name="beginDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.区域收费统计, AccessEnums.Read)]
		public JsonResult AnalysisAreaCharge(string pID, string beginDate, string endDate)
		{
			List<dynamic> areaAnalysisList = new AreaRule().AreaChargeAnalysis(pID, beginDate, endDate);
			var showList = from areaAnalysis in areaAnalysisList
						   select new
						   {
							   name = areaAnalysis.NAME,
							   y = Convert.ToDouble(areaAnalysis.FEECOUNT)
						   };
			return Json(showList, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 欠费管理
		/// <summary>
		/// 欠费管理页面
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.欠费提醒, AccessEnums.Read)]
		public ActionResult Arrear()
		{
			ViewBag.OrgName = "五莲环卫管理处";
			return View();
		}
		/// <summary>
		/// 获取欠费统计信息
		/// </summary>
		/// <param name="areaID">区域ID</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.欠费提醒, AccessEnums.Read)]
		public JsonResult GetArrearList(string areaID, string time)
		{
			return Json(new ChargeRule().GetArrearList(areaID, time), JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Reports
		/// <summary>
		/// 欠费统计
		/// </summary>
		/// <returns></returns>
		public ActionResult FeeStastics()
		{
			ChargeRule cRule = new ChargeRule();
			var customerArrearList = cRule.GetArrearList("", "");
			var stasticsList = from f in customerArrearList
							   select new
							   {
								   name = f.NAME,
								   areaName = f.AREANAME,
								   fee = cRule.CaculateCustomerFee(f.ID)
							   };
			List<Area> areaList = new AreaRule().GetList(@" and Pid in(select ID from T_area where PID is null) ");
			List<string> areaFirstLevel = new List<string>();
			foreach (Area a in areaList)
			{
				areaFirstLevel.Add(a.Name);
			}
			return Json(new { stasticsList = stasticsList, areaList = areaFirstLevel }, JsonRequestBehavior.AllowGet);
		}

		[AccessFilter(PoupEnums.报表分析, AccessEnums.Read)]
		public ActionResult Reports()
		{
			return View();
		}
		#endregion

		#region IntergrateQuery

		[AccessFilter(PoupEnums.综合查询, AccessEnums.Read)]
		public ActionResult IntergrateQuery()
		{
			return View();
		}
		#endregion
	}
}
