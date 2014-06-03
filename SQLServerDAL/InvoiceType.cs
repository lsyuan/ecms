using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.DBUtility;
using System.Collections.Generic;
using Ajax.Model;
using Ajax.Common;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:InvoiceType
    /// </summary>
    public partial class InvoiceTypeDAL
    {
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID, string Name)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<InvoiceType>(ID);
            }
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Ajax.Model.InvoiceType model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<InvoiceType>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.InvoiceType model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<InvoiceType>(model);
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
                return db.DeleteByID<InvoiceType>(ID);
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Ajax.Model.InvoiceType GetModelByID(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<InvoiceType>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<InvoiceType> GetList(InvoiceType IType)
        {
            List<InvoiceType> list = new List<InvoiceType>();
            StringBuilder strWhereSql = new StringBuilder();
            Dictionary<string, object> paramList = new Dictionary<string, object>();
            if (IType != null)
            {
                if (!string.IsNullOrEmpty(IType.ID))
                {
                    strWhereSql.Append(" and ID=@ID ");
                    paramList.Add("ID", IType.ID);
                }
                if (!string.IsNullOrEmpty(IType.Name))
                {
                    strWhereSql.Append(" and Standard=@Standard ");
                    paramList.Add("Standard", IType.Standard);
                }
            }
            using (DBHelper db = DBHelper.Create())
            {
                list = db.GetList<InvoiceType>(strWhereSql.ToString(), paramList, "ID", "");
            }
            return list;
        }
        /// <summary>
        /// search一览
        /// </summary>
        /// <param name="param"></param>
        /// <param name="IType"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<object> Search(EasyUIGridParamModel param, InvoiceType IType, out int itemCount)
        {
            List<object> list = new List<object>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from T_InvoiceType where 1=1");
            Dictionary<string, object> paramList = new Dictionary<string, object>();
            if (IType != null)
            {
                if (!string.IsNullOrEmpty(IType.ID))
                {
                    strSql.Append(" and ID=@ID ");
                    paramList.Add("ID", IType.ID);
                }
                if (!string.IsNullOrEmpty(IType.Name))
                {
                    strSql.Append(" and Standard=@Standard ");
                    paramList.Add("Standard", IType.Standard);
                }
            }
            using (DBHelper db = DBHelper.Create())
            {
                int pageIndex = Convert.ToInt32(param.page) - 1;
                int pageSize = Convert.ToInt32(param.rows);
                itemCount = db.GetCount(string.Format(DBHelper.StrGetCountSql,strSql.ToString()), paramList);
                list = db.GetDynaminObjectList(strSql.ToString(), pageIndex, pageSize, "ID", paramList);
            }
            return list;
        }

        /// <summary>
        /// 是否正在使用
        /// </summary>
        /// <returns></returns>
        public bool IsUsing(string typeID)
        {
            bool flag = false;
            string strSql = "select count(1) from T_InvoiceRegister where InvoiceType=@TypeID";
            Dictionary<string, object> paramDic = new Dictionary<string, object>();
            paramDic.Add("TypeID", typeID);
            using (DBHelper db = DBHelper.Create())
            {
                int count = db.GetCount(strSql, paramDic);
                flag = count > 0 ? true : false;
            }
            return flag;
        }
        #endregion  Method
    }
}

