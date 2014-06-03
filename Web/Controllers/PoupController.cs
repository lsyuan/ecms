using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajax.Model;
using Ajax.BLL;
using Web.Common;

namespace Web.Controllers.System
{
    public class PoupController : Controller
    {
        //
        // GET: /Poup/
        [AccessFilter(PoupEnums.菜单管理, AccessEnums.Read)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 返回菜单的所有节点，不分页
        /// </summary>
        /// <returns></returns>
        [AccessFilter(PoupEnums.菜单管理, AccessEnums.Read)]
        public ActionResult GetPoupList(Poup poup)
        {
            List<object> list = new PoupRule().GetPoupList(poup, "");
            // 暂时不添加任何验证，默认返回全部菜单节点
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据ID获取菜单节点数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.菜单管理, AccessEnums.Read)]
        public ActionResult GetPoup(string id)
        {
            return Json(new PoupRule().GetPoup(id), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增菜单节点，返回新增菜单节点的数据
        /// </summary>
        /// <param name="poup"></param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.菜单管理, AccessEnums.Add)]
        public ActionResult AddPoup(Poup poup)
        {
            // o为新增菜单节点的ID
            object o = new PoupRule().AddPoup(poup);
            if (o != null)
            {
                return GetPoup(o.ToString());
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 更新菜单节点
        /// </summary>
        /// <param name="ID">ID</param>
        /// <param name="Name">菜单名称</param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.菜单管理, AccessEnums.Update)]
        public ActionResult ModifyPoup(string ID, string Name, string Path)
        {
            if (ID == "00000000000000000000000000000000")
            {
                throw new Exception("错误：不允许修改系统根节点");
            }
            if (new PoupRule().ModifyPoup(ID, Name, Path))
            {
                return GetPoup(ID);
            }
            else
            {
                throw new Exception("更新菜单节点失败");
            }
        }
        /// <summary>
        /// 删除根节点
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.菜单管理, AccessEnums.Delete)]
        public ActionResult DeletePoup(string ID)
        {
            if (ID == "00000000000000000000000000000000")
            {
                throw new Exception("错误：不允许删除系统根节点");
            }
            PoupRule rule = new PoupRule();
            try
            {
                return Json(rule.DeletePoup(ID), JsonRequestBehavior.AllowGet);
            }
            catch
            {
            }
            return null;
        }
    }
}
