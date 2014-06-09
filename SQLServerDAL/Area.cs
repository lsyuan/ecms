using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using System.Collections.Generic;
namespace Ajax.DAL
{
	/// <summary>
	/// 数据访问类:Area
	/// </summary>
	public partial class AreaDAL
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public AreaDAL()
		{ }
		#region  Method

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				Area area = db.GetById<Area>(ID);
				return area != null && area.ID != null;
			}
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Area model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Insert<Area>(model);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Area model)
		{
			using (DBHelper db = DBHelper.Create())
			{
				db.Update<Area>(model);
				return true;
			}
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.DeleteByID<Area>(ID);
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from T_Area ");
			strSql.Append(" where ID in (" + IDlist + ")  ");
			using (DBHelper db = DBHelper.Create())
			{
				return db.ExecuteNonQuery(strSql.ToString()) > 0;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Area GetModel(string ID)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetById<Area>(ID);
			}
		}

		/// <summary>
		/// 获得地区列表
		/// </summary>
		public List<Area> GetList(string strWhere)
		{
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetList<Area>(strWhere, null, "", "");
			}
		}
		/// <summary>
		/// 获取树形grid数据
		/// </summary>
		/// <returns></returns>
		public List<dynamic> GetTreeGridList()
		{
			string strSql = @"select a.ID,PID,Code,a.Name,Manager,e.name as ManagerName 
							FROM T_Area a
							left join T_Employee e on a.manager=e.ID ";
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(strSql, null);
			}
		}
		#endregion  Method
		/// <summary>
		/// 获取所有区域
		/// </summary>
		/// <param name="area"></param>
		/// <returns></returns>
		public List<object> GetAreaList(Area area)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(" where 1 =1");
			if (!string.IsNullOrEmpty(area.ID))
			{
				sb.Append(" and ID=@ID");
			}
			if (string.IsNullOrEmpty(area.PID))
			{
				sb.Append(" and PID is null");
			}
			else
			{
				sb.Append(" and pid = '" + area.PID + "'");
			}
			if (!string.IsNullOrEmpty(area.Name))
			{
				sb.Append(" and Name like '%" + area.Name + "%'");
			}
			string sql = "select id,pid,code,name from T_Area {0}";
			sql = string.Format(sql, sb.ToString());
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(sql, null);
			}
		}
		/// <summary>
		/// 获取某个区域信息
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public object GetArea(string ID)
		{
			string sql = "select id,pid,code,name from T_Area where ID=@ID";
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> paramDic = new Dictionary<string, object>();
				paramDic.Add("ID", ID);
				return db.GetSingelDynaminObject(sql, paramDic);
			}
		}
		/// <summary>
		/// 新增区域,返回新增区域的对象
		/// </summary>
		/// <param name="area"></param>
		/// <returns></returns>
		public object AddArea(Area area)
		{
			using (DBHelper db = DBHelper.Create())
			{
				area.ID = Guid.NewGuid().ToString().Replace("-", "");
				string selectCode = "select max(code) from T_Area where PID = @PID";
				Dictionary<string, object> paramList = new Dictionary<string, object>();
				paramList.Add("PID", area.PID);
				object maxCode = db.ExcuteScular(selectCode, paramList);
				string code = maxCode == null ? "" : maxCode.ToString();
				string selectPCode = "select code from T_Area where ID = @ID";
				paramList.Clear();
				paramList.Add("ID", area.PID);
				object PCode = db.ExcuteScular(selectPCode, paramList);
				if (string.IsNullOrEmpty(code))
					code = PCode + "0001";
				else
					code = code.Substring(0, code.Length - 4) + (Convert.ToInt32(code.Substring(code.Length - 4)) + 1).ToString().PadLeft(4, '0');
				area.Code = code;
				area.Code = PCode + area.Code;
				try
				{
					return db.Insert<Area>(area);
				}
				catch (System.Exception ex)
				{
					area.State = "1";
					area.Errormsg = ex.Message;
					return area;
				}
			}
		}
		/// <summary>
		/// 修改区域
		/// </summary>
		/// <param name="area"></param>
		/// <returns></returns>
		public bool ModifyArea(Area area)
		{
			string sql = "update T_AREA set NAME=@NAME,Manager=@Manager where ID=@ID";
			using (DBHelper db = DBHelper.Create())
			{
				Dictionary<string, object> paramDic = new Dictionary<string, object>();
				paramDic.Add("NAME", area.Name);
				paramDic.Add("Manager", area.Manager);
				paramDic.Add("ID", area.ID);
				int effectLine = db.ExecuteNonQuery(sql, paramDic);
				return effectLine > 0 ? true : false;
			}
		}
		/// <summary>
		/// 获取区域树结构
		/// </summary>
		/// <param name="currentAreaID">当前区域ID</param>
		/// <returns></returns>
		public object GetAreaTree(Guid? currentAreaID)
		{
			string sql = string.Empty;
			if (!currentAreaID.HasValue)
			{
				sql = "SELECT id,pid,code ,NAME AS text,'false' AS expanded FROM T_Area ta";
			}
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(sql, null);
			}
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
			string strSql = string.Format(@"select a.ID,a.name,isnull(sum(cg.Money),0) as  feeCount 
                    from T_Area a
                    left join T_Customer c on c.AreaID=a.ID
                    left join T_charge cg on c.ID=cg.CustomerID and cg.CreateDate>='{0}' and cg.CreateDate<='{1}'
                    where a.pID='{2}'
                    group by a.ID,a.name", startDate, endDate + " 23:59:59", pID);
			using (DBHelper db = DBHelper.Create())
			{
				return db.GetDynaminObjectList(strSql, null);
			}
		}
	}
}

