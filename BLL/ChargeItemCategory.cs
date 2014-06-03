using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
    /// <summary>
    /// 缴费项表
    /// </summary>
    public partial class ChargeItemCategoryRule
    {
        private readonly Ajax.DAL.ChargeItemCategoryDAL dal = new Ajax.DAL.ChargeItemCategoryDAL();
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChargeItemCategoryRule()
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
        public void Add(Ajax.Model.ChargeItemCategory model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.ChargeItemCategory model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID)
        {
            if (new ChargeItemRule().ExistsCategory(ID))
            {
                return false;
            }
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
        public Ajax.Model.ChargeItemCategory GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.ChargeItemCategory GetModelByCache(string ID)
        {

            string CacheKey = "ChargeItemCategoryModel-" + ID;
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
            return (Ajax.Model.ChargeItemCategory)objModel;
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<ChargeItemCategory> GetAllList()
        {
            return dal.GetAllList();
        }
        /// <summary>
        /// 获取缴费项分类json
        /// </summary>
        /// <param name="param"></param>
        /// <param name="chargeItemType"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<dynamic> SearchChargeItemCategory(EasyUIGridParamModel param, ChargeItemCategory chargeItemType, out int itemCount)
        {
            return dal.SearchChargeItemCategory(param, chargeItemType, out itemCount);
        }


        #endregion  Method
    }
}

