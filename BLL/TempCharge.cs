using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
	/// <summary>
	/// 临时缴费表
	/// </summary>
	public partial class TempChargeRule
	{
		private readonly Ajax.DAL.TempChargeDAL dal = new Ajax.DAL.TempChargeDAL();
		public TempChargeRule()
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
		public void Add(Ajax.Model.TempCharge model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.TempCharge model)
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
		public Ajax.Model.TempCharge GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.TempCharge GetModelByCache(string ID)
		{

			string CacheKey = "TempChargeModel-" + ID;
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
			return (Ajax.Model.TempCharge)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<TempCharge> GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<TempCharge> GetAllList()
		{
			return GetList("");
		}

		/// <summary>
		/// 获取临时缴费信息json
		/// </summary>
		/// <param name="pageModel"></param>
		/// <param name="tCharge"></param>
		/// <param name="itemCount"></param>
		/// <returns></returns>
		public List<dynamic> Search(EasyUIGridParamModel pageModel, Model.TempCharge tCharge, out int itemCount)
		{
			return new DAL.TempChargeDAL().Search(pageModel, tCharge, out itemCount);
		}
		/// <summary>
		/// 新增临时收费
		/// </summary>
		/// <param name="tCharge"></param>
		/// <param name="tChargeDetails"></param>
		public void AddTempCharge(TempCharge tCharge, List<TempChargeDetail> tChargeDetails)
		{
			if (tChargeDetails != null)
			{
				tCharge.ID = Guid.NewGuid().ToString("N");
				tCharge.CreateTime = DateTime.Now;
				foreach (TempChargeDetail detail in tChargeDetails)
				{
					detail.ID = Guid.NewGuid().ToString("N");
					detail.TempChargeID = tCharge.ID;
					detail.CreateTime = DateTime.Now;
					if (!string.IsNullOrEmpty(detail.ItemID))
					{
						decimal itemPrice = new ChargeItemRule().GetPriceByItemID(detail.ItemID, detail.Count, "");
						detail.Money = Convert.ToDecimal(detail.Count) * itemPrice;
						tCharge.Money += detail.Money;
					}
				}
				dal.AddTempCharge(tCharge, tChargeDetails);
			}
		}
		/// <summary>
		/// 临时缴费审核操作
		/// </summary>
		/// <param name="guids">临时缴费记录ID</param>
		/// <param name="isPass">是否通过</param>
		/// <returns></returns>
		public bool TempChargeAudit(string guids, bool isPass)
		{
			string[] guidArray = guids.TrimEnd(',').Split(',');
			return dal.TempChargeAudit(new List<string>(guidArray), isPass);
		}
		#endregion  Method
	}
}

