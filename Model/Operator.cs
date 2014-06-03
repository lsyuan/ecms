using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 操作用户
	/// </summary>
	[Serializable]
	[Table("操作用户", "T_Operator")]
	[Key("PK_T_OPERATOR", "ID")]
	public partial class Operator 
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public Operator()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 职员ID
		/// </summary>
		[Column("EmployeeID", "EmployeeID", "varchar", 32)]
		public string EmployeeID { get; set; }
		/// <summary>
		/// 登录名
		/// </summary>
		[Column("Name", "Name", "varchar", 40)]
		public string Name { get; set; }
		/// <summary>
		/// 拼音
		/// </summary>
		[Column("PY", "PY", "varchar", 40)]
		public string PY { get; set; }
		/// <summary>
		/// 密码
		/// </summary>
		[Column("Pwd", "Pwd", "varchar", 40)]
		public string Pwd { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		[Column("CreateDate", "CreateDate", "DateTime")]
		public DateTime CreateDate { get; set; }
		/// <summary>
		/// 是否超级管理员
		/// </summary>
		[Column("IsAdmin", "IsAdmin", "int")]
		public int IsAdmin { get; set; }
		/// <summary>
		/// 账号分组
		/// </summary>
		[Column("GroupID", "GroupID", "varchar", 32)]
		public string GroupID { get; set; }
		/// <summary>
		/// 操作员状态
		/// </summary>
		[Column("Status", "Status", "int")]
		public int Status { get; set; }
		#endregion Model

	}
}

