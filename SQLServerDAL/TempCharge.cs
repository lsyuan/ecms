using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using System.Collections.Generic;
using Ajax.Common;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:TempCharge
    /// </summary>
    public partial class TempChargeDAL
    {
        public TempChargeDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<TempCharge>(ID);
            }
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(TempCharge model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<TempCharge>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(TempCharge model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<TempCharge>(model);
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
                return db.DeleteByID<TempCharge>(ID);
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_TempCharge ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) > 0;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public TempCharge GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<TempCharge>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<TempCharge> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<TempCharge>(strWhere);
            }
        }

        /// <summary>
        /// 获取临时缴费信息json
        /// </summary>
        /// <param name="pageModel"></param>
        /// <param name="tCharge"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<dynamic> Search(EasyUIGridParamModel pageModel, Model.TempCharge tCharge, out int itemCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT tCharge.ID,customerName,chargeName,Money,createTime,oper.Name  ");
            strSql.Append("FROM    T_TempCharge tCharge ");
            strSql.Append("left join T_Operator oper on oper.ID=tCharge.OperatorID ");
            strSql.Append("where 1=1 ");
            Dictionary<string, object> param = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(tCharge.CustomerName))
            {
                strSql.Append("and customerName like @customerName ");
                param.Add("customerName", string.Format("%{0}%", tCharge.CustomerName));
            }
            DateTime firstTime = new DateTime();
            if (tCharge.CreateTime > firstTime)
            {
                strSql.Append("and convert(varchar(10),createTime,121)=@createTime ");
                param.Add("createTime", Convert.ToDateTime(tCharge.CreateTime).ToString("yyyy-MM-dd"));
            }
            if (tCharge.Status != -1)
            {
                strSql.Append("and tCharge.Status=@status ");
                param.Add("status", tCharge.Status);
            }
            int pageIndex = Convert.ToInt32(pageModel.page) - 1;
            int pageSize = Convert.ToInt32(pageModel.rows);
            using (DBHelper db = DBHelper.Create())
            {
                string sql = strSql.ToString();
				itemCount = db.GetCount(string.Format(DBHelper.StrGetCountSql, sql), param);
                return db.GetDynaminObjectList(sql, pageIndex, pageSize, "ID", param);
            }
        }
        /// <summary>
        /// 新增临时收费
        /// </summary>
        /// <param name="tCharge"></param>
        /// <param name="tChargeDetails"></param>
        public void AddTempCharge(TempCharge tCharge, List<TempChargeDetail> tChargeDetails)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.BeginTransaction();
                db.Insert<TempCharge>(tCharge);
                db.InsertBatch<TempChargeDetail>(tChargeDetails.ToArray());
                db.Commit();
            } 
        }

        /// <summary>
        /// 临时缴费审核操作
        /// </summary>
        /// <param name="guidList">临时缴费记录ID</param>
        /// <param name="isPass">是否通过</param>
        /// <returns></returns>
        public bool TempChargeAudit(List<string> guidList, bool isPass)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("update  T_TempCharge set status={0} where ID in(", isPass ? 1 : 2);
            for (int i = 0; i < guidList.Count; i++)
            {
                if (!string.IsNullOrEmpty(guidList[i]))
                {
                    strSql.AppendFormat("'{0}'", guidList[i]);
                    if (i != guidList.Count - 1)
                    {
                        strSql.Append(",");
                    }
                }
            }
            strSql.Append(")");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) != -1 ? true : false;
            }
        }
        #endregion  Method
    }
}

