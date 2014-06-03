using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.DBUtility;
using System.Collections.Generic;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:InvoiceUseLog
    /// </summary>
    public partial class InvoiceUseLogDAL
    {
        public InvoiceUseLogDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID, string InvoiceCode, string OperatorID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<InvoiceUseLogDAL>(ID);
            }
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Ajax.Model.InvoiceUseLog model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<Ajax.Model.InvoiceUseLog>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.InvoiceUseLog model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<Ajax.Model.InvoiceUseLog>(model);
                return true;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID, string InvoiceCode, string OperatorID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.DeleteByID<Ajax.Model.InvoiceUseLog>(ID);
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Ajax.Model.InvoiceUseLog GetModel(string ID, string InvoiceCode, string OperatorID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<Ajax.Model.InvoiceUseLog>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Ajax.Model.InvoiceUseLog> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<Ajax.Model.InvoiceUseLog>(strWhere);
            }
        }

        #endregion  Method
    }
}

