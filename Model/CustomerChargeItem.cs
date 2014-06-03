using System;
using Ajax.DBUtility;

namespace Ajax.Model
{
	/// <summary>
	/// 客户交费对应表
	/// </summary>
	[Table("客户表", "T_CustomerChargeItem")]
	[Key("PK_T_CUSTOMERCHARGEITEM", "ID")]
	public class CustomerChargeItem
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public CustomerChargeItem()
		{ }
		/// <summary>
		/// 主键
		/// </summary>
		[Column("主键", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 缴费项ID
		/// </summary>
		[Column("缴费项ID", "ItemID", "varchar", 32)]
		public string ItemID { get; set; }
		/// <summary>
		/// 客户ID
		/// </summary>
		[Column("客户ID", "CustomerID", "varchar", 32)]
		public string CustomerID { get; set; }
		/// <summary>
		/// 数量
		/// </summary>
		[Column("数量", "Count", "decimal", 12)]
		public decimal Count { get; set; }
		/// <summary>
		/// 协议金额
		/// </summary>
		[Column("协议金额", "AgreementMoney", "decimal", 12)]
		public decimal AgreementMoney { get; set; }
	}
}
