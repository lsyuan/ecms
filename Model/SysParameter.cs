using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 系统参数表
	/// </summary>
	[Serializable]
	[Table("系统参数表", "T_SysParameter")]
	[Key("PK_T_SYSPARAMETER", "ID")]
	public partial class SysParameter
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public SysParameter()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 参数名称
		/// </summary>
		[Column("参数名称", "Name", "varchar", 200)]
		public string Name { get; set; }
		/// <summary>
		/// 参数值
		/// </summary>
		[Column("参数值", "Value", "varchar", 20)]
		public string Value { get; set; }
		/// <summary>
		/// 验证组
		/// </summary>
		//[Column("验证组", "ValidateGroup", "varchar", 200)]
		//public string ValidateGroup { get; set; }
		#endregion Model

	}
}

