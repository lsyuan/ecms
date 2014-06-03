using System;
using System.Collections.Generic;
using System.Text;

namespace Ajax.DBUtility
{
	/// <summary>
	/// 键值特性
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class KeyAttribute : BaseAttribute
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name">键名称</param>
		/// <param name="code">键编码</param>
		public KeyAttribute(string name, string code)
		{
			base.Name = name;
			base.Code = code;
		}

		public KeyAttribute()
		{ }

	}
}
