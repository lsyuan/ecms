using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 职工表
	/// </summary>
	[Serializable]
	[Table("职工表", "T_Employee")]
	[Key("PK_T_EMPLOYEE", "ID")]
	public partial class Employee
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public Employee()
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
		[Column("Name", "Name", "varchar", 200)]
		public string Name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("PY", "PY", "varchar", 200)]
		public string PY { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("BirthDate", "BirthDate", "DateTime")]
		public DateTime? BirthDate { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Sex", "Sex", "varchar", 4)]
		public string Sex { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("DeptID", "DeptID", "varchar", 32)]
		public string DeptID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Job", "Job", "varchar", 50)]
		public string Job { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("ICCardID", "ICCardID", "varchar", 32)]
		public string ICCardID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Address", "Address", "varchar", 200)]
		public string Address { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("OfficePhone", "OfficePhone", "varchar", 30)]
		public string OfficePhone { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("WorkTime", "WorkTime", "DateTime")]
		public DateTime? WorkTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("FireTime", "FireTime", "DateTime")]
		public DateTime? FireTime { get; set; }
		/// <summary>
		/// 1在职（正常），2离职（已删除），3停职
		/// </summary>
		[Column("Status", "Status", "int")]
		public int Status { get; set; }
		#endregion Model

	}
}

