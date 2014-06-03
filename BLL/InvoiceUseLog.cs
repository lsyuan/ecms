using System;
using System.Collections.Generic;
using System.Data;

namespace Ajax.BLL
{
    /// <summary>
    /// 票据使用记录
    /// </summary>
    public partial class InvoiceUseLogRule
    {
        private readonly Ajax.DAL.InvoiceUseLogDAL dal = new Ajax.DAL.InvoiceUseLogDAL();
        public InvoiceUseLogRule()
        { }
        #region  Method
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID, string InvoiceCode, string OperatorID)
        {
            return dal.Exists(ID, InvoiceCode, OperatorID);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Ajax.Model.InvoiceUseLog model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.InvoiceUseLog model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID, string InvoiceCode, string OperatorID)
        {

            return dal.Delete(ID, InvoiceCode, OperatorID);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Ajax.Model.InvoiceUseLog GetModel(string ID, string InvoiceCode, string OperatorID)
        {

            return dal.GetModel(ID, InvoiceCode, OperatorID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.InvoiceUseLog GetModelByCache(string ID, string InvoiceCode, string OperatorID)
        {

            string CacheKey = "InvoiceUseLogModel-" + ID + InvoiceCode + OperatorID;
            object objModel = Ajax.Common.DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = dal.GetModel(ID, InvoiceCode, OperatorID);
                    if (objModel != null)
                    {
                        int ModelCache = Ajax.Common.ConfigHelper.GetConfigInt("ModelCache");
                        Ajax.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
                    }
                }
                catch { }
            }
            return (Ajax.Model.InvoiceUseLog)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Ajax.Model.InvoiceUseLog> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Ajax.Model.InvoiceUseLog> DataTableToList(DataTable dt)
        {
            List<Ajax.Model.InvoiceUseLog> modelList = new List<Ajax.Model.InvoiceUseLog>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                Ajax.Model.InvoiceUseLog model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new Ajax.Model.InvoiceUseLog();
                    if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
                    {
                        model.ID = dt.Rows[n]["ID"].ToString();
                    }
                    if (dt.Rows[n]["InvoiceCode"] != null && dt.Rows[n]["InvoiceCode"].ToString() != "")
                    {
                        model.InvoiceCode = dt.Rows[n]["InvoiceCode"].ToString();
                    }
                    if (dt.Rows[n]["OperatorID"] != null && dt.Rows[n]["OperatorID"].ToString() != "")
                    {
                        model.OperatorID = dt.Rows[n]["OperatorID"].ToString();
                    }
                    if (dt.Rows[n]["Status"] != null && dt.Rows[n]["Status"].ToString() != "")
                    {
                        model.Status = int.Parse(dt.Rows[n]["Status"].ToString());
                    }
                    if (dt.Rows[n]["BusinessID"] != null && dt.Rows[n]["BusinessID"].ToString() != "")
                    {
                        model.BusinessID = dt.Rows[n]["BusinessID"].ToString();
                    }
                    if (dt.Rows[n]["BusinessType"] != null && dt.Rows[n]["BusinessType"].ToString() != "")
                    {
                        model.BusinessType = int.Parse(dt.Rows[n]["BusinessType"].ToString());
                    }
                    if (dt.Rows[n]["CreateTime"] != null && dt.Rows[n]["CreateTime"].ToString() != "")
                    {
                        model.CreateTime = DateTime.Parse(dt.Rows[n]["CreateTime"].ToString());
                    }
                    if (dt.Rows[n]["UpdateTime"] != null && dt.Rows[n]["UpdateTime"].ToString() != "")
                    {
                        model.UpdateTime = DateTime.Parse(dt.Rows[n]["UpdateTime"].ToString());
                    }
                    if (dt.Rows[n]["InvoiceRegisterID"] != null && dt.Rows[n]["InvoiceRegisterID"].ToString() != "")
                    {
                        model.InvoiceRegisterID = dt.Rows[n]["InvoiceRegisterID"].ToString();
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }
        #endregion  Method
    }
}

