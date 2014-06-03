using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 业务日志表
	/// </summary>
	[Serializable]
	[Table("业务日志表", "T_BusinessLog")]
	[Key("PK_T_BUSINESSLOG", "ID")]
	public partial class BusinessLog
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public BusinessLog()
		{ }
		#region Model
		/// <summary>
		/// ID
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 操作员
		/// </summary>
		[Column("操作员", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 日志类型
		/// </summary>
		[Column("日志类型", "LogTypeID", "varchar", 32)]
		public string LogTypeID { get; set; }
		/// <summary>
		/// 客户ID
		/// </summary>
		[Column("客户ID", "CustomerID", "varchar", 32)]
		public string CustomerID { get; set; }
		/// <summary>
		/// 协议ID，对某个客户进行协议操作时记录改值
		/// </summary>
		[Column("协议ID", "AgreeMentID", "varchar", 32)]
		public string AgreeMentID { get; set; }
		/// <summary>
		/// 记录时间
		/// </summary>
		[Column("记录时间", "CreateTime", "DateTime")]
		public DateTime CreateTime { get; set; }
		#endregion Model

	}
}

