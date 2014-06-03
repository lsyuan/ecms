using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ajax.Model;
using Ajax.Common;
using Ajax.BLL;
using Web.Common;
using System.IO;

namespace Web.Controllers.System
{
	public class SystemController : Controller
	{
		#region  收费项目管理
		[AccessFilter(PoupEnums.收费项目管理, AccessEnums.Read)]
		public ActionResult ChargeItemMgr()
		{
			return View();
		}
		/// <summary>
		/// 新增缴费项目
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项目管理, AccessEnums.Add)]
		public ActionResult AddChargeItem(ChargeItem item)
		{
			AjaxResult result = new AjaxResult();
			item.ID = Guid.NewGuid().ToString("N").ToUpper();
			item.PY = Pinyin.GetPinyin(item.Name);
			ChargeItemRule rule = new ChargeItemRule();
			item.Code = rule.GetNewCode();
			try
			{
				rule.Add(item);
				result.Success = true;
				result.Message = "缴费项目添加成功。";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 获取收费项目数据json
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项目管理, AccessEnums.Read)]
		public ActionResult SearchChargeItem(EasyUIGridParamModel param, string name)
		{
			int itemCount = 0;
			List<dynamic> ChargeItemList = new ChargeItemRule().SearchChargeItem(param, name, out itemCount);
			var showList = from charge in ChargeItemList
						   select new
						   {
							   ID = charge.ID,
							   Code = charge.CODE,
							   Name = charge.NAME,
							   IsRegular = Convert.ToString(charge.ISREGULAR).Replace("False", "否").Replace("True", "是"),
							   IsPloy = Convert.ToString(charge.ISPLOY).Replace("False", "否").Replace("True", "是"),
							   UnitPrice = string.Format("{0} ", charge.UNITPRICE),
							   Unit = string.Format(" 元 [{0}]", charge.UNIT),
							   CategoryName = charge.CATEGORYNAME,
							   UnitID1 = charge.UNITID1,
							   UnitID2 = charge.UNITID2
						   };
			return Json(new { total = itemCount, rows = showList }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 获取单个缴费项，用于前台列表数据展示
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项目管理, AccessEnums.Read)]
		public ActionResult GetChargeItem(string ID)
		{
			return Json(new ChargeItemRule().GetChargeItem(ID), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 修改缴费项
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项目管理, AccessEnums.Update)]
		public ActionResult ModifyChargeItem(ChargeItem item)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				item.PY = Pinyin.GetPinyin(item.Name);
				bool flag = new ChargeItemRule().Update(item);
				if (flag)
				{
					result.Success = true;
					result.Message = "收费项修改成功";
				}
				else
				{
					result.Success = false;
					result.Message = "收费项修改失败";
				}
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 删除缴费项
		/// </summary>
		/// <param name="ID">缴费项ID</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项目管理, AccessEnums.Delete)]
		public ActionResult DeleteChargeItem(string ID)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				bool flag = new ChargeItemRule().DeleteChargeItem(ID);
				if (flag)
				{
					result.Success = true;
					result.Message = "删除成功。";
				}
				else
				{
					result.Success = false;
					result.Message = "删除失败。";
				}
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}


		#endregion

		#region 收费项分类管理
		/// <summary>
		///收费项分类管理 
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项分类管理, AccessEnums.Read)]
		public ActionResult ChargeItemCategory()
		{
			return View();
		}
		/// <summary>
		/// 新增收费项分类
		/// </summary>
		/// <param name="cType"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项分类管理, AccessEnums.Add)]
		public ActionResult ChargeItemCategoryAdd(ChargeItemCategory cType)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				cType.ID = Guid.NewGuid().ToString("N");
				new Ajax.BLL.ChargeItemCategoryRule().Add(cType);
				result.Success = true;
				result.Message = "收费项分类添加成功。";
			}
			catch
			{
				result.Success = false;
				result.Message = "收费项分类添加失败。";
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 修改收费项分类
		/// </summary>
		/// <param name="cType"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项分类管理, AccessEnums.Update)]
		public ActionResult ModifyChargeItemCategory(ChargeItemCategory cType)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				new Ajax.BLL.ChargeItemCategoryRule().Update(cType);
				result.Success = true;
				result.Message = "收费项分类修改成功。";
			}
			catch
			{
				result.Success = false;
				result.Message = "收费项分类修改失败。";
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 获取所有收费项分类(下拉选择)
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项分类管理, AccessEnums.Read)]
		public ActionResult GetAllChargeItemCategory()
		{
			List<ChargeItemCategory> ChargeItemTypeList = new ChargeItemCategoryRule().GetAllList();
			var showList = from cType in ChargeItemTypeList
						   select new
						   {
							   id = cType.ID,
							   text = cType.Name
						   };
			return Json(showList, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 收费项分类管理 json
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项分类管理, AccessEnums.Read)]
		public ActionResult SearchChargeItemCategory(EasyUIGridParamModel param, ChargeItemCategory chargeItemType)
		{
			int itemCount = 0;
			List<dynamic> CategoryList = new ChargeItemCategoryRule().SearchChargeItemCategory(param, chargeItemType, out itemCount);
			var ShowList = from category in CategoryList
						   select new
						   {
							   ID = category.ID,
							   NAME = category.NAME
						   };
			return Json(new { total = itemCount, rows = ShowList }, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 删除收费项分类
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.收费项分类管理, AccessEnums.Delete)]
		public ActionResult DeleteChargeItemCategory(string ID)
		{
			AjaxResult actionResult = new AjaxResult();
			try
			{
				bool result = new ChargeItemCategoryRule().Delete(ID);
				if (result)
				{
					actionResult.Success = true;
					actionResult.Message = "删除成功";
					return Json(actionResult, JsonRequestBehavior.AllowGet);
				}
				else
				{
					actionResult.Success = false;
					actionResult.Message = "该分类下有子缴费项，不能删除";
					return Json(actionResult, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				actionResult.Success = false;
				actionResult.Message = ex.Message;
				return Json(actionResult, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion

		#region 单位维护
		[AccessFilter(PoupEnums.计费单位维护, AccessEnums.Read)]
		public ActionResult UnitMgr()
		{
			return View();
		}
		/// <summary>
		/// 获取单位列表
		/// </summary>
		/// <param name="level">单位级别，1为计数单位，2为计时单位</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.计费单位维护, AccessEnums.Read)]
		public ActionResult GetUnit(string level)
		{
			/// 如果没有特别指定level，则去计数单位
			if (string.IsNullOrEmpty(level))
			{
				level = "1";
			}
			List<dynamic> unitList = new List<dynamic>();
			if (DataCache.GetCache("UnitLevel" + level) == null)
			{
				unitList = new UnitRule().GetUnit(level);
				DataCache.SetCache("UnitLevel" + level, unitList);
				var showList = from unit in unitList
							   select new
							   {
								   id = unit.ID,
								   text = unit.NAME
							   };
				return Json(showList, JsonRequestBehavior.AllowGet);
			}
			else
			{
				unitList = (List<dynamic>)DataCache.GetCache("UnitLevel" + level);
				var showList = from unit in unitList
							   select new
							   {
								   id = unit.ID,
								   text = unit.NAME
							   };
				return Json(showList, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// 获取单位信息,用于JsonData
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.计费单位维护, AccessEnums.Read)]
		public ActionResult GetSingelUnit(string ID)
		{
			return Json(new UnitRule().GetSingelUnit(ID), JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 收费单位列表查询
		/// </summary>
		/// <param name="param"></param>
		/// <param name="unit"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.计费单位维护, AccessEnums.Read)]
		public ActionResult UnitSearch(EasyUIGridParamModel param, Unit unit)
		{
			int itemCount = 0;
			List<dynamic> uList = new UnitRule().GetSearchJson(param, unit, out itemCount);
			var listResult = from u in uList.ToList()
							 select new
							 {
								 ID = u.ID,
								 Name = u.NAME,
								 Level = u.LEVEL,
								 TimeValue = u.TIMEVALUE
							 };
			return Json(new { total = itemCount, rows = listResult.ToList() }, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 增加单位
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.计费单位维护, AccessEnums.Add)]
		public ActionResult AddUnit(Unit unit)
		{
			AjaxResult result = new AjaxResult();
			unit.PY = Pinyin.GetPinyin(unit.Name);
			unit.Status = 1;
			unit.ID = Guid.NewGuid().ToString().Replace("-", "");
			try
			{
				new UnitRule().Add(unit);
				result.Success = true;
				result.Message = "单位添加成功";
			}
			catch
			{
				result.Success = false;
				result.Message = "单位新增失败。";
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 修改单位
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.计费单位维护, AccessEnums.Update)]
		public ActionResult ModifyUnit(Unit unit)
		{
			unit.PY = Pinyin.GetPinyin(unit.Name);
			unit.Status = 1;
			try
			{
				bool result = new UnitRule().Update(unit);
				if (result)
				{
					return GetSingelUnitValue(unit.ID);
				}
				else
				{
					throw new Exception("更新失败");
				}
			}
			catch (Exception ex)
			{
				unit.State = "1";
				unit.Errormsg = ex.Message;
				return Json(unit, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// 获取单位信息,用于数据更新
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.计费单位维护, AccessEnums.Read)]
		public ActionResult GetSingelUnitValue(string ID)
		{
			return Json(new UnitRule().GetSingelUnitValue(ID), JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 删除单位
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.计费单位维护, AccessEnums.Delete)]
		public ActionResult DeleteUnit(string ID)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				result = new UnitRule().DeleteUnit(ID);
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
				return Json(result, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion

		#region 客户类型管理
		[AccessFilter(PoupEnums.客户类型管理, AccessEnums.Read)]
		public ActionResult CustomerTypeClass()
		{
			return View();
		}
		/// <summary>
		/// 获取所有客户类型json
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户类型管理, AccessEnums.Read)]
		public ActionResult GetAllCustomerType(EasyUIGridParamModel param, CustomerType cType)
		{
			int itemCount = 0;
			List<dynamic> customerTypeList = new CustomerTypeRule().GetAllCustomerType(param, cType, out itemCount);
			var showList = from c in customerTypeList
						   select new
						   {
							   ID = Convert.ToString(c.ID),
							   NAME = c.NAME
						   };
			return Json(new { total = itemCount, rows = showList }, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 新增客户类型
		/// </summary>
		/// <param name="customerType"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户类型管理, AccessEnums.Add)]
		public ActionResult AddCustomerType(CustomerType customerType)
		{
			AjaxResult result = new AjaxResult();
			customerType.ID = Guid.NewGuid().ToString().Replace("-", "");
			try
			{
				if (new CustomerTypeRule().AddCustomerType(customerType))
				{
					result.Success = true;
					result.Message = "客户类型新增成功。";
				}
				else
				{
					result.Success = false;
					result.Message = "客户类型新增失败。";
				}
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "新增失败：" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 删除客户类型
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户类型管理, AccessEnums.Delete)]
		public ActionResult DeleteCustomerType(string ID)
		{
			try
			{
				if (new CustomerTypeRule().DeleteCustomerType(ID))
				{
					return Json(true, JsonRequestBehavior.AllowGet);
				}
				else
				{
					throw new Exception("新增失败");
				}
			}
			catch
			{
				return Json(false, JsonRequestBehavior.AllowGet);
			}
		}
		/// <summary>
		/// 更新客户类型
		/// </summary>
		/// <param name="customerType"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户类型管理, AccessEnums.Update)]
		public ActionResult UpdateCustomerType(CustomerType customerType)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				if (new CustomerTypeRule().UpdateCustomerType(customerType))
				{
					result.Success = true;
					result.Message = "更新成功";
				}
				else
				{
					throw new Exception("更新失败");
				}
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;

			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 更新客户类型缴费项对应
		/// </summary>
		/// <param name="customerTypeID">客户类型ID</param>
		/// <param name="chargeItemArray">缴费项ID数组,以';'分割缴费项ID</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户类型管理, AccessEnums.Update)]
		public ActionResult ModifyTypeToItem(string customerTypeID, string chargeItemArray)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				bool flag = new CustomerTypeRule().ModifyTypeToItem(customerTypeID, chargeItemArray);
				if (flag)
				{
					result.Success = true;
					result.Message = "缴费项设置成功。";
				}
				else
				{
					throw new Exception("缴费项设置失败");
				}
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 获取某个客户类型包含的缴费项目
		/// </summary>
		/// <param name="ID">客户类型ID</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户类型管理, AccessEnums.Read)]
		public ActionResult GetMyChargeItem(string ID)
		{
			var showList = new CustomerTypeRule().GetMyChargeItem(ID);
			return Json(showList, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 获取客户类型json
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.客户类型管理, AccessEnums.Read)]
		public JsonResult GetChargeTypeList()
		{
			return Json(new CustomerTypeRule().GetChargeTypeList(), JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 区域管理
		[AccessFilter(PoupEnums.区域设置, AccessEnums.Read)]
		public ActionResult AreaMgr()
		{
			return View();
		}
		/// <summary>
		/// 获取区域列表
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.区域设置, AccessEnums.Read)]
		public JsonResult GetAreaList(Area area)
		{
			return Json(new AreaRule().GetAreaList(area), JsonRequestBehavior.AllowGet);

		}

		/// <summary>
		/// 获取一个区域信息
		/// </summary>
		/// <param name="ID">区域ID</param>
		/// <returns>区域Json对象</returns>
		[AccessFilter(PoupEnums.区域设置, AccessEnums.Read)]
		public JsonResult GetArea(string ID)
		{
			return Json(new AreaRule().GetArea(ID), JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 新增区域
		/// </summary>
		/// <param name="PID"></param>
		/// <param name="DeptName"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.区域设置, AccessEnums.Read)]
		public JsonResult AddArea(Area area)
		{
			AjaxResult result = new AjaxResult();
			dynamic a = new AreaRule().AddArea(area);
			result.Success = a.Errormsg != null ? false : true;
			result.Message = result.Success ? "地区信息添加成功" : "error:" + a.Errormsg;
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 删除区域
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.区域设置, AccessEnums.Delete)]
		public JsonResult DeleteArea(string ID)
		{
			return Json(new AreaRule().Delete(ID), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 更新区域信息
		/// </summary>
		/// <param name="area">区域对象</param>
		/// <returns>新的区域信息</returns>
		[AccessFilter(PoupEnums.区域设置, AccessEnums.Update)]
		public JsonResult ModifyArea(Area area)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				result.Success = new AreaRule().ModifyArea(area);
				result.Message = result.Success ? "地区信息修改成功。" : "地区信息修改失败。";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "error:" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 获取区域树结构
		/// </summary>
		/// <param name="currentAreaID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.区域设置, AccessEnums.Read)]
		public JsonResult GetAreaTree(Guid? currentAreaID)
		{
			return Json(new AreaRule().GetAreaTree(currentAreaID), JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 获取treeGrid树形json数据
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.区域设置, AccessEnums.Read)]
		public JsonResult GetAllAreaJson()
		{
			List<dynamic> list = new AreaRule().GetTreeGridList();
			List<Object> result = new List<object>();
			foreach (dynamic a in list)
			{
				if (string.IsNullOrEmpty(a.PID))
				{
					result.Add(new { Identifier = a.ID, Area_Name = a.NAME, Manager = a.MANAGER, ManagerName = a.MANAGERNAME });
				}
				else
				{
					result.Add(new { Identifier = a.ID, Area_Name = a.NAME, _parentId = a.PID, Manager = a.MANAGER, ManagerName = a.MANAGERNAME });
				}
			}
			Dictionary<string, object> json = new Dictionary<string, object>();
			json.Add("total", list.Count);
			json.Add("rows", result);
			return Json(json, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 密码修改
		/// <summary>
		/// 修改密码
		/// </summary>
		/// <returns></returns>
		public ActionResult ChangeMyPwd()
		{
			if (MyTicket.CurrentTicket == null)
			{
				return new HomeController().Login();
			}
			return View();
		}
		/// <summary>
		/// 密码修改
		/// </summary>
		/// <param name="old"></param>
		/// <param name="newPwd"></param>
		/// <returns></returns>
		public ActionResult ChangePwd(string old, string newPwd)
		{
			if (MyTicket.CurrentTicket == null)
			{
				return new HomeController().Login();
			}
			OperatorRule rule = new OperatorRule();
			bool result = rule.ChangePwd(MyTicket.CurrentTicket.UserID, old, newPwd);
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 账号管理
		[AccessFilter(PoupEnums.账号管理, AccessEnums.Read)]
		public ActionResult OperatorMgr()
		{
			Ajax.BLL.GroupRule group = new Ajax.BLL.GroupRule();
			List<Ajax.Model.Group> groupList = group.GetAllGroup();
			ViewBag.groupList = groupList;
			ViewBag.CurrentGroupID = "c862b19d482c41f9a48f59bcc4c91fbe";// MyTicket.CurrentTicket.GroupID;//当前用户所属的角色组 
			return View();
		}

		/// <summary>
		/// 系统用户管理json
		/// </summary>
		/// <param name="param"></param>
		/// <param name="oper"></param>
		/// <param name="emp"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.账号管理, AccessEnums.Read)]
		public ActionResult SearchOperator(EasyUIGridParamModel param, Operator oper, Employee emp)
		{
			int itemCount = 0;
			List<dynamic> operList = new OperatorRule().SearchOperator(param, oper, emp, out itemCount);
			var showList = from opers in operList
						   select new
						   {
							   ID = opers.ID,
							   EMPID = opers.EMPID,
							   STATUS = opers.STATUSNAME,
							   NAME = opers.NAME,
							   EMPNAME = opers.EMPNAME,
							   ISADMIN = opers.ISADMIN,
							   CREATEDATE = opers.CREATEDATE,
							   GROUPNAME = opers.GROUPNAME,
						   };
			return Json(new { total = itemCount, rows = showList }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 增加操作员
		/// </summary>
		/// <param name="opr"></param>
		/// <param name="EMPNAME"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.账号管理, AccessEnums.Add)]
		public ActionResult AddOperator(Operator opr, string EMPNAME)
		{
			AjaxResult result = new AjaxResult();
			opr.PY = Pinyin.GetPinyin(opr.Name);
			opr.ID = Guid.NewGuid().ToString("N");
			opr.CreateDate = DateTime.Now;
			opr.Status = 1;
			opr.Pwd = Ajax.Common.DEncrypt.DESEncrypt.Encrypt(opr.Pwd);
			OperatorRule rule = new OperatorRule();
			try
			{
				rule.Add(opr);
				result.Success = true;
				result.Message = "添加成功。";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "添加失败：" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 修改操作员
		/// </summary>
		/// <param name="opr"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.账号管理, AccessEnums.Update)]
		public ActionResult ModifyOperator(Operator opr)
		{
			AjaxResult result = new AjaxResult();
			opr.PY = Pinyin.GetPinyin(opr.Name);
			opr.Status = 1;
			opr.Pwd = Ajax.Common.DEncrypt.DESEncrypt.Encrypt(opr.Pwd);
			try
			{
				new OperatorRule().Update(opr);
				result.Success = true;
				result.Message = "修改成功。";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "修改失败：" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 删除操作员
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.账号管理, AccessEnums.Delete)]
		public ActionResult DeleteOperator(string IDs)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				result.Success = new OperatorRule().DeleteOperator(IDs.TrimEnd(',').Split(','));
				result.Message = result.Success ? "删除成功。" : "删除失败。";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "删除失败:" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 禁用操作员
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.账号管理, AccessEnums.Update)]
		public JsonResult OperatorDisable(string IDs)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				OperatorRule rule = new OperatorRule();
				result.Success = rule.OperatorDisable(IDs.TrimEnd(',').Split(','));
				result.Message = result.Success ? "系统用户状态更新成功。" : "系统用户状态更新失败！";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "系统用户状态更新失败:" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 自动完成输入联想
		/// </summary>
		/// <param name="q"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.账号管理, AccessEnums.Read)]
		public JsonResult GetOperatorByName(string q)
		{
			if (string.IsNullOrEmpty(q))
			{
				return Json(new { }, JsonRequestBehavior.AllowGet);
			}
			List<Operator> OperList = new OperatorRule().GetOperatorByName(q);
			var showlist = from oper in OperList
						   select new
						   {
							   id = oper.ID,
							   text = oper.Name
						   };
			return Json(showlist, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 获取单个操作员信息
		/// </summary>
		/// <param name="Id">操作员ID</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.账号管理, AccessEnums.Read)]
		public JsonResult GetSingelOperatorInfo(string Id)
		{
			return Json(new OperatorRule().GetSingelOperator(Id), JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region 角色管理
		/// <summary>
		/// 获取绑定下拉的json数据源
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.角色管理, AccessEnums.Read)]
		public ActionResult GroupSelectList()
		{
			List<Group> groupList = new GroupRule().GetAllGroup();
			return Json(groupList, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 角色选择视图
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.角色管理, AccessEnums.Read)]
		public ActionResult SelectGroup()
		{
			List<Group> groupList = new GroupRule().GetAllGroup();
			return PartialView(groupList);
		}

		/// <summary>
		/// 角色管理
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.角色管理, AccessEnums.Read)]
		public ActionResult Group()
		{
			return View();
		}
		/// <summary>
		/// 角色查询json
		/// </summary>
		/// <param name="param"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.角色管理, AccessEnums.Read)]
		public ActionResult SearchGroup(EasyUIGridParamModel param, Group group)
		{
			int itemCount = 0;
			List<dynamic> groupList = new GroupRule().SearchGroup(param, group, out itemCount);
			var showList = from groups in groupList
						   select new
						   {
							   ID = groups.ID,
							   NAME = groups.NAME
						   };
			return Json(new { total = itemCount, rows = showList }, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 新增角色
		/// </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.角色管理, AccessEnums.Add)]
		public ActionResult AddGroup(Group g)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				new GroupRule().AddGroup(g);
				result.Success = true;
				result.Message = "添加成功";
			}
			catch
			{
				result.Success = false;
				result.Message = "添加失败";
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 编辑角色
		/// </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.角色管理, AccessEnums.Update)]
		public ActionResult ModifyGroup(Group g)
		{
			AjaxResult result = new AjaxResult();
			try
			{
				new GroupRule().ModifyGroup(g);
				result.Success = true;
				result.Message = "修改成功";
			}
			catch
			{
				result.Success = false;
				result.Message = "修改失败";
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 删除角色
		/// </summary>
		/// <param name="guids"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.角色管理, AccessEnums.Delete)]
		public ActionResult DeleteGroup(string guids)
		{
			AjaxResult result = new AjaxResult();
			string[] guidArray = guids.Trim(';').Split(';');
			bool flag = new GroupRule().DeleteGroup(new List<string>(guidArray));
			if (flag)
			{
				result.Success = true;
				result.Message = "角色删除成功";
			}
			else
			{
				result.Success = false;
				result.Message = "已存在用户的角色不能删除";
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 根据角色ID获取权限列表
		/// </summary>
		/// <param name="groupID">角色ID</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.角色管理, AccessEnums.Read)]
		public ActionResult GetGroupVoteByID(string groupID)
		{
			List<dynamic> voteList = new GroupVoteRule().GetOperatorVoteByID(groupID);
			return Json(voteList, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 保存权限设置
		/// </summary>
		/// <param name="voteList">权限列表</param>
		/// <param name="groupID">角色ID</param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.角色管理, AccessEnums.Read)]
		public ActionResult SaveGroupVoteSetting(List<GroupVote> voteList, string groupID)
		{
			AjaxResult reslut = new AjaxResult();
			foreach (GroupVote gv in voteList)
			{
				gv.ID = Guid.NewGuid().ToString("N");
				gv.GroupID = groupID;
			}
			try
			{
				new GroupVoteRule().SaveGroupVoteSetting(voteList);
				reslut.Success = true;
				reslut.Message = "权限保存成功。";
			}
			catch (Exception ex)
			{
				reslut.Success = false;
				reslut.Message = "保存失败：" + ex.Message;
			}
			return Json(reslut, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region 首页信息显示
		/// <summary>
		/// 系统待处理提示信息
		/// </summary>
		/// <returns></returns>
		public ActionResult TipMsg()
		{
			if (MyTicket.CurrentTicket == null)
			{
				return new HomeController().Login();
			}
			return View();
		}
		/// <summary>
		/// 加载最近10条公告
		/// </summary>
		/// <returns></returns>
		public ActionResult LoadSystemMsg()
		{
			if (MyTicket.CurrentTicket == null)
			{
				return new HomeController().Login();
			}
			var showList = new MessageRule().GetLatestMsg(10, MyTicket.CurrentTicket.UserID);
			return Json(showList, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 加载协议到期提醒
		/// </summary>
		/// <returns></returns>
		public ActionResult LoadTipContracts()
		{
			if (MyTicket.CurrentTicket == null)
			{
				return new HomeController().Login();
			}
			var showList = new AgreementsRule().GetTipContracts(10);
			return Json(showList, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 系统自定义参数设置


		/// <summary>
		/// 系统自定义参数设置
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.系统设置)]
		public ActionResult Parameter()
		{
			return View();
		}

		/// <summary>
		/// 获取系统所有参数
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.系统设置)]
		public JsonResult GetAllSysParameter()
		{
			Ajax.BLL.SysParameterRule paramters = new Ajax.BLL.SysParameterRule();
			List<Ajax.Model.SysParameter> list = new List<Ajax.Model.SysParameter>();
			list = paramters.GetModelList("");
			return Json(list, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 保存系统所有参数
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.系统设置)]
		public JsonResult SaveAllSysParameter(List<Ajax.Model.SysParameter> list)
		{
			AjaxResult result = new AjaxResult();
			result.Success = new SysParameterRule().UpdateSysParameter(list);
			result.Message = result.Success ? "保存成功。" : "保存失败！";
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 登录日志

		/// <summary>
		/// 登录日志
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.登陆日志管理)]
		public ActionResult LoginLog()
		{
			return View();
		}
		/// <summary>
		/// 查询登录日志
		/// </summary>
		/// <param name="log"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		[AccessFilter(PoupEnums.登陆日志管理, AccessEnums.Read)]
		public ActionResult SearchLoginLog(EasyUIGridParamModel param, string OperatorID, string startDate, string endDate)
		{
			int itemCount = 0;
			DateTime sDate = TimeParser.ConvertStringToDateTime(startDate);
			DateTime eDate = TimeParser.ConvertStringToDateTime(endDate);
			List<dynamic> logList = new LoginLogRule().Search(param, OperatorID, sDate, eDate, out itemCount);
			var showList = from loginLog in logList
						   select new
						   {
							   ID = loginLog.ID,
							   OperatorName = loginLog.OPERATORID,
							   Type = loginLog.TYPE.Replace("1", "登录").Replace("0", "退出"),
							   CreateTime = loginLog.CREATETIME,
							   oprname = loginLog.OPRNAME
						   };
			return Json(new { total = itemCount, rows = showList }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 清除日志
		/// </summary>
		[AccessFilter(PoupEnums.登陆日志管理, AccessEnums.Delete)]
		public JsonResult DeleteAllLoginLog()
		{
			AjaxResult result = new AjaxResult();
			new LoginLogRule().DeleteAll();
			result.Success = true;
			result.Message = "日志清空完成。";
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}
