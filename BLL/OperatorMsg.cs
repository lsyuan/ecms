using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
    /// <summary>
    /// 操作员消息表
    /// </summary>
    public partial class OperatorMsgRule
    {
        private readonly Ajax.DAL.OperatorMsgDAL dal = new Ajax.DAL.OperatorMsgDAL();

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
        public void Add(Ajax.Model.OperatorMsg model)
        {
            dal.Add(model);
        }
        /// <summary>
        /// 增加多条数据
        /// </summary>
        /// <param name="modelList"></param>
        public void AddMul(List<Ajax.Model.OperatorMsg> modelList)
        {
            dal.AddMul(modelList);
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.OperatorMsg model)
        {
            return dal.Update(model);
        }
        /// <summary>
        /// 更改公告的状态（是否已查看）
        /// </summary>
        /// <param name="msgID">已读消息的ID</param> 
        /// <param name="operatorID">操作员ID</param>
        /// <returns></returns>
        public Message ReadMsg(string msgID, string operatorID)
        {
            return new Ajax.DAL.OperatorMsgDAL().ReadMsg(msgID, operatorID);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID)
        {

            return dal.Delete(ID);
        }
        /// <summary>
        /// 根据删除的公告ID，来批量删除操作员信息
        /// </summary>
        /// <param name="msgIDList"></param>
        /// <returns></returns>
        public bool DeleteMul(List<string> msgIDList)
        {
            return dal.DeleteMul(msgIDList);
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
        public Ajax.Model.OperatorMsg GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.OperatorMsg GetModelByCache(string ID)
        {

            string CacheKey = "OperatorMsgModel-" + ID;
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
            return (Ajax.Model.OperatorMsg)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<OperatorMsg> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<OperatorMsg> GetAllList()
        {
            return GetList("");
        }

        #endregion  Method
    }
}

