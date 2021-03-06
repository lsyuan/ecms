﻿using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
    /// <summary>
    /// 特殊权限表
    /// </summary>
    public partial class SpecialVoteRule
    {
        private readonly Ajax.DAL.SpecialVoteDAL dal = new Ajax.DAL.SpecialVoteDAL();
        public SpecialVoteRule()
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
        public void Add(Ajax.Model.SpecialVote model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.SpecialVote model)
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
        public Ajax.Model.SpecialVote GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.SpecialVote GetModelByCache(string ID)
        {

            string CacheKey = "SpecialVoteModel-" + ID;
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
            return (Ajax.Model.SpecialVote)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<SpecialVote> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        #endregion  Method
    }
}

