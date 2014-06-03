using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 登录日志
	/// </summary>
	[Serializable]
	[Table("登录日志", "T_LoginLog")]
	[Key("PK_T_LOGINLOG", "ID")]
	public partial class LoginLog
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public LoginLog()
		{ }
		#region Model
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 操作用户
		/// </summary>
		[Column("操作用户", "OperatorID", "varchar", 32)]
		public string OperatorID { get; set; }
		/// <summary>
		/// 日志记录时间
		/// </summary>
		[Column("日志记录时间", "CreateTime", "DateTime")]
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 登录类型，0登出，1登入
		/// </summary>
		[Column("登录类型", "Type", "int")]
		public int Type { get; set; }
		#endregion Model

	}
}

