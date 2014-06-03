using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Dynamic;
using Ajax.Common;
using System.Xml;
namespace Ajax.DBUtility
{
	/// <summary>
	/// 数据访问抽象基础类
	/// </summary>
	public abstract class DbHelperSQL
	{
		//数据库连接字符串(web.config来配置)，多数据库可使用DbHelperSQLP来实现.
		public static string connectionString = PubConstant.ConnectionString;
        /// <summary>
        /// 获取记录总数
        /// </summary>
        public static string strGetCountSql = "select count(ID)as item from({0})as tab"; 
		public DbHelperSQL()
		{
		}

		#region 公用方法

		/// <summary>
		/// 取得数据库连接
		/// </summary>
		/// <returns></returns>
		public static SqlConnection GetConncetion()
		{
			SqlConnection con = new SqlConnection(connectionString);
			if (con.State != ConnectionState.Open)
			{
				con.Open();
			}
			return con;
		}
		/// <summary>
		/// 判断是否存在某表的某个字段
		/// </summary>
		/// <param name="tableName">表名称</param>
		/// <param name="columnName">列名称</param>
		/// <returns>是否存在</returns>
		public static bool ColumnExists(string tableName, string columnName)
		{
			string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
			object res = GetSingle(sql);
			if (res == null)
			{
				return false;
			}
			return Convert.ToInt32(res) > 0;
		}
		public static int GetMaxID(string FieldName, string TableName)
		{
			string strsql = "select max(" + FieldName + ")+1 from " + TableName;
			object obj = GetSingle(strsql);
			if (obj == null)
			{
				return 1;
			}
			else
			{
				return int.Parse(obj.ToString());
			}
		}
		public static bool Exists(string strSql)
		{
			object obj = GetSingle(strSql);
			int cmdresult;
			if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
			{
				cmdresult = 0;
			}
			else
			{
				cmdresult = int.Parse(obj.ToString());
			}
			if (cmdresult == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		/// <summary>
		/// 表是否存在
		/// </summary>
		/// <param name="TableName"></param>
		/// <returns></returns>
		public static bool TabExists(string TableName)
		{
			string strsql = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
			//string strsql = "SELECT count(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')";
			object obj = GetSingle(strsql);
			int cmdresult;
			if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
			{
				cmdresult = 0;
			}
			else
			{
				cmdresult = int.Parse(obj.ToString());
			}
			if (cmdresult == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		public static bool Exists(string strSql, params SqlParameter[] cmdParms)
		{
			object obj = GetSingle(strSql, cmdParms);
			int cmdresult;
			if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
			{
				cmdresult = 0;
			}
			else
			{
				cmdresult = int.Parse(obj.ToString());
			}
			if (cmdresult == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		/// <summary>
		/// 获取SQLParameter，参数名和参数值必须对应
		/// </summary>
		/// <param name="paramName">参数名称数组</param>
		/// <param name="p">参数值</param>
		/// <returns>SQLParameter数组</returns>
		public static SqlParameter[] GetSqlParameter(string[] paramName, string[] p)
		{
			if (paramName != null && p != null)
			{
				SqlParameter[] param = new SqlParameter[paramName.Length];
				for (int i = 0; i < paramName.Length; i++)
				{
					SqlParameter temp = new SqlParameter("@" + paramName[i], string.IsNullOrEmpty(p[i]) ? "" : p[i]);
					param[i] = temp;
				}
				return param;
			}
			else
				return null;
		}
		/// <summary>
		/// 获取某个表的所有数据的行数
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public int GetTableDataCount(string tableName)
		{
			string sql = "select count(0) from " + tableName;
			object o = ExecuteScalar(connectionString, CommandType.Text, sql);
			int count = 0;
			if (o != null && o.ToString() != "")
			{
				int.TryParse(o.ToString(), out count);
			}
			return count;
		}
		#endregion

		#region  执行简单SQL语句

		/// <summary>
		/// 执行SQL语句，返回影响的记录数
		/// </summary>
		/// <param name="SQLString">SQL语句</param>
		/// <returns>影响的记录数</returns>
		public static int ExecuteSql(string SQLString)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(SQLString, connection))
				{
					try
					{
						connection.Open();
						int rows = cmd.ExecuteNonQuery();
						return rows;
					}
					catch (System.Data.SqlClient.SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
		}

		public static int ExecuteSqlByTime(string SQLString, int Times)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(SQLString, connection))
				{
					try
					{
						connection.Open();
						cmd.CommandTimeout = Times;
						int rows = cmd.ExecuteNonQuery();
						return rows;
					}
					catch (System.Data.SqlClient.SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
		}

		/// <summary>
		/// 执行Sql和Oracle的混合事务
		/// </summary>
		/// <param name="list">SQL命令行列表</param>
		/// <param name="oracleCmdSqlList">Oracle命令行列表</param>
		/// <returns>执行结果 0-由于SQL造成事务失败 -1 由于Oracle造成事务失败 1-整体事务执行成功</returns>
		public static int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				SqlTransaction tx = conn.BeginTransaction();
				cmd.Transaction = tx;
				try
				{
					foreach (CommandInfo myDE in list)
					{
						string cmdText = myDE.CommandText;
						SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
						PrepareCommand(cmd, conn, tx, cmdText, cmdParms);
						if (myDE.EffentNextType == EffentNextType.SolicitationEvent)
						{
							if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
							{
								tx.Rollback();
								throw new Exception("违背要求" + myDE.CommandText + "必须符合select count(..的格式");
								//return 0;
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
								myDE.OnSolicitationEvent();
							}
						}
						if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
						{
							if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
							{
								tx.Rollback();
								throw new Exception("SQL:违背要求" + myDE.CommandText + "必须符合select count(..的格式");
								//return 0;
							}

							object obj = cmd.ExecuteScalar();
							bool isHave = false;
							if (obj == null && obj == DBNull.Value)
							{
								isHave = false;
							}
							isHave = Convert.ToInt32(obj) > 0;

							if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
							{
								tx.Rollback();
								throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须大于0");
								//return 0;
							}
							if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
							{
								tx.Rollback();
								throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须等于0");
								//return 0;
							}
							continue;
						}
						int val = cmd.ExecuteNonQuery();
						if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
						{
							tx.Rollback();
							throw new Exception("SQL:违背要求" + myDE.CommandText + "必须有影响行");
							//return 0;
						}
						cmd.Parameters.Clear();
					}
					//string oraConnectionString = PubConstant.GetConnectionString("ConnectionStringPPC");
					//bool res = OracleHelper.ExecuteSqlTran(oraConnectionString, oracleCmdSqlList);
					//if (!res)
					//{
					//    tx.Rollback();
					//    throw new Exception("Oracle执行失败");
					//    // return -1;
					//}
					tx.Commit();
					return 1;
				}
				catch (System.Data.SqlClient.SqlException e)
				{
					tx.Rollback();
					throw e;
				}
				catch (Exception e)
				{
					tx.Rollback();
					throw e;
				}
			}
		}
		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="SQLStringList">多条SQL语句</param>		
		public static int ExecuteSqlTran(List<String> SQLStringList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				SqlTransaction tx = conn.BeginTransaction();
				cmd.Transaction = tx;
				try
				{
					int count = 0;
					for (int n = 0; n < SQLStringList.Count; n++)
					{
						string strsql = SQLStringList[n];
						if (strsql.Trim().Length > 1)
						{
							cmd.CommandText = strsql;
							count += cmd.ExecuteNonQuery();
						}
					}
					tx.Commit();
					return count;
				}
				catch
				{
					tx.Rollback();
					return 0;
				}
			}
		}
		/// <summary>
		/// 执行带一个存储过程参数的的SQL语句。
		/// </summary>
		/// <param name="SQLString">SQL语句</param>
		/// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
		/// <returns>影响的记录数</returns>
		public static int ExecuteSql(string SQLString, string content)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(SQLString, connection);
				System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
				myParameter.Value = content;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					int rows = cmd.ExecuteNonQuery();
					return rows;
				}
				catch (System.Data.SqlClient.SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
		}
		/// <summary>
		/// 执行带一个存储过程参数的的SQL语句。
		/// </summary>
		/// <param name="SQLString">SQL语句</param>
		/// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
		/// <returns>影响的记录数</returns>
		public static object ExecuteSqlGet(string SQLString, string content)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(SQLString, connection);
				System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
				myParameter.Value = content;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
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
				catch (System.Data.SqlClient.SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
		}
		/// <summary>
		/// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
		/// </summary>
		/// <param name="strSQL">SQL语句</param>
		/// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
		/// <returns>影响的记录数</returns>
		public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(strSQL, connection);
				System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@fs", SqlDbType.Image);
				myParameter.Value = fs;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					int rows = cmd.ExecuteNonQuery();
					return rows;
				}
				catch (System.Data.SqlClient.SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
		}

		/// <summary>
		/// 执行一条计算查询结果语句，返回查询结果（object）。
		/// </summary>
		/// <param name="SQLString">计算查询结果语句</param>
		/// <returns>查询结果（object）</returns>
		public static object GetSingle(string SQLString)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(SQLString, connection))
				{
					try
					{
						connection.Open();
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
					catch (System.Data.SqlClient.SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
		}
		public static object GetSingle(string SQLString, int Times)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(SQLString, connection))
				{
					try
					{
						connection.Open();
						cmd.CommandTimeout = Times;
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
					catch (System.Data.SqlClient.SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
		}
		/// <summary>
		/// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="strSQL">查询语句</param>
		/// <returns>SqlDataReader</returns>
		public static SqlDataReader ExecuteReader(string strSQL)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand cmd = new SqlCommand(strSQL, connection);
			try
			{
				connection.Open();
				SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				return myReader;
			}
			catch (System.Data.SqlClient.SqlException e)
			{
				throw e;
			}

		}
		/// <summary>
		/// 执行查询语句，返回DataSet
		/// </summary>
		/// <param name="SQLString">查询语句</param>
		/// <returns>DataSet</returns>
		public static DataSet Query(string SQLString)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				DataSet ds = new DataSet();
				try
				{
					connection.Open();
					SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
					command.Fill(ds, "ds");
				}
				catch (System.Data.SqlClient.SqlException ex)
				{
					throw new Exception(ex.Message);
				}
				return ds;
			}
		}
		public static DataSet Query(string SQLString, int Times)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				DataSet ds = new DataSet();
				try
				{
					connection.Open();
					SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
					command.SelectCommand.CommandTimeout = Times;
					command.Fill(ds, "ds");
				}
				catch (System.Data.SqlClient.SqlException ex)
				{
					throw new Exception(ex.Message);
				}
				return ds;
			}
		}

		#endregion

		#region 执行带参数的SQL语句

		/// <summary>
		/// 执行SQL语句，返回影响的记录数
		/// </summary>
		/// <param name="SQLString">SQL语句</param>
		/// <returns>影响的记录数</returns>
		public static int ExecuteSql(string SQLString, params SqlParameter[] cmdParms)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						PrepareCommand(cmd, connection, null, SQLString, cmdParms);
						int rows = cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();
						return rows;
					}
					catch (System.Data.SqlClient.SqlException e)
					{
						throw e;
					}
				}
			}
		}


		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
		public static void ExecuteSqlTran(Hashtable SQLStringList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						//循环
						foreach (DictionaryEntry myDE in SQLStringList)
						{
							string cmdText = myDE.Key.ToString();
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
							PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							cmd.Parameters.Clear();
						}
						trans.Commit();
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
		}
		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
		public static int ExecuteSqlTran(System.Collections.Generic.List<CommandInfo> cmdList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						int count = 0;
						//循环
						foreach (CommandInfo myDE in cmdList)
						{
							string cmdText = myDE.CommandText;
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
							PrepareCommand(cmd, conn, trans, cmdText, cmdParms);

							if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
							{
								if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
								{
									trans.Rollback();
									return 0;
								}

								object obj = cmd.ExecuteScalar();
								bool isHave = false;
								if (obj == null && obj == DBNull.Value)
								{
									isHave = false;
								}
								isHave = Convert.ToInt32(obj) > 0;

								if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
								{
									trans.Rollback();
									return 0;
								}
								if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
								{
									trans.Rollback();
									return 0;
								}
								continue;
							}
							int val = cmd.ExecuteNonQuery();
							count += val;
							if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
							{
								trans.Rollback();
								return 0;
							}
							cmd.Parameters.Clear();
						}
						trans.Commit();
						return count;
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
		}
		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
		public static void ExecuteSqlTranWithIndentity(System.Collections.Generic.List<CommandInfo> SQLStringList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						int indentity = 0;
						//循环
						foreach (CommandInfo myDE in SQLStringList)
						{
							string cmdText = myDE.CommandText;
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.InputOutput)
								{
									q.Value = indentity;
								}
							}
							PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.Output)
								{
									indentity = Convert.ToInt32(q.Value);
								}
							}
							cmd.Parameters.Clear();
						}
						trans.Commit();
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
		}
		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
		public static void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						int indentity = 0;
						//循环
						foreach (DictionaryEntry myDE in SQLStringList)
						{
							string cmdText = myDE.Key.ToString();
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.InputOutput)
								{
									q.Value = indentity;
								}
							}
							PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.Output)
								{
									indentity = Convert.ToInt32(q.Value);
								}
							}
							cmd.Parameters.Clear();
						}
						trans.Commit();
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
		}
		/// <summary>
		/// 执行一条计算查询结果语句，返回查询结果（object）。
		/// </summary>
		/// <param name="SQLString">计算查询结果语句</param>
		/// <returns>查询结果（object）</returns>
		public static object GetSingle(string SQLString, params SqlParameter[] cmdParms)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						PrepareCommand(cmd, connection, null, SQLString, cmdParms);
						object obj = cmd.ExecuteScalar();
						cmd.Parameters.Clear();
						if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
						{
							return null;
						}
						else
						{
							return obj;
						}
					}
					catch (System.Data.SqlClient.SqlException e)
					{
						throw e;
					}
				}
			}
		}

		/// <summary>
		/// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="strSQL">查询语句</param>
		/// <returns>SqlDataReader</returns>
		public static SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] cmdParms)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand cmd = new SqlCommand();
			try
			{
				PrepareCommand(cmd, connection, null, SQLString, cmdParms);
				SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				cmd.Parameters.Clear();
				return myReader;
			}
			catch (System.Data.SqlClient.SqlException e)
			{
				throw e;
			}
			//			finally
			//			{
			//				cmd.Dispose();
			//				connection.Close();
			//			}	

		}

		/// <summary>
		/// 执行查询语句，返回DataSet
		/// </summary>
		/// <param name="SQLString">查询语句</param>
		/// <returns>DataSet</returns>
		public static DataSet Query(string SQLString, params SqlParameter[] cmdParms)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand();
				PrepareCommand(cmd, connection, null, SQLString, cmdParms);
				using (SqlDataAdapter da = new SqlDataAdapter(cmd))
				{
					DataSet ds = new DataSet();
					try
					{
						da.Fill(ds, "ds");
						cmd.Parameters.Clear();
					}
					catch (System.Data.SqlClient.SqlException ex)
					{
						throw new Exception(ex.Message);
					}
					return ds;
				}
			}
		}


		private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
		{
			if (conn.State != ConnectionState.Open)
				conn.Open();
			cmd.Connection = conn;
			cmd.CommandText = cmdText;
			if (trans != null)
				cmd.Transaction = trans;
			cmd.CommandType = CommandType.Text;//cmdType;
			if (cmdParms != null)
			{


				foreach (SqlParameter parameter in cmdParms)
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

		#endregion

		#region 存储过程操作

		/// <summary>
		/// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlDataReader</returns>
		public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			SqlDataReader returnReader;
			connection.Open();
			SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
			command.CommandType = CommandType.StoredProcedure;
			returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
			return returnReader;

		}


		/// <summary>
		/// 执行存储过程
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <param name="tableName">DataSet结果中的表名</param>
		/// <returns>DataSet</returns>
		public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				DataSet dataSet = new DataSet();
				connection.Open();
				SqlDataAdapter sqlDA = new SqlDataAdapter();
				sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
				sqlDA.Fill(dataSet, tableName);
				connection.Close();
				return dataSet;
			}
		}
		public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				DataSet dataSet = new DataSet();
				connection.Open();
				SqlDataAdapter sqlDA = new SqlDataAdapter();
				sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
				sqlDA.SelectCommand.CommandTimeout = Times;
				sqlDA.Fill(dataSet, tableName);
				connection.Close();
				return dataSet;
			}
		}


		/// <summary>
		/// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
		/// </summary>
		/// <param name="connection">数据库连接</param>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlCommand</returns>
		private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand command = new SqlCommand(storedProcName, connection);
			command.CommandType = CommandType.StoredProcedure;
			foreach (SqlParameter parameter in parameters)
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

		/// <summary>
		/// 执行存储过程，返回影响的行数		
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <param name="rowsAffected">影响的行数</param>
		/// <returns></returns>
		public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				int result;
				connection.Open();
				SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
				rowsAffected = command.ExecuteNonQuery();
				result = (int)command.Parameters["ReturnValue"].Value;
				//Connection.Close();
				return result;
			}
		}

		/// <summary>
		/// 创建 SqlCommand 对象实例(用来返回一个整数值)	
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlCommand 对象实例</returns>
		private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
			command.Parameters.Add(new SqlParameter("ReturnValue",
				SqlDbType.Int, 4, ParameterDirection.ReturnValue,
				false, 0, 0, string.Empty, DataRowVersion.Default, null));
			return command;
		}
		#endregion

		#region 动态对象

		/// <summary>
		/// 获取某个SQL里面的所有字段的值的对象数组
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="paramName"></param>
		/// <param name="param"></param>
		/// <returns></returns>
		public static object[] GetModelValue(string sql, string[] paramName, string[] param)
		{
			ArrayList list = new ArrayList();
			using (SqlConnection con = new SqlConnection(connectionString))
			{
				if (con.State == ConnectionState.Closed)
				{
					con.Open();
				}
				SqlCommand cmd = new SqlCommand(sql, con);
				AttachParameters(cmd, GetSqlParameter(paramName, param));
				using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
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
		public static object GetSingelDynaminObject(string sql, string[] paramName, string[] param)
		{
			using (SqlConnection con = new SqlConnection(connectionString))
			{
				Type temp = ClassHelper.BuildType("MyDynamicObject");
				SqlCommand cmd = new SqlCommand(sql, con);
				AttachParameters(cmd, GetSqlParameter(paramName, param));
				object o = ClassHelper.CreateInstance(temp);
				try
				{
					con.Open();
					using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
					{
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
								/// 日期类型处理。如果数据库为date类型，则返回yyyy-MM-dd格式的日期，如果为datetime类型，则返回yyyy-MM-dd hh:mm:ss
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
										dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd hh:mm:ss");
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
				catch (System.Data.SqlClient.SqlException e)
				{
					throw e;
				}
				return o;
			}
		}

		/// <summary>
		/// 获取一个SQL构成的动态对象集合
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="p">可变参数</param>
		/// <returns>动态对象集合</returns>
		public static List<object> GetDynaminObjectList(string sql, string[] paramName, string[] param)
		{
			using (SqlConnection con = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(sql, con);
				AttachParameters(cmd, GetSqlParameter(paramName, param));
				List<object> dynamicList = new List<object>();
				try
				{
					if (con.State == ConnectionState.Closed)
						con.Open();
					using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
					{
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
								/// 日期类型处理。如果数据库为date类型，则返回yyyy-MM-dd格式的日期，如果为datetime类型，则返回yyyy-MM-dd hh:mm:ss
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
										dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd hh:mm:ss");
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
				catch (System.Data.SqlClient.SqlException e)
				{
					return dynamicList;
				}
				return dynamicList;
			}
		}
        /// <summary>
        /// 获取一个SQL构成的动态对象集合
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="paramList">参数数组</param>
        /// <returns>动态对象集合</returns>
        public static List<object> GetDynaminObjectList(string sql, List<SqlParameter> paramList)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                AttachParameters(cmd,paramList.ToArray());
                List<object> dynamicList = new List<object>();
                try
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
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
                                /// 日期类型处理。如果数据库为date类型，则返回yyyy-MM-dd格式的日期，如果为datetime类型，则返回yyyy-MM-dd hh:mm:ss
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
                                        dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd hh:mm:ss");
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
                catch (System.Data.SqlClient.SqlException e)
                {
                    return dynamicList;
                }
                return dynamicList;
            }
        }

        /// <summary>
		/// 获取一个SQL构成的动态对象集合
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="p">可变参数</param>
		/// <returns>动态对象集合</returns>
        public static List<object> GetDynaminObjectList(string sql, int pageIndex, int pageNumber, string orderBy,List<SqlParameter> paramList)
        { 
            List<string> paramName=new List<string>();
            List<string> paramValue=new List<string>();
            if (paramList != null)
            {
                foreach (SqlParameter param in paramList)
                {
                    paramName.Add(param.ParameterName.Replace("@", ""));
                    paramValue.Add(param.Value.ToString());
                }
            }
            return GetDynaminObjectList(sql,pageIndex,pageNumber,orderBy,paramName.ToArray(),paramValue.ToArray());
        }

		/// <summary>
		/// 获取一个SQL构成的动态对象集合
		/// </summary>
		/// <param name="sql">SQL，必须包含ID字段</param>
		/// <param name="pageIndex">当前页索引</param>
		/// <param name="pageNumber">表格显示行数</param>
		/// <param name="p">可变参数</param>
		/// <returns>动态对象集合</returns>
		public static List<object> GetDynaminObjectList(string sql, int pageIndex, int pageNumber, string orderBy, string[] paramName, string[] param)
		{
			///获取分页数据
			string PageSQL = @"select * from 
								(SELECT *,ROW_NUMBER() OVER ( ORDER BY td.ID) AS rownumber FROM ({0}) td) temp
								WHERE rownumber BETWEEN {1} AND {2} {3}";
			PageSQL = string.Format(PageSQL, sql, pageIndex * pageNumber + 1, (pageIndex + 1) * pageNumber, orderBy == null ? "" : "order by " + orderBy);
			using (SqlConnection con = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(PageSQL, con);
				AttachParameters(cmd, GetSqlParameter(paramName, param));
				List<object> dynamicList = new List<object>();
				try
				{
					if (con.State == ConnectionState.Closed)
						con.Open();
					using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
					{
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
								/// 日期类型处理。如果数据库为date类型，则返回yyyy-MM-dd格式的日期，如果为datetime类型，则返回yyyy-MM-dd hh:mm:ss
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
										dic[dr.GetName(j).ToUpper()] = dr.GetDateTime(j).ToString("yyyy-MM-dd hh:mm:ss");
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
				catch (System.Data.SqlClient.SqlException e)
				{
					return dynamicList;
				}
				return dynamicList;
			}
		}
		#endregion


		#region private utility methods & constructors

		/// <summary>
		/// 这个方法用来将命令对象和一组参数对象联系起来
		/// 
		/// This method will assign a value of DbNull to any parameter with a direction of
		/// InputOutput and a value of null.  给输出类型参数对象赋空值
		/// 
		/// This behavior will prevent default values from being used, but
		/// this will be the less common case than an intended pure output parameter (derived as InputOutput)
		/// where the user provided no input value.
		/// </summary>
		/// <param name="command">The command to which the parameters will be added</param>
		/// <param name="commandParameters">an array of SqlParameters tho be added to command</param>
		private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
		{
			if (commandParameters != null)
			{
				foreach (SqlParameter p in commandParameters)
				{
					//check for derived output value with no value assigned
					if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
					{
						p.Value = DBNull.Value;
					}

					command.Parameters.Add(p);
				}
			}
		}

		/// <summary>
		/// 这个方法用来给一组参数对象赋值
		/// </summary>
		/// <param name="commandParameters">array of SqlParameters to be assigned values</param>
		/// <param name="parameterValues">array of objects holding the values to be assigned</param>
		private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
		{
			if ((commandParameters == null) || (parameterValues == null))
			{
				//do nothing if we get no data
				return;
			}

			// we must have the same number of values as we pave parameters to put them in
			if (commandParameters.Length != parameterValues.Length)
			{
				throw new ArgumentException("Parameter count does not match Parameter Value count.");
			}

			//iterate through the SqlParameters, assigning the values from the corresponding position in the 
			//value array
			for (int i = 0, j = commandParameters.Length; i < j; i++)
			{
				commandParameters[i].Value = parameterValues[i];
			}
		}

		/// <summary>
		/// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
		/// to the provided command.
		/// </summary>
		/// <param name="command">the SqlCommand to be prepared</param>
		/// <param name="connection">a valid SqlConnection, on which to execute this command</param>
		/// <param name="transaction">a valid SqlTransaction, or 'null'</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
		private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
		{
			//if the provided connection is not open, we will open it
			if (connection.State != ConnectionState.Open)
			{
				connection.Open();
			}

			//associate the connection with the command
			command.Connection = connection;

			//set the command text (stored procedure name or SQL statement)
			command.CommandText = commandText;

			//if we were provided a transaction, assign it.
			if (transaction != null)
			{
				command.Transaction = transaction;
			}

			//set the command type
			command.CommandType = commandType;

			//attach the command parameters if they are provided
			if (commandParameters != null)
			{
				AttachParameters(command, commandParameters);
			}

			return;
		}


		#endregion private utility methods & constructors

		#region ExecuteNonQuery

		/// <summary>
		/// 执行一个指定连接串上的一个SqlCommand（不返回记录集也没有任何参数）
		/// Execute a SqlCommand (that returns no resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(connectionStringing, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// 执行一个指定连接串上的一个SqlCommand（不返回记录集），使用指定的参数集 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(connectionStringing, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create & open a SqlConnection, and dispose of it after we are done.
			using (SqlConnection cn = new SqlConnection(connectionString))
			{
				cn.Open();

				//call the overload that takes a connection in place of the connection string
				return ExecuteNonQuery(cn, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// 执行一个存储过程并赋值，这个方法将从数据库中获得存储过程的参数对象并根据其顺序赋值
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(connectionStringing, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="spName">the name of the stored prcedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create a command and prepare it for execution
			SqlCommand cmd = new SqlCommand();
			PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);

			//finally, execute the command.
			int retval = cmd.ExecuteNonQuery();

			// detach the SqlParameters from the command object, so they can be used again.
			cmd.Parameters.Clear();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create a command and prepare it for execution
			SqlCommand cmd = new SqlCommand();
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

			//finally, execute the command.
			int retval = cmd.ExecuteNonQuery();

			// detach the SqlParameters from the command object, so they can be used again.
			cmd.Parameters.Clear();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
			}
		}


		#endregion ExecuteNonQuery

		#region ExecuteDataSet

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connectionStringing, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connectionStringing, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create & open a SqlConnection, and dispose of it after we are done.
			using (SqlConnection cn = new SqlConnection(connectionString))
			{
				cn.Open();

				//call the overload that takes a connection in place of the connection string
				return ExecuteDataset(cn, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connectionStringing, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create a command and prepare it for execution
			SqlCommand cmd = new SqlCommand();
			PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);

			//create the DataAdapter & DataSet
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			DataSet ds = new DataSet();

			//fill the DataSet using default values for DataTable names, etc.
			da.Fill(ds);

			// detach the SqlParameters from the command object, so they can be used again.            
			cmd.Parameters.Clear();

			//return the dataset
			return ds;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create a command and prepare it for execution
			SqlCommand cmd = new SqlCommand();
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

			//create the DataAdapter & DataSet
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			DataSet ds = new DataSet();

			//fill the DataSet using default values for DataTable names, etc.
			da.Fill(ds);

			// detach the SqlParameters from the command object, so they can be used again.
			cmd.Parameters.Clear();

			//return the dataset
			return ds;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteDataSet

		#region ExecuteReader

		/// <summary>
		/// this enum is used to indicate whether the connection was provided by the caller, or created by SqlHelper, so that
		/// we can set the appropriate CommandBehavior when calling ExecuteReader()
		/// </summary>
		private enum SqlConnectionOwnership
		{
			/// <summary>Connection is owned and managed by SqlHelper</summary>
			Internal,
			/// <summary>Connection is owned and managed by the caller</summary>
			External
		}

		/// <summary>
		/// Create and prepare a SqlCommand, and call ExecuteReader with the appropriate CommandBehavior.
		/// </summary>
		/// <remarks>
		/// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
		/// 
		/// If the caller provided the connection, we want to leave it to them to manage.
		/// </remarks>
		/// <param name="connection">a valid SqlConnection, on which to execute this command</param>
		/// <param name="transaction">a valid SqlTransaction, or 'null'</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
		/// <param name="connectionOwnership">indicates whether the connection parameter was provided by the caller, or created by SqlHelper</param>
		/// <returns>SqlDataReader containing the results of the command</returns>
		private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
		{
			//create a command and prepare it for execution
			SqlCommand cmd = new SqlCommand();
			PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);

			//create a reader
			SqlDataReader dr;

			// call ExecuteReader with the appropriate CommandBehavior
			if (connectionOwnership == SqlConnectionOwnership.External)
			{
				dr = cmd.ExecuteReader();
			}
			else
			{
				dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
			}

			// detach the SqlParameters from the command object, so they can be used again.
			cmd.Parameters.Clear();

			return dr;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(connectionStringing, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(connectionStringing, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create & open a SqlConnection
			SqlConnection cn = new SqlConnection(connectionString);
			cn.Open();

			try
			{
				//call the private overload that takes an internally owned connection in place of the connection string
				return ExecuteReader(cn, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
			}
			catch
			{
				//if we fail to return the SqlDatReader, we need to close the connection ourselves
				cn.Close();
				throw;
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(connectionStringing, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//pass through the call to the private overload using a null transaction value and an externally owned connection
			return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteReader(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///   SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//pass through to private overload, indicating that the connection is owned by the caller
			return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteReader

		#region ExecuteScalar

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connectionStringing, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connectionStringing, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create & open a SqlConnection, and dispose of it after we are done.
			using (SqlConnection cn = new SqlConnection(connectionString))
			{
				cn.Open();

				//call the overload that takes a connection in place of the connection string
				return ExecuteScalar(cn, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connectionStringing, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create a command and prepare it for execution
			using (SqlCommand cmd = new SqlCommand())
			{
				PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);
				//execute the command & return the results
				object retval = cmd.ExecuteScalar();
				// detach the SqlParameters from the command object, so they can be used again.
				cmd.Parameters.Clear();
				return retval;
			}
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create a command and prepare it for execution
			SqlCommand cmd = new SqlCommand();
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

			//execute the command & return the results
			object retval = cmd.ExecuteScalar();

			// detach the SqlParameters from the command object, so they can be used again.
			cmd.Parameters.Clear();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteScalar

		#region ExecuteXmlReader

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <returns>an XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>an XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create a command and prepare it for execution
			SqlCommand cmd = new SqlCommand();
			PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);

			//create the DataAdapter & DataSet
			XmlReader retval = cmd.ExecuteXmlReader();

			// detach the SqlParameters from the command object, so they can be used again.
			cmd.Parameters.Clear();
			return retval;

		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="spName">the name of the stored procedure using "FOR XML AUTO"</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <returns>an XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			//pass through the call providing null for the set of SqlParameters
			return ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[])null);
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
		/// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
		/// <returns>an XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			//create a command and prepare it for execution
			SqlCommand cmd = new SqlCommand();
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

			//create the DataAdapter & DataSet
			XmlReader retval = cmd.ExecuteXmlReader();

			// detach the SqlParameters from the command object, so they can be used again.
			cmd.Parameters.Clear();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			//if we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

				//assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				//call the overload that takes an array of SqlParameters
				return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			//otherwise we can just call the SP without params
			else
			{
				return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
			}
		}


		#endregion ExecuteXmlReader

		#region GetCount
		/// <summary>
		/// 获得总数
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <param name="commandParameters">SqlParamter参数组</param>
		/// <returns></returns>
		public static int GetCount(string sql, params SqlParameter[] commandParameters)
		{
			int count = 0;
			object o = ExecuteScalar(GetConncetion(), CommandType.Text, sql, commandParameters);
			int.TryParse(o.ToString(),out count);
			return count;
		}
		#endregion

        #region CreateSqlParameter
        /// <summary>
        /// 构建sql参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public static SqlParameter CreateSqlParameter(string paramName, SqlDbType dbType, object value)
        {
            SqlParameter param = new SqlParameter(paramName, dbType);
            param.Value = value;
            return param;
        }
        #endregion
    }
	/// <summary>
	/// SqlHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
	/// ability to discover parameters for stored procedures at run-time.
	/// SqlHelperParameterCache支持函数来实现静态缓存存储过程参数，并支持在运行时得到存储过程的参数
	/// </summary>
	public sealed class SqlHelperParameterCache
	{
		#region private methods, variables, and constructors

		//Since this class provides only static methods, make the default constructor private to prevent 
		//instances from being created with "new SqlHelperParameterCache()".
		//类提供的都是静态方法，将默认构造函数设置为私有的以便阻止利用"new SqlHelperParameterCache()"来实例化类
		private SqlHelperParameterCache() { }

		//存储过程参数缓存导HashTable中
		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// resolve at run time the appropriate set of SqlParameters for a stored procedure
		/// 在运行时得到一个存储过程的一系列参数信息
		/// </summary>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="connectionString">一个连接对象的有效连接串</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="spName">存储过程名</param>
		/// <param name="includeReturnValueParameter">是否有返回值参数</param>
		/// <returns>参数对象数组，存储过程的所有参数信息</returns>
		private static SqlParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			using (SqlConnection cn = new SqlConnection(connectionString))
			using (SqlCommand cmd = new SqlCommand(spName, cn))
			{
				cn.Open();
				cmd.CommandType = CommandType.StoredProcedure;

				//从 SqlCommand 指定的存储过程中检索参数信息，并填充指定的 SqlCommand 对象的 Parameters 集。
				SqlCommandBuilder.DeriveParameters(cmd);

				if (!includeReturnValueParameter)
				{
					//移除第一个参数对象，因为没有返回值，而默认情况下，第一个参数对象是返回值
					cmd.Parameters.RemoveAt(0);
				}

				SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count]; ;

				cmd.Parameters.CopyTo(discoveredParameters, 0);

				return discoveredParameters;
			}
		}

		//deep copy of cached SqlParameter array
		//复制缓存参数数组（克隆）
		private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
		{
			SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

			for (int i = 0, j = originalParameters.Length; i < j; i++)
			{
				clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
			}

			return clonedParameters;
		}

		#endregion private methods, variables, and constructors

		#region caching functions

		/// <summary>
		/// 将参数数组添加到缓存中
		/// </summary>
		/// <param name="connectionString">有效的连接串</param>
		/// <param name="commandText">一个存储过程名或者T-SQL命令</param>
		/// <param name="commandParameters">一个要被缓存的参数对象数组</param>
		public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
		{
			string hashKey = connectionString + ":" + commandText;

			paramCache[hashKey] = commandParameters;
		}

		/// <summary>
		/// 从缓存中获得参数对象数组
		/// </summary>
		/// <param name="connectionString">有效的连接串</param>
		/// <param name="commandText">一个存储过程名或者T-SQL命令</param>
		/// <returns>一个参数对象数组</returns>
		public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			string hashKey = connectionString + ":" + commandText;

			SqlParameter[] cachedParameters = (SqlParameter[])paramCache[hashKey];

			if (cachedParameters == null)
			{
				return null;
			}
			else
			{
				return CloneParameters(cachedParameters);
			}
		}

		#endregion caching functions

		#region Parameter Discovery Functions

		/// <summary>
		/// 获得存储过程的参数集
		/// </summary>
		/// <remarks>
		/// 这个方法从数据库中获得信息，并将之存储在缓存，以便之后的使用
		/// </remarks>
		/// <param name="connectionString">有效的连接串</param>
		/// <param name="commandText">一个存储过程名或者T-SQL命令</param>
		/// <returns>一个参数对象数组</returns>
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return GetSpParameterSet(connectionString, spName, false);
		}

		/// <summary>
		/// 获得存储过程的参数集
		/// </summary>
		/// <remarks>
		/// 这个方法从数据库中获得信息，并将之存储在缓存，以便之后的使用
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">a bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>an array of SqlParameters</returns>
		/// <param name="connectionString">有效的连接串</param>
		/// <param name="commandText">一个存储过程名</param>
		/// /// <param name="includeReturnValueParameter">是否有返回值参数</param>
		/// <returns>一个参数对象数组</returns>
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			string hashKey = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

			SqlParameter[] cachedParameters;

			cachedParameters = (SqlParameter[])paramCache[hashKey];

			if (cachedParameters == null)
			{
				cachedParameters = (SqlParameter[])(paramCache[hashKey] = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter));
			}

			return CloneParameters(cachedParameters);
		}

		#endregion Parameter Discovery Functions

	}

	public class DynamicClass : DynamicObject
	{
		public Dictionary<string, object> dynamicData = new Dictionary<string, object>();
		//获取成员 
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			bool success = false;
			result = null;
			if (dynamicData.ContainsKey(binder.Name))
			{
				result = dynamicData[binder.Name];
				success = true;
			}
			else
			{
				result = "获取失败.";
			}
			return success;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			dynamicData[binder.Name] = value;
			return true;
		}
		//执行委托成员
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			dynamic d = dynamicData[binder.Name];//获取dynamic中func成员
			result = d((DateTime)args[0]);//获取dynamic的自定义的函数的第一个参数
			return result != null;
		}
	}

	//}
}
