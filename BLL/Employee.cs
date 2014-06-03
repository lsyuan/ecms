using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;


namespace Ajax.BLL
{
	/// <summary>
	/// 职工表
	/// </summary>
	public partial class EmployeeRule
	{
		//private readonly I_Employee dal = DataAccess.CreateEmployee();
		private readonly Ajax.DAL.EmployeeDAL dal = new Ajax.DAL.EmployeeDAL();
		public EmployeeRule()
		{ }
		#region  Method 

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Ajax.Model.Employee model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.Employee model)
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
			//IDlist = "'" + IDlist.TrimEnd(',').Replace(",", "','") + "'";//"1,2,3,4"->"1','2','3','4"
			return dal.DeleteList(IDlist);
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Ajax.Model.Employee GetModel(string ID)
		{

			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.Employee GetModelByCache(string ID)
		{

			string CacheKey = "T_EmployeeModel-" + ID;
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
			return (Ajax.Model.Employee)objModel;
		}
		/// <summary>
		/// 获取职员管理列表json
		/// </summary>
		/// <param name="param"></param>
		/// <param name="emp"></param>
        /// <param name="itemCount"></param>
		/// <returns></returns>
		public List<object> GetSearchJson(EasyUIGridParamModel param, Employee emp, out int itemCount)
		{
			return new DAL.EmployeeDAL().GetSearchJson(param, emp, out itemCount);
		}  
          
         
		/// <summary>
		/// 获取某个职员信息
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public object GetEmployee(string ID)
		{
			return dal.GetEmployee(ID); 
		} 
		/// <summary>
		/// 职员查询
		/// </summary>
		/// <param name="filterStr"></param>
		/// <returns></returns>
		public List<dynamic> QueryEmployee(string filterStr)
		{
			return dal.QueryEmployee(filterStr);
		}

        /// <summary>
        /// 职员缴费分析
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<dynamic> EmpChargeAnalysis(string startDate,string endDate)
        {
            return dal.EmpChargeAnalysis(startDate,endDate);
        }

		#endregion  Method
	}
}

