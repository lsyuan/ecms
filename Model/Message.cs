using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 消息表
	/// </summary>
	[Serializable]
	[Table("消息表", "T_Message")]
	[Key("PK_T_MESSAGE", "ID")]
	public partial class Message
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public Message()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 消息标题
		/// </summary>
		[Column("Title", "Title", "varchar", 400)]
		public string Title { get; set; }
		/// <summary>
		/// 内容
		/// </summary>
		[Column("Content", "Content", "varchar", 4000)]
		public string Content { get; set; }
		/// <summary>
		/// 操作员
		/// </summary>
		[Column("OperatorID", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 发布日期
		/// </summary>
		[Column("CreateDate", "CreateDate", "DateTime")]
		public DateTime CreateDate { get; set; }
		/// <summary>
		/// 状态，1正常，2已删除
		/// </summary>
		[Column("状态", "Status", "int")]
		public int Status { get; set; }
		#endregion Model

	}
}

