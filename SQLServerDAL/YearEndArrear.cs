using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using Ajax.Model;
using Ajax.DBUtility;
using Ajax.Common;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:YearEndArrear
    /// </summary>
    public partial class YearEndArrearDAL
    {
        public YearEndArrearDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<YearEndArrear>(ID);
            }
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(YearEndArrear model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<YearEndArrear>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(YearEndArrear model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<YearEndArrear>(model);
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
                return db.DeleteByID<YearEndArrear>(ID);
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_YearEndArrear ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) > 0;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public YearEndArrear GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<YearEndArrear>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<YearEndArrear> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<YearEndArrear>(strWhere);
            }
        } 

        /// <summary>
        /// 获取客户的欠费信息，包括子客户的欠费信息
        /// </summary>
        /// <param name="customerIDs">客户ID集合</param>
        /// <returns></returns>
        public List<dynamic> GetCustomerArrearRecord(List<string> customerIDs)
        {
            string sql = "select * from T_YearEndArrear where id in (@id)";
            StringBuilder ids = new StringBuilder();
            foreach (string item in customerIDs)
            {
                ids.Append(item).Append(",");
            }
            if (ids.Length > 0)
            {
                ids = ids.Remove(ids.Length - 1, 1);
            }
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("id", ids.ToString());
                return db.GetDynaminObjectList(sql, param);
            }
        }
        /// <summary>
        /// 获取年度欠费一览
        /// </summary>
        /// <param name="param"></param>
        /// <param name="yeArrear"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<dynamic> Search(EasyUIGridParamModel param, YearEndArrear yeArrear, out int itemCount)
        {
            Dictionary<string, object> DicParam = new Dictionary<string, object>();
            string strSql = @"select yea.*,c.Name from T_YearEndArrear yea
                             left join T_Customer c on c.ID=yea.CustomerID
                             where 1=1 ";
            if (!string.IsNullOrEmpty(yeArrear.CustomerID))
            {
                strSql += " and yea.CustomerID=@customerID";
                DicParam.Add("customerID", yeArrear.CustomerID);
            }
            if (yeArrear.Status != -1)
            {
                strSql += " and yea.Year=@Year";
                DicParam.Add("Year", yeArrear.Year);
            }
            //分页信息
            int pageIndex = Convert.ToInt32(param.page) - 1;
            int pageSize = Convert.ToInt32(param.rows);
            using (DBHelper db = DBHelper.Create())
            {
                itemCount = db.GetCount(string.Format(DBHelper.StrGetCountSql, strSql), DicParam);
                return db.GetDynaminObjectList(strSql, pageIndex, pageSize, "ID", DicParam);
            }
        }
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="strID"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        public bool UpdateStatus(string strID,string strStatus)
        {
            Dictionary<string, object> DicParam = new Dictionary<string, object>();
            string strSql = "update  T_YearEndArrear set status=@status where ID=@ID";
            DicParam.Add("status", strStatus);
            DicParam.Add("ID", strID);
            using (DBHelper db = DBHelper.Create())
            {
                int effectLine = db.ExecuteNonQuery(strSql, DicParam);
                return effectLine > 0 ? true : false;
            }
        }
        #endregion  Method
    }
}

