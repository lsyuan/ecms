using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data.Common;
using System.Data;

namespace Ajax.DBUtility
{
	/// <summary>
	/// 参数生成类
	/// </summary>
	public class ParameterGenerator
	{
		/// <summary>
		/// 生成参数字符串
		/// </summary>
		/// <typeparam name="T">泛型T</typeparam>
		/// <param name="model">实例</param>
		/// <returns>参数字符串</returns>
		public static string ParameterString<T>()
		{
			StringBuilder temp = new StringBuilder();

			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
			{
				if (propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false).Length == 0)
				{
					continue;
				}
				ColumnAttribute column = (ColumnAttribute)propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false)[0];
				temp.AppendFormat("@{0},", column.Code);
			}
			temp.Remove(temp.Length - 1, 1);

			return temp.ToString();
		}
		/// <summary>
		/// 生成参数字符串,不包含主键列
		/// </summary>
		/// <typeparam name="T">泛型T</typeparam>
		/// <param name="model">实例</param>
		/// <returns>参数字符串</returns>
		public static string ParameterStringWithoutKey<T>()
		{
			StringBuilder temp = new StringBuilder();
			string key = KeyString<T>().ToUpper();
			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
			{
				if (propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false).Length == 0)
				{
					continue;
				}
				// 排除主键列
				if (propertyInfo.Name.ToUpper() == key)
				{
					continue;
				}
				ColumnAttribute column = (ColumnAttribute)propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false)[0];
				temp.AppendFormat("{0}=@{0},", column.Code);
			}
			temp.Remove(temp.Length - 1, 1);

			return temp.ToString();
		}

		/// <summary>
		/// 根据模型实例获取参数数组
		/// </summary>
		/// <typeparam name="T">泛型T Model </typeparam>
		/// <typeparam name="P">参数类型 SqlParameter OracleParameter...</typeparam>
		/// <param name="model">实例</param>
		/// <returns></returns>
		public static List<P> ParametersArray<T, P>(T model) where P : DbParameter, new()
		{
			List<P> paramList = new List<P>();
			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
			{
				if (propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false).Length == 0)
				{
					continue;
				}
				ColumnAttribute column = (ColumnAttribute)propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false)[0];
				P temp = new P();
				temp.ParameterName = column.Code;
				if (column.Length != 0)
				{
					temp.Size = column.Length;
				}
				temp.DbType = GetDbTypeByName(column.TypeName);
				object obj = propertyInfo.GetValue(model, null);
				temp.Value = obj == null ? DBNull.Value : obj;
				if (obj == null)
				{
					temp.Value = DBNull.Value;
				}
				paramList.Add(temp);
			}
			return paramList;
		}

		/// <summary>
		/// 根据模型实例获取参数数组
		/// </summary>
		/// <typeparam name="T">泛型T Model </typeparam> 
		/// <param name="model">实例</param>
		/// <returns></returns>
		public static List<DbParameter> ParametersArray<T>(T model)
		{
			List<DbParameter> paramList = new List<DbParameter>();
			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
			{
				if (propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false).Length == 0)
				{
					continue;
				}
				ColumnAttribute column = (ColumnAttribute)propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false)[0];
				DbParameter temp = DBHelper.Create().CreateParameter();
				temp.ParameterName = column.Code;
				if (column.Length != 0)
				{
					temp.Size = column.Length;
				}
				temp.DbType = GetDbTypeByName(column.TypeName);
				object obj = propertyInfo.GetValue(model, null);
				temp.Value = obj == null ? DBNull.Value : obj;
				if (obj == null)
				{
					temp.Value = DBNull.Value;
				}
				paramList.Add(temp);
			}
			return paramList;
		}
		/// <summary>
		/// 根据模型实例获取参数数组,不包含Key column
		/// </summary>
		/// <typeparam name="T">泛型T Model </typeparam>
		/// <typeparam name="P">参数类型 SqlParameter OracleParameter...</typeparam>
		/// <param name="model">实例</param>
		/// <returns></returns>
		public static List<P> ParametersArrayWithoutKey<T, P>(T model) where P : DbParameter, new()
		{
			List<P> paramList = new List<P>();
			string key = KeyString<T>().ToUpper();
			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
			{
				// 去掉主键列parameter
				if (propertyInfo.Name.ToUpper() == key)
				{
					continue;
				}
				if (propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false).Length == 0)
				{
					continue;
				}
				ColumnAttribute column = (ColumnAttribute)propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false)[0];
				P temp = new P();
				temp.ParameterName = column.Code;
				temp.DbType = GetDbTypeByName(column.TypeName);
				temp.Value = propertyInfo.GetValue(model, null);
				paramList.Add(temp);
			}
			return paramList;
		}
		/// <summary>
		/// 根据模型实例获取参数数组,不包含Key column
		/// </summary>
		/// <typeparam name="T">泛型T Model </typeparam> 
		/// <param name="model">实例</param>
		/// <returns></returns>
		public static List<DbParameter> ParametersArrayWithoutKey<T>(T model)
		{
			List<DbParameter> paramList = new List<DbParameter>();
			string key = KeyString<T>().ToUpper();
			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
			{
				// 去掉主键列parameter
				if (propertyInfo.Name.ToUpper() == key)
				{
					continue;
				}
				if (propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false).Length == 0)
				{
					continue;
				}
				ColumnAttribute column = (ColumnAttribute)propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false)[0];
				DbParameter temp = DBHelper.Create().CreateParameter();
				temp.ParameterName = column.Code;
				temp.DbType = GetDbTypeByName(column.TypeName);
				temp.Value = propertyInfo.GetValue(model, null);
				paramList.Add(temp);
			}
			return paramList;
		}

		/// <summary>
		/// 获取模型的主键列 ID
		/// </summary>
		/// <typeparam name="T">Model</typeparam>
		/// <returns></returns>
		public static string KeyString<T>()
		{
			if (typeof(T).GetCustomAttributes(typeof(KeyAttribute), false).Length == 0)
			{
				throw new Exception("表未设置主键");
			}
			KeyAttribute tableAttribute = (KeyAttribute)typeof(T).GetCustomAttributes(typeof(KeyAttribute), false)[0];
			return tableAttribute.Code;
		}


		/// <summary>
		/// 获取模型的主键列参数变量 @ID
		/// </summary>
		/// <typeparam name="T">Model</typeparam>
		/// <returns>主键列参数变量 @ID</returns>
		public static string KeyParameterString<T>()
		{
			return "@" + KeyString<T>();
		}

		/// <summary>
		/// 获取主键参数
		/// </summary>
		/// <typeparam name="T">模型</typeparam>
		/// <typeparam name="P">SQL参数泛型</typeparam>
		/// <param name="model">实例</param>
		/// <returns>主键参数</returns>
		public static P KeyParameterArray<T, P>(T model) where P : DbParameter, new()
		{
			P keyParameter = new P();
			string key = KeyString<T>();
			keyParameter.ParameterName = key;
			PropertyInfo property = typeof(T).GetProperty(key);
			ColumnAttribute columnAttribute = (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false)[0];
			keyParameter.DbType = GetDbTypeByName(columnAttribute.TypeName);
			keyParameter.Size = columnAttribute.Length;
			keyParameter.Value = property.GetValue(model, null);
			return keyParameter;
		}

		/// <summary>
		/// 获取主键参数
		/// </summary>
		/// <typeparam name="T">模型</typeparam>
		/// <typeparam name="P">SQL参数泛型</typeparam>
		/// <param name="model">实例</param>
		/// <returns>主键参数</returns>
		public static DbParameter KeyParameterArray<T>(T model)
		{
			DbParameter keyParameter = DBHelper.Create().CreateParameter();
			string key = KeyString<T>();
			keyParameter.ParameterName = key;
			PropertyInfo property = typeof(T).GetProperty(key);
			ColumnAttribute columnAttribute = (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false)[0];
			keyParameter.DbType = GetDbTypeByName(columnAttribute.TypeName);
			keyParameter.Size = columnAttribute.Length;
			keyParameter.Value = property.GetValue(model, null);
			return keyParameter;
		}

		/// <summary>
		/// 根据列的TypeName特性值获取数据库类型
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static DbType GetDbTypeByName(string dbType)
		{
			switch (dbType.ToLower())
			{
				case "varchar":
					return DbType.String;
				case "int":
					return DbType.Int32;
				case "datetime":
					return DbType.DateTime;
				case "smalldatetime":
					return DbType.Date;
				case "char":
					return DbType.String;
				case "bit":
					return DbType.Byte;
				case "decimal":
					return DbType.Decimal;
				default:
					return DbType.String;
			}
		}
		/// <summary>
		/// 将模型数据推送到指定的参数集合中
		/// </summary>
		/// <param name="parameterList">参数集合</param>
		/// <param name="model">模型实例</param>
		/// <remarks>根据模型属性名进行参数赋值</remarks>
		public static void ModelValueToParamListPush(List<DbParameter> parameterList, object model)
		{
			foreach (DbParameter item in parameterList)
			{
				string parameterName = item.ParameterName;
				object value = model.GetType().GetProperty(parameterName).GetValue(model, null);
				item.Value = value ?? DBNull.Value;
			}
		}
		/// <summary>
		/// 将模型数据推送到指定的参数集合中
		/// </summary>
		/// <param name="parameterList">参数集合</param>
		/// <param name="model">模型实例</param>
		/// <remarks>根据模型属性名进行参数赋值</remarks>
		public static void ModelValueToParamListPush<T>(List<T> parameterList, object model) where T : DbParameter, new()
		{
			foreach (T item in parameterList)
			{
				string parameterName = item.ParameterName;
				item.Value = model.GetType().GetProperty(parameterName).GetValue(model, null);
			}
		}
	}
}
