using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using System.Linq;
using Ajax.DBUtility;
using System.Collections.Generic;
using Ajax.Common;
using System.Data.Common;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:ChargeItem
    /// </summary>
    public partial class ChargeItemDAL
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChargeItemDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            string sql = @"select count(1) from T_ChargeItem where ID=@ID ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ID", ID);
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist(sql, param);
            }
        }

        /// <summary>
        /// 是否存在指定分类的收费项记录
        /// <param name="categoryID">收费项分类</param>
        /// </summary>
        public bool ExistsCategory(string categoryID)
        {
            string sql = @"select count(1) from T_ChargeItem where CategoryID=@categoryID ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("categoryID", categoryID);
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist(sql, param);
            }
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void AddChargeItem(ChargeItem model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<ChargeItem>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(ChargeItem model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<ChargeItem>(model);
                return true;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID)
        {
            /// 判断是否有客户与缴费项绑定,如果有,不能删除该缴费项
            string sql = "select count(0) from T_TypeToItem where ItemID = @ID";
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ItemID", ID);
                if (db.Exist(sql, param))
                {
                    return false;
                    //throw new Exception("该缴费项与客户类型已绑定,不能删除");
                }
                string deleteSQL = @"delete from T_ChargeItem  where ID=@ID ";
                param.Clear();
                param.Add("ID", ID);
                return db.ExecuteNonQuery(deleteSQL, param) > 0;
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            string sql = @"delete from T_ChargeItem  where ID in (" + IDlist + ")  ";
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(sql) > 0;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public ChargeItem GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<ChargeItem>(ID);
            }
        }

        /// <summary>
        /// 获取收费项目数据json
        /// </summary>
        /// <param name="param"></param>
        /// <param name="chargeItem"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<dynamic> SearchChargeItem(EasyUIGridParamModel param, string name, out int itemCount)
        {
            List<SqlParameter> paramList = new List<SqlParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT tci.ID,code,tci.Name, case ISREGULAR when 0 then '否' else '是' end as ISREGULAR,");
            strSql.Append("case IsPloy when 0 then '否' else '是' end as IsPloy,");
            strSql.Append("tci.UnitPrice,('每'+tu.Name+'/每'+tu2.Name) AS UNIT,");
            strSql.Append("tcic.Name AS CATEGORYNAME,tci.UnitID1,tci.UnitID2 ");
            strSql.Append("FROM T_ChargeItem tci ");
            strSql.Append("LEFT JOIN T_Unit tu ON tu.id= tci.UnitID1 ");
            strSql.Append("LEFT JOIN T_Unit tu2 ON tci.UnitID2= tu2.ID ");
            strSql.Append("LEFT JOIN T_ChargeItemCategory tcic ON tcic.ID = tci.CategoryID ");
            strSql.Append("where 1=1 ");
            int pageIndex = Convert.ToInt32(param.page) - 1;
            int pageSize = Convert.ToInt32(param.rows);
            string countSql = @"select count(0) from  T_ChargeItem tci where 1=1 {0}";
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                StringBuilder whereStr = new StringBuilder();
                whereStr.Append(" and tci.Name like @Name");
                dic.Add("Name", string.Format("%{0}%", name));
                itemCount = db.GetCount(string.Format(countSql, whereStr.ToString()), dic);
                return db.GetDynaminObjectList(strSql.ToString(), pageIndex, pageSize, "ID", dic);
            }
        }

        /// <summary>
        /// 根据ID获取一个缴费项信息，用于前台列表数据展示
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public object GetChargeItem(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT ID,code,Name, ISREGULAR,IsPloy,IsAgreeMent, UnitPrice,CategoryID ,UnitID1,UnitID2 ");
            strSql.Append("FROM T_ChargeItem  ");
            strSql.Append("where ID = @ID ");
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ID", ID);
                return db.GetSingelDynaminObject(strSql.ToString(), param);
            }
        }
        /// <summary>
        /// 获取所有缴费项目数量
        /// </summary>
        /// <returns></returns>
        public int GetAllChargeItemCount()
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<ChargeItem>("").Count;
            }
        }
        /// <summary>
        /// 获取新的编号
        /// </summary>
        /// <returns></returns>
        public string GetNewCode()
        {
            string sql = "select max(code) from t_chargeItem ";
            using (DBHelper db = DBHelper.Create())
            {
                object o = db.ExcuteScular(sql, null);
                if (o != null && o.ToString() != "")
                {
                    // 编号+1
                    return (Convert.ToInt16(o.ToString()) + 1).ToString().PadLeft(4, '0');
                }
                else
                {
                    return "0001";
                }
            }
        }
        /// <summary>
        /// 获取缴费项,只显示ID,Name
        /// </summary>
        /// <param name="isRegular">是否周期性缴费,0否,1是</param>
        /// <returns></returns>
        public List<object> GetChargeItemForCheckBox(string isRegular)
        {
            string sql = "SELECT id,name,unitPrice FROM t_chargeItem WHERE IsRegular = @IsRegular";
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("IsRegular", isRegular);
                return db.GetDynaminObjectList(sql, param);
            }
        }

        /// <summary>
        /// 获取指定缴费项目的单价
        /// </summary>
        /// <param name="chargeItemID">缴费项目ID</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public decimal GetPriceByItemID(string chargeItemID, decimal count, string customerID)
        {
            decimal price = 0;
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string strSql = "select UnitPrice  FROM T_ChargeItem where ID=@ID";
                param.Add("ID", chargeItemID);
                // 读取没有分级收费的单价
                using (DbDataReader reader = db.ExecuteReader(strSql, param))
                {
                    if (reader != null && reader.Read())
                    {
                        object obj = reader["UnitPrice"];
                        if (obj != null)
                        {
                            price = Convert.ToDecimal(obj);
                        }
                    }
                }
                if (count != 0)
                {
                    // 如果某个缴费项目是按协议收费，则读取协议金额
                    if (!customerID.Equals(""))
                    {
                        string agreementMoneySQL = "select AgreementMoney from T_CustomerChargeItem where customerID = @customerID and ItemID = @ItemID";
                        param.Clear();
                        param.Add("customerID", customerID);
                        param.Add("ItemID", chargeItemID);

                        using (DbDataReader reader = db.ExecuteReader(agreementMoneySQL, param))
                        {
                            if (reader != null && reader.Read())
                            {
                                object obj = reader["AgreementMoney"];
                                if (obj != null && obj.ToString() != "0.00")
                                {
                                    decimal.TryParse(obj.ToString(), out price);
                                    return price != -1 ? price : 0;
                                }
                            }
                        }
                    }
                    // 如果不是按协议收费则读取分级收费
                    string polySQL = "SELECT UnitPrice FROM T_poly WHERE itemID = @ItemID AND LowerBound < @COUNT AND HignerBound > @COUNT";
                    param.Add("COUNT", count);
                    using (DbDataReader reader = db.ExecuteReader(polySQL, param))
                    {
                        if (reader != null && reader.Read())
                        {
                            object obj = reader["UnitPrice"];
                            if (obj != null)
                            {
                                price = Convert.ToDecimal(obj);
                            }
                        }
                    }
                }
            }
            return price;
        }

        /// <summary>
        /// 获取周期性缴费的缴费项记录
        /// </summary>
        /// <returns></returns>
        public List<dynamic> SelectChargeItemByType()
        {
            string strSql = @"select c.ID,c.Name chargeName,ci.Name chargeTypeName,c.UnitPrice,u1.Name+'/'+u2.Name unitName from T_ChargeItem c
								left join T_ChargeItemCategory ci on ci.ID=c.CategoryID
								left join T_Unit u1 on u1.ID=c.UnitID1
								left join T_Unit u2 on u2.ID=c.UnitID2
								where c.IsRegular=1
								order by ci.ID";
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetDynaminObjectList(strSql.ToString(), null);
            }
        }
        /// <summary>
        /// 获取指定客户缴费项
        /// </summary>
        /// <param name="customerID">客户编号</param>
        /// <returns></returns>
        public List<dynamic> SearchChargeItem(string customerID)
        {
            StringBuilder strSql = new StringBuilder();
            string sql = @"SELECT  tci.ID,
								   tci.Code,
								   tci.Name,
								   tci.IsRegular,
								   tci.IsAgreeMent,
								   tci.IsPloy,
								   tci.UnitPrice,
								   tu.Name   AS unit1,
								   tu2.Name  AS unit2,
								   tcc.[Count],
								   tcc.AgreementMoney
							FROM   T_CustomerChargeItem tcc
								   INNER JOIN T_ChargeItem tci
										ON  tci.ID = tcc.ItemID
								   INNER JOIN T_Customer tc
										ON  tc.ID = tcc.CustomerID
								   INNER JOIN T_Unit tu
										ON  tu.ID = tci.UnitID1
								   INNER JOIN t_unit tu2
										ON  tu2.ID = tci.UnitID2
							WHERE  tcc.CustomerID = @customerID";
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("customerID", customerID);
                return db.GetDynaminObjectList(sql, param);
            }
        }
        /// <summary>
        /// 获取所有子客户及其缴费项
        /// </summary>
        /// <param name="customerID">父客户编号</param>
        /// <returns></returns>
        public List<dynamic> GetCustomerChildrenInfo(string customerID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select c.Name,a.Name as areaName,c.address,ci.name as ChargeName,ci.unitPrice,cc.count ");
            strSql.Append("from t_customer c ");
            strSql.Append("left join t_area a on a.id=c.areaID ");
            strSql.Append("left join T_CustomerChargeItem cc on cc.customerID=c.ID ");
            strSql.Append("left join t_chargeItem ci on ci.id=cc.itemID ");
            strSql.Append("where c.PID=@customerID and c.status = 1");

            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("customerID", customerID);
                return db.GetDynaminObjectList(strSql.ToString(), param);
            }
        }
        /// <summary>
        /// 获取所有子客户的计费信息
        /// </summary>
        /// <param name="customerID">父客户编号</param>
        /// <returns></returns>
        public DataTable GetCustomerChildrenFeeInfo(string customerID)
        {
            DataTable dt = new DataTable();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select c.Name,c.address,ci.id as chargeItemID ,ci.name as ChargeName,ci.unitPrice,cc.count,a.Money  ");
            strSql.Append(" from t_customer c  ");
            strSql.Append(" left join T_Agreements a on a.customerID=c.id and a.status=1");
            strSql.Append(" left join T_CustomerChargeItem cc on cc.customerID=c.ID  ");
            strSql.Append(" left join t_chargeItem ci on ci.id=cc.itemID  ");
            strSql.Append(" where c.Status=1 and c.PID=@customerID");
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@customerID", customerID);
                using (System.Data.Common.DbDataReader ddr = db.ExecuteReader(strSql.ToString(), paramDic))
                {
                    dt.Load(ddr);
                }
            }
            return dt;
        }
        #endregion  Method
    }
}

