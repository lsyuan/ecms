using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 票据使用记录
	/// </summary>
	[Serializable]
	[Table("票据使用记录", "T_InvoiceUseLog")]
	[Key("PK_T_INVOICEUSELOG", "ID")]
	public partial class InvoiceUseLog
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public InvoiceUseLog()
		{ }
		#region Model
		/// <summary>
		/// ID
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 票据编号
		/// </summary>
		[Column("票据编号", "InvoiceCode", "varchar", 40)]
		public string InvoiceCode { get; set; }
		/// <summary>
		/// 使用人
		/// </summary>
		[Column("使用人", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 票据状态，1正常使用，2已作废，3直接作废
		/// </summary>
		[Column("票据状态", "Status", "int")]
		public int Status { get; set; }
		/// <summary>
		/// 业务来源：正常缴费、临时缴费、其他缴费，插入该表时记录对应收费表的ID
		/// </summary>
		[Column("业务来源", "BusinessID", "varchar", 30)]
		public string BusinessID { get; set; }
		/// <summary>
		/// 业务类型，1正常缴费，2临时缴费，3其他缴费，4罚款单
		/// </summary>
		[Column("业务类型", "BusinessType", "int")]
		public int BusinessType { get; set; }
		/// <summary>
		/// 使用时间
		/// </summary>
		[Column("使用时间", "CreateTime", "DateTime")]
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("最后更新时间", "UpdateTime", "DateTime")]
		public DateTime UpdateTime { get; set; }
		/// <summary>
		/// 票据登记ID
		/// </summary>
		[Column("票据登记ID", "InvoiceRegisterID", "varchar", 30)]
		public string InvoiceRegisterID { get; set; }
		#endregion Model

	}
	/// <summary>
	/// 票据使用状态
	/// </summary>
	public enum InvoiceStatus
	{
		正常使用 = 1,
		已作废 = 2,
		直接作废 = 3
	}
}

