using System;
using System.Collections.Generic;
using System.Data;
using Ajax.Model;
using Ajax.Common;

namespace Ajax.BLL
{
	/// <summary>
	/// 票据类型字典
	/// </summary>
	public partial class InvoiceTypeRule
	{
		private readonly Ajax.DAL.InvoiceTypeDAL dal=new Ajax.DAL.InvoiceTypeDAL();
		#region  Method
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID,string Name)
		{
			return dal.Exists(ID,Name);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Ajax.Model.InvoiceType model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.InvoiceType model)
		{
			return dal.Update(model);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(string ID)
		{
            if (!dal.IsUsing(ID))
            {
                return dal.Delete(ID);
            }
            else
            {
                throw new Exception("分类正在使用，不能删除");
            }
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Ajax.Model.InvoiceType GetModel(string ID)
		{
			
			return dal.GetModelByID(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.InvoiceType GetModelByCache(string ID)
		{
			
			string CacheKey = "InvoiceTypeModel-" + ID;
			object objModel = Ajax.Common.DataCache.GetCache(CacheKey);
			if (objModel == null)
			{
				try
				{
					objModel = dal.GetModelByID(ID);
					if (objModel != null)
					{
						int ModelCache = Ajax.Common.ConfigHelper.GetConfigInt("ModelCache");
						Ajax.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
					}
				}
				catch{}
			}
			return (Ajax.Model.InvoiceType)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Ajax.Model.InvoiceType> GetList(Ajax.Model.InvoiceType IType)
		{
			return dal.GetList(IType);
		}
		
		
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Ajax.Model.InvoiceType> DataTableToList(DataTable dt)
		{
			List<Ajax.Model.InvoiceType> modelList = new List<Ajax.Model.InvoiceType>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				Ajax.Model.InvoiceType model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = new Ajax.Model.InvoiceType();
					if(dt.Rows[n]["ID"]!=null && dt.Rows[n]["ID"].ToString()!="")
					{
					model.ID=dt.Rows[n]["ID"].ToString();
					}
					if(dt.Rows[n]["Name"]!=null && dt.Rows[n]["Name"].ToString()!="")
					{
					model.Name=dt.Rows[n]["Name"].ToString();
					}
					if(dt.Rows[n]["Standard"]!=null && dt.Rows[n]["Standard"].ToString()!="")
					{
					model.Standard=dt.Rows[n]["Standard"].ToString();
					}
					modelList.Add(model);
				}
			}
			return modelList;
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="IType"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<object> Search(EasyUIGridParamModel param, InvoiceType IType, out int itemCount)
        {
            return dal.Search(param,IType,out itemCount);
        }

		#endregion  Method
	}
}

