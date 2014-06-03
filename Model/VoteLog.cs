using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 授权日志表
	/// </summary>
	[Serializable]
	[Table("授权日志表", "T_VoteLog")]
	[Key("PK_T_VoteLog", "ID")]
	public partial class VoteLog
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public VoteLog()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 操作员
		/// </summary>
		[Column("OperatorID", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 被授权操作员
		/// </summary>
		[Column("ToOperatorID", "ToOperatorID", "varchar", 32)]
		public string ToOperatorID { get; set; }
		/// <summary>
		/// 授权时间
		/// </summary>
		[Column("CreateTime", "CreateTime", "DateTime")]
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 权限ID
		/// </summary>
		[Column("VoteID", "VoteID", "varchar", 32)]
		public string VoteID { get; set; }

		/// <summary>
		/// 权限值
		/// </summary>
		[Column("VoteValue", "VoteValue", "int")]
		public int VoteValue { get; set; }
		#endregion Model

	}
}

