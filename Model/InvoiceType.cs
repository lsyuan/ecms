using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 票据类型字典
	/// </summary>
	[Serializable]
	[Table("票据类型字典", "T_InvoiceType")]
	[Key("PK_T_INVOICETYPE", "ID")]
	public partial class InvoiceType
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public InvoiceType()
		{ }
		#region Model
		/// <summary>
		/// ID
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 票据类型名称
		/// </summary>
		[Column("票据类型名称", "Name", "varchar", 40)]
		public string Name { get; set; }
		/// <summary>
		/// 执行收费标准
		/// </summary>
		[Column("执行收费标准", "Standard", "varchar", 80)]
		public string Standard { get; set; }
        /// <summary>
        /// 发票代码
        /// </summary>
        [Column("发票代码", "Code", "varchar", 40)]
        public string Code { get; set; }
        /// <summary>
        /// 号码增量
        /// </summary>
        [Column("号码增量", "StepValue", "int")]
        public int StepValue { get; set; }
		#endregion Model

	}
}

