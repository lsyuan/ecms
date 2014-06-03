using System;
using System.Collections.Generic;

namespace Ajax.BLL
{
	/// <summary>
	/// 缴费详细信息表
	/// </summary>
	public partial class ChargeDetail
	{
		private readonly Ajax.DAL.ChargeDetailDAL dal = new Ajax.DAL.ChargeDetailDAL();
		public ChargeDetail()
		{ }
		#region  Method
		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Ajax.Model.ChargeDetail model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.ChargeDetail model)
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
		public Ajax.Model.ChargeDetail GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.ChargeDetail GetModelByCache(string ID)
		{

			string CacheKey = "ChargeDetailModel-" + ID;
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
			return (Ajax.Model.ChargeDetail)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Ajax.Model.ChargeDetail> GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		} 
		#endregion  Method
	}
}

