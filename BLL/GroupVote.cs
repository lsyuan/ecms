using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;

namespace Ajax.BLL
{
    /// <summary>
    /// 角色权限业务操作类
    /// </summary>
    public partial class GroupVoteRule
    {
        Ajax.DAL.GroupVoteDAL dal = new DAL.GroupVoteDAL();
        /// <summary>
        /// 新增角色权限
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="voteIDs"></param>
        /// <returns></returns>
        public bool AddGroupVote(string groupID, List<string> voteIDs)
        {
            if (string.IsNullOrEmpty(groupID))
            {
                return false;
            }
            List<Ajax.Model.GroupVote> groupVoteList = new List<Model.GroupVote>();
            foreach (string voteID in voteIDs)
            {
                Ajax.Model.GroupVote groupVote = new Model.GroupVote();
                groupVote.ID = Guid.NewGuid().ToString("N");
                groupVote.PoupID = voteID;
                groupVote.GroupID = groupID;
                groupVoteList.Add(groupVote);
            }
            return dal.AddGroupVote(groupVoteList);
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<dynamic> GetOperatorVoteByID(string groupID)
        {
            return dal.GetOperatorVoteByID(groupID);
        }

        /// <summary>
		/// 保存角色对应的权限
		/// </summary>
		/// <param name="voteList"></param>
        public void SaveGroupVoteSetting(List<Ajax.Model.GroupVote> voteList)
        {
             dal.SaveGroupVoteSetting(voteList);
        }

        /// <summary>
        /// 获取操作员所有权限
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="operatorID"></param>
        /// <returns></returns>
        public List<OperatorVote> GetOperVotes(string groupID, string operatorID)
        {
            return dal.GetOperVotes(groupID,operatorID);
        }
    }
}
