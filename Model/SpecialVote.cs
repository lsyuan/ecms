using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 特殊权限表
	/// </summary>
	[Serializable]
	[Table("特殊权限表", "T_SpecialVote")]
	[Key("PK_T_SpecialVote", "ID")]
	public partial class SpecialVote
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public SpecialVote()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 特殊权限编码
		/// </summary>
		[Column("Code", "Code", "varchar", 200)]
		public string Code { get; set; }
		/// <summary>
		/// 特殊权限名称
		/// </summary>
		[Column("Name", "Name", "varchar", 200)]
		public string Name { get; set; }
		#endregion Model

	}
}

