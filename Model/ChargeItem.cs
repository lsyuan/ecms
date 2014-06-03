using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 缴费项表
	/// </summary>
	[Serializable]
	[Table("缴费项表", "T_ChargeItem")]
	[Key("PK_T_CHARGEITEM", "ID")]
	public partial class ChargeItem : BaseResult
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public ChargeItem()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 缴费项编号
		/// </summary>
		[Column("缴费项编号", "Code", "varchar", 40)]
		public string Code { get; set; }
		/// <summary>
		/// 缴费项名称
		/// </summary>
		[Column("缴费项名称", "Name", "varchar", 400)]
		public string Name { get; set; }
		/// <summary>
		/// 缴费项拼音
		/// </summary>
		[Column("缴费项拼音", "PY", "varchar", 400)]
		public string PY { get; set; }
		/// <summary>
		/// 单位1 如 4.4元/平方米/月中的平方米
		/// </summary>
		[Column("单位1", "UnitID1", "varchar", 32)]
		public string UnitID1 { get; set; }
		/// <summary>
		/// 单位2 如 4.4元/平方米/月中的月
		/// </summary>
		[Column("单位2", "UnitID2", "varchar", 32)]
		public string UnitID2 { get; set; }
		/// <summary>
		/// 是否周期性缴费，0不是，1是。为1时主要用于处理临时收费项目
		/// </summary>
		[Column("是否周期性缴费", "IsRegular", "int")]
		public int IsRegular { get; set; }
		/// <summary>
		/// 是否可按照协议收费，0不可以，1可以
		/// </summary>
		[Column("是否可按照协议收费", "IsAgreeMent", "int")]
		public int IsAgreeMent { get; set; }
		/// <summary>
		/// 是否可代理收费,0不可以，1可以
		/// </summary>
		[Column("是否可代理收费", "IsPloy", "int")]
		public int IsPloy { get; set; }
		/// <summary>
		/// 缴费单价
		/// </summary>
		[Column("缴费单价", "UnitPrice", "decimal", 18, 2)]
		public decimal UnitPrice { get; set; }
		/// <summary>
		/// 缴费项所属分类
		/// </summary>
		[Column("缴费项所属分类", "CategoryID", "varchar", 32)]
		public string CategoryID { get; set; }
		#endregion Model

	}
}

