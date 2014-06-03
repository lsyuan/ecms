using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.DBUtility;
using Ajax.Model;
using System.Collections.Generic;
using Ajax.Common;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:InvoiceRegister
    /// </summary>
    public partial class InvoiceRegisterDAL
    {
        public InvoiceRegisterDAL()
        { }
        #region  Method


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(InvoiceRegister model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<InvoiceRegister>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(InvoiceRegister model)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            string strSql = @"update T_InvoiceRegister set
                                 BeginCode=@beginCode,EndCode=@endCode ,InvoiceType=@InvoiceType
                                 where ID=@ID";
            param.Add("beginCode", model.BeginCode);
            param.Add("endCode", model.EndCode);
            param.Add("InvoiceType", model.InvoiceType);
            param.Add("ID", model.ID);
            using (DBHelper db = DBHelper.Create())
            {
                int effectLine=db.ExecuteNonQuery(strSql, param);
                return effectLine>0?true:false;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID, string OperatorID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.DeleteByID<InvoiceRegister>(ID);
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Ajax.Model.InvoiceRegister GetModel(string ID, string OperatorID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<InvoiceRegister>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<InvoiceRegister> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<InvoiceRegister>(strWhere);
            }
        }
        /// <summary>
        /// 票据一览查询
        /// </summary>
        /// <param name="param"></param>
        /// <param name="strWhere"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<dynamic> Search(EasyUIGridParamModel param, string strWhere, out int itemCount)
        {
            string strSql = @"select ir.*,e.Name,it.Name as InvoiceTypeName from T_InvoiceRegister ir
                                    left join t_Employee e on ir.OperatorID=e.ID
                                    left join t_InvoiceType it on it.ID=ir.InvoiceType
                                    where 1=1 ";
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql += strWhere;
            }
            using (DBHelper db = DBHelper.Create())
            {
                int pageIndex = Convert.ToInt32(param.page) - 1;
                int pageSize = Convert.ToInt32(param.rows);
                itemCount = db.GetCount(string.Format(DBHelper.StrGetCountSql, strSql), null);
                return db.GetDynaminObjectList(strSql, pageIndex, pageSize, "ID", null);
            }
        }
        #endregion  Method
    }
}

