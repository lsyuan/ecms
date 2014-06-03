using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
    /// <summary>
    /// 操作用户权限表
    /// </summary>
    public partial class OperatorVoteRule
    {
        private readonly Ajax.DAL.OperatorVoteDAL dal = new Ajax.DAL.OperatorVoteDAL();
        public OperatorVoteRule()
        { }
        #region  Method
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID, string OperatorID)
        {
            return dal.Exists(ID, OperatorID);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Ajax.Model.OperatorVote model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.OperatorVote model)
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
        public Ajax.Model.OperatorVote GetModel(string ID, string OperatorID)
        {

            return dal.GetModel(ID, OperatorID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.OperatorVote GetModelByCache(string ID, string OperatorID)
        {

            string CacheKey = "OperatorVoteModel-" + ID + OperatorID;
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
            return (Ajax.Model.OperatorVote)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<OperatorVote> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        } 
        #endregion  Method
    }
}

