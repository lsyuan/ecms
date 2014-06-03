using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using Ajax.Common;

namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:FirstMoney
    /// </summary>
    public partial class FirstMoneyDAL
    {
        public FirstMoneyDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<FirstMoney>(ID);
            }
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(FirstMoney model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<FirstMoney>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(FirstMoney model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<FirstMoney>(model);
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
                return db.DeleteByID<FirstMoney>(ID);
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_FirstMoney ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) > 0;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FirstMoney GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<FirstMoney>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<FirstMoney> GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,CustomerID,Money,Status,ChargeDate ");
            strSql.Append(" FROM T_FirstMoney ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<FirstMoney>("");
            }
        }
        /// <summary>
        /// 获取期初欠费一览
        /// </summary>
        /// <param name="param"></param>
        /// <param name="fMoney"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<dynamic> Search(EasyUIGridParamModel param, FirstMoney fMoney, out int itemCount)
        {
            Dictionary<string, object> DicParam = new Dictionary<string, object>();
            string strSql = @"select fm.*,c.Name from T_FirstMoney fm
                             left join T_Customer c on c.ID=fm.CustomerID
                             where 1=1 ";
            if (!string.IsNullOrEmpty(fMoney.CustomerID))
            {
                strSql+=" and fm.CustomerID=@customerID";
                DicParam.Add("customerID", fMoney.CustomerID);
            }
            if (fMoney.Status!=-1)
            {
                strSql += " and fm.Status=@Status";
                DicParam.Add("Status", fMoney.Status);
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
        /// 更改缴费状态
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <param name="status">0未缴费，1待审核，2已缴费，3已删除</param>
        /// <returns></returns>
        public bool UpdateStatus(string customerID,int status)
        {
            Dictionary<string, object> DicParam = new Dictionary<string, object>();
            string strSql = "update T_FirstMoney set status=@status ";
            if (status == 1)
            {
                strSql += ",chargeDate=@chargeDate";
                DicParam.Add("chargeDate",DateTime.Now);
            }
            strSql += " where id=@ID";
            DicParam.Add("status",status);
            DicParam.Add("ID",customerID);
            using (DBHelper db = DBHelper.Create())
            {
                int effectLine = db.ExecuteNonQuery(strSql, DicParam);
                return effectLine > 0 ? true : false;
            }
        }
        /// <summary>
        /// 期初缴费审核
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="isPass"></param>
        /// <returns></returns>
        public bool ChargeAudit(string guids,bool isPass)
        {
            string[] guidList = guids.TrimEnd(',').Split(',');
            Dictionary<string, object> param = new Dictionary<string, object>();
            string sql = string.Format("update T_FirstMoney set status={0} where status=1 and ID=@ID", isPass ? 2 : 3);
            bool flag = false;
            using (DBHelper db = DBHelper.Create())
            {
                db.BeginTransaction();
                for (int i = 0; i < guidList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(guidList[i]))
                    {
                        param.Clear();
                        param.Add("ID", guidList[i]);
                        db.ExecuteNonQuery(sql, param);
                    }
                }
                db.Commit();
                flag = true;
            }
            return flag;
        }
        #endregion  Method
    }
}

