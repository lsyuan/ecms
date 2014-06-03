using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 操作员消息表
	/// </summary>
	[Serializable]
	[Table("操作员消息表", "T_OperatorMsg")]
	[Key("PK_T_OPERATORMSG", "ID")]
	public partial class OperatorMsg
	{
		public OperatorMsg()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("MsgID", "MsgID", "varchar", 32)]
		public string MsgID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("OperatorID", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 状态，0未读，1已读
		/// </summary>
		[Column("Status", "Status", "int")]
		public int Status { get; set; }
		#endregion Model

	}
}

