using System;
using System.Collections.Generic;
using System.Data;
using Ajax.Model;
using Ajax.Common;

namespace Ajax.BLL
{
    /// <summary>
    /// 票据登记
    /// </summary>
    public partial class InvoiceRegisterRule
    {
        private readonly Ajax.DAL.InvoiceRegisterDAL dal = new Ajax.DAL.InvoiceRegisterDAL();
        public InvoiceRegisterRule()
        { }
        #region  Method

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Ajax.Model.InvoiceRegister model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.InvoiceRegister model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID, string OperatorID)
        {

            return dal.Delete(ID, OperatorID);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Ajax.Model.InvoiceRegister GetModel(string ID, string OperatorID)
        {

            return dal.GetModel(ID, OperatorID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.InvoiceRegister GetModelByCache(string ID, string OperatorID)
        {

            string CacheKey = "InvoiceRegisterModel-" + ID + OperatorID;
            object objModel = Ajax.Common.DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = dal.GetModel(ID, OperatorID);
                    if (objModel != null)
                    {
                        int ModelCache = Ajax.Common.ConfigHelper.GetConfigInt("ModelCache");
                        Ajax.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
                    }
                }
                catch { }
            }
            return (Ajax.Model.InvoiceRegister)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Ajax.Model.InvoiceRegister> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        } 
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<InvoiceRegister> GetAllList()
        {
            return GetList("");
        }

        /// <summary>
        /// 票据一览查询
        /// </summary>
        /// <param name="param"></param>
        /// <param name="strWhere"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<dynamic> Search(EasyUIGridParamModel param, string strWhere, out int itemCount)
        {
            return dal.Search(param, strWhere, out itemCount);
        }
        #endregion  Method
    }
}

