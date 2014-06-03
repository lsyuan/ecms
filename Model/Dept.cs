using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 部门表
	/// </summary>
	[Serializable]
	[Table("部门表", "T_Dept")]
	[Key("PK_T_DEPT", "ID")]
	public partial class Dept
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public Dept()
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
		[Column("PID", "PID", "varchar", 32)]
		public string PID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Code", "Code", "varchar", 40)]
		public string Code { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Name", "Name", "varchar", 200)]
		public string Name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("PY", "PY", "varchar", 200)]
		public string PY { get; set; }
		/// <summary>
		/// 部门状态：0删除，1正常
		/// </summary>
		[Column("Status", "Status", "int")]
		public int Status { get; set; }
		#endregion Model

	}
}

