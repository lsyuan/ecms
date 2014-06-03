using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 分级收费，该表记录某些缴费项目在不同的项目数量时收取不同的单价，如地面清洁费，20平米时为3元每平米，40平米时为2元每平米
	/// </summary>
	[Serializable]
	[Table("分级收费", "T_Poly")]
	[Key("PK_T_POLY", "ID")]
	public partial class Poly
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public Poly()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 缴费项ID
		/// </summary>
		[Column("ItemID", "ItemID", "varchar", 32)]
		public string ItemID { get; set; }
		/// <summary>
		/// 单价
		/// </summary>
		[Column("UnitPrice", "UnitPrice", "decimal", 12)]
		public decimal UnitPrice { get; set; }
		/// <summary>
		/// 下限
		/// </summary>
		[Column("LowerBound", "LowerBound", "int")]
		public int LowerBound { get; set; }
		/// <summary>
		/// 上限
		/// </summary>
		[Column("上限", "HignerBound", "int")]
		public int HignerBound { get; set; }
		#endregion Model

	}
}

