using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
	/// <summary>
	/// 日志类型，业务操作日志，初始化定义，包含登陆日志，授权日志等
	/// </summary>
	[Serializable]
	[Table("日志类型", "T_LOGTYPE")]
	[Key("PK_T_LOGTYPE", "ID")]
	public partial class LogType
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public LogType()
		{ }
		#region Model
		/// <summary>
		/// 日志类型
		/// </summary>
		[Column("日志类型", "ID", "varchar", 32)]
		public string ID { get; set; }
		/// <summary>
		/// 日志类型名称
		/// </summary>
		[Column("日志类型名称", "Name", "varchar", 200)]
		public string Name { get; set; }
		#endregion Model

	}
}

