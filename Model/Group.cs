using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 角色表
	/// </summary>
	[Serializable]
	[Table("角色表", "T_GROUP")]
	[Key("PK_T_GROUP", "ID")]
	public partial class Group
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public Group()
		{ }
		#region Model
		/// <summary>
		/// ID
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 角色名称
		/// </summary>
		[Column("角色名称", "Name", "varchar", 40)]
		public string NAME { get; set; }
		#endregion Model

	}
}

