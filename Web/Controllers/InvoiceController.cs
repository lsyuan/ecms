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
    public class InvoiceController : Controller
    {
        #region view页面
        /// <summary>
        /// 发票管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 票据分类一览页面
        /// </summary>
        /// <returns></returns>
        public ActionResult TypeMgr()
        {
            return View();
        }
        #endregion

        #region 票据分类操作
        /// <summary>
        /// 票据分类一览数据
        /// </summary>
        /// <returns></returns>
        public ActionResult TypeSearch(EasyUIGridParamModel param)
        {
            int itemCount = 0;
            List<dynamic> inTypeList = new InvoiceTypeRule().Search(param, new InvoiceType(), out itemCount);
            var showList = from inType in inTypeList
                           select new
                           {
                               ID = inType.ID,
                               Name = inType.NAME,
                               Standard = inType.STANDARD,
                               Code = inType.CODE,
                               StepValue = inType.STEPVALUE
                           };
            return Json(new { total = itemCount, rows = showList }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增票据分类
        /// </summary>
        /// <param name="IType"></param>
        /// <returns></returns>
        public ActionResult AddInvoiceType(InvoiceType IType)
        {
            AjaxResult result = new AjaxResult();
            IType.ID = Guid.NewGuid().ToString("N");
            try
            {
                new InvoiceTypeRule().Add(IType);
                result.Success = true;
                result.Message = "添加成功。";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "error:" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增票据分类
        /// </summary>
        /// <param name="IType"></param>
        /// <returns></returns>
        public ActionResult EditInvoiceType(InvoiceType IType)
        {
            AjaxResult result = new AjaxResult();
            if (!string.IsNullOrEmpty(IType.ID))
            {
                result.Success = new InvoiceTypeRule().Update(IType);
                result.Message = result.Success ? "修改成功。" : "修改失败";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除票据分类
        /// </summary>
        /// <param name="TypeID"></param>
        /// <returns></returns>
        public ActionResult DeleteInvoiceType(string TypeID)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                result.Success = new InvoiceTypeRule().Delete(TypeID);
                result.Message = result.Success ? "删除成功。" : "删除失败。";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "error：" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取所有票据类型json
        /// </summary>
        /// <returns></returns>
        public ActionResult InvoiceAllTypeJson()
        {
            List<InvoiceType> invoiceTypeList = new InvoiceTypeRule().GetList(null);
            var showList = from inType in invoiceTypeList
                           select new
                           {
                               id = inType.ID,
                               text = inType.Name
                           };
            return Json(showList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 票据管理
        /// <summary>
        /// 票据管理一览json
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult InvoiceSearch(EasyUIGridParamModel param, string InvoiceType)
        {
            int itemCount = 0;
            string strWhere = string.IsNullOrEmpty(InvoiceType) ? "" : string.Format(" and InvoiceType='{0}' ", InvoiceType);
            List<dynamic> IrList = new InvoiceRegisterRule().Search(param, strWhere, out itemCount);
            var showList = from Ir in IrList
                           select new
                           {
                               ID = Ir.ID,
                               BeginCode = Ir.BEGINCODE,
                               EndCode = Ir.ENDCODE,
                               OperatorName = Ir.NAME,
                               CurrentCode = Ir.CURRENTCODE,
                               UseStatus = Ir.USESTATUS == "0" ? "已登记未使用" : "正在使用",
                               InvoiceTypeName = Ir.INVOICETYPENAME,
                               InvoiceType = Ir.INVOICETYPE
                           };
            return Json(new { total = itemCount, rows = showList }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增票据登记
        /// </summary>
        /// <param name="IR"></param>
        /// <returns></returns>
        public ActionResult AddInvoice(InvoiceRegister IR)
        {
            AjaxResult result = new AjaxResult();
            IR.ID = Guid.NewGuid().ToString("N");
            IR.RegisterTime = DateTime.Now;
            IR.OperatorID = MyTicket.CurrentTicket.EmployeeID;
            IR.UseStatus = 0;
            try
            {
                new InvoiceRegisterRule().Add(IR);
                result.Success = true;
                result.Message = "票据已经成功登记。";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "票据登记失败" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 编辑票据
        /// </summary>
        /// <param name="IR"></param>
        /// <returns></returns>
        public ActionResult ModifyInvoice(InvoiceRegister IR)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                result.Success = new InvoiceRegisterRule().Update(IR);
                result.Message = result.Success ? "更新成功" : "更新失败";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
