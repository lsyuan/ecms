using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
	/// <summary>
	/// 客户类型表
	/// </summary>
	public partial class CustomerTypeRule
	{
		private readonly Ajax.DAL.CustomerTypeDAL dal = new Ajax.DAL.CustomerTypeDAL();
		public CustomerTypeRule()
		{ }
		#region  Method 
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.CustomerType model)
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
		public Ajax.Model.CustomerType GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.CustomerType GetModelByCache(string ID)
		{

			string CacheKey = "CustomerTypeModel-" + ID;
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
			return (Ajax.Model.CustomerType)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<CustomerType> GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		} 
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Ajax.Model.CustomerType> GetModelList(string strWhere)
		{
			return dal.GetList(strWhere);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Ajax.Model.CustomerType> DataTableToList(DataTable dt)
		{
			List<Ajax.Model.CustomerType> modelList = new List<Ajax.Model.CustomerType>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				Ajax.Model.CustomerType model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = new Ajax.Model.CustomerType();
					if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
					{
						model.ID = dt.Rows[n]["ID"].ToString();
					}
					if (dt.Rows[n]["Name"] != null && dt.Rows[n]["Name"].ToString() != "")
					{
						model.Name = dt.Rows[n]["Name"].ToString();
					}
					modelList.Add(model);
				}
			}
			return modelList;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<CustomerType> GetAllList()
		{
			return GetList("");
		}


		/// <summary>
		/// 获取所有的客户类型
		/// </summary>
		/// <param name="param"></param>
		/// <param name="orderType"></param>
		/// <param name="sortingCols"></param>
		/// <returns></returns>
		public List<dynamic> GetAllCustomerType(EasyUIGridParamModel param, CustomerType cType, out int itemCount)//(JQueryDataTableParamModel param, string orderType, string sortingCols)
		{
			return dal.GetAllCustomerType(param, cType, out itemCount);
		}
		/// <summary>
		/// 获取所有的客户类型数量
		/// </summary>
		/// <returns></returns>
		public int GetAllCustomerTypeCount()
		{
			return dal.GetAllCustomerTypeCount();
		} 
		/// <summary>
		/// 新增客户类型
		/// </summary>
		/// <param name="customerTYpe"></param>
		/// <returns></returns>
		public bool AddCustomerType(CustomerType customerTYpe)
		{
			return dal.AddCustomerType(customerTYpe);
		}
		/// <summary>
		/// 删除客户类型
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public bool DeleteCustomerType(string ID)
		{
			return dal.DeleteCustomerType(ID);
		}

		/// <summary>
		/// 更新客户类型
		/// </summary>
		/// <param name="customerType"></param>
		/// <returns></returns>
		public bool UpdateCustomerType(CustomerType customerType)
		{
			return dal.UpdateCustomerType(customerType);
		}
		/// <summary>
		/// 获取某个客户类型包含的缴费项
		/// </summary>
		/// <param name="ID">客户类型ID</param>
		/// <returns></returns>
		public List<object> GetMyChargeItem(string ID)
		{
			return dal.GetMyChargeItem(ID);
		}
		/// <summary>
		/// 修改客户类型对应的缴费项
		/// </summary>
		/// <param name="customerTypeID"></param>
		/// <param name="chargeItemArrary"></param>
		/// <returns></returns>
		public bool ModifyTypeToItem(string customerTypeID, string chargeItemArrary)
		{
			List<string> chargeItemIDList = new List<string>(chargeItemArrary.Split(';'));
			return dal.ModifyTypeToItem(customerTypeID, chargeItemIDList);
		}
		/// <summary>
		/// 获取缴费类型集合
		/// </summary>
		/// <returns></returns>
		public List<object> GetChargeTypeList()
		{
			return dal.GetChargeTypeList();
		}
		#endregion  Method
	}
}

