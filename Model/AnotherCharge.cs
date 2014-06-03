using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 其他收费信息表，主要满足不包含在既定缴费项目中的缴费
	/// </summary>
	[Serializable]
	[Table("其他收费信息表", "T_AnotherCharge")]
	[Key("PK_T_ANOTHERCHARGE", "ID")]
	public partial class AnotherCharge
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public AnotherCharge()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 缴费客户名称
		/// </summary>
		[Column("缴费客户名称", "CustomerName", "varchar", 40)]
		public string CustomerName { get; set; }
		/// <summary>
		/// 应收总金额
		/// </summary>
		[Column("应收总金额", "Money", "decimal", 12)]
		public decimal Money { get; set; }
		/// <summary>
		/// 实收金额
		/// </summary>
		[Column("实收金额", "ActMoney", "decimal", 12)]
		public decimal ActMoney { get; set; }
		/// <summary>
		/// 收费状态
		/// </summary>
		[Column("收费状态", "Status", "int")]
		public int Status { get; set; }
		/// <summary>
		/// 缴费时间
		/// </summary>
		[Column("缴费时间", "ChargeDate", "DateTime")]
		public DateTime ChargeDate { get; set; }
		/// <summary>
		/// 操作用户ID
		/// </summary>
		[Column("操作用户ID", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		[Column("备注", "Remark", "varchar", 400)]
		public string Remark { get; set; }
		#endregion Model

	}
}

