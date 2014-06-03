using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 客户类型缴费项对应表
	/// </summary>
	[Serializable]
	[Table("缴费项单位", "T_TypeToItem")]
	[Key("PK_T_TYPETOITEM", "ID")]
	public partial class TypeToItem
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public TypeToItem()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar",32)]
		public string ID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("TypeID", "TypeID", "varchar", 32)]
		public string TypeID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("ItemID", "ItemID", "varchar", 32)]
		public string ItemID { get; set; }
		#endregion Model

	}
}

