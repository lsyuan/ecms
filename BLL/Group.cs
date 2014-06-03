using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ajax.Model;
using Ajax.Common;

namespace Ajax.BLL
{
    /// <summary>
    /// ��ɫҵ����
    /// </summary>
    public partial class GroupRule
    {
        private readonly Ajax.DAL.GroupDAL groupDal = new DAL.GroupDAL();
        /// <summary>
        /// ������ɫ
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
        /// ��ɫ��ѯjson
        /// </summary>
        /// <param name="param"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public List<dynamic> SearchGroup(EasyUIGridParamModel param, Group g,out int itemCount)
        {
            return groupDal.SearchGroup(param, g,out itemCount);
        }
        /// <summary>
        /// ��ȡ���н�ɫ
        /// </summary>
        /// <returns></returns>
        public List<Ajax.Model.Group> GetAllGroup()
        {
            return groupDal.GetAllList();
        }
        /// <summary>
        /// ��ȡָ����ɫ������Ȩ��
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<Model.Poup> GetAllVoteByGroupID(string groupId)
        {
            return groupDal.GetAllVoteByGroupID(groupId);
        }
        /// <summary>
        /// �༭��ɫ
        /// </summary>
        /// <param name="g"></param>
        public void ModifyGroup(Group g)
        {
            groupDal.ModifyGroup(g);
        }
        /// <summary>
        /// ɾ����ɫ
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public bool DeleteGroup(List<string> guids)
        {
            return groupDal.DeleteGroup(guids);
        }
    }
}
