using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using Ajax.Common;
using System.Collections.Generic;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:CustomerType
    /// </summary>
    public partial class CustomerTypeDAL
    {
        public CustomerTypeDAL()
        { }
        #region  Method

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(CustomerType model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<CustomerType>(model);
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
                return db.DeleteByID<CustomerType>(ID);
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_CustomerType ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) > 0;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public CustomerType GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<CustomerType>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<CustomerType> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<CustomerType>(strWhere);
            }
        }

        /// <summary>
        /// 获取所有客户类型
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cType"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<dynamic> GetAllCustomerType(EasyUIGridParamModel param, CustomerType cType, out int itemCount)
        {
            List<SqlParameter> paramList = new List<SqlParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,name from t_customerType ");
            strSql.Append("where 1=1 ");
            Dictionary<string, object> paramDic = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(cType.Name))
            {
                strSql.Append("and name like @name ");
                paramDic.Add("name", string.Format("%{0}%", cType.Name));
            }
            int pageIndex = Convert.ToInt32(param.page) - 1;
            int pageSize = Convert.ToInt32(param.rows);
            using (DBHelper db = DBHelper.Create())
            {
				itemCount = db.GetListCount(strSql.ToString(), paramDic);
                return db.GetDynaminObjectList(strSql.ToString(), pageIndex, pageSize, "name", paramDic);
            }
        }
        /// <summary>
        /// 获取客户类型数量
        /// </summary>
        /// <returns></returns>
        public int GetAllCustomerTypeCount()
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetCount<CustomerType>();
            }
        }

        /// <summary>
        /// 新增客户类型
        /// </summary>
        /// <param name="customerType"></param>
        /// <returns>新增是否成功</returns>
        public bool AddCustomerType(CustomerType customerType)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<CustomerType>(customerType);
                return true;
            }
        }

        //***********************************************************************************
        /*将客户类型对应到缴费项目*/
        //***********************************************************************************
        /// <summary>
        /// 删除客户类型
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>删除是否成功</returns>
        public bool DeleteCustomerType(string ID)
        {
            string sql = "select count(0) from t_customer where typeid = @ID";
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> paramList = new Dictionary<string, object>();
                paramList.Add("ID", ID);
                object o = db.ExcuteScular(sql, paramList);
                if (o != null && o.ToString() != "0")
                {
                    throw new Exception("该客户类型已分配到客户,不能删除");
                }
                return db.DeleteByID<CustomerType>(ID);
            }
        }

        /// <summary>
        /// 更新客户类型
        /// </summary>
        /// <param name="customerType"></param>
        /// <returns>更新是否成功</returns>
        public bool UpdateCustomerType(CustomerType customerType)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<CustomerType>(customerType);
                return true;
            }
        }
        /// <summary>
        /// 获取某个客户类型的缴费项
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<object> GetMyChargeItem(string ID)
        {
            string sql = "SELECT ITEMID FROM T_TypeToItem ttti WHERE ttti.TypeID = @ID";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ID", ID);
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetDynaminObjectList(sql, param);
            }
        }
        /// <summary>
        /// 修改客户类型对应的缴费项
        /// </summary>
        /// <param name="customerTypeID"></param>
        /// <param name="chargeItemList"></param>
        /// <returns></returns>
        public bool ModifyTypeToItem(string customerTypeID, List<string> chargeItemList)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.BeginTransaction();
                string deleteSql = "delete from T_TYPETOITEM where TYPEID=@CUSTOMERTYPEID";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("CUSTOMERTYPEID", customerTypeID);
                db.ExecuteNonQuery(deleteSql, param);
                //新增收费项
                if (chargeItemList == null || chargeItemList.Count == 0)
                {
                    db.RollBack();
                    return false;
                }
                List<TypeToItem> list = new List<TypeToItem>();
                foreach (string itemID in chargeItemList)
                {
                    if (string.IsNullOrEmpty(itemID)) continue;
                    TypeToItem newItem = new TypeToItem();
                    newItem.ID = Guid.NewGuid().ToString("N");
                    newItem.TypeID = customerTypeID;
                    newItem.ItemID = itemID;
                    list.Add(newItem);
                };
                db.InsertBatch<TypeToItem>(list.ToArray());
                db.Commit();
                return true;
            }
        }
        /// <summary>
        /// 获取客户类型集合
        /// </summary>
        /// <returns></returns>
        public List<object> GetChargeTypeList()
        {
            string sql = "select id,name from t_customerType order by name";
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetDynaminObjectList(sql, null);
            }
        }
        #endregion  Method
    }
}

