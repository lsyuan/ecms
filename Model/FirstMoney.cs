using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 期初欠费表
	/// </summary>
	[Serializable]
	[Table("期初欠费表", "T_FirstMoney")]
	[Key("PK_T_FIRSTMONEY", "ID")]
	public partial class FirstMoney
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public FirstMoney()
		{ }
		#region Model
		/// <summary>
		/// ID
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 客户ID
		/// </summary>
		[Column("客户ID", "CustomerID", "varchar", 32)]
		public string CustomerID { get; set; }
		/// <summary>
		/// 欠费金额
		/// </summary>
		[Column("欠费金额", "Money", "decimal", 12)]
		public decimal Money { get; set; }
		/// <summary>
		/// 状态，0未缴费，1待审核，2已缴费，3已删除
		/// </summary>
		[Column("状态", "Status", "int")]
		public int Status { get; set; }
		/// <summary>
		/// 缴费时间
		/// </summary>
		[Column("缴费时间", "ChargeDate", "DateTime")]
		public DateTime? ChargeDate { get; set; }
		/// <summary>
		/// 欠费年份
		/// </summary>
		[Column("欠费年份", "Year", "varchar", 4)]
		public string Year { get; set; }
		#endregion Model

	}
	/// <summary>
	/// 初始费用状态
	/// </summary>
	public enum FirstMoneStatus
	{
		未缴费 = 0,
		已缴费 = 1,
		已删除 = 2
	}
}

