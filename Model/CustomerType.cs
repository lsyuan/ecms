using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 客户类型表
	/// </summary>
	[Serializable]
	[Table("客户表", "T_CustomerType")]
	[Key("PK_T_CustomerType", "ID")]
	public partial class CustomerType : BaseResult
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public CustomerType()
		{ }
		#region Model
		/// <summary>
		/// ID
		/// </summary>
		[Column("主键", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 分类名称 
		/// </summary>
		[Column("分类名称", "Name", "varchar", 200)]
		public string Name { get; set; }
		#endregion Model

	}
}

