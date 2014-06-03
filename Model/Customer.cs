using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 客户表
	/// </summary>
	[Serializable]
	[Table("客户表", "T_Customer")]
	[Key("PK_T_CUSTOMER", "ID")]
	public partial class Customer
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public Customer()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 客户编号
		/// </summary>
		[Column("客户编号", "Code", "int", 40)]
		public int Code { get; set; }
		/// <summary>
		/// 父客户ID，即代理收费客户ID
		/// </summary>
		[Column("父客户ID", "PID", "varchar", 32)]
		public string PID { get; set; }
		/// <summary>
		/// 客户名称
		/// </summary>
		[Column("客户名称", "Name", "varchar", 200)]
		public string Name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("客户名称拼音", "PY", "varchar", 200)]
		public string PY { get; set; }
		/// <summary>
		/// 联系人
		/// </summary>
		[Column("联系人", "Contactor", "varchar", 30)]
		public string Contactor { get; set; }
		/// <summary>
		/// 联系电话
		/// </summary>
		[Column("联系电话", "Phone", "varchar", 30)]
		public string Phone { get; set; }
		/// <summary>
		/// 手机
		/// </summary>
		[Column("手机", "MobilePhone", "varchar", 30)]
		public string MobilePhone { get; set; }
		/// <summary>
		/// 地址
		/// </summary>
		[Column("地址", "Address", "varchar", 200)]
		public string Address { get; set; }
		/// <summary>
		/// 客户类型
		/// </summary>
		[Column("客户类型", "TypeID", "varchar", 32)]
		public string TypeID { get; set; }
		/// <summary>
		/// 所在区域
		/// </summary>
		[Column("所在区域", "AreaID", "varchar", 32)]
		public string AreaID { get; set; }
		/// <summary>
		/// 客户开始缴费时间，默认取本月
		/// </summary>
		[Column("客户开始缴费时间", "BeginChargeDate", "DateTime")]
		public DateTime? BeginChargeDate { get; set; }
		/// <summary>
		/// 操作用户
		/// </summary>
		[Column("操作用户", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 客户状态，-1全部 0待审批，1启用，2禁用，3已删除，4 审批未通过
		/// </summary>
		[Column("客户状态", "Status", "int")]
		public int Status { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		[Column("创建时间", "CreateTime", "DateTime")]
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 更新时间
		/// </summary>
		[Column("更新时间", "UpdateTime", "DateTime")]
		public DateTime UpdateTime { get; set; }
		/// <summary>
		/// 负责人
		/// </summary>
		[Column("负责人", "ManagerID", "varchar", 32)]
        public string ManagerID { get; set; }
        /// <summary>
        /// 代收费权限（0：无，1：有）
        /// </summary>
        [Column("代收费权限", "Agent", "int")]
        public int Agent { get; set; }
		#endregion Model

	}
}

