using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Collections;

namespace Ajax.DBUtility
{
	/// <summary>
	/// CRUD代码生成类
	/// </summary>
	public class SQLGenerator
	{

		#region Insert
		/// <summary>
		/// Insert语句生成
		/// </summary>
		/// <typeparam name="T">Model泛型</typeparam>
		/// <typeparam name="P">sql参数类型，SqlParameter/OracleParameter</typeparam>
		/// <param name="model">实例</param>
		/// <param name="parameterList">参数数组</param>
		/// <returns>Insert语句</returns>
		public string InsertSqlGenerate<T, P>(T model, ref List<P> parameterList) where P : DbParameter, new()
		{
			string cacheSQL = SQLDataCache.GetHashTableData(model, CRUDEnum.INSERT);
			string result = string.Empty;
			if (!string.IsNullOrEmpty(cacheSQL))
			{
				result = cacheSQL;
			}
			else
			{
				result = "insert into {0}({2}) values({1})";
				result = string.Format(result, TableGenerator.Table<T>(), ParameterGenerator.ParameterString<T>(), TableGenerator.AllColumn<T>());
			}
			List<P> paramList = SQLDataCache.GetParameterCacheData<P>(model);
			if (paramList == null)
			{
				parameterList = ParameterGenerator.ParametersArray<T, P>(model);
				SQLDataCache.ModelParameterPush<DbParameter>(model, parameterList);
			}
			else
			{
				parameterList.AddRange(paramList.ToArray());
				ParameterGenerator.ModelValueToParamListPush(parameterList, model);
			}
			return result;
		}
		/// <summary>
		/// Insert语句生成
		/// </summary>
		/// <typeparam name="T">Model泛型</typeparam>
		/// <typeparam name="P">sql参数类型，SqlParameter/OracleParameter</typeparam>
		/// <param name="model">实例</param>
		/// <param name="parameterList">参数数组</param>
		/// <returns>Insert语句</returns>
		public string InsertSqlGenerate<T>(T model, ref List<DbParameter> parameterList)
		{
			string cacheSQL = SQLDataCache.GetHashTableData(model, CRUDEnum.INSERT);
			string result = string.Empty;
			if (!string.IsNullOrEmpty(cacheSQL))
			{
				result = cacheSQL;
			}
			else
			{
				result = "insert into {0}({2}) values({1})";
				result = string.Format(result, TableGenerator.Table<T>(), ParameterGenerator.ParameterString<T>(), TableGenerator.AllColumn<T>());
			}
			List<DbParameter> paramList = SQLDataCache.GetParameterCacheData<DbParameter>(model); ;
			if (paramList == null)
			{
				parameterList = ParameterGenerator.ParametersArray<T>(model);
				SQLDataCache.ModelParameterPush<DbParameter>(model, parameterList);
			}
			else
			{
				parameterList.AddRange(paramList.ToArray());
				ParameterGenerator.ModelValueToParamListPush(parameterList, model);
			}
			return result;
		}

		#endregion

		#region Delete

		/// <summary>
		/// 生成删除语句
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <typeparam name="P">sql参数，SqlParameter/OracleParameter</typeparam>
		/// <param name="model">实例</param>
		/// <param name="parameter">参数数组</param>
		/// <returns></returns>
		public string DeleteSqlGenerate<T, P>(T model, ref P parameter) where P : DbParameter, new()
		{
			string cacheSQL = SQLDataCache.GetHashTableData(model, CRUDEnum.DELETE);
			string result = string.Empty;
			if (!string.IsNullOrEmpty(cacheSQL))
			{
				result = cacheSQL;
			}
			else
			{
				result = "delete from {0} where {1} = {2}";
				result = string.Format(result, TableGenerator.Table<T>(), ParameterGenerator.ParameterString<T>(), TableGenerator.AllColumn<T>());
			}

			List<P> paramList = SQLDataCache.GetParameterCacheData<P>(model);
			parameter = paramList != null ? paramList.ToArray()[0] : ParameterGenerator.KeyParameterArray<T, P>(model);
			// 从缓存中取出后需对键列重新赋值
			parameter.Value = model.GetType().GetProperty(parameter.ParameterName).GetValue(model, null);
			return result;
		}
		/// <summary>
		/// 生成删除语句
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam> 
		/// <param name="model">实例</param>
		/// <param name="parameter">参数数组</param>
		/// <returns></returns>
		public string DeleteSqlGenerate<T>(T model, ref DbParameter parameter)
		{
			string cacheSQL = SQLDataCache.GetHashTableData(model, CRUDEnum.DELETE);
			string result = string.Empty;
			if (!string.IsNullOrEmpty(cacheSQL))
			{
				result = cacheSQL;
			}
			else
			{
				result = "delete from {0} where {1} = {2}";
				result = string.Format(result, TableGenerator.Table<T>(), ParameterGenerator.ParameterString<T>(), TableGenerator.AllColumn<T>());
			}

			List<DbParameter> paramList = SQLDataCache.GetParameterCacheData<DbParameter>(model);
			parameter = paramList != null ? paramList.ToArray()[0] : ParameterGenerator.KeyParameterArray<T>(model);
			// 从缓存中取出后需对键列重新赋值
			parameter.Value = model.GetType().GetProperty(parameter.ParameterName).GetValue(model, null);
			return result;
		}


		#endregion

		#region Update

		/// <summary>
		/// 生成更新语句
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <typeparam name="P">sql参数，SqlParameter/OracleParameter</typeparam>
		/// <param name="model">实例</param>
		/// <param name="parameterList">参数数组</param>
		/// <returns></returns>
		/// <remarks>
		/// eg:UPDATE USERS SET NAME = @NAME WHERE ID = @ID
		/// </remarks>
		public string UpdateSqlGenerate<T, P>(T model, ref List<P> parameterList) where P : DbParameter, new()
		{
			string cacheSQL = SQLDataCache.GetHashTableData(model, CRUDEnum.UDPATE);
			string result = string.Empty;
			if (!string.IsNullOrEmpty(cacheSQL))
			{
				result = cacheSQL;
			}
			else
			{
				result = "update {0} set {1} where {2} = {3}";
				result = string.Format(result, TableGenerator.Table<T>(), ParameterGenerator.ParameterStringWithoutKey<T>(), ParameterGenerator.KeyString<T>(), ParameterGenerator.KeyParameterString<T>());
				SQLDataCache.ModelCURDHandleSQLPush(model, CRUDEnum.UDPATE, result);
			}

			List<P> paramList = SQLDataCache.GetParameterCacheData<P>(model);
			if (paramList == null)
			{
				parameterList = ParameterGenerator.ParametersArray<T, P>(model);
				SQLDataCache.ModelParameterPush<DbParameter>(model, parameterList);
			}
			else
			{
				parameterList.AddRange(paramList.ToArray());
				ParameterGenerator.ModelValueToParamListPush(parameterList, model);
			}
			return result;
		}
		/// <summary>
		/// 生成更新语句
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam> 
		/// <param name="model">实例</param>
		/// <param name="parameterList">参数数组</param>
		/// <returns></returns>
		/// <remarks>
		/// eg:UPDATE USERS SET NAME = @NAME WHERE ID = @ID
		/// </remarks>
		public string UpdateSqlGenerate<T>(T model, ref List<DbParameter> parameterList)
		{
			string cacheSQL = SQLDataCache.GetHashTableData(model, CRUDEnum.UDPATE);
			string result = string.Empty;
			if (!string.IsNullOrEmpty(cacheSQL))
			{
				result = cacheSQL;
			}
			else
			{
				result = "update {0} set {1} where {2} = {3}";
				result = string.Format(result, TableGenerator.Table<T>(), ParameterGenerator.ParameterStringWithoutKey<T>(), ParameterGenerator.KeyString<T>(), ParameterGenerator.KeyParameterString<T>());
				SQLDataCache.ModelCURDHandleSQLPush(model, CRUDEnum.UDPATE, result);
			}

			List<DbParameter> paramList = SQLDataCache.GetParameterCacheData<DbParameter>(model);
			if (paramList == null)
			{
				parameterList = ParameterGenerator.ParametersArray<T>(model);
				SQLDataCache.ModelParameterPush<DbParameter>(model, parameterList);
			}
			else
			{
				parameterList.AddRange(paramList.ToArray());
				ParameterGenerator.ModelValueToParamListPush(parameterList, model);
			}
			return result;
		}

		#endregion

		#region Select
		/// <summary>
		/// 生成FindByKey查询语句
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <typeparam name="P">sql参数，SqlParameter/OracleParameter</typeparam>
		/// <param name="model">实例</param>
		/// <param name="parameterList">参数数组</param>
		/// <returns></returns>
		public string SelectBykeySqlGenerate<T, P>(T model, ref P parameter) where P : DbParameter, new()
		{
			string cacheSQL = SQLDataCache.GetHashTableData(model, CRUDEnum.GETBYID);
			string result = string.Empty;
			if (!string.IsNullOrEmpty(cacheSQL))
			{
				result = cacheSQL;
			}
			else
			{
				result = "select * from {0} where {1} = {2}";
				result = string.Format(result, TableGenerator.Table<T>(), ParameterGenerator.KeyString<T>(), ParameterGenerator.KeyParameterString<T>());
				SQLDataCache.ModelCURDHandleSQLPush(model, CRUDEnum.GETBYID, result);
			}

			List<P> paramList = SQLDataCache.GetParameterCacheData<P>(model);
			parameter = paramList != null ? paramList.ToArray()[0] : ParameterGenerator.KeyParameterArray<T, P>(model);
			// 从缓存中取出后需对键列重新赋值
			parameter.Value = model.GetType().GetProperty(parameter.ParameterName).GetValue(model, null);
			return result;
		}
		/// <summary> 
		/// 生成FindByKey查询语句 SELECT * FROM {0} WHERE {1} = {2}
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <typeparam name="P">sql参数，SqlParameter/OracleParameter</typeparam>
		/// <param name="model">实例</param>
		/// <param name="parameterList">参数数组</param>
		/// <returns>SELECT * FROM {0} WHERE {1} = {2}</returns>
		public string SelectBykeySqlGenerate<T>(T model, ref DbParameter parameter)
		{
			string cacheSQL = SQLDataCache.GetHashTableData(model, CRUDEnum.GETBYID);
			string result = string.Empty;
			if (!string.IsNullOrEmpty(cacheSQL))
			{
				result = cacheSQL;
			}
			else
			{
				result = "select * from {0} where {1} = {2}";
				result = string.Format(result, TableGenerator.Table<T>(), ParameterGenerator.KeyString<T>(), ParameterGenerator.KeyParameterString<T>());
				SQLDataCache.ModelCURDHandleSQLPush(model, CRUDEnum.GETBYID, result);
			}

			List<DbParameter> paramList = SQLDataCache.GetParameterCacheData<DbParameter>(model);
			parameter = paramList != null ? paramList.ToArray()[0] : ParameterGenerator.KeyParameterArray<T>(model);
			// 从缓存中取出后需对键列重新赋值
			parameter.Value = model.GetType().GetProperty(parameter.ParameterName).GetValue(model, null);
			//SQLDataCache.ModelParameterPush<DbParameter>(model, parameter);
			return result;
		}
		/// <summary>
		/// 生成查询全表语句 SELECT * FROM {0} WHERE 1 = 1 
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <returns>SELECT * FROM {0} WHERE 1 = 1 </returns>
		public string SelectSqlGenerate<T>() where T : new()
		{
			object model = new T();
			string cacheSQL = SQLDataCache.GetHashTableData(model, CRUDEnum.SELECT);
			string result = string.Empty;
			if (!string.IsNullOrEmpty(cacheSQL))
			{
				result = cacheSQL;
			}
			else
			{
				result = "select * from {0} where 1 = 1 ";
				result = string.Format(result, TableGenerator.Table<T>());
				SQLDataCache.ModelCURDHandleSQLPush(model, CRUDEnum.SELECT, result);
			}
			return result;
		}
		/// <summary>
		/// 生成查询全表记录数语句 SELECT COUNT(0) FROM TABLE WHERE 1=1
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <returns>SELECT COUNT(0) FROM TABLE1  WHERE 1=1</returns>
		public string CountSqlGenerate<T>() where T : new()
		{
			object model = new T();
			string cacheSQL = SQLDataCache.GetHashTableData(model, CRUDEnum.COUNT);
			string result = string.Empty;
			if (!string.IsNullOrEmpty(cacheSQL))
			{
				result = cacheSQL;
			}
			else
			{
				result = "select count(0) from {0}  where 1=1";
				result = string.Format(result, TableGenerator.Table<T>());
				SQLDataCache.ModelCURDHandleSQLPush(model, CRUDEnum.COUNT, result);
			}
			return result;
		}
		#endregion

	}
}
