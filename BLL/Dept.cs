using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;

namespace Ajax.BLL
{
    /// <summary>
    /// 部门表
    /// </summary>
    public partial class DeptRule
    {
        private readonly Ajax.DAL.DeptDAL dal = new Ajax.DAL.DeptDAL();
        public DeptRule()
        { }
        #region  Method
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Dept model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Dept model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID)
        {

            return dal.Delete(ID);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            return dal.DeleteList(IDlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Dept GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Dept GetModelByCache(string ID)
        {

            string CacheKey = "DeptModel-" + ID;
            object objModel = DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = dal.GetModel(ID);
                    if (objModel != null)
                    {
                        int ModelCache = ConfigHelper.GetConfigInt("ModelCache");
                        DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
                    }
                }
                catch { }
            }
            return (Dept)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Dept> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Dept> GetModelList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Dept> DataTableToList(DataTable dt)
        {
            List<Dept> modelList = new List<Dept>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                Dept model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new Dept();
                    if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
                    {
                        model.ID = dt.Rows[n]["ID"].ToString();
                    }
                    if (dt.Rows[n]["PID"] != null && dt.Rows[n]["PID"].ToString() != "")
                    {
                        model.PID = dt.Rows[n]["PID"].ToString();
                    }
                    if (dt.Rows[n]["Code"] != null && dt.Rows[n]["Code"].ToString() != "")
                    {
                        model.Code = dt.Rows[n]["Code"].ToString();
                    }
                    if (dt.Rows[n]["Name"] != null && dt.Rows[n]["Name"].ToString() != "")
                    {
                        model.Name = dt.Rows[n]["Name"].ToString();
                    }
                    if (dt.Rows[n]["PY"] != null && dt.Rows[n]["PY"].ToString() != "")
                    {
                        model.PY = dt.Rows[n]["PY"].ToString();
                    }
                    if (dt.Rows[n]["Status"] != null && dt.Rows[n]["Status"].ToString() != "")
                    {
                        model.Status = Convert.ToInt32(dt.Rows[n]["Status"].ToString());
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }


        #endregion  Method

        /// <summary>
        /// 获取部门动态对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramName"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public object GetDeptDynamic(string sql, string[] paramName, string[] p)
        {
            return dal.GetDeptDynamic(sql, paramName, p);
        }
        /// <summary>
        /// 获取部门动态对象集合
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramName"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<dynamic> GetDeptDynamicList(string sql, string[] paramName, string[] p)
        {
            return dal.GetDeptDynamicList(sql, paramName, p);
        }
        /// <summary>
        /// 获取部门编码
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramName"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public string GetDeptCode(string sql, string[] paramName, string[] p)
        {
            return dal.GetDeptCode(sql, paramName, p);
        }
        /// <summary>
        /// 更新部门信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool Update(string ID, string Name)
        {
            return dal.Update(ID, Name);
        }
    }
}

