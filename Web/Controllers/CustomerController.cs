using System;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using Ajax.Common;
using System.Collections.Generic;
using Ajax.BLL;
using Ajax.Model;
using Web.Common;

namespace Web.Controllers.Customer
{
    /// <summary>
    /// 客户信息
    /// </summary>
    public class CustomerController : Controller
    {
        //
        // GET: /Customer/

        /// <summary>
        /// 新增客户
        /// </summary>
        /// <returns></returns>
        [AccessFilter(PoupEnums.客户管理, AccessEnums.Add)]
        public ActionResult Add()
        {
            return View();
        }
        /// <summary>
        /// 选择用户的公用view
        /// </summary>
        /// <param name="customerParentID">父级客户ID（为空时默认为所有父级单位）</param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.客户信息查询, AccessEnums.Read)]
        public ActionResult SelectCustomer(string customerParentID)
        {
            List<Ajax.Model.Customer> customerList;
            if (string.IsNullOrEmpty(customerParentID))
            {
                customerList = new CustomerRule().GetList("and PID IS NULL ");
            }
            else
            {
                customerList = new CustomerRule().GetList(string.Format("and PID='{0}' ", customerParentID));
            }
            return PartialView(customerList);
        }

        /// <summary>
        /// 客户审核
        /// </summary>
        /// <returns></returns>
        [AccessFilter(PoupEnums.客户审批, AccessEnums.Update)]
        public ActionResult Confirm()
        {
            return View();
        }
        /// <summary>
        /// 审批操作
        /// </summary>
        /// <param name="customerID">需要审批的客户ID</param>
        /// <param name="status">1通过，4未通过</param>
        /// <returns></returns>
        [HttpPost]
        [AccessFilter(PoupEnums.客户审批, AccessEnums.Update)]
        public ActionResult Audit(string customerID, int status)
        {
            AjaxResult result = new AjaxResult();
            if (!string.IsNullOrEmpty(customerID))
            {
                CustomerRule cr = new CustomerRule();
                try
                {
                    cr.Audit(new List<string>(customerID.TrimEnd(',').Split(',')), status);
                    result.Success = false;
                    result.Message = "审批成功.";
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "操作失败：" + ex.Message;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="customerIDs"></param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.客户审批, AccessEnums.Update)]
        public ActionResult SetEnabled(string customerIDs, int status)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                if (!string.IsNullOrEmpty(customerIDs))
                {
                    string[] customerIDArray = customerIDs.TrimEnd(';').Split(';');
                    new CustomerRule().SetEnabled(new List<string>(customerIDArray), status);
                }
                result.Success = true;
                result.Message = "状态更新成功";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 客户信息查询
        /// </summary>
        /// <returns></returns>
        [AccessFilter(PoupEnums.客户信息查询, AccessEnums.Read)]
        public ActionResult Query()
        {
            //地区选择绑定
            AreaRule area = new AreaRule();
            ViewBag.area = area.GetAllList();
            return View();
        }

        /// <summary>
        /// 查询客户json数据
        /// </summary>
        /// <returns></returns>
        [AccessFilter(PoupEnums.客户信息查询, AccessEnums.Read)]
        public ActionResult Search(EasyUIGridParamModel param, Ajax.Model.Customer customer)
        {
            int itemCount = 0;
            CustomerRule cr = new CustomerRule();
            List<dynamic> list = cr.Search(param, customer, out itemCount);
            var showList = from customers in list
                           select new
                           {
                               ID = customers.ID,
                               Name = customers.NAME,
                               TypeName = customers.CUSTOMERTYPE,
                               Status = customers.STATUS.Replace("1", "正常").Replace("0", "未审核").Replace("2", "禁用").Replace("3", "已删除"),
                               CreateTime = Ajax.Common.TimeParser.FormatDateTime(customers.CREATETIME),
                               Contactor = customers.CONTACTOR,
                               FeeType = string.IsNullOrEmpty(customers.AGREEID) ? "非协议缴费" : "协议缴费",
                               Address = customers.ADDRESS
                           };
            return Json(new { total = itemCount, rows = showList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 增加客户信息，返回table显示数据
        /// </summary>
        /// <param name="customer">客户对象</param>
        /// <param name="ccItems">客户缴费项对应表</param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.新增客户, AccessEnums.Add)]
        public ActionResult AddCustomer(Ajax.Model.Customer customer, List<CustomerChargeItem> ccItems)
        {
            AjaxResult result = new AjaxResult();
            customer.ID = Guid.NewGuid().ToString("N");
            customer.CreateTime = DateTime.Now;
            customer.UpdateTime = DateTime.Now;
            customer.PY = Pinyin.GetPinyin(customer.Name);
            customer.Status = 0;//默认无效，需要审核
            customer.OperatorID = MyTicket.CurrentTicket.EmployeeID;
            customer.Code = 0;
            //对应缴费项
            foreach (CustomerChargeItem ccItem in ccItems)
            {
                ccItem.ID = WebHelper.GetNewGuidUpper();
                ccItem.CustomerID = customer.ID;
            }
            try
            {
                new CustomerRule().Add(customer, ccItems);
                result.Success = true;
                result.Message = "客户添加成功";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 客户信息修改
        /// </summary>
        /// <param name="customer">客户对象</param>
        /// <param name="ccItems">客户缴费项对应表</param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.新增客户, AccessEnums.Update)]
        public JsonResult CustomerModify(Ajax.Model.Customer customer, List<CustomerChargeItem> ccItems)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                customer.UpdateTime = DateTime.Now;
                customer.PY = Pinyin.GetPinyin(customer.Name);
                //对应缴费项
                foreach (CustomerChargeItem ccItem in ccItems)
                {
                    ccItem.ID = Guid.NewGuid().ToString("N");
                    ccItem.CustomerID = customer.ID;
                }
                new CustomerRule().Modify(customer, ccItems);
                result.Success = true;
                result.Message = "客户信息修改成功。";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 客户删除
        /// </summary>
        /// <param name="customerID">客户ID</param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.新增客户, AccessEnums.Delete)]
        public JsonResult CustomerDelete(string customerID)
        {
            CustomerRule rule = new CustomerRule();
            return Json(rule.Delete(customerID), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 客户所有类型json
        /// </summary>
        /// <returns></returns>
        [AccessFilter(PoupEnums.客户信息查询, AccessEnums.Read)]
        public JsonResult CustomerAllTypeJson()
        {
            List<CustomerType> list = new CustomerTypeRule().GetAllList();
            var json = from ctype in list
                       select new
                       {
                           id = ctype.ID,
                           text = ctype.Name
                       };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取客户下拉列表
        /// </summary>
        /// <param name="q"></param>
        /// <param name="IsParent"></param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.客户信息查询, AccessEnums.Read)]
        public ActionResult GetCustomerListByID(string q, string IsParent)
        {
            List<Ajax.Model.Customer> customerList = new List<Ajax.Model.Customer>();

            customerList = new CustomerRule().GetCustomerListByID(q, IsParent);
            var showList = from customers in customerList
                           select new
                           {
                               id = customers.ID,
                               text = customers.Name
                           };
            return Json(showList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取单个客户完整信息
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.客户信息查询, AccessEnums.Read)]
        public JsonResult CustomerDetail(string customerID)
        {
            if (!string.IsNullOrEmpty(customerID))
            {
                // 客户信息
                var customer = new CustomerRule().CustomerDetail(customerID);

                // 缴费项目信息
                List<dynamic> chargeItemList = new ChargeItemRule().SearchChargeItem(customerID);

                var chargeItem = from chargeItems in chargeItemList
                                 select new
                                 {
                                     Name = chargeItems.NAME,
                                     Price = chargeItems.UNITPRICE,
                                     Count = chargeItems.COUNT,
                                     AgreeMentMoney = chargeItems.AGREEMENTMONEY,
                                     ItemCount = 0
                                 };
                // 协议信息
				var agreements = new AgreementsRule().GetAgreementObjectByCustomerID(customerID);
                // 子客户信息
                var childCustomer = new CustomerRule().GetChildrenCustomer(customerID);
                // 缴费历史
                //int itemCount = 0;
                var chargeHistory = new ChargeRule().ChargeSearch(customerID);
                // 欠费记录
                var arrearRecord = new YearEndArrearRule().GetArrearRecordByCustomerID(customerID);
                //chargeHistory,
                return Json(new { customer, chargeItem, agreements, childCustomer, arrearRecord, chargeHistory }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new Ajax.Model.Customer());
            }
        }

        /// <summary>
        /// 获取子客户信息
        /// </summary>
        /// <param name="customerID">客户ID</param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.客户信息查询, AccessEnums.Read)]
        public JsonResult GetCustomerChildren(string customerID)
        {
            var childCustomer = new CustomerRule().GetChildrenCustomer(customerID);
            return Json(childCustomer, JsonRequestBehavior.AllowGet);
        }

        [AccessFilter(PoupEnums.客户信息查询, AccessEnums.Read)]
        public JsonResult CustomerDetailOnly(string customerID)
        {
            if (!string.IsNullOrEmpty(customerID))
            {
                // 客户信息
                var customer = new CustomerRule().CustomerDetail(customerID);
                return Json(customer, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new Ajax.Model.Customer());
            }
        }
        /// <summary>
        /// 根据客户guid或者客户类型获取对应缴费项
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <param name="customerTypeID">客户类型编号</param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.客户信息查询, AccessEnums.Read)]
        public ActionResult GetChargeItem(string customerID, string customerTypeID)
        {
            List<dynamic> chargeItems = new CustomerRule().GetChargeItemByCustomerID(customerID);
            if (chargeItems.Count == 0)
            {
                chargeItems = new CustomerRule().GetChargeItemByCustomerTypeID(customerTypeID);
            }
            var showList = from cItems in chargeItems
                           select new
                           {
                               ID = cItems.ID,
                               Code = cItems.CODE,
                               Name = cItems.NAME,
                               UnitID1 = cItems.UNIT,
                               CategoryID = cItems.CATEGORYID,
                               IsRegular = cItems.ISREGULAR,
                               IsAgreeMent = cItems.ISAGREEMENT,
                               IsPloy = cItems.ISPLOY,
                               UnitPrice = cItems.UNITPRICE,
                               Count = cItems.COUNT,
                               AgreementMoney = cItems.AGREEMENTMONEY
                           };
            return Json(showList, JsonRequestBehavior.AllowGet);
        }
    }
}
