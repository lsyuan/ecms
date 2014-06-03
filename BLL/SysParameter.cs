using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
using Ajax.DAL;
namespace Ajax.BLL
{
	/// <summary>
	/// 系统参数表
	/// </summary>
	public partial class SysParameterRule
	{
		private readonly Ajax.DAL.SysParameterDAL dal = new Ajax.DAL.SysParameterDAL();
		public SysParameterRule()
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
		public void Add(Ajax.Model.SysParameter model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.SysParameter model)
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
		public Ajax.Model.SysParameter GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.SysParameter GetModelByCache(string ID)
		{

			string CacheKey = "SysParameterModel-" + ID;
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
			return (Ajax.Model.SysParameter)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Ajax.Model.SysParameter> GetModelList(string strWhere)
		{
			return dal.GetList(strWhere);
		}


		public bool UpdateSysParameter(List<Ajax.Model.SysParameter> list)
		{
			return dal.UpdateSysParameter(list);

		}

		/// <summary>
		/// 获取系统某个参数的值
		/// </summary>
		/// <param name="parameterName">参数名</param>
		/// <returns></returns>
		public object GetSysParameterValue(string parameterName)
		{
			return dal.GetSysParameterValue(parameterName);
		}

		#endregion  Method
	}
}

