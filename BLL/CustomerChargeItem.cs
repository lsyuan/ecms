using System;
using System.Collections.Generic;
using System.Data;
using Ajax.DAL;
using Ajax.Model;

namespace Ajax.BLL
{
	/// <summary>
	/// 客户对应缴费表Bll
	/// </summary>
	public class CustomerChargeItemRule
	{
		CustomerChargeItemDAL dal = new CustomerChargeItemDAL();
		/// <summary>
		/// 获取客户对应缴费项
		/// </summary>
		/// <param name="customerID">客户编号</param>
		/// <returns></returns>
		public DataTable GetTableBycustomerID(string customerID)
		{
			return new DataTable();// CustomerChargeItemDAL().GetTableBycustomerID(customerID);
		}
		/// <summary>
		/// 获取客户对应缴费项
		/// </summary>
		/// <param name="customerID">客户编号</param>
		/// <returns></returns>
		public List<CustomerChargeItem> GetListBycustomerID(string customerID)
		{
			return dal.GetListBycustomerID(customerID);
		}
	}
}
