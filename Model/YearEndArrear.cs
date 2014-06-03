using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 年终欠费表
	/// </summary>
	[Serializable]
	[Table("年终欠费表", "T_YearEndArrear")]
	[Key("PK_T_YearEndArrear", "ID")]
	public partial class YearEndArrear
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public YearEndArrear()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 客户ID
		/// </summary>
		[Column("CustomerID", "CustomerID", "varchar", 32)]
		public string CustomerID { get; set; }
		/// <summary>
		/// 欠费金额
		/// </summary>
		[Column("Money", "Money", "decimal", 12)]
		public decimal Money { get; set; }
		/// <summary>
		/// 年份
		/// </summary>
		[Column("Year", "Year", "int")]
		public int Year { get; set; }
		/// <summary>
		/// 0欠费未缴纳，1欠费已缴纳，2呆坏账
		/// </summary>
		[Column("Status", "Status", "int")]
		public int Status { get; set; }
		/// <summary>
		/// 缴费时间，没有缴纳欠费金额之前取空值
		/// </summary>
		[Column("ChargeDate", "ChargeDate", "DateTime")]
		public DateTime? ChargeDate { get; set; }
		#endregion Model

	}
}

