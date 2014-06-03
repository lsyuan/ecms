using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
using System.Text;
namespace Ajax.BLL
{
    /// <summary>
    /// 临时收费详细表
    /// </summary>
    public partial class TempChargeDetailRule
    {
        private readonly Ajax.DAL.TempChargeDetailDAL dal = new Ajax.DAL.TempChargeDetailDAL();
        public TempChargeDetailRule()
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
        public void Add(Ajax.Model.TempChargeDetail model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.TempChargeDetail model)
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
        public Ajax.Model.TempChargeDetail GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.TempChargeDetail GetModelByCache(string ID)
        {

            string CacheKey = "TempChargeDetailModel-" + ID;
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
            return (Ajax.Model.TempChargeDetail)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<TempChargeDetail> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<TempChargeDetail> GetAllList()
        {
            return GetList("");
        }
        /// <summary>
        /// 根据缴费项ID获取缴费详细信息
        /// </summary>
        /// <param name="chargeID">缴费项ID</param>
        /// <returns></returns>
        public List<dynamic> GetTempleChargeDetailByID(string chargeID)
        {
            return dal.GetTempleChargeDetailByID(chargeID);
        }

        #endregion  Method
    }
}

