using System;
using Ajax.DBUtility;
namespace Ajax.Model
{

	/// <summary>
	/// 树节点
	/// </summary>
	[Serializable]
	[Table("系统参数表", "T_SysParameter")]
	[Key("PK_T_SYSPARAMETER", "ID")]
	public class SysNode
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public SysNode()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		[Column("ID", "ID", "varchar")]
		public int NodeID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int ParentID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Location { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int OrderID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Comment { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int PermissionID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ImageUrl { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int ModuleID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int KeShiDM { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string KeshiPublic { get; set; }

	}
}
