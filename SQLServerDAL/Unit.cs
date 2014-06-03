using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Ajax.Model;
using Ajax.DBUtility;
using System.Collections.Generic;
using Ajax.Common;
namespace Ajax.DAL
{
    /// <summary>
    /// 数据访问类:Unit
    /// </summary>
    public partial class UnitDAL
    {
        public UnitDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<Unit>(ID);
            }
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Unit model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<Unit>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Unit model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<Unit>(model);
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
                return db.DeleteByID<Unit>(ID);
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.BeginTransaction();
                string[] id = IDlist.Split(',');
                foreach (string item in id)
                {
                    db.DeleteByID<Unit>(item);
                }
                db.Commit();
                return true;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Unit GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<Unit>(ID);
            }
        }

        /// <summary>
        /// 获取单元列表
        /// </summary>
        /// <returns></returns>
        public List<object> GetUnit(string level)
        {
            string sql = "select id,name from T_Unit where level = @level";
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> paramList = new Dictionary<string, object>();
                paramList.Add("level", level);
                return db.GetDynaminObjectList(sql, paramList);
            }
        }
        /// <summary>
        /// 获取系统所有单位个数
        /// </summary>
        /// <returns></returns>
        public int GetAllUnitCount()
        {
            // 不等于0表示已删除
            string sql = "select count(0) from t_unit where  status != 0";
            using (DBHelper db = DBHelper.Create())
            {
                object o = db.ExcuteScular(sql, null);
                if (o != null)
                {
                    return Convert.ToInt32(o.ToString());
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 删除单位
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool DeleteUnit(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.DeleteByID<Unit>(ID);
            }
        }
        /// <summary>
        /// 检查单位是否在使用中
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool IsUsed(string ID)
        {
            string sql = "select count(0) from T_ChargeItem where UNITID1=@ID1 or UNITID2=@ID2";
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ID1", ID);
                param.Add("ID2", ID);
                object o = db.ExcuteScular(sql, param);
                if (o != null && o.ToString() != "0")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 查询付费单位列表json
        /// </summary>
        /// <param name="param"></param>
        /// <param name="unit"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<object> GetSerachJson(EasyUIGridParamModel param, Unit unit, out int itemCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,NAME,CASE tu.LEVEL WHEN 1 THEN '计量' else '计时' end as level,timevalue ");
            strSql.Append("FROM T_Unit tu where 1=1 ");
            Dictionary<string, object> paramList = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(unit.Name))
            {
                strSql.Append("and (Name like @Name or PY like @PY)");
                paramList.Add("Name", string.Format("%{0}%", unit.Name));
                paramList.Add("PY", string.Format("%{0}%", Pinyin.GetPinyin(unit.Name)));
            }
            if (unit.Status != null)
            {
                strSql.Append("and Status=@Status ");
                paramList.Add("Status", unit.Status);
            }
            int pageIndex = Convert.ToInt32(param.page) - 1;
            int pageSize = Convert.ToInt32(param.rows);
            using (DBHelper db = DBHelper.Create())
            {
                string sql = strSql.ToString();
                itemCount = db.GetCount(sql, paramList);
                return db.GetDynaminObjectList(sql, pageIndex, pageSize, "ID", paramList);
            }
        }
        /// <summary>
        /// 获取单位信息,用于JsonData
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public object GetSingelUnit(string ID)
        {
            string sql = "select ID,Name,level,TimeValue from t_Unit where id=@ID";
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ID", ID);
                return db.GetSingelDynaminObject(sql, param);
            }
        }
        /// <summary>
        /// 获取单位信息,用于数据更新
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public object GetSingelUnitValue(string ID)
        {
            string sql = "select ID,Name,CASE LEVEL WHEN 1 THEN '计量' else '计时' end as level,timeValue from t_Unit where id=@ID";
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ID",ID);
                return db.GetModelValue(sql, param);
            } 
        }
        #endregion  Method
    }

}

