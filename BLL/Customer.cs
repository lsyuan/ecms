using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;

namespace Ajax.BLL
{
    /// <summary>
    /// 客户表
    /// </summary>
    public partial class CustomerRule
    {
        private readonly Ajax.DAL.CustomerDAL dal = new Ajax.DAL.CustomerDAL();
        #region  Method

        /// <summary>
        /// 新增客户
        /// </summary>
        public void Add(Ajax.Model.Customer model)
        {
            dal.Add(model);
        }
        /// <summary>
        /// 新增客户重载
        /// </summary>
        /// <param name="model">客户基本信息</param>
        /// <param name="ccItems">客户对应缴费项</param>
        public void Add(Ajax.Model.Customer model, List<CustomerChargeItem> ccItems)
        {
            dal.Add(model, ccItems);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.Customer model)
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
        public Ajax.Model.Customer GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.Customer GetModelByCache(string ID)
        {

            string CacheKey = "CustomerModel-" + ID;
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
            return (Ajax.Model.Customer)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Customer> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Ajax.Model.Customer> GetModelList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Ajax.Model.Customer> DataTableToList(DataTable dt)
        {
            List<Ajax.Model.Customer> modelList = new List<Ajax.Model.Customer>();
            return modelList;
        }

        #endregion  Method
        #region 客户查询列表
        /// <summary>
        /// 客户信息的分页查询
        /// </summary>
        /// <param name="pageModel">分页信息</param>
        /// <param name="c">保存查询信息的实体类</param>
        /// <param name="itemCount">查询结果总数</param>
        /// <returns></returns>
        public List<dynamic> Search(EasyUIGridParamModel pageModel, Customer c, out int itemCount)
        {
            return dal.Search(pageModel, c, out itemCount);
        }

        /// <summary>
        /// 客户信息修改
        /// </summary>
        /// <param name="customer">客户基本信息</param>
        /// <param name="ccItems">客户对应缴费项</param>
        /// <returns></returns>
        public bool Modify(Customer customer, List<CustomerChargeItem> ccItems)
        {
            return dal.Update(customer, ccItems);
        }

        /// <summary>
        /// 客户详细信息查询
        /// </summary>
        /// <param name="customerID">客户ID</param>
        /// <returns></returns>
        public object CustomerDetail(string customerID)
        {
            return dal.CustomerDetail(customerID);
        }
        /// <summary>
        /// 客户状态审批
        /// </summary>
        /// <param name="customerIDList">客户ID集合</param>
        /// <param name="status">审批结果1通过，4未通过</param>
        /// <returns></returns>
        public bool Audit(List<string> customerIDList, int status)
        {
            return dal.Audit(customerIDList, status);
        }
        /// <summary>
        /// 启用/禁用 
        /// </summary>
        /// <param name="customerIDList"></param>
        /// <param name="status"></param>
        public void SetEnabled(List<string> customerIDList, int status)
        {
            dal.SetEnabled(customerIDList, status);
        }
        #endregion

        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <param name="filter">过滤字符串</param>
        /// <param name="queryType">
        /// 客户查询类型
        /// 1：只查询子客户，2只查询父客户，为空时查询全部
        /// </param>
        /// <returns></returns>
        public List<Ajax.Model.Customer> GetCustomerListByID(string filter, string queryType)
        {
            return dal.GetCustomerList(filter, queryType);
        }

        /// <summary>
        /// 根据客户类型获取对应缴费项
        /// </summary>
        /// <param name="customerTypeID"></param>
        /// <returns></returns>
        public List<dynamic> GetChargeItemByCustomerTypeID(string customerTypeID)
        {
            return dal.GetChargeItemByCustomerTypeID(customerTypeID);
        }
        /// <summary>
        /// 根据客户guid获取对应缴费项
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public List<dynamic> GetChargeItemByCustomerID(string customerID)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                return new List<dynamic>();
            }
            return dal.GetChargeItemByCustomerID(customerID);
        }
        /// <summary>
        /// 客户文本框自动完成
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public List<Customer> QueryCustomer(string q)
        {
            if (!string.IsNullOrEmpty(q))
            {
                return dal.QueryCustomer(q);
            }
            return null;
        }
        /// <summary>
        /// 获取指定客户的子客户
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public List<Customer> GetChildrenCustomer(string customerID)
        {
            return dal.GetChildrenCustomer(customerID);
        }
    }
}

