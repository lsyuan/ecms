using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.DBUtility;
using Ajax.Model;
using System.Collections.Generic;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:OperatorVote
    /// </summary>
    public partial class OperatorVoteDAL
    {
        public OperatorVoteDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID, string OperatorID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<OperatorVote>(ID);
            }
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(OperatorVote model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<OperatorVote>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.OperatorVote model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<OperatorVote>(model);
                return true;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID, string OperatorID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.DeleteByID<OperatorVote>(ID);
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Ajax.Model.OperatorVote GetModel(string ID, string OperatorID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<OperatorVote>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<OperatorVote> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<OperatorVote>(strWhere);
            }
        } 
        #endregion  Method
    }
}

