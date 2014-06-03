using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajax.BLL;
using Ajax.Common;
using Ajax.Model;
using System.Text;
using Web.Common;

namespace Web.Controllers.Internal
{
    /// <summary>
    /// 职工管理
    /// </summary>
    public class EmployeeController : Controller
    {
        [AccessFilter(PoupEnums.职员管理, AccessEnums.Read)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取职员的json
        /// </summary>
        /// <returns></returns>
        [AccessFilter(PoupEnums.职员管理, AccessEnums.Read)]
        public ActionResult Search(EasyUIGridParamModel param, Employee emp)
        {
            int ItemCount = 0;
            List<dynamic> list = new EmployeeRule().GetSearchJson(param, emp, out ItemCount);
            var listResult = from m in list.ToList()
                             select new
                             {
                                 ID = m.ID,
                                 NAME = m.NAME,
                                 SEX = m.SEX,
                                 BIRTHDATE = m.BIRTHDATE,
                                 CARDTYPE = m.CARDTYPE,
                                 CARDID = m.CARDID,
                                 ADDRESS = m.ADDRESS,
                                 OfficePhone = m.OFFICEPHONE,
                                 DEPTNAME = m.DEPTNAME,
                                 CURRENTSTATUS = m.STATUS.Replace("1", "在职").Replace("3", "停职")
                             };
            return Json(new { total = ItemCount, rows = listResult.ToList() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取某个职员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.职员管理, AccessEnums.Read)]
        public ActionResult GetEmployee(string empID)
        {
            object empObj = new EmployeeRule().GetEmployee(empID);
            return Json(empObj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 更新职员信息
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.职员管理, AccessEnums.Update)]
        public ActionResult ModifyEmployee(Employee emp)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                emp.PY = Pinyin.GetPinyin(emp.Name);
                bool flag = new EmployeeRule().Update(emp);
                if (flag)
                {
                    result.Success = true;
                    result.Message = "客户信息更新成功。";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "客户信息更新失败:" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增职工信息
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.职员管理, AccessEnums.Add)]
        public ActionResult AddEmployee(Employee emp)
        {
            AjaxResult result = new AjaxResult();
            emp.ID = Guid.NewGuid().ToString("N");
            emp.PY = Pinyin.GetPinyin(emp.Name);
            emp.Status = 1;
            try
            {
                new EmployeeRule().Add(emp);
                result.Success = true;
                result.Message = "职员信息新增成功。";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "职员信息新增失败。" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除职员
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.职员管理, AccessEnums.Delete)]
        public ActionResult DeleteEmployee(string guids)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                if (!string.IsNullOrEmpty(guids))
                {
                    new EmployeeRule().DeleteList(guids.TrimEnd(','));
                }
                result.Success = true;
                result.Message = "删除成功。";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "删除失败：" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据职员姓名获取下拉列表
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.职员管理, AccessEnums.Read)]
        public ContentResult GetEmployeeListByName(string q)
        {
            if (!string.IsNullOrEmpty(q))
            {
                q = q.Trim().ToUpper();
            }
            else
            {
                return Content(string.Empty);
            }
            var empList = new EmployeeRule().QueryEmployee(q);
            StringBuilder sbresult = new StringBuilder();
            foreach (dynamic p in empList)
            {
                sbresult.Append(p.NAME);
                sbresult.Append("|");
                sbresult.Append(p.ID);
                sbresult.Append("|");
                sbresult.Append(p.DEPTID);
                sbresult.Append("|");
                sbresult.Append(p.DEPTNAME);
                sbresult.Append("\n");
            }
            return Content(sbresult.ToString());
        }
        /// <summary>
        /// 职员姓名输入联想框
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.职员管理, AccessEnums.Read)]
        public ActionResult GetEmployeesByName(string q)
        {
            List<dynamic> emps = new EmployeeRule().QueryEmployee(q);
            var empList = from emp in emps
                          select new
                          {
                              id = emp.ID,
                              text = emp.NAME
                          };
            return Json(empList, JsonRequestBehavior.AllowGet);
        }
    }
}

