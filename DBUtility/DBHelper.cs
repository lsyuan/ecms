using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Ajax.Common;
using System.Globalization;
namespace Ajax.DBUtility
{
	/// <summary>
	/// 数据库访问Helper
	/// </summary>
	public class DBHelper : IDisposable
	{
		#region 变量&实例函数

		/// <summary>
		/// 连接字符串
		/// </summary>
		private readonly string CONNECTIONSTRING = "";

		/// <summary>
		/// 数据库驱动名称
		/// </summary>
		private readonly string PROVIDERNAME = "";

		/// <summary>
		/// 控制命令
		/// </summary>
		private DbCommand MYCOMMAND;
		/// <summary>
		/// 数据库连接
		/// </summary>
		private readonly DbConnection MYCONNECTION;
		/// <summary>
		/// 事务
		/// </summary>
		private DbTransaction TRANSACTION;
		/// <summary>
		/// 当前是否有事务
		/// </summary>
		private bool InTransaction { get; set; }
		/// <summary>
		/// 存储过程参数缓存导HashTable中
		/// </summary> 
		private static Hashtable PARAMCACHE = Hashtable.Synchronized(new Hashtable());
		/// <summary>
		/// 获取记录总数
		/// </summary>
		public static string StrGetCountSql = "select count(ID) as item from({0}) as tab";
		/// <summary>
		/// 创建DBHelper实例
		/// </summary>
		/// <returns>DBHelper实例</returns>
		public static DBHelper Create()
		{
			return new DBHelper();
		}

		/// <summary>
		/// 数据库驱动工厂类
		/// </summary>
		private static DbProviderFactory MYPROVIDERFACTORY;

		/// <summary>
		/// 访问外部资源的句柄
		/// </summary>
		private IntPtr _handle;
		/// <summary>
		/// 标记Dispose是否被调用
		/// </summary>
		private bool disposed = false;
		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数,读取连接字符串
		/// </summary>
		private DBHelper()
		{
			// 连接字符串
			string conStr = ConfigHelper.GetConfigString("ConnectionString");

			bool encrypted = false;
			bool.TryParse(ConfigHelper.GetConfigString("CONSTRENCRYPTED"), out encrypted);
			if (encrypted)
			{
				CONNECTIONSTRING = conStr;// DESEncrypt.Decrypt(conStr);     // 获取解密字符串
			}
			else
			{
				CONNECTIONSTRING = conStr;
			}

			PROVIDERNAME = ConfigHelper.GetConfigString("ProviderName");

			MYPROVIDERFACTORY = DbProviderFactories.GetFactory(PROVIDERNAME);

			// 获取一个已打开的数据库连接
			MYCONNECTION = GetCurrentConnection();

		}

		#endregion

		#region Transaction
		/// <summary>
		/// 创建数据库事务
		/// </summary>
		public void BeginTransaction()
		{
			if (MYCONNECTION != null && MYCONNECTION.State == ConnectionState.Open)
			{
				TRANSACTION = MYCONNECTION.BeginTransaction();
				InTransaction = true;
			}
		}

		/// <summary>
		/// 提交当前事务
		/// </summary>
		public void Commit()
		{
			if (TRANSACTION != null)
			{
				TRANSACTION.Commit();
				InTransaction = false;
				MYCONNECTION.Close();
			}
		}
		/// <summary>
		/// 数据库回滚
		/// </summary>
		public void RollBack()
		{
			if (TRANSACTION != null)
			{
				TRANSACTION.Rollback();
				InTransaction = false;
				MYCONNECTION.Dispose();
				MYCONNECTION.Close();
			}
		}
		/// <summary>
		/// 数据访问对象
		/// </summary>
		public DBHelper DB
		{
			get
			{
				return DBHelper.Create();
			}
		}
		#endregion

		#region Insert
		/// <summary>
		/// 通过存储过程插入数据
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <param name="model">实例</param>
		/// <returns></returns>
		public T InsertByProc<T>(T model)
		{
			Type t = typeof(T);
			TableAttribute myAttr = (TableAttribute)t.GetCustomAttributes(typeof(TableAttribute), false)[0];
			string procedureName = myAttr.Code + "_ADD";
			DbCommand cmd = CreateCommand(procedureName);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddRange(PreparInsertSqlParameter(t, model));
			try
			{
				cmd.Connection = MYCONNECTION;
				if (TRANSACTION != null)
				{
					cmd.Transaction = TRANSACTION;
				}
				cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return model;
		}

		/// <summary>
		/// 通过语句插入数据
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <param name="model">实例</param>
		/// <returns></returns>
		public T Insert<T>(T model)
		{
			List<DbParameter> paramList = new List<DbParameter>();
			string sql = new SQLGenerator().InsertSqlGenerate(model, ref paramList);
			DbCommand cmd = CreateCommand(sql);
			cmd.Parameters.AddRange(paramList.ToArray());
            try
            {
                cmd.Connection = MYCONNECTION;
                if (TRANSACTION != null)
                {
                    cmd.Transaction = TRANSACTION;
                }
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { cmd.Dispose(); }
			return model;
		}

		/// <summary>
		/// 数据库批量插入数据
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <param name="models">Model集合</param>
		public void InsertBatch<T>(T[] models)
		{
			DbCommand cmd;
			foreach (T model in models)
			{
				List<DbParameter> paramList = new List<DbParameter>();
				string sql = new SQLGenerator().InsertSqlGenerate(model, ref paramList);
				cmd = CreateCommand(sql);
				cmd.Parameters.Clear();
				cmd.Parameters.AddRange(paramList.ToArray());
				try
				{
					cmd.Transaction = TRANSACTION;
					cmd.Connection = MYCONNECTION;
					cmd.ExecuteNonQuery();
					cmd.Parameters.Clear();
					cmd.Dispose();
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
		}

		#endregion

		#region Delete
		/// <summary>
		/// 通过主键值删除实体对象
		/// </summary>
		/// <param name="target">目标模型</param>
		/// <returns></returns>
		public bool DeleteByProc<T>(T target)
		{
			Type t = typeof(T);
			TableAttribute myAttr = (TableAttribute)t.GetCustomAttributes(typeof(TableAttribute), false)[0];
			string procedureName = myAttr.Name + "_Delete";

			DbCommand cmd = CreateCommand(procedureName);
			cmd.CommandType = CommandType.StoredProcedure;
			try
			{
				cmd.Connection = MYCONNECTION;
				if (TRANSACTION != null)
				{
					cmd.Transaction = TRANSACTION;
				}
				cmd.Parameters.AddRange(PreparDeleteSqlParameter(t, target));
				int result = cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();
				return result > 0;
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// 通过主键值删除实体对象(必须对模型的ID进行赋值)
		/// </summary>
		/// <param name="model">目标模型</param>
		/// <returns></returns>
		public bool Delete<T>(T model)
		{
			string sql = string.Empty;
			SQLGenerator generator = new SQLGenerator();
			var param = CreateParameter();
			sql = generator.DeleteSqlGenerate<T>(model, ref param);
			DbCommand cmd = CreateCommand(sql);
			try
			{
				cmd.Connection = MYCONNECTION;
				if (TRANSACTION != null)
				{
					cmd.Transaction = TRANSACTION;
				}
				cmd.Parameters.Add(param);
				int result = cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();
				return result > 0;
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// 通过ID删除一条记录
		/// </summary>
		/// <typeparam name="T">Model泛型</typeparam>
		/// <param name="value">主键值</param>
		/// <returns></returns>
		public bool DeleteByID<T>(object value)
		{
			Type t = typeof(T);
			string sql = "delete from {0} where ID = @ID";
			sql = string.Format(sql, TableGenerator.Table<T>());
			DbCommand cmd = CreateCommand(sql);
			try
			{
				cmd.Connection = MYCONNECTION;
				if (TRANSACTION != null)
				{
					cmd.Transaction = TRANSACTION;
				}
				cmd.Parameters.Add(PreparPrimaryKey(t, value));
				int result = cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();
				return result > 0;
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// 批量删除数据
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <param name="models">数据集合</param>
		public void DeleteBatch<T>(T[] models)
		{
			string sql = string.Empty;
			SQLGenerator generator = new SQLGenerator();
			DbCommand cmd;
			foreach (T model in models)
			{
				DbParameter param = CreateParameter();
				sql = generator.DeleteSqlGenerate<T>(model, ref param);
				cmd = CreateCommand(sql);
				cmd.Connection = MYCONNECTION;
				try
				{
					cmd.Parameters.Add(param);
					if (TRANSACTION != null)
					{
						cmd.Transaction = TRANSACTION;
					}
					cmd.ExecuteNonQuery();
					cmd.Parameters.Clear();
				}
				catch (System.Exception ex)
				{
					throw ex;
				}
			}
		}

		#endregion

		#region Update
		/// <summary>
		/// 更新一条记录
		/// </summary>
		/// <param name="target">目标模型</param>
		/// <returns></returns>
		public T UpdateByProc<T>(T target)
		{
			Type t = typeof(T);
			TableAttribute myAttr = (TableAttribute)t.GetCustomAttributes(typeof(TableAttribute), false)[0];
			string procedureName = myAttr.Name + "_Update";
			DbCommand cmd = CreateCommand(procedureName);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddRange(PreparInsertSqlParameter(t, target));
			try
			{
				cmd.Connection = MYCONNECTION;
				cmd.Transaction = TRANSACTION;
				cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return target;
		}

		/// <summary>
		/// 更新一条记录
		/// </summary>
		/// <param name="model">目标模型</param>
		/// <returns></returns>
		public T Update<T>(T model)
		{
			SQLGenerator generator = new SQLGenerator();
			List<DbParameter> list = new List<DbParameter>();
			string sql = generator.UpdateSqlGenerate<T>(model, ref list);
			DbCommand cmd = CreateCommand(sql);
			cmd.Parameters.Clear();
			cmd.Parameters.AddRange(list.ToArray());
			try
			{
				cmd.Connection = MYCONNECTION;
				cmd.Transaction = TRANSACTION;
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				cmd.Parameters.Clear();
			}
			return model;
		}

		/// <summary>
		/// 批量更新
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <param name="models">数据集合</param>
		public void UpdateBatch<T>(T[] models)
		{
			SQLGenerator generator = new SQLGenerator();
			List<DbParameter> list = new List<DbParameter>();
			DbCommand cmd;
			if (TRANSACTION == null)
			{
				BeginTransaction();
			}
			foreach (T model in models)
			{
				list.Clear();
				string sql = generator.UpdateSqlGenerate<T>(model, ref list);
				cmd = CreateCommand(sql);
				cmd.Parameters.Clear();
				cmd.Parameters.AddRange(list.ToArray());
				try
				{
					cmd.Connection = MYCONNECTION;
					cmd.Transaction = TRANSACTION;
					cmd.ExecuteNonQuery();
					cmd.Parameters.Clear();
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					cmd.Parameters.Clear();
				}
			}
		}

		#endregion

		#region Query

		/// <summary>
		/// 通过主键查找一条记录
		/// </summary>
		/// <param name="value">主键值</param>
		/// <returns></returns>
		public T GetByIdProc<T>(object value)
		{
			T result = System.Activator.CreateInstance<T>();		// instance object
			Type t = typeof(T);
			TableAttribute myAttr = (TableAttribute)t.GetCustomAttributes(false)[0];
			string procedureName = myAttr.Code + "_GetModel";
			DbCommand cmd = CreateCommand(procedureName);
			try
			{
				cmd.Connection = MYCONNECTION;
				cmd.Parameters.Add(PreparPrimaryKey(t, value));
				using (DbDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						System.Reflection.PropertyInfo[] infos = t.GetProperties();
						foreach (PropertyInfo info in infos)
						{
							info.SetValue(result, reader[info.Name], null);
						}
						return result;
					}
				}
			}
			catch (System.Exception ex)
			{
				throw ex;
			}

			finally
			{
				cmd.Parameters.Clear();
			}
			return result;
		}


		/// <summary>
		/// 查询数据集合，限强类型Model
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <param name="whereStr">where子句，ID=@ID</param>
		/// <param name="param">参数键值对</param>
		/// <returns></returns>
		public T GetModel<T>(string whereStr, Dictionary<string, object> param) where T : new()
		{
			SQLGenerator generator = new SQLGenerator();
			StringBuilder temp = new StringBuilder();
			temp.Append(generator.SelectSqlGenerate<T>());	// select * from Table where 1=1 
			if (whereStr.Length > 0)
			{
				temp.Append(whereStr);
			}
			DbCommand cmd = CreateCommand();

			cmd.CommandText = temp.ToString();

			// 参数处理
			cmd.Parameters.AddRange(PreparParameter<T>(param));
			T model = new T();
			// 遍历Reader
			using (DbDataReader reader = cmd.ExecuteReader())
			{
				if (reader != null && reader.Read())
				{
					PropertyInfo[] properties = typeof(T).GetProperties();
					// 循环属性赋值
					foreach (PropertyInfo item in properties)
					{
						SetPropertyValue<T>(model, item, reader[item.Name]);
					}
				}
				else
				{
					return default(T);
				}
				cmd.Parameters.Clear();
			}
			return model;
		}

		/// <summary>
		/// 通过主键查找一条记录
		/// </summary>
		/// <param name="value">主键值</param>
		/// <returns></returns>
		public T GetById<T>(object value)
		{
			T result = System.Activator.CreateInstance<T>();
			SQLGenerator generator = new SQLGenerator();
			DbParameter param = CreateParameter();
			string sql = generator.SelectBykeySqlGenerate<T>(result, ref param);
			DbCommand cmd = CreateCommand();
			param.Value = value;
			cmd.CommandText = sql;
			cmd.Parameters.Clear();
			cmd.Parameters.Add(param);
			try
			{
				cmd.Connection = MYCONNECTION;
				cmd.Transaction = TRANSACTION;
				using (DbDataReader reader = cmd.ExecuteReader())
				{
					if (reader != null && reader.Read())
					{
						PropertyInfo[] infos = result.GetType().GetProperties();
						foreach (PropertyInfo info in infos)
						{
							info.SetValue(result, reader[info.Name] == DBNull.Value ? null : reader[info.Name], null);
						}
					}
				}
			}
			catch (System.Exception ex)
			{
				return result;
			}
			finally
			{
				cmd.Parameters.Clear();
			}
			return result;
		}
		/// <summary>
		/// 查询数据集合，限强类型Model
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <param name="whereStr">where子句，ID=@ID</param>
		/// <param name="groupBy">groupby 子句</param>
		/// <param name="orderBy">排序</param>
		/// <param name="param">参数键值对</param>
		/// <returns></returns>
		public List<T> GetList<T>(string whereStr) where T : new()
		{
			List<T> list = new List<T>();
			SQLGenerator generator = new SQLGenerator();
			StringBuilder temp = new StringBuilder();
			temp.Append(generator.SelectSqlGenerate<T>());	// select * from Table where 1=1 
			if (whereStr.Length > 0)
			{
				temp.Append(whereStr);
			}
			DbCommand cmd = CreateCommand();

			cmd.CommandText = temp.ToString();

			cmd.Connection = MYCONNECTION;
			if (TRANSACTION != null)
			{
				cmd.Transaction = TRANSACTION;
			}
			// 遍历Reader
			using (DbDataReader reader = cmd.ExecuteReader())
			{
				while (reader != null && reader.Read())
				{
					T current = new T();
					PropertyInfo[] properties = typeof(T).GetProperties();
					// 循环属性赋值
					foreach (PropertyInfo item in properties)
					{
						if (item.GetCustomAttributes(typeof(ColumnAttribute), false).Length > 0)
						{
							if (reader[item.Name] == DBNull.Value)
							{
								if (item.PropertyType == typeof(string))
								{
									item.SetValue(current, "", null);
								}
								else if (item.PropertyType == typeof(int))
								{
									item.SetValue(current, 0, null);
								}
								else if (item.PropertyType == typeof(DateTime))
								{
									item.SetValue(current, DateTime.MinValue, null);
								}
								else if (item.PropertyType == typeof(decimal))
								{
									item.SetValue(current, 0m, null);
								}
								continue;
							}
							string value = reader[item.Name].ToString();
							item.SetValue(current, reader[item.Name], null);
						}
					}
					// 添加到集合
					list.Add(current);
				}
			}
			return list;
		}
		/// <summary>
		/// 查询数据集合，限强类型Model
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <param name="whereStr">where子句，ID=@ID</param>
		/// <param name="groupBy">groupby 子句</param>
		/// <param name="orderBy">排序</param>
		/// <param name="param">参数键值对</param>
		/// <returns></returns>
		public List<T> GetList<T>(string whereStr, Dictionary<string, object> param, string orderBy, string groupBy) where T : new()
		{
			List<T> list = new List<T>();
			SQLGenerator generator = new SQLGenerator();
			StringBuilder temp = new StringBuilder();
			temp.Append(generator.SelectSqlGenerate<T>());	// select * from Table where 1=1 
			if (whereStr.Length > 0)
			{
				temp.Append(whereStr);
			}
			if (!string.IsNullOrEmpty(groupBy))
			{
				temp.Append(" group by ").Append(groupBy);
			}
			if (!string.IsNullOrEmpty(orderBy))
			{
				temp.Append(" order by ").Append(orderBy);
			}
			DbCommand cmd = CreateCommand();

			cmd.CommandText = temp.ToString();

			// 参数处理
			if (param != null && param.Count > 0)
			{
				cmd.Parameters.AddRange(PreparParameter<T>(param));
			}
			cmd.Connection = MYCONNECTION;
			if (TRANSACTION != null)
			{
				cmd.Transaction = TRANSACTION;
			}
			// 遍历Reader
			using (DbDataReader reader = cmd.ExecuteReader())
			{
				while (reader != null && reader.Read())
				{
					T current = new T();
					PropertyInfo[] properties = typeof(T).GetProperties();
					// 循环属性赋值
					foreach (PropertyInfo item in properties)
					{
						if (item.GetCustomAttributes(typeof(ColumnAttribute), false).Length > 0)
						{
							if (reader[item.Name] == DBNull.Value)
							{
								if (item.PropertyType == typeof(string))
								{
									item.SetValue(current, "", null);
								}
								else if (item.PropertyType == typeof(int))
								{
									item.SetValue(current, 0, null);
								}
								else if (item.PropertyType == typeof(DateTime))
								{
									item.SetValue(current, DateTime.MinValue, null);
								}
								else if (item.PropertyType == typeof(decimal))
								{
									item.SetValue(current, 0m, null);
								}
								continue;
							}
							string value = reader[item.Name].ToString();
							item.SetValue(current, reader[item.Name], null);
						}
					}
					// 添加到集合
					list.Add(current);
				}
			}
			return list;
		}

		/// <summary>
		/// 获取Table记录数
		/// </summary>
		/// <typeparam name="T">模型T</typeparam> 
		/// <param name="param">参数键值对</param>
		/// <returns></returns>
		public int GetCount<T>(Dictionary<string, object> param) where T : new()
		{
			SQLGenerator generator = new SQLGenerator();
			StringBuilder temp = new StringBuilder();
			temp.Append(generator.CountSqlGenerate<T>());	// select count(0) from Table where 1=1  
			DbCommand cmd = CreateCommand();
			cmd.CommandText = temp.ToString();

			cmd.Connection = MYCONNECTION;
			if (TRANSACTION != null)
			{
				cmd.Transaction = TRANSACTION;
			}
			DbParameter[] parameters = PreparParameter(param);
			cmd.Parameters.AddRange(parameters);
			object o = cmd.ExecuteScalar();
			cmd.Parameters.Clear();
			int count = 0;
			int.TryParse(o.ToString(), out count);
			return count;
		}
		/// <summary>
		/// 获取Table记录数
		/// </summary>
		/// <typeparam name="T">模型T</typeparam> 
		/// <returns></returns>
		public int GetCount<T>() where T : new()
		{
			SQLGenerator generator = new SQLGenerator();
			StringBuilder temp = new StringBuilder();
			temp.Append(generator.CountSqlGenerate<T>());	// select count(0) from Table where 1=1  
			DbCommand cmd = CreateCommand();
			cmd.CommandText = temp.ToString();

			cmd.Connection = MYCONNECTION;
			if (TRANSACTION != null)
			{
				cmd.Transaction = TRANSACTION;
			}
			object o = cmd.ExecuteScalar();
			cmd.Parameters.Clear();
			int count = 0;
			int.TryParse(o.ToString(), out count);
			return count;
		}

		/// <summary>
		/// 执行查询语句，返回DataSet
		/// </summary>
		/// <param name="SQLString">查询语句</param>
		/// <returns>DataSet</returns>
		public DataSet Query(string SQLString)
		{
			//DbDataAdapter adapter = CreateDataAdapter();
			DbCommand cmd = CreateCommand(SQLString);
			cmd.Connection = MYCONNECTION;
			DataSet ds = new DataSet();
			using (DbDataReader reader = cmd.ExecuteReader())
			{
				ds.Load(reader, LoadOption.OverwriteChanges, "Table1");
				cmd.Parameters.Clear();
				return ds;
			}
		}

		#endregion

		#region SimpleMethod

		#region ExecuteQuery
		/// <summary>
		/// 执行SQL语句，返回影响的记录数
		/// </summary>
		/// <param name="SQLString">SQL语句</param>
		/// <returns>影响的记录数</returns>
		public int ExecuteNonQuery(string SQLString)
		{
			DbCommand cmd = CreateCommand();
			cmd.Connection = MYCONNECTION;
			cmd.CommandText = SQLString;
			cmd.Transaction = TRANSACTION;
			int result = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return result;
		}

		/// <summary>
		/// 执行多个事务
		/// </summary>
		/// <param name="list">SQL命令行列表</param> 
		/// <returns>执行结果 0-由于SQL造成事务失败 -1 由于Oracle造成事务失败 1-整体事务执行成功</returns>
		public int ExecuteNonQuery(List<CommandInfo> list)
		{
			DbCommand cmd = CreateCommand();
			cmd.Connection = MYCONNECTION;
			try
			{
				if (TRANSACTION == null)
				{
					BeginTransaction();
				}
				cmd.Transaction = TRANSACTION;
				foreach (CommandInfo commandInfo in list)
				{
					string cmdText = commandInfo.CommandText;
					DbParameter[] cmdParms = (DbParameter[])commandInfo.Parameters;
					PrepareCommand(cmd, cmdText, cmdParms);
					if (commandInfo.EffentNextType == EffentNextType.SolicitationEvent)
					{
						if (commandInfo.CommandText.ToLower().IndexOf("count(") == -1)
						{
							RollBack();
							throw new Exception("违背要求" + commandInfo.CommandText + "必须符合select count(..的格式");
						}

						object obj = cmd.ExecuteScalar();
						bool isHave = false;
						if (obj == null && obj == DBNull.Value)
						{
							isHave = false;
						}
						isHave = Convert.ToInt32(obj) > 0;
						if (isHave)
						{
							//引发事件
							commandInfo.OnSolicitationEvent();
						}
					}
					if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine || commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine)
					{
						if (commandInfo.CommandText.ToLower().IndexOf("count(") == -1)
						{
							RollBack();
							throw new Exception("SQL:违背要求" + commandInfo.CommandText + "必须符合select count(..的格式");
						}

						object obj = cmd.ExecuteScalar();
						bool isHave = false;
						if (obj == null && obj == DBNull.Value)
						{
							isHave = false;
						}
						isHave = Convert.ToInt32(obj) > 0;

						if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
						{
							RollBack();
							throw new Exception("SQL:违背要求" + commandInfo.CommandText + "返回值必须大于0");
						}
						if (commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
						{
							RollBack();
							throw new Exception("SQL:违背要求" + commandInfo.CommandText + "返回值必须等于0");
						}
						continue;
					}
					int val = cmd.ExecuteNonQuery();

					if (commandInfo.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
					{
						RollBack();
						throw new Exception("SQL:违背要求" + commandInfo.CommandText + "必须有影响行");
					}
					cmd.Parameters.Clear();
				}
				Commit();
				return 1;
			}
			catch (DbException e)
			{
				RollBack();
				return 0;
			}
			catch (Exception e)
			{
				RollBack();
				return 0;
			}
		}

		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="SQLStringList">多条SQL语句</param>		
		public int ExecuteNonQuery(List<String> SQLStringList)
		{
			DbCommand cmd = CreateCommand();
			cmd.Connection = MYCONNECTION;
			try
			{

				if (TRANSACTION == null)
				{
					BeginTransaction();
				}
				cmd.Transaction = TRANSACTION;
				int count = 0;
				for (int n = 0; n < SQLStringList.Count; n++)
				{
					string strsql = SQLStringList[n];
					if (strsql.Trim().Length > 1)
					{
						cmd.CommandText = strsql;
						count += cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();
					}
				}
				Commit();
				return count;
			}
			catch (Exception ex)
			{
				RollBack();
				return 0;
			}

		}

		/// <summary>
		/// 执行带一个存储过程参数的的SQL语句。
		/// </summary>
		/// <param name="SQLString">SQL语句</param>
		/// <param name="paramList">参数</param>
		/// <returns>影响的记录数</returns>
		public int ExecuteNonQuery(string SQLString, Dictionary<string, object> paramList)
		{
			DbCommand cmd = CreateCommand();
			cmd.Connection = MYCONNECTION;
			cmd.CommandText = SQLString;
			cmd.Parameters.AddRange(PreparParameter(paramList));
			try
			{
				if (TRANSACTION != null)
				{
					cmd.Transaction = TRANSACTION;
				}
				int rows = cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();
				return rows;
			}
			catch (DbException e)
			{
				return 0;
			}
			finally
			{
				cmd.Dispose();
			}
		}

		/// <summary>
		/// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
		/// </summary>
		/// <param name="strSQL">SQL语句</param>
		/// <param name="paramList">参数</param>
		/// <returns>影响的记录数</returns>
		public int ExecuteSqlInsertImg(string strSQL, Dictionary<string, object> paramList)
		{
			DbCommand cmd = CreateCommand();
			cmd.Connection = MYCONNECTION;
			cmd.Parameters.AddRange(PreparParameter(paramList));
			try
			{
				if (TRANSACTION != null)
				{
					cmd.Transaction = TRANSACTION;
				}
				int rows = cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();
				return rows;
			}
			catch (DbException e)
			{
				return 0;
			}
			finally
			{
				cmd.Dispose();
			}
		}

		/// <summary>
		/// 执行存储过程，返回影响的行数		
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="paramList">存储过程参数</param>
		/// <param name="rowsAffected">影响的行数</param>
		/// <returns></returns>
		public int RunProcedure(string storedProcName, Dictionary<string, object> paramList, out int rowsAffected)
		{
			int result = 0;
			try
			{
				DbParameter[] parameters = PreparParameter(paramList);
				DbCommand command = BuildQueryCommand(MYCONNECTION, storedProcName, parameters);
				DbParameter parameter = CreateParameter();
				parameter.ParameterName = "ReturnValue";
				parameter.DbType = DbType.Int32;
				parameter.Size = 4;
				parameter.Value = default(Int32);
				command.Parameters.Add(parameter);
				if (TRANSACTION != null)
				{
					command.Transaction = TRANSACTION;
				}
				rowsAffected = command.ExecuteNonQuery();
				command.Parameters.Clear();
				result = (int)command.Parameters["ReturnValue"].Value;
				return result;
			}
			catch (System.Exception ex)
			{
				rowsAffected = 0;
				return result;
			}
		}

		/// <summary>
		/// 执行存储过程，返回受影响的行数
		/// </summary>
		/// <param name="spName">存储过程名称</param>
		/// <param name="parameterValues">参数值数组</param>
		/// <returns>受影响行数</returns>
		public int ExecuteNonQuery(string spName, params object[] parameterValues)
		{
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				DbParameter[] commandParameters = GetSpParameterSet(spName);
				AssignParameterValues(commandParameters, parameterValues);
				return ExecuteNonQuery(CONNECTIONSTRING, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return ExecuteNonQuery(CONNECTIONSTRING, CommandType.StoredProcedure, spName);
			}
		}
		/// <summary>
		/// 执行SQL，返回受影响行数
		/// </summary>
		/// <param name="commandType">DBCommand类型</param>
		/// <param name="commandText">命令文本</param>
		/// <param name="commandParameters">参数</param>
		/// <returns>受影响行数</returns>
		public int ExecuteNonQuery(string commandText, CommandType commandType, params DbParameter[] commandParameters)
		{
			DbCommand cmd = CreateCommand();
			PrepareCommand(cmd, commandText, commandParameters);
			if (TRANSACTION != null)
			{
				cmd.Transaction = TRANSACTION;
			}
			int retval = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return retval;
		}
		/// <summary>
		/// 执行SQL，返回受影响行数
		/// </summary>
		/// <param name="commandType">DBCommand类型</param>
		/// <param name="commandText">命令文本</param>
		/// <param name="paramList">参数值数组</param>
		/// <returns>受影响行数</returns>
		public int ExecuteNonQuery(string commandText, CommandType commandType, Dictionary<string, object> paramList)
		{
			DbCommand cmd = CreateCommand();
			DbParameter[] param = PreparParameter(paramList);
			PrepareCommand(cmd, commandText, param);
			if (TRANSACTION != null)
			{
				cmd.Transaction = TRANSACTION;
			}
			int retval = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return retval;
		}



		#endregion

		#region ExecuteScular

		/// <summary>
		/// 执行一条计算查询结果语句，返回查询结果（object）
		/// </summary>
		/// <param name="SQLString">计算查询结果语句</param>
		/// <param name="paramList">参数</param>
		/// <returns>查询结果（object）</returns>
		public object ExcuteScular(string SQLString, Dictionary<string, object> paramList)
		{
			DbCommand cmd = CreateCommand();
			try
			{
				cmd.Connection = MYCONNECTION;
				cmd.CommandText = SQLString;
				if (InTransaction)
				{
					cmd.Transaction = TRANSACTION;
				}
				DbParameter[] parameters = PreparParameter(paramList);
				cmd.Parameters.AddRange(parameters);
				object obj = cmd.ExecuteScalar();
				if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
				{
					return null;
				}
				else
				{
					return obj;
				}
			}
			catch (DbException e)
			{
				return null;
			}
			finally
			{
				cmd.Parameters.Clear();
				cmd.Dispose();
			}
		}

		/// <summary>
		/// 执行一条计算查询结果语句，返回查询结果（object）
		/// </summary>
		/// <param name="spName">存储过程名称</param>
		/// <param name="paramValues">参数值数组</param>
		/// <returns>object</returns>
		public object ExecuteScalar(string spName, params object[] paramValues)
		{
			if ((paramValues != null) && (paramValues.Length > 0))
			{
				DbParameter[] param = GetSpParameterSet(spName);
				AssignParameterValues(param, paramValues);
				return ExecuteScalar(CommandType.StoredProcedure, spName, param);
			}
			else
			{
				return ExecuteScalar(CommandType.StoredProcedure, spName);
			}
		}
		/// <summary>
		/// 执行一条计算查询结果语句，返回查询结果（object）
		/// </summary>
		/// <param name="commandType">命令类型</param>
		/// <param name="commandText">命令SQL</param>
		/// <param name="paramList">参数列表</param>
		/// <returns></returns>
		public object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] paramList)
		{
			try
			{
				DbCommand cmd = CreateCommand();
				PrepareCommand(cmd, commandText, paramList);
				if (InTransaction)
				{
					cmd.Transaction = TRANSACTION;
				}
				object retval = cmd.ExecuteScalar();
				cmd.Parameters.Clear();
				return retval;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		#endregion

		#region DataReader

		/// <summary>
		/// 执行查询语句，返回DBDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="strSQL">查询语句</param>
		/// <param name="paramList">参数</param>
		/// <returns>DBDataReader</returns>
		public DbDataReader ExecuteReader(string strSQL, Dictionary<string, object> paramList)
		{
			DbCommand cmd = CreateCommand();
			try
			{
				cmd.Connection = MYCONNECTION;
				cmd.CommandText = strSQL;
				cmd.Parameters.AddRange(PreparParameter(paramList));
				if (TRANSACTION != null)
				{
					cmd.Transaction = TRANSACTION;
				}
				DbDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				cmd.Parameters.Clear();
				return myReader;
			}
			catch (DbException e)
			{
				return null;
			}
		}
		/// <summary>
		/// 执行查询语句，返回DBDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="strSQL">查询语句</param> 
		/// <returns>DBDataReader</returns>
		public DbDataReader ExecuteReader(string strSQL)
		{
			DbCommand cmd = CreateCommand();
			try
			{
				cmd.Connection = MYCONNECTION;
				cmd.CommandText = strSQL;
				if (TRANSACTION != null)
				{
					cmd.Transaction = TRANSACTION;
				}
				DbDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				cmd.Parameters.Clear();
				return myReader;
			}
			catch (DbException e)
			{
				return null;
			}
		}
		/// <summary>
		/// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="spName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlDataReader</returns>
		public DbDataReader ExecuteReader(string spName, params object[] paramList)
		{
			DbDataReader returnReader;
			DbParameter[] parameters = DiscoverSpParameterSet(spName, false);
			AssignParameterValues(parameters, paramList);
			DbCommand cmd = BuildQueryCommand(MYCONNECTION, spName, parameters);
			cmd.CommandType = CommandType.StoredProcedure;
			if (TRANSACTION != null)
			{
				cmd.Transaction = TRANSACTION;
			}
			AttachParameters(cmd, parameters);
			returnReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
			cmd.Parameters.Clear();
			return returnReader;
		}
		/// <summary>
		/// 构建 DbCommand 对象(用来返回一个结果集，而不是一个整数值)
		/// </summary>
		/// <param name="connection">数据库连接</param>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlCommand</returns>
		private DbCommand BuildQueryCommand(DbConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			DbCommand command = CreateCommand();
			command.CommandText = storedProcName;
			command.Connection = MYCONNECTION;
			command.CommandType = CommandType.StoredProcedure;
			foreach (DbParameter parameter in parameters)
			{
				if (parameter != null)
				{
					// 检查未分配值的输出参数,将其分配以DBNull.Value.
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
						(parameter.Value == null))
					{
						parameter.Value = DBNull.Value;
					}
					command.Parameters.Add(parameter);
				}
			}

			return command;
		}
		#endregion

		#region DataSet

		/// <summary>
		/// 执行查询语句，返回DataSet
		/// </summary>
		/// <param name="SQLString">查询语句</param>
		/// <returns>DataSet</returns>
		public DataSet GetDataSet(string SQLString, Dictionary<string, object> paramList)
		{
			DbCommand cmd = CreateCommand();
			cmd.Parameters.AddRange(PreparParameter(paramList));
			cmd.Connection = MYCONNECTION;
			if (TRANSACTION != null)
			{
				cmd.Transaction = TRANSACTION;
			}
			DataSet ds = new DataSet();
			using (DbDataReader reader = cmd.ExecuteReader())
			{
				DataTable temp = new DataTable();
				temp.Load(reader);
				ds.Tables.Add(temp);
				return ds;
			}

		}

		/// <summary>
		/// 获取Dataset
		/// </summary>
		/// <param name="spName">存储过程名称</param>
		/// <param name="parameterValues">参数值数组</param>
		/// <returns></returns>
		public DataSet ExecuteDataset(string spName, params object[] parameterValues)
		{
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				DbParameter[] commandParameters = GetSpParameterSet(spName);
				AssignParameterValues(commandParameters, parameterValues);
				return ExecuteDataset(CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				return ExecuteDataset(CommandType.StoredProcedure, spName);
			}
		}
		/// <summary>
		/// 获取Dataset
		/// </summary>
		/// <param name="commandType">Command类型</param>
		/// <param name="commandText">命令语句</param>
		/// <param name="commandParameters">参数</param>
		/// <returns></returns>
		public DataSet ExecuteDataset(CommandType commandType, string commandText, params DbParameter[] commandParameters)
		{
			DbCommand cmd = CreateCommand();
			PrepareCommand(cmd, commandText, commandParameters);
			DataSet ds = new DataSet();
			using (DbDataReader reader = cmd.ExecuteReader())
			{
				ds.Load(reader, LoadOption.OverwriteChanges, "Table1");
				return ds;
			}
		}
		#endregion

		#region GetCount
		/// <summary>
		/// 获得总数
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="commandParameters">SqlParamter参数组</param>
		/// <returns></returns>
		public int GetCount(string sql, Dictionary<string, object> paramList)
		{
			int count = 0;
			DbParameter[] param = PreparParameter(paramList);
			object o = ExecuteScalar(CommandType.Text, sql, param);
			int.TryParse(o.ToString(), out count);
			return count;
		}
		/// <summary>
		/// 获得总数
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="commandParameters">SqlParamter参数组</param>
		/// <returns></returns>
		public int GetListCount(string sql, Dictionary<string, object> paramList)
		{
			int count = 0;
			DbParameter[] param = PreparParameter(paramList);
			List<object> o = GetDynaminObjectList(sql, paramList);
			return o == null ? 0 : o.Count;
		}

		#endregion

		#region Exist
		/// <summary>
		/// 查询是否存在记录
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="commandParameters">SqlParamter参数组</param>
		/// <returns></returns>
		public bool Exist(string sql, Dictionary<string, object> paramList)
		{
			int count = 0;
			DbParameter[] param = PreparParameter(paramList);
			object o = ExecuteScalar(CommandType.Text, sql, param);
			int.TryParse(o.ToString(), out count);
			return count > 0;
		}
		/// <summary>
		/// 查询是否存在记录
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="commandParameters">SqlParamter参数组</param>
		/// <returns></returns>
		public bool Exist(string sql)
		{
			int count = 0;
			object o = ExecuteScalar(CommandType.Text, sql);
			int.TryParse(o.ToString(), out count);
			return count > 0;
		}


		/// <summary>
		/// 通过主键查找是否存在改条记录
		/// </summary>
		/// <param name="value">主键值</param>
		/// <returns></returns>
		public bool Exist<T>(object value)
		{
			T result = System.Activator.CreateInstance<T>();
			SQLGenerator generator = new SQLGenerator();
			DbParameter param = CreateParameter();
			string sql = generator.SelectBykeySqlGenerate<T>(result, ref param);
			DbCommand cmd = CreateCommand();
			param.Value = value;
			cmd.CommandText = sql;
			cmd.Parameters.Add(param);
			try
			{
				cmd.Connection = MYCONNECTION;
				cmd.Transaction = TRANSACTION;
				using (DbDataReader reader = cmd.ExecuteReader())
				{
					if (reader != null && reader.Read())
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (System.Exception ex)
			{
				return false;
			}
			finally
			{
				cmd.Parameters.Clear();
			}
		}

		/// <summary>
		/// 判断是否存在该条记录
		/// </summary>
		/// <typeparam name="T">模型</typeparam>
		/// <param name="whereStr">whereStr</param>
		/// <returns></returns>
		public bool Exist<T>(string whereStr) where T : new()
		{
			SQLGenerator generator = new SQLGenerator();
			StringBuilder temp = new StringBuilder();
			temp.Append(generator.SelectSqlGenerate<T>());	// select * from Table where 1=1 
			if (whereStr.Length > 0)
			{
				temp.Append(whereStr);
			}
			DbCommand cmd = CreateCommand();

			cmd.CommandText = temp.ToString();

			cmd.Connection = MYCONNECTION;
			if (TRANSACTION != null)
			{
				cmd.Transaction = TRANSACTION;
			}
			// 遍历Reader
			using (DbDataReader reader = cmd.ExecuteReader())
			{
				cmd.Parameters.Clear();
				return reader != null && reader.Read();
			}
		}

		/// <summary>
		/// 判断是否存在该条记录
		/// </summary>
		/// <typeparam name="T">模型</typeparam>
		/// <param name="whereStr">whereStr</param>
		/// <param name="paramList">参数</param>
		/// <returns></returns>
		public bool Exist<T>(string whereStr, Dictionary<string, object> paramList) where T : new()
		{
			SQLGenerator generator = new SQLGenerator();
			StringBuilder temp = new StringBuilder();
			temp.Append(generator.SelectSqlGenerate<T>());	// select * from Table where 1=1 
			if (whereStr.Length > 0)
			{
				temp.Append(whereStr);
			}

			DbCommand cmd = CreateCommand();

			cmd.CommandText = temp.ToString();
			if (paramList != null && paramList.Count > 0)
			{
				DbParameter[] paramss = PreparParameter(paramList);
				cmd.Parameters.Add(paramss);
			}
			cmd.Connection = MYCONNECTION;
			if (TRANSACTION != null)
			{
				cmd.Transaction = TRANSACTION;
			}
			// 遍历Reader
			using (DbDataReader reader = cmd.ExecuteReader())
			{
				cmd.Parameters.Clear();
				return reader != null && reader.Read();
			}
		}
		#endregion

		#endregion

		#region 动态对象

		/// <summary>
		/// 获取某个SQL里面的所有字段的值的对象数组
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="paramList">参数集合</param>
		/// <returns></returns>
		public object[] GetModelValue(string sql, Dictionary<string, object> paramList)
		{
			ArrayList list = new ArrayList();
			using (MYCONNECTION)
			{
				if (MYCONNECTION.State == ConnectionState.Closed)
				{
					MYCONNECTION.Open();
				}
				DbCommand cmd = CreateCommand();
				cmd.CommandText = sql;
				cmd.Connection = MYCONNECTION;
				cmd.Parameters.AddRange(PreparParameter(paramList));
				using (DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					cmd.Parameters.Clear();
					// 只返回一行数据，用于前台数据更新时的数据显示
					if (dr != null && dr.Read())
					{
						for (int i = 0; i < dr.FieldCount; i++)
						{
							list.Add(dr[i].ToString());
						}
					}
				}
			}
			return list.ToArray();
		}

		/// <summary>
		/// 获取一个SQL构成的动态对象
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="p"></param>
		/// <returns></returns>
		public object GetSingelDynaminObject(string sql, Dictionary<string, object> paramList)
		{
			Type temp = ClassHelper.BuildType("MyDynamicObject");
			DbCommand cmd = CreateCommand();
			cmd.Connection = MYCONNECTION;
			cmd.CommandText = sql;
			cmd.Parameters.AddRange(PreparParameter(paramList));
			object o = ClassHelper.CreateInstance(temp);
			try
			{
				using (DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					cmd.Parameters.Clear();
					List<ClassHelper.CustPropertyInfo> lists = new List<ClassHelper.CustPropertyInfo>();
					// 使用字典存放目标属性的名称-值 eg:Address-----"西一段"
					// 并将SqlDataReader的字段-值填充至字典
					Dictionary<string, string> dic = new Dictionary<string, string>();
					for (int i = 0; i < dr.FieldCount; i++)
					{
						lists.Add(new ClassHelper.CustPropertyInfo(dr.GetName(i).ToUpper()));
						dic.Add(dr.GetName(i).ToUpper(), null);
					}
					// 为动态类添加属性和值
					temp = ClassHelper.AddProperty(temp, lists);
					o = ClassHelper.CreateInstance(temp);
					if (dr != null && dr.Read())
					{
						for (int j = 0; j < dic.Count; j++)
						{
							/// 日期类型处理。如果数据库为date类型，则返回yyyy-MM-dd格式的日期，如果为datetime类型，则返回yyyy-MM-dd HH:mm:ss
							string dataType = dr.GetDataTypeName(j);
							if (dataType == "date" || dataType == "datetime")
							{
								if (string.IsNullOrEmpty(dr[j].ToString()))
								{
									dic[dr.GetName(j).ToUpper()] = "";
									continue;
								}
								if (dataType == "date")
								{
									dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd");
								}
								else
								{
									dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd HH:mm:ss");
								}
							}
							else
							{
								dic[dr.GetName(j).ToUpper()] = dr[j].ToString();
							}
						}
						o = ClassHelper.CreateInstance(temp);
						ClassHelper.SetPropertyValue(o, dic);
					}
					else
					{
						return null;
					}
				}
			}
			catch
			{
				return o;
			}
			return o;
		}
		/// <summary>
		/// 获取一个SQL构成的动态对象
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="p"></param>
		/// <returns></returns>
		public object GetSingelDynaminObject(string sql, string[] paramName, string[] param)
		{
			Type temp = ClassHelper.BuildType("MyDynamicObject");
			DbCommand cmd = CreateCommand();
			cmd.Connection = MYCONNECTION;
			cmd.CommandText = sql;
			if (param.Length != paramName.Length)
			{
				return null;
			}
			Dictionary<string, object> paramss = new Dictionary<string, object>();
			for (int i = 0; i < paramName.Length; i++)
			{
				paramss.Add(paramName[i], param[i]);
			}
			cmd.Parameters.AddRange(PreparParameter(paramss));
			object o = ClassHelper.CreateInstance(temp);
			try
			{
				using (DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					cmd.Parameters.Clear();
					List<ClassHelper.CustPropertyInfo> lists = new List<ClassHelper.CustPropertyInfo>();
					// 使用字典存放目标属性的名称-值 eg:Address-----"西一段"
					// 并将SqlDataReader的字段-值填充至字典
					Dictionary<string, string> dic = new Dictionary<string, string>();
					for (int i = 0; i < dr.FieldCount; i++)
					{
						lists.Add(new ClassHelper.CustPropertyInfo(dr.GetName(i).ToUpper()));
						dic.Add(dr.GetName(i).ToUpper(), null);
					}
					// 为动态类添加属性和值
					temp = ClassHelper.AddProperty(temp, lists);
					o = ClassHelper.CreateInstance(temp);
					if (dr != null && dr.Read())
					{
						for (int j = 0; j < dic.Count; j++)
						{
							/// 日期类型处理。如果数据库为date类型，则返回yyyy-MM-dd格式的日期，如果为datetime类型，则返回yyyy-MM-dd HH:mm:ss
							string dataType = dr.GetDataTypeName(j);
							if (dataType == "date" || dataType == "datetime")
							{
								if (string.IsNullOrEmpty(dr[j].ToString()))
								{
									dic[dr.GetName(j).ToUpper()] = "";
									continue;
								}
								if (dataType == "date")
								{
									dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd");
								}
								else
								{
									dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd HH:mm:ss");
								}
							}
							else
							{
								dic[dr.GetName(j).ToUpper()] = dr[j].ToString();
							}
						}
						o = ClassHelper.CreateInstance(temp);
						ClassHelper.SetPropertyValue(o, dic);
					}
					else
					{
						return null;
					}
				}
			}
			catch
			{
				return o;
			}
			return o;
		}
		/// <summary>
		/// 获取一个SQL构成的动态对象集合
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="paramList">参数数组</param>
		/// <returns>动态对象集合</returns>
		public List<object> GetDynaminObjectList(string sql, Dictionary<string, object> paramList)
		{
			DbCommand cmd = CreateCommand(sql);
			cmd.Parameters.AddRange(PreparParameter(paramList));
			List<object> dynamicList = new List<object>();
			try
			{
				using (DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					cmd.Parameters.Clear();
					while (dr != null && dr.Read())
					{
						Type temp = ClassHelper.BuildType("MyDynamicObject");
						object o = ClassHelper.CreateInstance(temp);
						List<ClassHelper.CustPropertyInfo> lists = new List<ClassHelper.CustPropertyInfo>();
						// 使用字典存放目标属性的名称-值 eg:Address-----"西一段"
						// 并将SqlDataReader的字段-值填充至字典
						Dictionary<string, string> dic = new Dictionary<string, string>();
						for (int i = 0; i < dr.FieldCount; i++)
						{
							lists.Add(new ClassHelper.CustPropertyInfo(dr.GetName(i).ToUpper()));
							dic.Add(dr.GetName(i).ToUpper(), null);
						}
						// 为动态类添加属性和值
						temp = ClassHelper.AddProperty(temp, lists);
						o = ClassHelper.CreateInstance(temp);
						for (int j = 0; j < dic.Count; j++)
						{
							/// 日期类型处理。如果数据库为date类型，则返回yyyy-MM-dd格式的日期，如果为datetime类型，则返回yyyy-MM-dd HH:mm:ss
							string dataType = dr.GetDataTypeName(j);
							if (dataType == "date" || dataType == "datetime")
							{
								if (string.IsNullOrEmpty(dr[j].ToString()))
								{
									dic[dr.GetName(j).ToUpper()] = "";
									continue;
								}
								if (dataType == "date")
								{
									dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd");
								}
								else
								{
									dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd HH:mm:ss");
								}
							}
							else
							{
								dic[dr.GetName(j).ToUpper()] = dr[j].ToString();
							}
						}
						o = ClassHelper.CreateInstance(temp);
						ClassHelper.SetPropertyValue(o, dic);
						dynamicList.Add(o);
					}
				}
			}
			catch
			{
				return dynamicList;
			}
			return dynamicList;
		}

		/// <summary>
		/// 获取一个SQL构成的动态对象集合
		/// </summary>
		/// <param name="sql">SQL，必须包含ID字段</param>
		/// <param name="pageIndex">当前页索引</param>
		/// <param name="pageNumber">表格显示行数</param>
		/// <param name="p">可变参数</param>
		/// <returns>动态对象集合</returns>
		public List<object> GetDynaminObjectList(string sql, int pageIndex, int pageNumber, string orderBy, Dictionary<string, object> paramList)
		{
			///获取分页数据
			string PageSQL = @"select * from 
								(SELECT *,ROW_NUMBER() OVER ( ORDER BY td.ID) AS rownumber FROM ({0}) td) temp
								WHERE rownumber BETWEEN {1} AND {2} {3}";
			PageSQL = string.Format(PageSQL, sql, pageIndex * pageNumber + 1, (pageIndex + 1) * pageNumber, string.IsNullOrEmpty(orderBy) ? "" : "order by " + orderBy);

			DbCommand cmd = CreateCommand(PageSQL);
			cmd.Parameters.AddRange(PreparParameter(paramList));
			List<object> dynamicList = new List<object>();
			try
			{
				using (DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					cmd.Parameters.Clear();
					while (dr != null && dr.Read())
					{
						Type temp = ClassHelper.BuildType("MyDynamicObject");
						object o = ClassHelper.CreateInstance(temp);
						List<ClassHelper.CustPropertyInfo> lists = new List<ClassHelper.CustPropertyInfo>();
						// 使用字典存放目标属性的名称-值 eg:Address-----"西一段"
						// 并将SqlDataReader的字段-值填充至字典
						Dictionary<string, string> dic = new Dictionary<string, string>();
						for (int i = 0; i < dr.FieldCount; i++)
						{
							lists.Add(new ClassHelper.CustPropertyInfo(dr.GetName(i).ToUpper()));
							dic.Add(dr.GetName(i).ToUpper(), null);
						}
						// 为动态类添加属性和值
						temp = ClassHelper.AddProperty(temp, lists);
						o = ClassHelper.CreateInstance(temp);
						for (int j = 0; j < dic.Count; j++)
						{
							/// 日期类型处理。如果数据库为date类型，则返回yyyy-MM-dd格式的日期，如果为datetime类型，则返回yyyy-MM-dd HH:mm:ss
							string dataType = dr.GetDataTypeName(j);
							if (dataType == "date" || dataType == "datetime")
							{
								if (string.IsNullOrEmpty(dr[j].ToString()))
								{
									dic[dr.GetName(j).ToUpper()] = "";
									continue;
								}
								if (dataType == "date")
								{
									dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd");
								}
								else
								{
									dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd HH:mm:ss");
								}
							}
							else
							{
								dic[dr.GetName(j).ToUpper()] = dr[j].ToString();
							}
						}
						o = ClassHelper.CreateInstance(temp);
						ClassHelper.SetPropertyValue(o, dic);
						dynamicList.Add(o);
					}
				}
			}
			catch
			{
				return dynamicList;
			}
			return dynamicList;
		}

		/// <summary>
		/// 获取一个SQL构成的动态对象集合
		/// </summary>
		/// <param name="sql">SQL，必须包含ID字段</param>
		/// <param name="pageIndex">当前页索引</param>
		/// <param name="pageNumber">表格显示行数</param>
		/// <param name="p">可变参数</param>
		/// <returns>动态对象集合</returns>
		public List<object> GetDynaminObjectList(string sql, int pageIndex, int pageNumber, string orderBy, string[] paramName, string[] param)
		{
			///获取分页数据
			string PageSQL = @"select * from 
								(SELECT *,ROW_NUMBER() OVER ( ORDER BY td.ID) AS rownumber FROM ({0}) td) temp
								WHERE rownumber BETWEEN {1} AND {2} {3}";
			PageSQL = string.Format(PageSQL, sql, pageIndex * pageNumber + 1, (pageIndex + 1) * pageNumber, orderBy == null ? "" : "order by " + orderBy);

			DbCommand cmd = CreateCommand(PageSQL);
			Dictionary<string, object> paramList = new Dictionary<string, object>();
			if (paramName.Length != param.Length)
			{
				return new List<object>();
			}
			for (int i = 0; i < paramName.Length; i++)
			{
				paramList.Add(paramName[i], param[i]);
			}
			cmd.Parameters.AddRange(PreparParameter(paramList));
			List<object> dynamicList = new List<object>();
			try
			{
				using (DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					cmd.Parameters.Clear();
					while (dr != null && dr.Read())
					{
						Type temp = ClassHelper.BuildType("MyDynamicObject");
						object o = ClassHelper.CreateInstance(temp);
						List<ClassHelper.CustPropertyInfo> lists = new List<ClassHelper.CustPropertyInfo>();
						// 使用字典存放目标属性的名称-值 eg:Address-----"西一段"
						// 并将SqlDataReader的字段-值填充至字典
						Dictionary<string, string> dic = new Dictionary<string, string>();
						for (int i = 0; i < dr.FieldCount; i++)
						{
							lists.Add(new ClassHelper.CustPropertyInfo(dr.GetName(i).ToUpper()));
							dic.Add(dr.GetName(i).ToUpper(), null);
						}
						// 为动态类添加属性和值
						temp = ClassHelper.AddProperty(temp, lists);
						o = ClassHelper.CreateInstance(temp);
						for (int j = 0; j < dic.Count; j++)
						{
							/// 日期类型处理。如果数据库为date类型，则返回yyyy-MM-dd格式的日期，如果为datetime类型，则返回yyyy-MM-dd HH:mm:ss
							string dataType = dr.GetDataTypeName(j);
							if (dataType == "date" || dataType == "datetime")
							{
								if (string.IsNullOrEmpty(dr[j].ToString()))
								{
									dic[dr.GetName(j).ToUpper()] = "";
									continue;
								}
								if (dataType == "date")
								{
									dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd");
								}
								else
								{
									dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd HH:mm:ss");
								}
							}
							else
							{
								dic[dr.GetName(j).ToUpper()] = dr[j].ToString();
							}
						}
						o = ClassHelper.CreateInstance(temp);
						ClassHelper.SetPropertyValue(o, dic);
						dynamicList.Add(o);
					}
				}
			}
			catch
			{
				return dynamicList;
			}
			return dynamicList;
		}

		#endregion

		#region 参数准备

		/// <summary>
		/// 准备插入实体对象存储过程的参数
		/// </summary>
		/// <param name="targetModel">目标模型</param>
		/// <returns></returns>
		public DbParameter[] PreparInsertSqlParameter<T>(Type targetModel, T model)
		{
			List<DbParameter> parameterList = new List<DbParameter>();
			System.Reflection.PropertyInfo[] infos = targetModel.GetProperties();
			foreach (PropertyInfo item in infos)
			{
				var attrs = item.GetCustomAttributes(false);
				foreach (var columnAttribute in attrs)
				{
					if (columnAttribute is ColumnAttribute)
					{
						ColumnAttribute ca = columnAttribute as ColumnAttribute;
						DbParameter param = CreateParameter();
						param.ParameterName = ca.Code;
						param.Value = item.GetValue(model, null);
						parameterList.Add(param);
					}
				}
			}
			return parameterList.ToArray();
		}

		/// <summary>
		/// 准备删除实体对象存储过程的参数
		/// </summary>
		/// <param name="targetModel">目标模型</param>
		/// <returns></returns>
		public DbParameter[] PreparDeleteSqlParameter<T>(Type targetModel, T model)
		{
			List<DbParameter> parameterList = new List<DbParameter>();
			System.Reflection.PropertyInfo[] infos = targetModel.GetProperties();
			foreach (PropertyInfo item in infos)
			{
				var attrs = item.GetCustomAttributes(false);
				foreach (var columnAttribute in attrs)
				{
					if (columnAttribute is KeyAttribute)
					{
						ColumnAttribute ca = columnAttribute as ColumnAttribute;
						DbParameter param = CreateParameter();
						param.ParameterName = ca.Name;
						param.Value = item.GetValue(model, null);
						parameterList.Add(param);
						return parameterList.ToArray();		// once get the pramary key ,go back 
					}
				}
			}
			return parameterList.ToArray();
		}

		/// <summary>
		/// 准备删除实体对象存储过程的参数
		/// </summary>
		/// <param name="targetModel">目标模型</param>
		/// <param name="value">主键值</param>
		/// <returns></returns>
		public DbParameter PreparPrimaryKey(Type targetModel, object value)
		{
			DbParameter parameterList = CreateParameter();
			System.Reflection.PropertyInfo[] infos = targetModel.GetProperties();
			bool flag = false;
			if (targetModel.GetCustomAttributes(typeof(KeyAttribute), false).Length == 0)
			{
				throw new Exception("数据库异常：表未设置主键");
			}
			KeyAttribute keyAttribute = (KeyAttribute)targetModel.GetCustomAttributes(typeof(KeyAttribute), false)[0];
			DbParameter param = CreateParameter();
			PropertyInfo pInfo = targetModel.GetProperty(keyAttribute.Code);
			param.ParameterName = keyAttribute.Code;
			ColumnAttribute columnAttribute = (ColumnAttribute)pInfo.GetCustomAttributes(typeof(ColumnAttribute), false)[0];
			param.DbType = GetDbTypeByName(columnAttribute.TypeName);
			if (columnAttribute.Length != 0)
			{
				param.Size = columnAttribute.Length;
			}
			param.Value = value;
			return param;
		}

		/// <summary>
		/// 准备参数
		/// </summary>
		/// <typeparam name="T">泛型T Model</typeparam>
		/// <param name="param">参数字典，Key为参数名，value为参数值</param>
		/// <returns>DBParameter集合</returns>
		protected DbParameter[] PreparParameter<T>(Dictionary<string, object> param)
		{
			List<DbParameter> list = new List<DbParameter>();
			if (param == null)
			{
				return list.ToArray();
			}
			foreach (KeyValuePair<string, object> item in param)
			{
				DbParameter tempParam = CreateParameter();
				tempParam.ParameterName = item.Key;
				PropertyInfo pInfo = typeof(T).GetProperty(item.Key);
				string paramDBType = ((ColumnAttribute)(pInfo.GetCustomAttributes(typeof(ColumnAttribute), false)[0])).TypeName;
				tempParam.DbType = GetDbTypeByName(paramDBType);
				tempParam.Value = item.Value;
				list.Add(tempParam);
			}
			return list.ToArray();
		}
		/// <summary>
		/// 准备参数
		/// </summary>
		/// <param name="param">参数字典，Key为参数名，value为参数值</param>
		/// <returns>DBParameter集合</returns>
		protected DbParameter[] PreparParameter(Dictionary<string, object> param)
		{
			List<DbParameter> list = new List<DbParameter>();
			if (param == null)
			{
				return list.ToArray();
			}
			foreach (KeyValuePair<string, object> item in param)
			{
				DbParameter tempParam = CreateParameter();
				tempParam.ParameterName = item.Key;
				tempParam.Value = item.Value == null ? DBNull.Value : item.Value;
				list.Add(tempParam);
			}
			return list.ToArray();
		}

		#endregion

		#region 功能函数

		/// <summary>
		/// 获取DbParameter，参数名和参数值必须对应
		/// </summary>
		/// <param name="paramName">参数名称数组</param>
		/// <param name="p">参数值</param>
		/// <returns>DbParameter数组</returns>
		public DbParameter[] GetSqlParameter(string[] paramName, string[] p)
		{
			if (paramName != null && p != null)
			{
				List<DbParameter> list = new List<DbParameter>();
				for (int i = 0; i < paramName.Length; i++)
				{
					DbParameter temp = CreateParameter(paramName[i], string.IsNullOrEmpty(p[i]) ? "" : p[i]);
					list.Add(temp);
				}
				return list.ToArray();
			}
			else
				return null;
		}

		/// <summary>
		/// 附加参数到DBCommand
		/// </summary>
		/// <param name="command">DBCommand</param>
		/// <param name="commandParameters">DBParameter</param>
		private void AttachParameters(DbCommand command, DbParameter[] commandParameters)
		{
			if (commandParameters != null)
			{
				foreach (DbParameter p in commandParameters)
				{
					if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
					{
						p.Value = DBNull.Value;
					}
					command.Parameters.Add(p);
				}
			}
		}
		/// <summary>
		/// 创建 SqlCommand 对象实例(用来返回一个整数值)	
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlCommand 对象实例</returns>
		private DbCommand BuildIntCommand(DbConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			DbCommand command = BuildQueryCommand(connection, storedProcName, parameters);
			DbParameter parameter = CreateParameter();
			parameter.ParameterName = "ReturnValue";
			parameter.DbType = DbType.Int32;
			parameter.Size = 4;
			parameter.Direction = ParameterDirection.ReturnValue;
			parameter.SourceColumnNullMapping = true;
			parameter.Value = null;
			command.Parameters.Add(parameter);
			return command;
		}

		/// <summary>
		/// 获取模型属性数组
		/// </summary>
		/// <returns></returns>
		public String[] GetModelProperties<T>()
		{
			Type t = typeof(T);
			System.Reflection.PropertyInfo[] infos = t.GetProperties();
			ArrayList propertyList = new ArrayList();
			foreach (PropertyInfo item in infos)
			{
				propertyList.Add(item.Name);
			}
			return propertyList.ToArray() as string[];
		}

		/// <summary>
		/// 根据列的TypeName特性值获取数据库类型
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public DbType GetDbTypeByName(string dbType)
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
				default:
					return DbType.String;
			}
		}

		/// <summary>
		/// 获取数据库连接
		/// </summary>
		/// <returns></returns>
		public DbConnection GetCurrentConnection()
		{
			DbConnection con;
			con = MYPROVIDERFACTORY.CreateConnection();
			con.ConnectionString = CONNECTIONSTRING;
			try
			{
				con.Open();
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			return con;
		}
		/// <summary>
		/// 跟据不同的数据库驱动创建参数实例
		/// </summary>
		/// <returns></returns>
		public DbParameter CreateParameter()
		{
			return MYPROVIDERFACTORY.CreateParameter();
		}
		/// <summary>
		/// 跟据不同的数据库驱动创建参数实例
		/// </summary>
		/// <returns></returns>
		public DbParameter[] CreateParameter(Dictionary<string, object> paramList)
		{
			List<DbParameter> list = new List<DbParameter>();
			foreach (KeyValuePair<string, object> item in paramList)
			{
				list.Add(CreateParameter(item.Key, item.Value));
			}
			return list.ToArray();
		}

		/// <summary>
		/// 跟据不同的数据库驱动创建参数实例
		/// </summary>
		/// <returns></returns>
		public DbParameter CreateParameter(string paramName, object paramValue)
		{
			DbParameter temp = CreateParameter();
			temp.ParameterName = paramName;
			temp.Value = paramValue;
			return temp;
		}

		/// <summary>
		/// 根据不同的数据库驱动创建不同的DBCommand
		/// </summary>
		/// <returns></returns>
		private DbCommand CreateCommand()
		{
			DbCommand cmd = MYPROVIDERFACTORY.CreateCommand();
			if (MYCONNECTION.State != ConnectionState.Open)
			{
				MYCONNECTION.Open();
			}
			cmd.Connection = MYCONNECTION;
			cmd.Parameters.Clear();
			return cmd;
		}
		/// <summary>
		/// 根据不同的数据库驱动创建不同的DBCommand
		/// </summary>
		/// <returns></returns>
		private DbCommand CreateCommand(string sql)
		{
			DbCommand command = MYPROVIDERFACTORY.CreateCommand();
			Debug.Assert(command != null, "command != null");
			command.CommandText = sql;
			if (MYCONNECTION.State != ConnectionState.Open)
			{
				MYCONNECTION.Open();
			}
			command.Connection = MYCONNECTION;
			command.Parameters.Clear();
			return command;
		}
		/// <summary>
		/// 根据不同的数据库驱动创建不同的DBDataAdpatper
		/// </summary>
		/// <returns></returns>
		private DbDataAdapter CreateDataAdapter()
		{
			return MYPROVIDERFACTORY.CreateDataAdapter();
		}

		/// <summary>
		/// 参数准备
		/// </summary>
		/// <param name="cmd">会话Command</param>
		/// <param name="conn">会话连接</param>
		/// <param name="trans">会话事务</param>
		/// <param name="cmdText">会话SQL</param>
		/// <param name="cmdParms">参数</param>
		private void PrepareCommand(DbCommand cmd, string cmdText, DbParameter[] cmdParms)
		{
			if (MYCONNECTION.State != ConnectionState.Open)
				MYCONNECTION.Open();
			//cmd.Connection = MYCONNECTION;
			cmd.CommandText = cmdText;
			if (TRANSACTION != null)
				cmd.Transaction = TRANSACTION;
			cmd.CommandType = CommandType.Text;//cmdType;
			if (cmdParms != null)
			{
				foreach (DbParameter parameter in cmdParms)
				{
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
						(parameter.Value == null))
					{
						parameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(parameter);
				}
			}
		}

		/// <summary>
		/// Parameter赋值
		/// </summary>
		/// <param name="commandParameters">参数数组</param>
		/// <param name="parameterValues">值数组</param>
		private void AssignParameterValues(DbParameter[] commandParameters, object[] parameterValues)
		{
			if ((commandParameters == null) || (parameterValues == null))
			{
				return;
			}
			if (commandParameters.Length != parameterValues.Length)
			{
				throw new ArgumentException("参数不正确");
			}
			for (int i = 0, j = commandParameters.Length; i < j; i++)
			{
				commandParameters[i].Value = parameterValues[i];
			}
		}

		/// <summary>
		/// 获得存储过程的参数集
		/// </summary>
		/// <param name="commandText">一个存储过程名或者T-SQL命令</param>
		/// <returns>一个参数对象数组</returns>
		/// <remarks>
		/// 这个方法从数据库中获得信息，并将之存储在缓存，以便之后的使用
		/// </remarks>
		public DbParameter[] GetSpParameterSet(string spName)
		{
			return GetSpParameterSet(spName, false);
		}
		/// <summary>
		/// 获得存储过程的参数集
		/// </summary>
		/// <remarks>
		/// 这个方法从数据库中获得信息，并将之存储在缓存，以便之后的使用
		/// </remarks> 
		/// <param name="connectionString">有效的连接串</param>
		/// <param name="commandText">一个存储过程名</param>
		/// /// <param name="includeReturnValueParameter">是否有返回值参数</param>
		/// <returns>一个参数对象数组</returns>
		public DbParameter[] GetSpParameterSet(string spName, bool includeReturnValueParameter)
		{
			string hashKey = CONNECTIONSTRING + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
			DbParameter[] cachedParameters;
			cachedParameters = (DbParameter[])PARAMCACHE[hashKey];
			if (cachedParameters == null)
			{
				cachedParameters = (DbParameter[])(PARAMCACHE[hashKey] = DiscoverSpParameterSet(spName, includeReturnValueParameter));
			}
			return CloneParameters(cachedParameters);
		}


		/// <summary> 
		/// 在运行时得到一个存储过程的一系列参数信息
		/// </summary>
		/// <param name="spName">存储过程名</param>
		/// <param name="includeReturnValueParameter">是否有返回值参数</param>
		/// <returns>参数对象数组，存储过程的所有参数信息</returns>
		private DbParameter[] DiscoverSpParameterSet(string spName, bool includeReturnValueParameter)
		{
			DbCommand cmd = CreateCommand();
			cmd.CommandText = spName;
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Connection = MYCONNECTION;
			if (TRANSACTION != null)
			{
				cmd.Transaction = TRANSACTION;
			}
			//从 DBCommand 指定的存储过程中检索参数信息，并填充指定的 DBCommand 对象的 Parameters 集。
			DriveParameters(cmd);

			if (!includeReturnValueParameter && cmd.Parameters.Count > 0)
			{
				//移除第一个参数对象，因为没有返回值，而默认情况下，第一个参数对象是返回值
				cmd.Parameters.RemoveAt(0);
			}
			DbParameter[] discoveredParameters = new DbParameter[cmd.Parameters.Count];
			cmd.Parameters.CopyTo(discoveredParameters, 0);
			return discoveredParameters;
		}

		/// <summary>
		/// 参数驱动，push参数到指定的DBCommand
		/// </summary>
		/// <param name="cmd"></param>
		/// <remarks>不同的数据库驱动使用不同的参数指定方法</remarks>
		private void DriveParameters(DbCommand cmd)
		{
			DeriveParameters(MYPROVIDERFACTORY, cmd);
		}


		/// <summary>
		/// 参数提取
		/// </summary>
		/// <param name="providerFactory">数据库驱动</param>
		/// <param name="command">DBCommand</param>
		/// <remarks>根据数据库驱动Factory创建不同的CommandBuilder，和数据库驱动类型分离，运行时构建对象</remarks>
		public static void DeriveParameters(DbProviderFactory providerFactory, IDbCommand command)
		{
			MethodInfo method;
			DbCommandBuilder commandBuilder;
			Type commandType;
			if (providerFactory != null)
			{
				commandBuilder = providerFactory.CreateCommandBuilder();
				commandType = commandBuilder.GetType();
				method = commandType.GetMethod("DeriveParameters");
				if (method != null)
				{
					method.Invoke(null, new object[] { command });
				}
				else
				{
					throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "指定的ProviderFactory不支持存储过程: {0}", providerFactory.GetType().Name));
				}
			}
			else
			{
				throw new ArgumentNullException("providerFactory");
			}
		}
		/// <summary>
		/// 参数克隆
		/// </summary>
		/// <param name="originalParameters">原始参数</param>
		/// <returns>克隆的参数数组</returns>
		private static DbParameter[] CloneParameters(DbParameter[] originalParameters)
		{
			DbParameter[] clonedParameters = new DbParameter[originalParameters.Length];

			for (int i = 0, j = originalParameters.Length; i < j; i++)
			{
				clonedParameters[i] = (DbParameter)((ICloneable)originalParameters[i]).Clone();
			}
			return clonedParameters;
		}

		/// <summary>
		/// 将参数数组添加到缓存中
		/// </summary>
		/// <param name="commandText">一个存储过程名或者T-SQL命令，通常为存储过程</param>
		/// <param name="commandParameters">一个要被缓存的参数对象数组</param>
		public void CacheParameterSet(string connectionString, string commandText, params DbParameter[] commandParameters)
		{
			string hashKey = CONNECTIONSTRING + ":" + commandText;
			PARAMCACHE[hashKey] = commandParameters;
		}

		/// <summary>
		/// 从缓存中获得参数对象数组
		/// </summary>
		/// <param name="commandText">一个存储过程名或者T-SQL命令</param>
		/// <returns>一个参数对象数组</returns>
		public DbParameter[] GetCachedParameterSet(string commandText)
		{
			string hashKey = CONNECTIONSTRING + ":" + commandText;
			DbParameter[] cachedParameters = (DbParameter[])PARAMCACHE[hashKey];
			if (cachedParameters == null)
			{
				return null;
			}
			else
			{
				return CloneParameters(cachedParameters);
			}
		}

		/// <summary>
		/// 获得指定属性的默认值
		/// </summary>
		/// <param name="p">属性对象</param>
		/// <returns>获得的默认值</returns>
		private object GetDefaultValue(PropertyInfo p)
		{
			Type pt = p.PropertyType;
			System.ComponentModel.DefaultValueAttribute dva = (System.ComponentModel.DefaultValueAttribute)Attribute.GetCustomAttribute(p, typeof(System.ComponentModel.DefaultValueAttribute));
			if (dva == null)
			{
				if (pt.Equals(typeof(byte)))
					return (byte)0;
				else if (pt.Equals(typeof(short)))
					return (short)0;
				else if (pt.Equals(typeof(int)))
					return 0;
				else if (pt.Equals(typeof(uint)))
					return (uint)0;
				else if (pt.Equals(typeof(long)))
					return (long)0;
				else if (pt.Equals(typeof(float)))
					return (float)0;
				else if (pt.Equals(typeof(double)))
					return (double)0;
				else if (pt.Equals(typeof(DateTime)))
					return DateTime.MinValue;
				else if (pt.Equals(typeof(char)))
					return char.MinValue;
				else
					return null;
			}
			else
			{
				System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(pt);
				if (dva.Value != null)
				{
					Type t = dva.Value.GetType();
					if (t.Equals(pt) || t.IsSubclassOf(pt))
					{
						return dva.Value;
					}
				}
				if (converter == null)
				{
					return dva.Value;
				}
				else
				{
					return converter.ConvertFrom(dva.Value);
				}
			}
		}
		/// <summary>
		/// 设置属性值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="model"></param>
		/// <param name="propertyInfo"></param>
		/// <param name="value"></param>
		protected void SetPropertyValue<T>(T model, PropertyInfo propertyInfo, object value)
		{
			if (propertyInfo.PropertyType == typeof(string))
			{
				if (value.Equals(DBNull.Value))
				{
					propertyInfo.SetValue(model, "", null);
				}
				else
				{
					propertyInfo.SetValue(model, value.ToString(), null);
				}
			}
			else if (propertyInfo.PropertyType == typeof(int))
			{
				if (value.Equals(DBNull.Value))
				{
					propertyInfo.SetValue(model, 0, null);
				}
				else
				{
					propertyInfo.SetValue(model, int.Parse(value.ToString()), null);
				}
			}
			else if (propertyInfo.PropertyType == typeof(DateTime))
			{
				if (value.Equals(DBNull.Value))
				{
					propertyInfo.SetValue(model, DateTime.MinValue, null);
				}
				else
				{
					propertyInfo.SetValue(model, Convert.ToDateTime(value), null);
				}
			}
			else if (propertyInfo.PropertyType == typeof(decimal))
			{
				if (value.Equals(DBNull.Value))
				{
					propertyInfo.SetValue(model, 0m, null);
				}
				else
				{
					propertyInfo.SetValue(model, Convert.ToDecimal(value), null);
				}
			}
		}

		#endregion

		#region 销毁
		/// <summary>
		/// 释放资源
		/// </summary> 
		public void Dispose()
		{
			if (MYCONNECTION.State == ConnectionState.Open)
			{
				// 如果没有手动提交事务，则在DBHelper释放时回滚事务
				if (InTransaction)
				{
					TRANSACTION.Rollback();
					TRANSACTION.Dispose();
				}
				InTransaction = false;
				if (MYCONNECTION != null)
				{
					MYCONNECTION.Dispose();
				}
				if (MYCONNECTION != null)
				{
					MYCONNECTION.Close();
				}
				if (MYCOMMAND != null)
				{
					MYCOMMAND.Dispose();
				}

			}
		}

		protected virtual void Dispose(bool disposing)
		{
		}
		#endregion
	}

}
