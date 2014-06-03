using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 缴费项单位
	/// </summary>
	[Serializable]
	[Table("缴费项单位", "T_Unit")]
	[Key("PK_T_UNIT", "ID")]
	public partial class Unit : BaseResult
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public Unit()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar",32)]
		public string ID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Name", "Name", "varchar",400)]
		public string Name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("PY", "PY", "varchar", 400)]
		public string PY { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Level", "Level", "int")]
		public int Level { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("TimeValue", "TimeValue", "int")]
		public int TimeValue { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Status", "Status", "int")]
		public int Status { get; set; }
		#endregion Model

	}
}

