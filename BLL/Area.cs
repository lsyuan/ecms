using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
namespace Ajax.BLL
{
    /// <summary>
    /// 区域信息表
    /// </summary>
    public partial class AreaRule
    {
        private readonly Ajax.DAL.AreaDAL dal = new Ajax.DAL.AreaDAL();
        /// <summary>
        /// 构造函数
        /// </summary>
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
        public void Add(Ajax.Model.Area model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Ajax.Model.Area model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID)
        {
            // 顶级节点检查
            if (dal.GetModel(ID).PID == null)
            {
                return false;
            }
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
        public Ajax.Model.Area GetModel(string ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Ajax.Model.Area GetModelByCache(string ID)
        {

            string CacheKey = "AreaModel-" + ID;
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
            return (Ajax.Model.Area)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Area> GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        } 
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Ajax.Model.Area> GetModelList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Ajax.Model.Area> DataTableToList(DataTable dt)
        {
            List<Ajax.Model.Area> modelList = new List<Ajax.Model.Area>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                Ajax.Model.Area model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new Ajax.Model.Area();
                    if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
                    {
                        model.ID = dt.Rows[n]["ID"].ToString();
                    }
                    if (dt.Rows[n]["PID"] != null && dt.Rows[n]["PID"].ToString() != "")
                    {
                        model.PID = dt.Rows[n]["PID"].ToString();
                    }
                    if (dt.Rows[n]["Code"] != null && dt.Rows[n]["Code"].ToString() != "")
                    {
                        model.Code = dt.Rows[n]["Code"].ToString();
                    }
                    if (dt.Rows[n]["Name"] != null && dt.Rows[n]["Name"].ToString() != "")
                    {
                        model.Name = dt.Rows[n]["Name"].ToString();
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Area> GetAllList()
        {
            return GetList("");
        }



        /// <summary>
        /// 分页获取数据列表
        /// </summary>
		public List<dynamic> GetTreeGridList()
		{
			return dal.GetTreeGridList();
		}

        #endregion  Method
        /// <summary>
        /// 获取所有区域
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public List<object> GetAreaList(Area area)
        {
            return dal.GetAreaList(area);
        }
        /// <summary>
        /// 获取某个区域信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public object GetArea(string ID)
        {
            return dal.GetArea(ID);
        }
        /// <summary>
        /// 新增区域
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public object AddArea(Area area)
        {
            return dal.AddArea(area);
        }
        /// <summary>
        /// 修改区域
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool ModifyArea(Area area)
        {
            return dal.ModifyArea(area);
        }

        /// <summary>
        /// 获取区域树结构
        /// </summary>
        /// <param name="currentAreaID"></param>
        /// <returns></returns>
        public object GetAreaTree(Guid? currentAreaID)
        {
            return dal.GetAreaTree(currentAreaID);
        }
         /// <summary>
        /// 地区缴费分析统计
        /// </summary>
        /// <param name="pID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<dynamic> AreaChargeAnalysis(string pID, string startDate, string endDate)
        {
            return dal.AreaChargeAnalysis(pID,startDate,endDate);
        }
    }
}

