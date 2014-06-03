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
    /// 数据访问类:Poup
    /// </summary>
    public partial class PoupDAL
    {
        public PoupDAL()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.Exist<Poup>(ID);
            }
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(Poup model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<Poup>(model);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Poup model)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<Poup>(model);
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
                return db.DeleteByID<Poup>(ID);
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_Poup ");
            strSql.Append(" where ID in (" + IDlist + ")  ");
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(strSql.ToString()) > 0;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Poup GetModel(string ID)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetById<Poup>(ID);
            }
        }

        /// <summary>
        /// 获得数据列表 
        /// </summary>
        public List<Poup> GetList(string strWhere)
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<Poup>(strWhere);
            }
        }

        /// <summary>
        /// 获取菜单节点
        /// </summary>
        /// <returns></returns>
        public List<object> GetPoupList(Poup poup, string userID)
        {
            using (DBHelper db = DBHelper.Create())
            {

                if (poup.PID == null)
                {
                    string sql = "select * from t_poup  order by value asc";
                    return db.GetDynaminObjectList(sql, null);
                }
                else
                {
                    string sql = "select * from t_poup where PID = @PID order by value asc";
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("PID", poup.PID);
                    return db.GetDynaminObjectList(sql, param);
                }
            }
        }
        /// <summary>
        /// 根据ID获取单个菜单节点
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public object GetPoup(string ID)
        {
            string sql = "select * from t_poup where ID = @ID";
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ID", ID);
                return db.GetSingelDynaminObject(sql, param);
            }
        }
        /// <summary>
        /// 新增菜单节点
        /// </summary>
        /// <param name="poup"></param>
        /// <returns></returns>
        public object AddPoup(Poup poup)
        {
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string selectCode = "select max(Value) from T_Poup where PID = @PID or ID = @ID";
                param.Add("PID", poup.PID);
                param.Add("ID", poup.ID);
                string code = db.ExcuteScular(selectCode, param).ToString();
                string selectPCode = "select Value from T_Poup where ID = @ID";
                param.Clear();
                param.Add("ID", poup.PID);
                string PCode = db.ExcuteScular(selectPCode, param).ToString();
                code = (Convert.ToInt32(code) + 1).ToString();
                string id = Guid.NewGuid().ToString().Replace("-", "");
                Poup myPoup = new Poup() { ID = id, Name = poup.Name, IsValid = 1, Path = poup.Path, PID = poup.PID, PValue = "", Value = code };
                try
                {
                    db.Insert<Poup>(myPoup);
                    return id;
                }
                catch (System.Exception ex)
                {
                    return null;
                    throw ex;
                }
            }

        }
        /// <summary>
        /// 更新菜单节点
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Name">菜单名称</param>
        /// <param name="Path">菜单节点路径</param>
        /// <returns></returns>
        public bool ModifyPoup(string ID, string Name, string Path)
        {
            string sql = "update T_Poup set Name= @Name,Path=@Path where ID =@ID";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("NAME", Name);
            param.Add("Path", Path);
            param.Add("ID", ID);
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(sql, param) > 0;
            }
        }
        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool DeletePoup(string ID)
        {
            string sql = "delete from T_Poup where ID = @ID or PID = @PID";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ID", ID);
            param.Add("PID", ID);
            using (DBHelper db = DBHelper.Create())
            {
                return db.ExecuteNonQuery(sql, param) > 0;
            }
        }
        #endregion  Method

    }
}

