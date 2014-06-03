using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
	/// <summary>
	/// 其他收费信息表
	/// </summary>
	public partial class AnotherChargeRule
	{
		private readonly Ajax.DAL.AnotherChargeDAL dal = new Ajax.DAL.AnotherChargeDAL();
		public AnotherChargeRule()
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
		public void Add(Ajax.Model.AnotherCharge model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.AnotherCharge model)
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
		public Ajax.Model.AnotherCharge GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.AnotherCharge GetModelByCache(string ID)
		{

			string CacheKey = "AnotherChargeModel-" + ID;
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
			return (Ajax.Model.AnotherCharge)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<AnotherCharge> GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Ajax.Model.AnotherCharge> GetModelList(string strWhere)
		{
			List<AnotherCharge> lists = dal.GetList(strWhere);
			return lists;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<AnotherCharge> GetAllList()
		{
			return GetList("");
		}

		/// <summary>
		/// 获取其他缴费的一览json
		/// </summary>
		/// <param name="param"></param>
		/// <param name="itemCount"></param>
		/// <returns></returns>
		public List<dynamic> AnotherChargeSearch(EasyUIGridParamModel param, AnotherCharge aCharge, out int itemCount)
		{
			return dal.AnotherChargeSearch(param, aCharge, out itemCount);
		}

		/// <summary>
		/// 得到一个对象实体
		/// <param name="ID"></param>
		/// </summary>
		public dynamic GetModelByID(string ID)
		{
			return dal.GetModelByID(ID);
		}

		public bool Aduit(string guids, bool isPass)
		{
			return dal.Aduit(guids, isPass); 
		}
		#endregion  Method
	}
}

