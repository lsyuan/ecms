using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
	/// <summary>
	/// 期初欠费表
	/// </summary>
	public partial class FirstMoneyRule
	{
		private readonly Ajax.DAL.FirstMoneyDAL dal = new Ajax.DAL.FirstMoneyDAL();
		/// <summary>
		/// 构造函数
		/// </summary>
		public FirstMoneyRule()
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
		/// 增加一条数据
		/// </summary>
		public void Add(Ajax.Model.FirstMoney model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.FirstMoney model)
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
		public Ajax.Model.FirstMoney GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.FirstMoney GetModelByCache(string ID)
		{

			string CacheKey = "FirstMoneyModel-" + ID;
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
			return (Ajax.Model.FirstMoney)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Ajax.Model.FirstMoney> GetModelList(string strWhere)
		{
			return dal.GetList(strWhere);
		}

		/// <summary>
		/// 获取期初欠费一览
		/// </summary>
		/// <param name="param"></param>
		/// <param name="fMoney"></param>
		/// <param name="itemCount"></param>
		/// <returns></returns>
		public List<dynamic> Search(EasyUIGridParamModel param, FirstMoney fMoney, out int itemCount)
		{
			return dal.Search(param, fMoney, out itemCount);
		}

		/// <summary>
		/// 更改缴费状态
		/// </summary>
		/// <param name="customerID">客户编号</param>
		/// <param name="status">0未缴费，1待审核，2已缴费，3已删除</param>
		/// <returns></returns>
		public bool UpdateStatus(string customerID, int status)
		{
			return dal.UpdateStatus(customerID, status);
		}
        /// <summary>
        /// 期初缴费审核
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="isPass"></param>
        /// <returns></returns>
        public bool ChargeAudit(string guids, bool isPass)
        {
            if (string.IsNullOrEmpty(guids))
            {
                return false;
            }
            return dal.ChargeAudit(guids,isPass);
        }
		#endregion  Method
	}
}

