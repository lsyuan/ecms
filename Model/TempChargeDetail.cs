using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 临时收费详细表
	/// </summary>
	[Serializable]
	[Table("临时收费详细表", "T_TempChargeDetail")]
	[Key("PK_T_TEMPCHARGEDETAIL", "ID")]
	public partial class TempChargeDetail
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public TempChargeDetail()
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
		[Column("TempChargeID", "TempChargeID", "varchar", 32)]
		public string TempChargeID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("ItemID", "ItemID", "varchar", 32)]
		public string ItemID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Count", "Count", "decimal", 12)]
		public decimal Count { get; set; }
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
		[Column("Status", "Status", "int")]
		public int Status { get; set; }
		#endregion Model

	}
}

