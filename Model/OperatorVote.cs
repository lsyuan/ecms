using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 操作用户权限表，对用户授权之后根据分配的菜单和权限类型写入记录。PK_T_OPERATORVOTE
	/// </summary>
	[Serializable]
	[Table("操作用户权限表", "T_OperatorVote")]
	[Key("PK_T_OPERATORVOTE", "ID")]
	public partial class OperatorVote
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public OperatorVote()
		{ }
		#region Model
		/// <summary>
		/// ID
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 操作用户ID
		/// </summary>
		[Column("操作用户ID", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 菜单节点ID
		/// </summary>
		[Column("菜单节点ID", "PoupID", "varchar", 32)]
		public string PoupID { get; set; }
		/// <summary>
		/// 权限分配取值，为增删改查的四个权限组合，约定1增，2删，4改，8查。如取值为3表示含有增加和删除的权限；取0则表示没有任何权限
		/// </summary>
		[Column("权限分配取值", "VoteType", "int")]
		public int VoteType { get; set; }
		#endregion Model

	}
}

