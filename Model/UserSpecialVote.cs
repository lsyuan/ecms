using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 用户特殊权限表
	/// </summary>
	[Serializable]
	[Table("用户特殊权限表", "T_UserSpecialVote")]
	[Key("PK_T_UserSpecialVote", "ID")]
	public partial class UserSpecialVote
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public UserSpecialVote()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar",32)]
		public string ID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("OperatorID", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("VoteID", "VoteID", "varchar", 32)]
		public string VoteID { get; set; }
		#endregion Model

	}
}

