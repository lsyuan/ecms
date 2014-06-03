using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
using Ajax.DAL;
namespace Ajax.BLL
{
    /// <summary>
    /// 年终欠费表
    /// </summary>
    public partial class YearEndArrearRule
    {
        private readonly Ajax.DAL.YearEndArrearDAL dal = new Ajax.DAL.YearEndArrearDAL();
        public YearEndArrearRule()
        { }
        #region  public Method
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            return dal.Exists(ID);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Ajax.Model.YearEndArrear model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.YearEndArrear model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID)
        {

            return dal.Delete(ID);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            return dal.DeleteList(IDlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Ajax.Model.YearEndArrear GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.YearEndArrear GetModelByCache(string ID)
        {

            string CacheKey = "YearEndArrearModel-" + ID;
            object objModel = Ajax.Common.DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = dal.GetModel(ID);
                    if (objModel != null)
                    {
                        int ModelCache = Ajax.Common.ConfigHelper.GetConfigInt("ModelCache");
                        Ajax.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
                    }
                }
                catch { }
            }
            return (Ajax.Model.YearEndArrear)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<YearEndArrear> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<YearEndArrear> GetAllList()
        {
            return GetList("");
        }

        /// <summary>
        /// 根据客户ID获取欠费记录
        /// </summary>
        /// <param name="customerID">客户ID</param>
        /// <returns></returns>
        public List<dynamic> GetArrearRecordByCustomerID(string customerID)
        {
            List<string> CustomerIDs = new List<string>();
            foreach (Customer item in new CustomerRule().GetChildrenCustomer(customerID))
            {
                CustomerIDs.Add(item.ID);
            }
            return dal.GetCustomerArrearRecord(CustomerIDs);
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
            return dal.Search(param, yeArrear, out itemCount);
        }
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="strID"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        public bool UpdateStatus(string strID, string strStatus)
        {
            return dal.UpdateStatus(strID, strStatus);
        }
        /// <summary>
        /// 执行年终结算
        /// </summary>
        /// <param name="strYear">结算年份</param>
        /// <returns></returns>
        public void CaculateYearFee(string strYear)
        {
            decimal totalFee = 0;
            //获取欠费用户列表
            string strWhere = @" and PID is null and status=1 
                                 and beginChargeDate between '{0}-1-1 0:00:00' and '{0}-12-31 23:59:59'";
            List<Customer> arrearsCustomerList = new CustomerRule().GetList(string.Format(strWhere, strYear));
            foreach (Customer c in arrearsCustomerList)
            {
                totalFee = new ChargeRule().CaculateCustomerFee(c.ID);
                YearEndArrear yeaInfo = new YearEndArrear()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    CustomerID = c.ID,
                    Money = totalFee,
                    Status = 0,
                    Year = Convert.ToInt32(strYear)
                };
                new YearEndArrearRule().Add(yeaInfo);
                //更新起始缴费时间
                c.BeginChargeDate = Convert.ToDateTime(string.Format("{0}-1-1", DateTime.Now.Year - 1));
                new CustomerRule().Update(c);
            }
        }
        #endregion  Method
    }
}