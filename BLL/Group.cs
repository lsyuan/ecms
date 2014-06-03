using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ajax.Model;
using Ajax.Common;

namespace Ajax.BLL
{
    /// <summary>
    /// 角色业务类
    /// </summary>
    public partial class GroupRule
    {
        private readonly Ajax.DAL.GroupDAL groupDal = new DAL.GroupDAL();
        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool AddGroup(Ajax.Model.Group group)
        {
            if (string.IsNullOrEmpty(group.ID))
            {
				group.ID = Guid.NewGuid().ToString("N");
            }
            return groupDal.AddGroup(group);
        }
        /// <summary>
        /// 角色查询json
        /// </summary>
        /// <param name="param"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public List<dynamic> SearchGroup(EasyUIGridParamModel param, Group g,out int itemCount)
        {
            return groupDal.SearchGroup(param, g,out itemCount);
        }
        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        public List<Ajax.Model.Group> GetAllGroup()
        {
            return groupDal.GetAllList();
        }
        /// <summary>
        /// 获取指定角色的所有权限
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<Model.Poup> GetAllVoteByGroupID(string groupId)
        {
            return groupDal.GetAllVoteByGroupID(groupId);
        }
        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="g"></param>
        public void ModifyGroup(Group g)
        {
            groupDal.ModifyGroup(g);
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public bool DeleteGroup(List<string> guids)
        {
            return groupDal.DeleteGroup(guids);
        }
    }
}
