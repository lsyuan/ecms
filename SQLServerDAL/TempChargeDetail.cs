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
    /// 数据访问类:TempChargeDetail
    /// </summary>
    public partial class TempChargeDetailDAL
    {
        public TempChargeDetailDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<TempChargeDetail>(ID);
            }
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(TempChargeDetail model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<TempChargeDetail>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(TempChargeDetail model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<TempChargeDetail>(model);
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
                return db.DeleteByID<TempChargeDetail>(ID);
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_TempChargeDetail ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) > 0;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public TempChargeDetail GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<TempChargeDetail>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<TempChargeDetail> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<TempChargeDetail>(strWhere);
            }
        }
        /// <summary>
        /// 根据缴费项ID获取缴费详细信息
        /// </summary>
        /// <param name="chargeID">缴费项ID</param>
        /// <returns></returns>
        public List<dynamic> GetTempleChargeDetailByID(string chargeID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select td.ID,t.chargeName,ci.Name,td.Count,td.Money,td.CreateTime ");
            strSql.Append("from T_TempChargeDetail td ");
            strSql.Append("left join T_TempCharge t on td.TempChargeID=t.ID ");
            strSql.Append("left join T_ChargeItem ci on ci.ID=td.ItemID ");
            strSql.Append("where tempChargeID=@tempChargeID");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("tempChargeID", chargeID);
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetDynaminObjectList(strSql.ToString(), param);
            }
        }

        #endregion  Method
    }
}

