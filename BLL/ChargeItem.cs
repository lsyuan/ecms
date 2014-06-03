using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
	/// <summary>
	/// 缴费项表
	/// </summary>
	public partial class ChargeItemRule
	{
		private readonly Ajax.DAL.ChargeItemDAL dal = new Ajax.DAL.ChargeItemDAL();
		/// <summary>
		/// 构造函数
		/// </summary>
		public ChargeItemRule()
		{ }
		#region  Method
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			return dal.Exists(ID);
		}

		/// <summary>
		/// 是否存在指定分类的收费项记录
		/// <param name="categoryID">收费项分类</param>
		/// </summary>
		public bool ExistsCategory(string categoryID)
		{
			return dal.ExistsCategory(categoryID);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Ajax.Model.ChargeItem model)
		{
			dal.AddChargeItem(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.ChargeItem model)
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
		public Ajax.Model.ChargeItem GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.ChargeItem GetModelByCache(string ID)
		{

			string CacheKey = "ChargeItemModel-" + ID;
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
			return (Ajax.Model.ChargeItem)objModel;
		}

		/// <summary>
		/// 获取单个缴费项信息，用于前台列表数据展示
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public object GetChargeItem(string ID)
		{
			return dal.GetChargeItem(ID);
		}


		/// <summary>
		/// 获取所有缴费项数量
		/// </summary>
		/// <returns></returns>
		public int GetAllChargeItemCount()
		{
			return dal.GetAllChargeItemCount();
		}
		/// <summary>
		/// 获取新的编号
		/// </summary>
		/// <returns></returns>
		public string GetNewCode()
		{
			return dal.GetNewCode();
		}
		/// <summary>
		/// 删除缴费项
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public bool DeleteChargeItem(string ID)
		{
			return dal.Delete(ID);
		}
		/// <summary>
		/// 获取缴费项用于页面checkBox展示
		/// </summary>
		/// <param name="isRegular"></param>
		/// <returns></returns>
		public List<object> GetChargeItemForCheckBox(string isRegular)
		{
			isRegular = isRegular.ToUpper().Replace("FALSE", "0").Replace("TRUE", "1");
			return dal.GetChargeItemForCheckBox(isRegular);
		}
		/// <summary>
		/// 获取收费项目数据json
		/// </summary>
		/// <param name="param"></param>
		/// <param name="chargeItem"></param>
		/// <returns></returns>
		public List<dynamic> SearchChargeItem(EasyUIGridParamModel param, string name, out int itemCount)
		{
			return dal.SearchChargeItem(param, name, out itemCount);
		}
		/// <summary>
		/// 获取指定缴费想的单价
		/// </summary>
		/// <param name="chargeItemID"></param>
		/// <param name="count">数量</param>
		/// <returns></returns>
		public decimal GetPriceByItemID(string chargeItemID,decimal count,string customerID)
		{
			return dal.GetPriceByItemID(chargeItemID, count,customerID);
		}
		/// <summary>
		/// 根据分类选择缴费项
		/// </summary>
		/// <returns></returns>
		public List<dynamic> SelectChargeItemByType()
		{
			return dal.SelectChargeItemByType();
		}
		///// <summary>
		///// 获取缴费客户列表
		///// </summary>
		///// <param name="param"></param>
		///// <param name="c"></param>
		///// <returns></returns>
		//public List<dynamic> SearchCustomerCharge(EasyUIGridParamModel param,Ajax.Model.Customer c,out int itemCount)
		//{
		//    return dal.SearchCustomerCharge(param,c,out itemCount);
		//}
		/// <summary>
		/// 获取指定客户缴费项
		/// </summary>
		/// <param name="customerID">客户编号</param>
		/// <returns></returns>
		public List<dynamic> SearchChargeItem(string customerID)
		{
			if (!string.IsNullOrEmpty(customerID))
			{
				return dal.SearchChargeItem(customerID);
			}
			else
			{
				return null;
			}
		}
		/// <summary>
		/// 获取所有子客户及其缴费项
		/// </summary>
		/// <param name="customerID"></param>
		/// <returns></returns>
		public List<dynamic> GetCustomerChildrenInfo(string customerID)
		{
			if (!string.IsNullOrEmpty(customerID))
			{
				return dal.GetCustomerChildrenInfo(customerID);
			}
			else
			{
				return null;
			}
		}
		#endregion  Method
	}
}

