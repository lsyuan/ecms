using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
	/// <summary>
	/// 消息表
	/// </summary>
	public partial class MessageRule
	{
		private readonly Ajax.DAL.MessageDAL dal = new Ajax.DAL.MessageDAL();

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
		public void Add(Ajax.Model.Message model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.Message model)
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
		/// 批量删除公告
		/// </summary>
		public bool DeleteMul(List<string> msgIDlist)
		{
			return dal.DeleteMul(msgIDlist);
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Ajax.Model.Message GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.Message GetModelByCache(string ID)
		{

			string CacheKey = "MessageModel-" + ID;
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
			return (Ajax.Model.Message)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Message> GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Ajax.Model.Message> DataTableToList(DataTable dt)
		{
			List<Ajax.Model.Message> modelList = new List<Ajax.Model.Message>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				Ajax.Model.Message model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = new Ajax.Model.Message();
					if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
					{
						model.ID = dt.Rows[n]["ID"].ToString();
					}
					if (dt.Rows[n]["Title"] != null && dt.Rows[n]["Title"].ToString() != "")
					{
						model.Title = dt.Rows[n]["Title"].ToString();
					}
					if (dt.Rows[n]["Content"] != null && dt.Rows[n]["Content"].ToString() != "")
					{
						model.Content = dt.Rows[n]["Content"].ToString();
					}
					if (dt.Rows[n]["OperatorID"] != null && dt.Rows[n]["OperatorID"].ToString() != "")
					{
						model.OperatorID = dt.Rows[n]["OperatorID"].ToString();
					}
					if (dt.Rows[n]["CreateDate"] != null && dt.Rows[n]["CreateDate"].ToString() != "")
					{
						model.CreateDate = DateTime.Parse(dt.Rows[n]["CreateDate"].ToString());
					}
					modelList.Add(model);
				}
			}
			return modelList;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Message> GetAllList()
		{
			return GetList("");
		}
		/// <summary>
		/// 获取json数据
		/// </summary>
		/// <param name="gridParam"></param>
		/// <param name="status"></param>
		/// <param name="itemCount"></param>
		/// <returns></returns>
		public List<object> GetSearchJson(EasyUIGridParamModel gridParam, int status, string userID, out int itemCount)
		{
			return dal.GetSearchJson(gridParam, status, userID, out itemCount);
		}

		/// <summary>
		/// 获取最新消息/公告
		/// </summary>
		/// <param name="topCount">前N条</param>
		/// <returns></returns>
		public List<dynamic> GetLatestMsg(int topCount, string operatorID)
		{
			return dal.GetLatestMsg(topCount, operatorID);
		}

		#endregion  Method
	}
}

