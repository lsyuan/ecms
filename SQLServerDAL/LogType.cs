﻿using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using System.Collections.Generic;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:LogType
    /// </summary>
    public partial class LogTypeDAL
    {
        public LogTypeDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<LogType>(ID);
            }
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(LogType model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<LogType>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(LogType model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<LogType>(model);
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
                return db.DeleteByID<LogType>(ID);
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_LogType ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) > 0;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public LogType GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<LogType>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<LogType> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<LogType>(strWhere);
            }
        }
        #endregion  Method
    }
}

