using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 缴费项分类表
	/// </summary>
	[Serializable]
	[Table("缴费项分类表", "T_ChargeItemCategory")]
	[Key("PK_T_CHARGEITEMCATEGORY", "ID")]
	public partial class ChargeItemCategory : BaseResult
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public ChargeItemCategory()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar",32)]
		public string ID { get; set; }
		/// <summary>
		/// 分类名称
		/// </summary>
		[Column("分类名称", "Name", "varchar")]
		public string Name { get; set; }
		#endregion Model

	}
}

