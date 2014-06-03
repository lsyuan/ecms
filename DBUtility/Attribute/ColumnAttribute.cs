using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Ajax.DBUtility
{
	/// <summary>
	/// 列特性
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ColumnAttribute : BaseAttribute
	{

		private string typeName;
		private bool notNull;
		private int length;
		private int scale;
		private string readFormat;
		private string writeFormat;

		/// <summary>
		/// 数据类型
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}

		/// <summary>
		/// 长度
		/// </summary>
		public int Length
		{
			get { return length; }
			set { length = value; }
		}
		/// <summary>
		/// 精度
		/// </summary>
		public int Scale
		{
			get { return scale; }
			set { scale = value; }
		}

		/// <summary>
		/// 写入格式
		/// </summary>
		public string WriteFormat
		{
			get { return writeFormat; }
			set { writeFormat = value; }
		}

		/// <summary>
		/// 读取格式
		/// </summary>
		public string ReadFormat
		{
			get { return readFormat; }
			set { readFormat = value; }
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name">列名</param>
		/// <param name="code">列编码</param>
		/// <param name="typeName">该列的数据类型</param>
		public ColumnAttribute(string name, string code, string typeName)
		{
			base.Code = code;
			base.Name = name;
			this.TypeName = typeName;
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name">列名</param>
		/// <param name="code">列编码</param>
		/// <param name="typeName">该列的数据类型</param>
		/// <param name="length">字符串类型时最大长度</param>
		/// <param name="scale">精度</param>
		public ColumnAttribute(string name, string code, string typeName, int length)
		{
			base.Code = code;
			base.Name = name;
			this.TypeName = typeName;
			this.Length = length;
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name">列名</param>
		/// <param name="code">列编码</param>
		/// <param name="typeName">该列的数据类型</param>
		/// <param name="length">字符串类型时最大长度</param>
		/// <param name="scale">精度</param>
		public ColumnAttribute(string name, string code, string typeName, int length, int scale)
		{
			base.Code = code;
			base.Name = name;
			this.TypeName = typeName;
			this.Length = length;
			this.Scale = scale;
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name">列名</param>
		/// <param name="code">列编码</param>
		/// <param name="typeName">该列的数据类型</param>
		/// <param name="length">字符串类型时最大长度</param>
		/// <param name="readFormat">读取格式</param>
		/// <param name="wirteFormat">写入格式</param>
		public ColumnAttribute(string name, string code, string typeName, string readFormat, string wirteFormat)
		{
			base.Code = code;
			base.Name = name;
			this.TypeName = typeName;
			this.Length = length;
			this.ReadFormat = readFormat;
			this.WriteFormat = writeFormat;
		}
	}
}
