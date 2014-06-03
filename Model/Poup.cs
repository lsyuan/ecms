using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 菜单节点表
	/// </summary>
	[Serializable]
	[Table("菜单节点表", "T_Poup")]
	[Key("PK_T_POUP", "ID")]
	public partial class Poup
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public Poup()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 菜单编码
		/// </summary>
		[Column("菜单编码", "Value", "varchar", 40)]
		public string Value { get; set; }
		/// <summary>
		/// 菜单名称
		/// </summary>
		[Column("菜单名称", "Name", "varchar", 100)]
		public string Name { get; set; }
		/// <summary>
		/// 路径
		/// </summary>
		[Column("路径", "Path", "string", 100)]
		public string Path { get; set; }
		/// <summary>
		/// 是否启用
		/// </summary>
		[Column("是否启用", "IsValid", "int")]
		public int IsValid { get; set; }
		/// <summary>
		/// 父级菜单
		/// </summary>
		[Column("父级菜单", "PID", "varchar", 32)]
		public string PID { get; set; }
		/// <summary>
		/// 父级编码
		/// </summary>
		[Column("父级编码", "PValue", "varchar", 40)]
		public string PValue { get; set; }
		#endregion Model

	}
}

