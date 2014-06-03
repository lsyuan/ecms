using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 票据登记
	/// </summary>
	[Serializable]
	[Table("票据登记", "T_InvoiceRegister")]
	[Key("PK_T_INVOICEREGISTER", "ID")]
	public partial class InvoiceRegister
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public InvoiceRegister()
		{ }
		#region Model
		/// <summary>
		/// ID
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 起始票据编号
		/// </summary>
		[Column("起始票据编号", "BeginCode", "varchar", 30)]
		public string BeginCode { get; set; }
		/// <summary>
		/// 结束票据编号
		/// </summary>
		[Column("结束票据编号", "EndCode", "varchar", 30)]
		public string EndCode { get; set; }
		/// <summary>
		/// 当前使用编号
		/// </summary>
		[Column("当前使用编号", "CurrentCode", "varchar", 30)]
		public string CurrentCode { get; set; }
		/// <summary>
		/// 登记人
		/// </summary>
		[Column("登记人", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 登记时间
		/// </summary>
		[Column("登记时间", "RegisterTime", "DateTime")]
		public DateTime RegisterTime { get; set; }
		/// <summary>
		/// 最后使用时间
		/// </summary>
        [Column("最后使用时间", "LastUseTime", "DateTime")]
		public DateTime? LastUseTime { get; set; }
		/// <summary>
		/// 整个票段的使用状态，0已登记未使用，1正在使用
		/// </summary>
		[Column("整个票段的使用状态", "UseStatus", "int")]
		public int UseStatus { get; set; }
		/// <summary>
		/// 票据类型
		/// </summary>
		[Column("票据类型", "InvoiceType", "varchar", 32)]
		public string InvoiceType { get; set; }
		#endregion Model

	}
}

