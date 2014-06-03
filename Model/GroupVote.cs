using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 角色权限表
	/// </summary>
	[Serializable]
	[Table("角色对应菜单权限详细信息表", "T_Group_Vote")]
	[Key("PK_T_GROUP_VOTE", "ID")]
	public partial class GroupVote
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public GroupVote()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 菜单节点ID
		/// </summary>
		[Column("菜单节点ID", "PoupID", "varchar", 32)]
		public string PoupID { get; set; }
		/// <summary>
		/// 角色ID
		/// </summary>
		[Column("角色ID", "GroupID", "varchar", 32)]
		public string GroupID { get; set; }
		/// <summary>
		/// 权限分配
		/// </summary>
		[Column("", "VoteType", "int")]
		public int VoteType { get; set; }
		#endregion Model

	}
}

