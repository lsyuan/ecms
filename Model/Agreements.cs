using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 客户协议信息表
	/// </summary>
	[Serializable]
	[Table("客户协议信息表", "T_Agreements")]
	[Key("PK_T_AGREEMENTS", "ID")]
	public partial class Agreements
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public Agreements()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 协议编号,自定义
		/// </summary>
		[Column("Code", "Code", "varchar", 40)]
		public string Code { get; set; }
		/// <summary>
		/// 客户ID
		/// </summary>
		[Column("客户ID", "CustomerID", "varchar", 32)]
		public string CustomerID { get; set; }
		/// <summary>
		/// 本时段的协议金额
		/// </summary>
		[Column("本时段的协议金额", "Money", "decimal", 12)]
		public decimal Money { get; set; }
		/// <summary>
		/// 协议状态 ,0 未审核,1 审核已通过，2已删除，3审核未通过，4已缴纳完费用
		/// </summary>
		[Column("协议状态", "Status", "int")]
		public int Status { get; set; }
		/// <summary>
		/// 协议开始时间
		/// </summary>
		[Column("协议开始时间", "BeginDate", "DateTime")]
		public DateTime BeginDate { get; set; }
		/// <summary>
		/// 协议结束时间
		/// </summary>
		[Column("协议结束时间", "EndDate", "DateTime")]
		public DateTime EndDate { get; set; }
		/// <summary>
		/// 操作员ID
		/// </summary>
		[Column("操作员ID", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 审核人
		/// </summary>
		[Column("审核人", "CheckOperatorID", "varchar", 32)]
		public string CheckOperatorID { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[Column("创建时间", "CreateTime", "DateTime")]
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 审核时间
		/// </summary>
		[Column("审核时间", "CheckTime", "DateTime")]
		public DateTime CheckTime { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[Column("备注", "Remark", "varchar", 200)]
		public string Remark { get; set; }
		#endregion Model

	}
	/// <summary>
	/// 协议状态
	/// </summary>
	public enum AgreementStatus
	{
		未审批 = 0,
		已审批 = 1,
		已删除 = 2,
		已过期 = 3
	}
}

