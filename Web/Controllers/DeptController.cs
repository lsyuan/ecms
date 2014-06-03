using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ajax.BLL;
using Ajax.Model;
using Ajax.Common;
using System.Text;
using Web.Common;

namespace Web.Controllers.Internal
{
    /// <summary>
    /// 部门管理
    /// </summary>
    public class DeptController : Controller
    {
        //
        // GET: /Dept/
        [AccessFilter(PoupEnums.部门管理, AccessEnums.Read)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <returns></returns>
        [AccessFilter(PoupEnums.部门管理, AccessEnums.Read)]
        public JsonResult GetDeptList(Dept dept)
        {
            StringBuilder temp = new StringBuilder("select id,pid,name,code,status,case status when 0 then '在用' else '停用' end as statusName from t_dept where 1=1 ");
            if (dept != null)
            {
                if (!string.IsNullOrEmpty(dept.Code))
                {
                    temp.Append(" and code like '" + dept.Code + "%'");
                }
                if (!string.IsNullOrEmpty(dept.ID))
                {
                    temp.Append(" and ID = '" + dept.ID + "'");
                }
                if (string.IsNullOrEmpty(dept.PID))
                {
                    temp.Append(" and PID is null");
                }
                else
                {
                    temp.Append(" and pid = '" + dept.PID + "'");
                }
                if (!string.IsNullOrEmpty(dept.Name))
                {
                    temp.Append(" and Name like '%" + dept.Name + "%'");
                }
                if (!string.IsNullOrEmpty(dept.PY))
                {
                    temp.Append(" and NamePY like '%" + dept.PY + "%'");
                }
                temp.Append(" and status = " + dept.Status);
            }
            List<object> lists = new List<object>();
            lists = new DeptRule().GetDeptDynamicList(temp.ToString(), null, null);
            return Json(lists, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取一个部门信息
        /// </summary>
        /// <param name="ID">部门ID</param>
        /// <returns>部门Json对象</returns>
        [AccessFilter(PoupEnums.部门管理, AccessEnums.Read)]
        public JsonResult GetDept(string ID)
        {
            StringBuilder temp = new StringBuilder("select id,pid,name,code,status,case status when 0 then '在用' else '停用' end as statusName from t_dept where ID=@ID");
            object o = new DeptRule().GetDeptDynamic(temp.ToString(), new string[] { "ID" }, new string[] { ID });
            return Json(o, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取所有部门的json数据
        /// </summary>
        /// <returns></returns>
        [AccessFilter(PoupEnums.部门管理, AccessEnums.Read)]
        public JsonResult GetAllDeptJson()
        {
            List<Dept> deptList = new DeptRule().GetModelList("");
            List<Object> result = new List<object>();
            foreach (Dept d in deptList)
            {
                if (string.IsNullOrEmpty(d.PID))
                {
                    result.Add(new { Identifier = d.ID, Dept_Name = d.Name, Dept_Status = d.Status == 1 ? "正常" : "删除" });
                }
                else
                {
                    result.Add(new { Identifier = d.ID, Dept_Name = d.Name, _parentId = d.PID, Dept_Status = d.Status == 1 ? "正常" : "删除" });
                }
            }
            Dictionary<string, object> json = new Dictionary<string, object>();
            json.Add("total", deptList.Count);
            json.Add("rows", result);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="PID"></param>
        /// <param name="DeptName"></param>
        /// <returns></returns>
        [AccessFilter(PoupEnums.部门管理, AccessEnums.Add)]
        public JsonResult AddDept(string PID, string DeptName)
        {

            string selectCode = "select max(code) from t_dept where PID = @PID";
            DeptRule rule = new DeptRule();
            string code = rule.GetDeptCode(selectCode, new string[] { "PID" }, new string[] { PID });
            string selectPCode = "select code from t_dept where ID = @ID";
            string PCode = rule.GetDeptCode(selectPCode, new string[] { "ID" }, new string[] { PID });
            if (string.IsNullOrEmpty(code))
                code = PCode + "0001";
            else
                code = code.Substring(0, code.Length - 4) + (Convert.ToInt32(code.Substring(code.Length - 4)) + 1).ToString().PadLeft(4, '0');
            string id = Guid.NewGuid().ToString().Replace("-", "");
            Dept dept = new Dept() { ID = id, PY = Pinyin.GetPinyin(DeptName), Status = 1, Code = code, PID = PID, Name = DeptName };
            rule.Add(dept);
            string sql = "select id,pid,name,code,status,case status when 0 then '在用' else '停用' end as statusName from t_dept where id=@ID";
            return Json(rule.GetDeptDynamic(sql, new string[] { "ID" }, new string[] { id }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <returns></returns>
        [AccessFilter(PoupEnums.部门管理, AccessEnums.Delete)]
        public JsonResult Delete(string ID)
        {
            DeptRule rule = new DeptRule();
            try
            {
                return Json(rule.Delete(ID), JsonRequestBehavior.AllowGet);
            }
            catch
            {
            } return null;
        }

        /// <summary>
        /// 更新部门信息
        /// </summary>
        /// <param name="dept">部门对象</param>
        /// <returns>新的部门信息</returns>
        [AccessFilter(PoupEnums.部门管理, AccessEnums.Update)]
        public JsonResult Modify(Dept dept)
        {
            DeptRule rule = new DeptRule();
            try
            {
                if (rule.Update(dept.ID, dept.Name))
                {
                    return GetDept(dept.ID);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                return null;
            }
        }

        //public JsonResult GetInfo()
        //{
        //    string start = Request.Params["start"].ToString();
        //    string limit = Request.Params["limit"].ToString();
        //    List<infos> rows = new List<infos>();
        //    for (int i = 0; i < 20; i++)
        //    {
        //        infos info = new infos();
        //        info.Address = i.ToString();
        //        info.Ip = "192.168.1.1";
        //        info.Name = DateTime.Now.ToShortDateString();
        //        rows.Add(info);
        //    }
        //    int total = rows.Count;
        //    rows = rows.GetRange(Convert.ToInt16(start) * Convert.ToInt16(limit), Convert.ToInt16(limit));
        //    return Json(new { rows, total }, JsonRequestBehavior.AllowGet);
        //}

    }

    //public class infos
    //{
    //    public string Address;
    //    public string Ip;
    //    public string Name;
    //}
}
