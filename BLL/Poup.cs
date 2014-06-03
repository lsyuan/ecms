using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;


namespace Ajax.BLL
{
	/// <summary>
	/// 菜单节点表
	/// </summary>
	public partial class PoupRule
	{
		Ajax.DAL.PoupDAL dal = new Ajax.DAL.PoupDAL();
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
		public void Add(Ajax.Model.Poup model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.Poup model)
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
		public Ajax.Model.Poup GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.Poup GetModelByCache(string ID)
		{

			string CacheKey = "T_PoupModel-" + ID;
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
			return (Ajax.Model.Poup)objModel;
		}

		/// <summary>
		/// 获取菜单节点
		/// </summary>
		/// <returns></returns>
		public List<object> GetPoupList(Poup poup, string userID)
		{
			return dal.GetPoupList(poup, userID);
		}
		/// <summary>
		/// 根据ID获取单个菜单节点
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public object GetPoup(string ID)
		{
			return dal.GetPoup(ID);
		}
		/// <summary>
		/// 新增菜单节点
		/// </summary>
		/// <param name="poup"></param>
		/// <returns></returns>
		public object AddPoup(Poup poup)
		{
			return dal.AddPoup(poup);
		}
		/// <summary>
		/// 更新菜单节点
		/// </summary>
		/// <param name="ID"></param>
		/// <param name="Name"></param>
		/// <param name="Path">菜单节点路径</param>
		/// <returns></returns>
		public bool ModifyPoup(string ID, string Name, string Path)
		{
			return dal.ModifyPoup(ID, Name, Path);
		}
		/// <summary>
		/// 删除菜单
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public bool DeletePoup(string ID)
		{
			return dal.DeletePoup(ID);
		}
		#endregion  Method


		#region 新的菜单生成方法
		/// <summary>
		/// 获取所有菜单
		/// </summary>
		/// <returns></returns>
		public List<Poup> GetMenuJson()
		{
			return dal.GetList(" and IsValid = 1 ");
		}
		#endregion
	}
}

