using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using System.Collections.Generic;
using Ajax.Common;
using System.Data.Common;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:ChargeItemCategory
    /// </summary>
    public partial class ChargeItemCategoryDAL
    {
        /// <summary>
        /// 
        /// </summary>
        public ChargeItemCategoryDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<ChargeItemCategory>(ID);
            }
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Ajax.Model.ChargeItemCategory model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<ChargeItemCategory>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.ChargeItemCategory model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<ChargeItemCategory>(model);
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
                return db.DeleteByID<ChargeItemCategory>(ID);
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            string strSql = @"delete from T_ChargeItemCategory   where ID in (" + IDlist + ")";
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql) > 0;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Ajax.Model.ChargeItemCategory GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<ChargeItemCategory>(ID);
            }
        }

        /// <summary>
        /// 获取缴费项分类json
        /// </summary>
        /// <param name="param"></param>
        /// <param name="chargeItemType"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<dynamic> SearchChargeItemCategory(EasyUIGridParamModel param, ChargeItemCategory chargeItemType, out int itemCount)
        {
            Dictionary<string, object> paramList = new Dictionary<string, object>();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,Name from T_ChargeItemCategory where 1=1 ");
            if (!string.IsNullOrEmpty(chargeItemType.Name))
            {
                strSql.Append("and Name like @Name");
                paramList.Add("Name", string.Format("%{0}%", chargeItemType.Name));
            }
            int pageIndex = Convert.ToInt32(param.page) - 1;
            int pageSize = Convert.ToInt32(param.rows);
            using (DBHelper db = DBHelper.Create())
            {
                itemCount = db.GetCount(strSql.ToString(), paramList);
                return db.GetDynaminObjectList(strSql.ToString(), paramList);
            }
        }
        /// <summary>
        /// 获得所有分类数据列表
        /// </summary>
        public List<ChargeItemCategory> GetAllList()
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<ChargeItemCategory>("");
            }
        } 

        #endregion  Method
    }
}

