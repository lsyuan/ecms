using System;
using System.Collections.Generic;
using System.Data;
using Ajax.Common;
using Ajax.Model;

namespace Ajax.BLL
{
    /// <summary>
    /// 缴费项单位
    /// </summary>
    public partial class UnitRule
    {
        private readonly Ajax.DAL.UnitDAL dal = new Ajax.DAL.UnitDAL();
        public UnitRule()
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
        public void Add(Ajax.Model.Unit model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.Unit model)
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
        public Ajax.Model.Unit GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.Unit GetModelByCache(string ID)
        {

            string CacheKey = "UnitModel-" + ID;
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
            return (Ajax.Model.Unit)objModel;
        }
         
        /// <summary>
        /// 分页获取数据列表
        /// </summary> 
        public List<object> GetUnit(string level)
        {
            return dal.GetUnit(level);
        }
        /// <summary>
        /// 获取系统所有单位的个数
        /// </summary>
        /// <returns></returns>
        public int GetAllUnitCount()
        {
            return dal.GetAllUnitCount();
        }
        /// <summary>
        /// 删除单位
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public AjaxResult DeleteUnit(String ID)
        {
            AjaxResult result = new AjaxResult();
            if (dal.IsUsed(ID))
            {
                result.Success = false;
                result.Message = "单位已经在使用，不能删除";
            }
            else
            {
                dal.DeleteUnit(ID);
                result.Success = true;
                result.Message = "单位删除成功";
            }
            return result;
        }
        /// <summary>
        /// 获取单位信息,用于JsonData
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public object GetSingelUnit(string ID)
        {
            return dal.GetSingelUnit(ID);
        }
        /// <summary>
        /// 获取单位信息,用于数据更新
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public object GetSingelUnitValue(string ID)
        {
            return dal.GetSingelUnitValue(ID);
        }
        /// <summary>
        /// 获取付费单位列表json
        /// </summary>
        /// <param name="param"></param>
        /// <param name="u"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<object> GetSearchJson(EasyUIGridParamModel param, Unit u, out int itemCount)
        {
            return dal.GetSerachJson(param, u, out itemCount);
        }
        #endregion  Method
    }
}

