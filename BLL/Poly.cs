using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
    /// <summary>
    /// 分级收费
    /// </summary>
    public partial class PolyRule
    {
        private readonly Ajax.DAL.PolyDAL dal = new Ajax.DAL.PolyDAL();
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
        public void Add(Ajax.Model.Poly model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.Poly model)
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
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            return dal.DeleteList(IDlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Ajax.Model.Poly GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.Poly GetModelByCache(string ID)
        {

            string CacheKey = "PolyModel-" + ID;
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
            return (Ajax.Model.Poly)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Poly> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        } 

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Poly> GetAllList()
        {
            return GetList("");
        }

        /// <summary>
        /// 获取指定缴费项的缴费策略
        /// </summary>
        /// <param name="ItemID">缴费项</param>
        /// <returns></returns>
        public List<Poly> GetPolyListByItemID(string ItemID)
        {
            return dal.GetPolyListByItemID(ItemID);
        }

        /// <summary>
        /// 获取指定缴费项的缴费策略
        /// </summary>
        /// <param name="ItemID">缴费项ID</param>
        /// <returns></returns>
        public List<dynamic> GetPolicyListByItemID(string ItemID)
        {
            return dal.GetPolicyListByItemID(ItemID);
        }
        /// <summary>
        /// 新增多个策略
        /// </summary>
        /// <param name="polys">策略数组</param>
        /// <param name="delItemID">缴费项ID</param>
        public void SavePolys(List<Poly> polys, string delItemID)
        {
            dal.SavePolys(polys, delItemID);
        }

        public decimal GetUnitPrice(string itemID, decimal count)
        {
            return 0;
        }
        #endregion  Method
    }
}

