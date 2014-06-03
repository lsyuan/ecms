using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
    /// <summary>
    /// 业务日志表
    /// </summary>
    public partial class BusinessLog
    {
        private readonly Ajax.DAL.BusinessLogDAL dal = new Ajax.DAL.BusinessLogDAL();
        public BusinessLog()
        { }
        #region  Method
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            return dal.Exists(ID);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Ajax.Model.BusinessLog model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.BusinessLog model)
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
        public Ajax.Model.BusinessLog GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.BusinessLog GetModelByCache(string ID)
        { 
            string CacheKey = "BusinessLogModel-" + ID;
            object objModel = Ajax.Common.DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = dal.GetModel(ID);
                    if (objModel != null)
                    {
                        int ModelCache = Ajax.Common.ConfigHelper.GetConfigInt("ModelCache");
                        Ajax.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
                    }
                }
                catch { }
            }
            return (Ajax.Model.BusinessLog)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Ajax.Model.BusinessLog> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Ajax.Model.BusinessLog> GetModelList(string strWhere)
        {
            List<Ajax.Model.BusinessLog> lists = dal.GetList(strWhere);
            return lists;
        }

        #endregion  Method
    }
}

