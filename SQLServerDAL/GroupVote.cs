using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Ajax.Model;
using Ajax.DBUtility;
using System.Data.Common;

namespace Ajax.DAL
{
    /// <summary>
    /// 角色权限数据操作类
    /// </summary>
    public partial class GroupVoteDAL
    {
        /// <summary>
        /// 新增角色权限
        /// </summary>
        public bool AddGroupVote(List<Ajax.Model.GroupVote> groupVoteList)
        {
            using (DBHelper db = DBHelper.Create())
            {
                db.InsertBatch<GroupVote>(groupVoteList.ToArray());
                return true;
            }
        }
        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<dynamic> GetOperatorVoteByID(string groupID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select p.id,p.name,gv.VoteType from t_group g ");
            strSql.Append("inner join t_group_vote gv on gv.groupID=g.id ");
            strSql.Append("inner join t_poup p on p.id=gv.poupid ");
            strSql.Append("where g.id=@groupID");
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("groupID", groupID);
                return db.GetDynaminObjectList(strSql.ToString(), param);
            }
        }
        /// <summary>
        /// 保存角色对应的权限
        /// </summary>
        /// <param name="voteList"></param>
        public void SaveGroupVoteSetting(List<Ajax.Model.GroupVote> voteList)
        {
            if (voteList.Count == 0) return;
            List<CommandInfo> commandList = new List<CommandInfo>();
            //删除旧权限
            string strSqlDelOld = "delete from  T_Group_Vote where GroupID=@groupID";
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("groupID", voteList[0].GroupID);
                db.BeginTransaction();
                db.ExecuteNonQuery(strSqlDelOld, param);
                db.InsertBatch<GroupVote>(voteList.ToArray());
                db.Commit();
            }
        }
        /// <summary>
        /// 获取操作员所有权限
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="operatorID"></param>
        /// <returns></returns>
        public List<OperatorVote> GetOperVotes(string groupID, string operatorID)
        {
            List<OperatorVote> voteList = new List<OperatorVote>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select gv.VoteType,p.Path from T_Group_vote gv ");
            strSql.Append("left join T_Poup p on p.ID=gv.PoupID ");
            strSql.Append("where gv.GroupID=@GroupID ");
            strSql.Append("union ");
            strSql.Append("select ov.VoteType,p.Path from T_OperatorVote ov ");
            strSql.Append("left join T_Poup p on p.ID=ov.PoupID ");
            strSql.Append("where ov.OperatorID=@OperatorID"); 
            using (DBHelper db = DBHelper.Create())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("GroupID", groupID);
                param.Add("OperatorID", operatorID);
                using (DbDataReader sdr = db.ExecuteReader(strSql.ToString(), param))
                {
                    while (sdr != null && sdr.Read())
                    {
                        OperatorVote ov = new OperatorVote();
                        string strVoteCount = sdr["VoteType"].ToString();
                        ov.PoupID = sdr["Path"].ToString();
                        if (!string.IsNullOrEmpty(strVoteCount))
                        {
                            ov.VoteType = Convert.ToInt32(strVoteCount);
                        }
                        voteList.Add(ov);
                    }
                }
            }
            return voteList;
        }

    }
}
