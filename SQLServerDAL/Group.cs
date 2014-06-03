using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using Ajax.Model;
using Ajax.DBUtility;
using Ajax.Common;
using System.Data.Common;

namespace Ajax.DAL
{
    /// <summary>
    /// 系统角色
    /// </summary>
    public partial class GroupDAL
    {
        public GroupDAL()
        { }
        /// <summary>
        /// 获取角色所有信息
        /// </summary>
        /// <returns></returns>
        public List<Ajax.Model.Group> GetAllList()
        {
            using (DBHelper db = DBHelper.Create())
            {
                return db.GetList<Group>("");
            }
        }
        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="gorup"></param>
        public bool AddGroup(Model.Group group)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Insert<Group>(group);
                return true;
            }
        }
        /// <summary>
        /// 获取指定角色的所有权限
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<Model.Poup> GetAllVoteByGroupID(string groupID)
        {
            List<Model.Poup> poupList = new List<Poup>();
            string strSql = @"select gv.poupID from T_group g
                              left join t_group_vote gv on g.ID=gv.GroupID
                              where g.ID=@GroupID";
            SqlParameter parame = new SqlParameter("@GroupID", SqlDbType.VarChar);
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("GroupID", groupID);
            using (DBHelper db = DBHelper.Create())
            {
                using (DbDataReader sdr = db.ExecuteReader(strSql, param))
                {
                    while (sdr != null && sdr.Read())
                    {
                        Model.Poup p = new Poup();
                        p.ID = sdr["poupID"].ToString();
                        poupList.Add(p);
                    }
                }
            }
            return poupList;
        }

        /// <summary>
        /// 角色查询json
        /// </summary>
        /// <param name="param"></param>
        /// <param name="g"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public List<dynamic> SearchGroup(EasyUIGridParamModel param, Group g, out int itemCount)
        {
            StringBuilder strSql = new StringBuilder("select ID,Name from T_group g ");
            strSql.Append("where 1=1 ");
            Dictionary<string, object> paramLists = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(g.NAME))
            {
                strSql.Append("and Name like @Name ");
                paramLists.Add("Name", string.Format("%{0}%", g.NAME));
            }
            int pageIndex = Convert.ToInt32(param.page) - 1;
            int pageSize = Convert.ToInt32(param.rows);
            using (DBHelper db = DBHelper.Create())
            {
                string sql = strSql.ToString();
                itemCount = db.GetCount(sql, paramLists);
                return db.GetDynaminObjectList(sql, pageIndex, pageSize, "ID", paramLists);
            }
        }
        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="g"></param>
        public void ModifyGroup(Group g)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.Update<Group>(g);
            }
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public bool DeleteGroup(List<string> guids)
        {
            bool flag = true;
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string deleteGroupVote = "delete from T_Group_Vote where GroupID=@groupID";
                string deleteGroup = "delete from T_group where ID=@ID";
                string strSqlExist = "select Count(ID) from t_Operator where GroupID=@GroupID";
                for (int i = 0; i < guids.Count; i++)
                {
                    db.BeginTransaction();
                    param.Add("GroupID", guids[i]);
                    int Count = db.GetCount(strSqlExist, param);
                    if (Count == 0)
                    {
                        //关系表
                        param.Clear();
                        param.Add("GroupID", guids[i]);
                        db.ExecuteNonQuery(deleteGroupVote, param);
                        //角色表
                        param.Clear();
                        param.Add("ID", guids[i]);
                        db.ExecuteNonQuery(deleteGroup, param);
                    }
                    else
                    {
                        flag = false;  
                    }
                }
                if (!flag)
                {
                    db.RollBack();
                }
                db.Commit();
            }
            return flag;
        }
    }
}
