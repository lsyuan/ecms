using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 区域信息表
	/// </summary>
	[Serializable]
	[Table("区域信息表", "T_Area")]
	[Key("PK_T_AREA", "ID")]
	public partial class Area : BaseResult
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public Area()
		{ }
		#region Model

		/// <summary>
		/// ID
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 父级区域
		/// </summary>
		[Column("父级区域", "PID", "varchar", 32)]
		public string PID { get; set; }
		/// <summary>
		/// 区域编号，001,001001,001001001
		/// </summary>
		[Column("区域编号", "Code", "varchar", 40)]
		public string Code { get; set; }
		/// <summary>
		/// 区域名称
		/// </summary>
		[Column("区域名称", "Name", "varchar", 40)]
		public string Name { get; set; }
		/// <summary>
		/// 区域负责人
		/// </summary>
		[Column("区域负责人", "Manager", "varchar", 36)]
		public string Manager { get; set; }

		#endregion Model

	}
}

