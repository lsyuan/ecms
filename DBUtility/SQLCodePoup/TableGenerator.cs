using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Ajax.DBUtility
{
	public class TableGenerator
	{
		/// <summary>
		/// 获取表名
		/// </summary>
		/// <typeparam name="T">泛型T</typeparam>
		/// <returns>表名</returns>
		public static string Table<T>()
		{
			TableAttribute tableAttribute = (TableAttribute)typeof(T).GetCustomAttributes(typeof(TableAttribute), false)[0];
			return tableAttribute.Code;
		}

		/// <summary>
		/// 获取一个表的所有列明集合
		/// </summary>
		/// <typeparam name="T">泛型T</typeparam>
		/// <returns></returns>
		/// <remarks>
		/// eg:ID,NAME
		/// </remarks>
		public static string AllColumn<T>()
		{
			StringBuilder columnAll = new StringBuilder();
			foreach (PropertyInfo item in typeof(T).GetProperties())
			{
				if (!item.Name.Equals("State") && !item.Name.Equals("Errormsg"))
				{
					columnAll.AppendFormat("{0},", item.Name);
				}
			}
			return columnAll.Remove(columnAll.Length - 1, 1).ToString();
		}
	}
}
