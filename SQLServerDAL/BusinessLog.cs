using System;
using System.Collections.Generic;
using System.Text;
using Ajax.DBUtility;
using Ajax.Model;


namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:BusinessLog
    /// </summary>
    public partial class BusinessLogDAL
    {
        public BusinessLogDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                BusinessLog businessLog = db.GetById<BusinessLog>(ID);
                return businessLog != null && businessLog.ID != null;
            }
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(BusinessLog model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<BusinessLog>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(BusinessLog model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<BusinessLog>(model);
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
                return db.DeleteByID<BusinessLog>(ID);
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_BusinessLog ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) > 0;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public BusinessLog GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<BusinessLog>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<BusinessLog> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<BusinessLog>(strWhere);
            }
        }
        #endregion  Method
    }
}

