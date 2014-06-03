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
	/// 数据访问类:CustomerChargeItem
	/// </summary>
	public class CustomerChargeItemDAL
	{
		/// <summary>
		/// 获取客户对应缴费项
		/// </summary>
		/// <param name="customerID">客户编号</param>
		/// <returns></returns>
		public List<CustomerChargeItem> GetListBycustomerID(string customerID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<CustomerChargeItem>(" and CustomerID='" + customerID + "' and count >0");
			}
		}
	}
}
