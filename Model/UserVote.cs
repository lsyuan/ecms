using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 用户权限表
	/// </summary>
	[Serializable]
	[Table("用户权限表", "T_UserVote")]
	[Key("PK_T_UserVote", "ID")]
	public partial class UserVote
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public UserVote()
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
		[Column("菜单节点ID", "PID", "varchar", 32)]
		public string PID { get; set; }
		/// <summary>
		/// 操作用户ID
		/// </summary>
		[Column("操作用户ID", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		#endregion Model

	}
}

