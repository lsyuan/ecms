using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 缴费详细信息表 
	/// </summary>
	[Serializable]
	[Table("缴费详细信息表", "T_ChargeDetail")]
	[Key("PK_T_CHARGEDETAIL", "ID")]
	public partial class ChargeDetail
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public ChargeDetail()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 缴费表ID
		/// </summary>
		[Column("缴费表ID", "ChargeID", "varchar", 32)]
		public string ChargeID { get; set; }
		/// <summary>
		/// 缴费项目ID
		/// </summary>
		[Column("缴费项目ID", "ChargeItemID", "varchar", 32)]
		public string ChargeItemID { get; set; }
		/// <summary>
		/// 缴费项数量，以月为单位
		/// </summary>
		[Column("缴费项数量", "Month", "int")]
		public int Month { get; set; }
		/// <summary>
		/// 单项金额
		/// </summary>
		[Column("单项金额", "ItemMoney", "decimal")]
		public decimal ItemMoney { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("缴费时间", "CreateDate", "DateTime")]
		public DateTime CreateDate { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("状态", "Status", "int")]
		public int Status { get; set; }
		#endregion Model

	}
}

