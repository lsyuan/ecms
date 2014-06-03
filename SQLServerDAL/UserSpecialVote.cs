using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using System.Collections.Generic;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:UserSpecialVote
    /// </summary>
    public partial class UserSpecialVoteDAL
    {
        public UserSpecialVoteDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<UserSpecialVote>(ID);
            }
        } 

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(UserSpecialVote model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<UserSpecialVote>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(UserSpecialVote model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<UserSpecialVote>(model);
                return true;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.DeleteByID<UserSpecialVote>(ID);
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_UserSpecialVote ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) > 0;
            }
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public UserSpecialVote GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<UserSpecialVote>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<UserSpecialVote> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<UserSpecialVote>(strWhere);
            }
        }
        #endregion  Method
    }
}

