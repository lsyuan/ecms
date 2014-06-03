using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 临时缴费表
	/// </summary>
	[Serializable]
	[Table("临时缴费表", "T_TempCharge")]
	[Key("PK_T_TEMPCHARGE", "ID")]
	public partial class TempCharge
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public TempCharge()
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
		[Column("CustomerName", "CustomerName", "varchar", 400)]
		public string CustomerName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("ChargeName", "ChargeName", "varchar", 400)]
		public string ChargeName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Money", "Money", "decimal", 12)]
		public decimal Money { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("CreateTime", "CreateTime", "DateTime")]
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("OperatorID", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Remark", "Remark", "varchar", 400)]
		public string Remark { get; set; }
		/// <summary>
		/// 0待审核，1审核通过，2审核不通过
		/// </summary>
		[Column("Status", "Status", "int")]
		public int Status { get; set; }

		/// <summary>
		/// 实际缴费金额
		/// </summary>
		[Column("RealChargeMoney", "RealChargeMoney", "decimal", 12)]
		public decimal RealChargeMoney { get; set; }
		#endregion Model

	}
}

