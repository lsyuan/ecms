
using System;

namespace MyORM
{
	/// <summary>
	/// 数据表名绑定特性
	/// </summary>
	/// <remarks>
	/// 本特性用于标记某个类型映射到指定名称的数据表上。可以指定数据表名，
	/// 若不指定数据表名则认为类型的名称就是绑定的数据表名。
	/// 本特许只能用于class 类型上面，不能用于其它类型。
	/// </remarks>
	[System.AttributeUsage( System.AttributeTargets.Class , AllowMultiple = false ) ]
	public class BindTableAttribute : System.Attribute
	{
		/// <summary>
		/// 初始化对象
		/// </summary>
		public BindTableAttribute( )
		{
		}

		/// <summary>
		/// 初始化对象
		/// </summary>
		/// <param name="name">数据表名</param>
		public BindTableAttribute( string name )
		{
			strName = name ;
		}

		private string strName = null;
		/// <summary>
		/// 数据表名
		/// </summary>
		public string Name
		{
			get
			{
				return strName ;
			}
		}
	}//public class BindTableAttribute : System.Attribute

	/// <summary>
	/// 数据字段名绑定信息
	/// </summary>
	/// <remarks>
	/// 本特性用于标记某个属性映射到指定名称的字段上，可以指定字段名，
	/// 若不指定则认为属性的名称就是影射的字段名。
	/// 本特性只能用于公开属性上面，不能用于其它类型成员或类型声明上面。
	/// </remarks>
	[System.AttributeUsage( System.AttributeTargets.Property , AllowMultiple = false ) ]
	public class BindFieldAttribute : System.Attribute
	{
		/// <summary>
		/// 初始化对象
		/// </summary>
		public BindFieldAttribute( )
		{
		}

		/// <summary>
		/// 初始化对象
		/// </summary>
		/// <param name="name">字段名</param>
		public BindFieldAttribute( string name )
		{
			strName = name ;
		}

		private string strName = null;
		/// <summary>
		/// 数据字段名
		/// </summary>
		public string Name
		{
			get
			{
				return strName ;
			}
		}

		private bool bolKey = false;
		/// <summary>
		/// 该字段为关键字段，可用作查询条件
		/// </summary>
		/// <remarks>
		/// 框架程序在修改和删除对象时，对象类型必须至少有一个属性标记为Key = true .
		/// </remarks>
		public bool Key
		{
			get
			{
				return bolKey ;
			}
			set
			{
				bolKey = value;
			}
		}

		private string strReadFormat = null;
		/// <summary>
		/// 数据读取格式化字符串
		/// </summary>
		/// <remarks>
		/// 本属性用于指定数据库中的数据存储格式，比如数据库中保存的是类似"20080603"
		/// 这样的表示日期数据的格式为"yyyyMMdd"的字符串。此时设置对象属性的ReadFormat
		/// 格式为"yyyyMMdd",并且对象属性的数据类型为DateTime类型。则框架读取数据时会
		/// 尝试使用"yyyyMMdd"格式解析读取的原始数据为一个日期数据并设置到对象属性值。
		/// </remarks>
		public string ReadFormat
		{
			get
			{
				return strReadFormat ;
			}
			set
			{
				strReadFormat = value ;
			}
		}

		private string strWriteFormat = null;
		/// <summary>
		/// 数据存储格式化字符串
		/// </summary>
		/// <remarks>
		/// 本属性用于指定对象属性值保存到数据库中的格式，比如对象属性数据类型为DateTime，
		/// 而数据库中保存的是类似"20080603"这样的表示日期数据的格式为"yyyyMMdd"的字符串。
		/// 此时设置对象属性的WriteFormat格式为"yyyyMMdd",则框架保存数据时会尝试使用
		/// "yyyyMMdd"格式将对象属性值进行格式化，将转化结果保存到数据库中。
		/// </remarks>
		public string WriteFormat
		{
			get
			{
				return strWriteFormat ;
			}
			set
			{
				strWriteFormat = value;
			}
		}
	}//public class BindFieldAttribute : System.Attribute

	/// <summary>
	/// 我的对象-数据库映射关系处理框架
	/// </summary>
	/// <remarks>
	/// 本对象为一个轻量级的对象-数据库映射关系处理框架，能根据编程对象向
	/// 数据库进行简单的查询，新增，修改和删除数据库记录。本框架处理的数据库
	/// 映射类型都必须使用 BindTableAttribute 和 BindFieldAttribute 进行标记。
	/// </remarks>
	public class MyORMFramework : System.IDisposable
	{
		/// <summary>
		/// 初始化对象
		/// </summary>
		public MyORMFramework()
		{
		}

		/// <summary>
		/// 初始化对象
		/// </summary>
		/// <param name="conn">数据库连接对象，该对象必须已经打开</param>
		public MyORMFramework( System.Data.IDbConnection conn )
		{
			myConnection = conn ;
		}
		
		private System.Data.IDbConnection myConnection = null;
		/// <summary>
		/// 对象使用的数据库连接对象，该对象必须已经打开。
		/// </summary>
		public System.Data.IDbConnection Connection
		{
			get
			{
				return myConnection ;
			}
			set
			{
				myConnection = value;
			}
		}
	
		/// <summary>
		/// 判断数据库中是否存在指定的对象
		/// </summary>
		/// <param name="obj">对象</param>
		/// <returns>true:数据库中存在指定关键字的记录 false:数据库中不存在指定关键字的记录</returns>
		public bool Contains( object obj )
		{
			if( obj == null )
			{
				throw new ArgumentNullException("obj");
			}
			this.CheckBindInfo( obj.GetType() , true );
			this.CheckConnetion();
			TableBindInfo table = this.GetBindInfo( obj.GetType());
			System.Collections.ArrayList values = new System.Collections.ArrayList();
			string strSQL = this.BuildCondition( obj , values );
			bool result = false;
			if( strSQL != null )
			{
				using( System.Data.IDbCommand cmd = myConnection.CreateCommand())
				{
					strSQL = "Select 1 from " + FixTableName( table.TableName ) + " Where " + strSQL ;
					cmd.CommandText = strSQL ;
					foreach( object v in values )
					{
						System.Data.IDbDataParameter p = cmd.CreateParameter();
						p.Value = v ;
						cmd.Parameters.Add( p );
					}
					object v2 = cmd.ExecuteScalar();
					if( v2 != null && DBNull.Value.Equals( v2 ) == false )
					{
						result = ( Convert.ToInt32( v2 ) == 1 );
					}
				}
			}
			return false;
		}

		#region 更新数据库记录的接口 ******************************************

		/// <summary>
		/// 更新一个对象
		/// </summary>
		/// <param name="obj">要更新的对象</param>
		/// <returns>更新修改的数据库记录个数</returns>
		public int UpdateObject( object obj )
		{
			if( obj == null )
			{
				throw new ArgumentNullException("obj");
			}
			this.CheckBindInfo( obj.GetType() , true );
			this.CheckConnetion();
			return UpdateObjects( new object[]{ obj } );
		}

		/// <summary>
		/// 更新多个对象
		/// </summary>
		/// <param name="Objects">对象列表</param>
		/// <returns>更新修改的数据库记录个数</returns>
		public int UpdateObjects( System.Collections.IEnumerable Objects )
		{
			if( Objects == null )
			{
				throw new ArgumentNullException("Objects");
			}
			this.CheckBindInfo( Objects , true );
			this.CheckConnetion();
			int RecordCount = 0 ;
			using( System.Data.IDbCommand cmd = myConnection.CreateCommand())
			{
				foreach( object obj in Objects )
				{
					TableBindInfo table = this.GetBindInfo( obj.GetType());
					// 拼凑生成SQL更新语句
					System.Collections.ArrayList values = new System.Collections.ArrayList();
					System.Text.StringBuilder myStr = new System.Text.StringBuilder();
					foreach( FieldBindInfo field in table.Fields )
					{
						object v = field.Property.GetValue( obj , null );
						if( myStr.Length > 0 )
						{
							myStr.Append(" , " + System.Environment.NewLine );
						}
						myStr.Append( FixFieldName( field.FieldName ) + " = ? " );
						values.Add( field.ToDataBase( v ));
					}
					myStr.Insert( 0 , "Update " + FixTableName( table.TableName ) + " Set " );
					string strSQL = BuildCondition( obj , values );
					myStr.Append( " Where " + strSQL );
					strSQL = myStr.ToString();
					// 设置SQL命令对象，填充参数
					cmd.Parameters.Clear();
					cmd.CommandText = strSQL ;
					foreach( object v in values )
					{
						System.Data.IDbDataParameter p = cmd.CreateParameter();
						cmd.Parameters.Add( p );
						p.Value = v ;
					}

					RecordCount += cmd.ExecuteNonQuery();
				}//foreach
			}//using
			return RecordCount ;
		}

		#endregion 

		#region 删除数据库记录的接口 ******************************************

		/// <summary>
		/// 删除一个对象记录
		/// </summary>
		/// <param name="obj">要删除的对象</param>
		/// <returns>删除的数据库记录个数</returns>
		public int DeleteObject( object obj )
		{
			if( obj == null )
			{
				throw new ArgumentNullException("obj");
			}
			this.CheckBindInfo( obj.GetType() , true );
			return DeleteObjects( new object[]{ obj } );
		}

		/// <summary>
		/// 删除若干条对象的数据
		/// </summary>
		/// <param name="Objects">对象列表</param>
		/// <returns>删除的记录个数</returns>
		public int DeleteObjects( System.Collections.IEnumerable Objects )
		{
			if( Objects == null )
			{
				throw new ArgumentNullException("Objects");
			}
			this.CheckBindInfo( Objects , true );
			this.CheckConnetion();
			int RecordCount = 0 ;
			using( System.Data.IDbCommand cmd = myConnection.CreateCommand())
			{
				foreach( object obj in Objects )
				{
					TableBindInfo table = this.GetBindInfo( obj.GetType() );
					// 拼凑SQL语句
					System.Collections.ArrayList values = new System.Collections.ArrayList();
					string strSQL = BuildCondition( obj , values );
					strSQL = "Delete From " + FixTableName( table.TableName ) + " Where " + strSQL ;

					// 设置SQL命令对象
					cmd.Parameters.Clear();
					cmd.CommandText = strSQL ;
					foreach( object v in values )
					{
						System.Data.IDbDataParameter p = cmd.CreateParameter();
						p.Value = v ;
						cmd.Parameters.Add( p );
					}
					// 执行SQL，删除记录
					RecordCount += cmd.ExecuteNonQuery();
				}
			}
			return RecordCount ;
		}

		#endregion

		#region 查询数据库记录的接口 ******************************************

		/// <summary>
		/// 读取指定类型的所有的对象
		/// </summary>
		/// <param name="ObjectType">记录对象类型</param>
		/// <returns>读取的记录对象数组</returns>
		public object[] ReadAllObjects( Type ObjectType )
		{
			if( ObjectType == null )
			{
				throw new ArgumentNullException("ObjectType");
			}
			this.CheckBindInfo( ObjectType , false );
			TableBindInfo table = this.GetBindInfo( ObjectType );
			string strSQL = "Select " + table.FieldNameList + " From " + FixTableName( table.TableName );
			return ReadObjects( strSQL , ObjectType , 0 );
		}

		/// <summary>
		/// 使用指定的SQL查询语句查询数据并读取一条数据库记录对象
		/// </summary>
		/// <param name="strSQL">SQL查询语句</param>
		/// <param name="ObjectType">要读取的对象类型</param>
		/// <returns>读取的对象</returns>
		public object ReadObject( string strSQL , Type ObjectType )
		{
			// 检查参数
			if( strSQL == null )
			{
				throw new ArgumentNullException("strSQL");
			}
			if( ObjectType == null )
			{
				throw new ArgumentNullException("ObjectType");
			}
			this.CheckBindInfo( ObjectType , false );
			object[] objs = ReadObjects( strSQL , ObjectType , 1 );
			if( objs != null && objs.Length == 1 )
				return objs[ 0 ] ;
			else
				return null;
		}

		/// <summary>
		/// 使用指定的SQL查询语句查询数据库并读取多条数据库记录对象
		/// </summary>
		/// <param name="strSQL">SQL查询语句</param>
		/// <param name="ObjectType">要读取的对象类型</param>
		/// <returns>读取的对象组成的数组</returns>
		public object[] ReadObjects( string strSQL , Type ObjectType)
		{
			if( ObjectType == null )
			{
				throw new ArgumentNullException("ObjectType");
			}
			// 检查数据库映射信息
			this.CheckBindInfo( ObjectType , false );
			return ReadObjects( strSQL , ObjectType , 0 );
		}

		/// <summary>
		/// 使用指定的SQL查询语句查询数据库并读取多条数据库记录对象
		/// </summary>
		/// <param name="strSQL">SQL查询语句</param>
		/// <param name="ObjectType">要读取的对象类型</param>
		/// <param name="MaxObjectCount">最多读取的对象个数</param>
		/// <returns>读取的对象组成的数组</returns>
		public object[] ReadObjects( string strSQL , Type ObjectType , int MaxObjectCount )
		{
			// 检查参数
			if( strSQL == null )
			{
				throw new ArgumentNullException("strSQL");
			}
			if( ObjectType == null )
			{
				throw new ArgumentNullException("ObjectType");
			}
			// 检查数据库映射信息
			this.CheckBindInfo( ObjectType , false );
			// 检查数据库连接
			this.CheckConnetion();
			// 创建SQL命令对象
			using( System.Data.IDbCommand cmd = myConnection.CreateCommand())
			{
				// 执行SQL查询,获得一个数据读取器
				cmd.CommandText = strSQL ;
				System.Data.IDataReader reader = cmd.ExecuteReader(
					MaxObjectCount == 1 ?
					System.Data.CommandBehavior.SingleRow : 
					System.Data.CommandBehavior.SingleResult );

				System.Collections.ArrayList list = new System.Collections.ArrayList();
				TableBindInfo table = this.GetBindInfo( ObjectType );
				lock( table )
				{
					// 设置字段序号，提高性能
					foreach( FieldBindInfo field in table.Fields )
					{
						field.FieldIndex = - 1 ;
					}
					for( int iCount = 0 ; iCount < reader.FieldCount ; iCount ++ )
					{
						string name = reader.GetName( iCount );
						foreach( FieldBindInfo field in table.Fields )
						{
							if( EqualsFieldName( name , field.FieldName ))
							{
								field.FieldIndex = iCount ;
							}
						}
					}
					while( reader.Read())
					{
						// 根据对象类型创建对象实例
						object obj = System.Activator.CreateInstance( ObjectType );
						// 读取对象属性值
						if( InnerReadValues( obj , table , reader ) > 0 )
						{
							list.Add( obj );
						}
						if( MaxObjectCount > 0 || list.Count == MaxObjectCount )
						{
							break;
						}
					}//while
				}//lock
				reader.Close();
				// 返回读取的对象数组
				return list.ToArray();
			}//using
		}

		#endregion

		#region  插入数据库记录的接口 *****************************************

		/// <summary>
		/// 将一个对象插入到数据库中
		/// </summary>
		/// <param name="obj">对象</param>
		/// <returns>插入的数据库记录个数</returns>
		public int InsertObject( object obj )
		{
			if( obj == null )
			{
				throw new ArgumentNullException("obj");
			}
			this.CheckBindInfo( obj.GetType() ,false );
			return InsertObject( obj , null );
		}

		/// <summary>
		/// 将一个对象插入到指定的数据表中
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="TableName">指定的数据表，若未指定则为默认数据表名</param>
		/// <returns>插入的数据库记录的个数</returns>
		public int InsertObject( object obj , string TableName )
		{
			if( obj == null )
			{
				throw new ArgumentNullException("obj");
			}
			this.CheckBindInfo( obj.GetType() , false );
			return InsertObjects( new object[]{ obj } , TableName );
		}

		/// <summary>
		/// 将若干个对象插入到数据库中
		/// </summary>
		/// <param name="Objects">对象列表</param>
		/// <param name="TableName">制定的数据表，若未指定则使用默认的数据表名</param>
		/// <returns>插入的数据库记录的个数</returns>
		public int InsertObjects( System.Collections.IEnumerable Objects , string TableName )
		{
			if( Objects == null )
			{
				throw new ArgumentNullException("Objects");
			}
			this.CheckBindInfo( Objects , false );
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			foreach( object obj in Objects )
			{
				list.Add( obj );
			}
			if( list.Count == 0 )
			{
				return 0 ;
			}
			this.CheckConnetion();
			// 上一次执行的SQL语句
			string strLastSQL = null ;
			int InsertCount = 0 ;
			using( System.Data.IDbCommand cmd = myConnection.CreateCommand())
			{
				foreach( object obj in list )
				{
					TableBindInfo table = this.GetBindInfo( obj.GetType());
					string TableName2 = TableName ;
					if( TableName2 == null || TableName.Trim().Length == 0 )
					{
						TableName2 = table.TableName ;
					}
					System.Collections.ArrayList values = new System.Collections.ArrayList();
					// 拼凑SQL语句
					System.Text.StringBuilder myStr = new System.Text.StringBuilder();
					System.Text.StringBuilder myFields = new System.Text.StringBuilder();
					foreach( FieldBindInfo field in table.Fields )
					{
						if( field.Property.CanRead == false )
						{
							throw new Exception("属性 " + field.Property.Name + " 是不可写的");
						}
						object v = field.Property.GetValue( obj , null );
						if( v == null || DBNull.Value.Equals( v ))
						{
							continue ;
						}
						values.Add( field.ToDataBase( v ));
						if( myStr.Length > 0 )
						{
							myStr.Append(" , ");
							myFields.Append( " , " );
						}

						myStr.Append(" ? " );
						myFields.Append( FixFieldName( field.FieldName ));

					}//foreach
					myStr.Insert( 0 , "Insert Into " + FixTableName( TableName2 ) 
						+  " ( " + myFields.ToString() + " ) Values ( " );
					myStr.Append( " ) " );
					string strSQL = myStr.ToString();

					if( strSQL != strLastSQL )
					{
						// 重新设置SQL命令对象
						strLastSQL = strSQL ;
						cmd.Parameters.Clear();
						cmd.CommandText = strSQL ;
						for( int iCount = 0 ; iCount < values.Count ; iCount ++ )
						{
							cmd.Parameters.Add( cmd.CreateParameter());
						}
					}

					// 填充SQL命令参数值
					for( int iCount = 0 ; iCount < values.Count ; iCount ++ )
					{
						( ( System.Data.IDbDataParameter ) cmd.Parameters[ iCount ]).Value = values[ iCount ] ;
					}

					// 执行SQL命令向数据表新增记录
					InsertCount += cmd.ExecuteNonQuery();
				}//foreach
			}//using
			return InsertCount ;
		}

		#endregion

		/// <summary>
		/// 修正数据表名
		/// </summary>
		/// <param name="TableName">旧的数据表名</param>
		/// <returns>修正后的数据表名</returns>
		/// <remarks>
		/// 在某些情况下，程序指定的数据表名是不合法的，需要对其进行修正。比如对于Access则在表的两边加上方括号。
		/// </remarks>
		protected virtual string FixTableName( string TableName )
		{
			return TableName ;
		}

		/// <summary>
		/// 修正数据字段名
		/// </summary>
		/// <param name="FieldName">旧的数据字段名</param>
		/// <returns>修正后的字段名</returns>
		/// <remarks>
		/// 在某些情况下，程序指定的字段名不合法，需要对其进行修正，比如对于Access则在表的两边加上方括号。
		/// </remarks>
		protected virtual string FixFieldName( string FieldName )
		{
			return FieldName ;
		}
		
		/// <summary>
		/// 销毁对象
		/// </summary>
		public void Dispose()
		{
			if( myConnection != null )
			{
				if( myConnection.State == System.Data.ConnectionState.Open )
				{
					myConnection.Close();
				}
				myConnection = null ;
			}
		}

		#region 内部私有成员 **************************************************

		/// <summary>
		/// 检查数据库连接是否可用
		/// </summary>
		private void CheckConnetion()
		{
			if( myConnection == null || myConnection.State != System.Data.ConnectionState.Open )
			{
				throw new InvalidOperationException("数据库连接无效");
			}
		}
		
		/// <summary>
		/// 根据对象数值创建查询条件子SQL语句
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="values">SQL参数值列表</param>
		/// <returns>创建的SQL语句字符串</returns>
		private string BuildCondition( object obj , System.Collections.ArrayList values )
		{
			TableBindInfo table = this.GetBindInfo( obj.GetType() );
			// 拼凑查询条件SQL语句
			System.Text.StringBuilder mySQL = new System.Text.StringBuilder();
			foreach( FieldBindInfo field in table.Fields )
			{
				if( field.Attribute.Key )
				{
					object v = field.Property.GetValue( obj , null );
					if( v == null || DBNull.Value.Equals( v ))
					{
						throw new Exception("关键字段属性 " + field.Property.Name + " 未指定值" ) ;
					}
					if( mySQL.Length > 0 )
					{
						mySQL.Append(" And " );
					}
					mySQL.Append( FixFieldName( field.FieldName ));
					mySQL.Append( " = ? " );
					values.Add( field.ToDataBase( v ));
				}
			}//foreach
			if( mySQL.Length == 0 )
			{
				throw new Exception("类型 " + obj.GetType().FullName + " 未能生成查询条件");
			}
			return mySQL.ToString();
		}

		/// <summary>
		/// 从数据读取器中读取数据并填充到一个对象中
		/// </summary>
		/// <param name="ObjInstance">对象实例</param>
		/// <param name="info">数据库绑定信息对象</param>
		/// <param name="reader">数据读取器</param>
		private int InnerReadValues( object ObjInstance , TableBindInfo info , System.Data.IDataReader reader )
		{
			// 检查参数
			if( ObjInstance == null )
			{
				throw new ArgumentNullException("ObjectInstance");
			}
			if( info == null )
			{
				throw new ArgumentNullException("info");
			}
			if( reader == null )
			{
				throw new ArgumentNullException("reader");
			}
			int FieldCount = 0 ;
			// 依次读取各个属性值
			foreach( FieldBindInfo field in info.Fields )
			{
				// 绑定的属性是只读的
				if( field.Property.CanWrite == false )
				{
					continue ;
				}
				// 没有找到绑定的字段
				if( field.FieldIndex < 0 )
				{
					continue ;
				}
				// 读取数据字段值
				object v = reader.GetValue( field.FieldIndex );
				v = field.FromDataBase( v );
				// 设置对象属性值
				field.Property.SetValue( ObjInstance , v , null );
				FieldCount ++ ;
			}//foreach
			return FieldCount ;
		}

		/// <summary>
		/// 检查若干个对象的数据库绑定状态，若检查不通过则抛出异常。
		/// </summary>
		/// <param name="Objects">对象列表</param>
		/// <param name="CheckKey">是否检查定义了关键字段属性</param>
		private void CheckBindInfo( System.Collections.IEnumerable Objects , bool CheckKey )
		{
			Type LastType = null ;
			foreach( object obj in Objects )
			{
				if( obj == null )
				{
					throw new Exception("对象集合中出现空引用");
				}
				Type t = obj.GetType();
				if( t.Equals( LastType ) == false )
				{
					LastType = t ;
					CheckBindInfo( t , CheckKey );
				}
			}
		}

		/// <summary>
		/// 检查一个对象类型的数据库绑定状态，若检查不通过则抛出异常。
		/// </summary>
		/// <param name="t">对象类型</param>
		/// <param name="CheckKey">是否检查定义了关键字段属性</param>
		private void CheckBindInfo( Type t , bool CheckKey )
		{
			TableBindInfo table = this.GetBindInfo( t );
			if( table == null )
			{
				throw new Exception("类型 " + t.FullName + " 未映射到数据库");
			}
			if( CheckKey )
			{
				foreach( FieldBindInfo field in table.Fields )
				{
					if( field.Attribute.Key )
					{
						return ;
					}
				}
				throw new Exception("类型 " + t.FullName + " 未定义关键字段");
			}
		}

		/// <summary>
		/// 在内部缓存的映射信息列表，此处为了提高速度。
		/// </summary>
		private static System.Collections.Hashtable myBufferedInfos = new System.Collections.Hashtable();

		/// <summary>
		/// 获得指定类型的数据表映射信息对象
		/// </summary>
		/// <param name="ObjectType">对象类型</param>
		/// <returns>获得的映射信息对象</returns>
		/// <remarks>
		/// 本函数内部使用了 myBufferedInfos 来缓存信息，提高性能。
		/// </remarks>
		private TableBindInfo GetBindInfo( Type ObjectType )
		{
			if( ObjectType == null )
			{
				throw new ArgumentNullException("OjbectType");
			}
			// 查找已缓存的映射信息对象
			TableBindInfo info = ( TableBindInfo ) myBufferedInfos[ ObjectType ] ;
			if( info != null )
			{
				return info ;
			}

			// 若未找到则创建新的映射信息对象
			BindTableAttribute ta = ( BindTableAttribute ) System.Attribute.GetCustomAttribute( 
				ObjectType , typeof( BindTableAttribute ));
			if( ta == null )
			{
				return null;
			}

			TableBindInfo NewInfo = new TableBindInfo();
			NewInfo.ObjectType = ObjectType ;
			NewInfo.Attribute = ta ;
			NewInfo.TableName = ta.Name ;
			if( NewInfo.TableName == null || NewInfo.TableName.Trim().Length == 0 )
			{
				// 若在特性中没有指明绑定的表名则使用默认的对象类型名称
				NewInfo.TableName = ObjectType.Name ;
			}
			System.Text.StringBuilder myFieldList = new System.Text.StringBuilder();
			System.Collections.ArrayList fields = new System.Collections.ArrayList();
			// 遍历所有的公开的实例属性来获得字段绑定信息
			foreach( System.Reflection.PropertyInfo p in ObjectType.GetProperties(
				System.Reflection.BindingFlags.Instance 
				| System.Reflection.BindingFlags.Public
				))
			{
				BindFieldAttribute fa = ( BindFieldAttribute ) Attribute.GetCustomAttribute(
					p , typeof( BindFieldAttribute ));
				if( fa != null )
				{
					FieldBindInfo NewFieldInfo = new FieldBindInfo();
					NewFieldInfo.Attribute = fa ;
					NewFieldInfo.FieldName = fa.Name ;
					if( NewFieldInfo.FieldName == null || NewFieldInfo.FieldName.Trim().Length == 0 )
					{
						// 若在特性中没有指明绑定的字段名则使用默认的属性名称
						NewFieldInfo.FieldName = p.Name ;
					}
					if( myFieldList.Length > 0 )
					{
						myFieldList.Append(",");
					}
					myFieldList.Append( FixFieldName( NewFieldInfo.FieldName ) ) ;
					NewFieldInfo.Property = p ;
					NewFieldInfo.ValueType = p.PropertyType ;
					NewFieldInfo.DefaultValue = GetDefaultValue( p );
					fields.Add( NewFieldInfo );
				}
			}
			NewInfo.Fields = ( FieldBindInfo[]) fields.ToArray( typeof( FieldBindInfo ));
			NewInfo.FieldNameList = myFieldList.ToString();
			// 缓存绑定信息对象
			myBufferedInfos[ ObjectType ] = NewInfo ;
			return NewInfo ;
		}
 

		/// <summary>
		/// 判断两个字段名是否等价
		/// </summary>
		/// <param name="name1">字段名1</param>
		/// <param name="name2">字段名2</param>
		/// <returns>true:两个字段名等价 false:字段名不相同</returns>
		private bool EqualsFieldName( string name1 , string name2 )
		{
			if( name1 == null || name2 == null )
			{
				throw new ArgumentNullException("name1 or name2");
			}
			name1 = name1.Trim();
			name2 = name2.Trim();
			// 进行不区分大小写的比较
			if( string.Compare( name1 , name2 , true ) == 0 )
			{
				return true ;
			}
			int index = name1.IndexOf(".");
			if( index > 0 )
			{
				name1 = name1.Substring( index + 1 ).Trim();
			}
			index = name2.IndexOf(".");
			if( index > 0 )
			{
				name2 = name2.Substring( index + 1 ).Trim();
			}
			return string.Compare( name1 , name2 , true ) == 0 ;
		}

		/// <summary>
		/// 获得指定属性的默认值
		/// </summary>
		/// <param name="p">属性对象</param>
		/// <returns>获得的默认值</returns>
		private object GetDefaultValue( System.Reflection.PropertyInfo p )
		{
			Type pt = p.PropertyType ;
			System.ComponentModel.DefaultValueAttribute dva =
				( System.ComponentModel.DefaultValueAttribute )
				Attribute.GetCustomAttribute(
				p , typeof( System.ComponentModel.DefaultValueAttribute ));
			if( dva == null )
			{
				if( pt.Equals( typeof( byte )))
					return ( byte ) 0 ;
				else if( pt.Equals( typeof( short )))
					return ( short ) 0 ;
				else if( pt.Equals( typeof( int )))
					return ( int ) 0 ;
				else if( pt.Equals( typeof( uint )))
					return ( uint ) 0 ;
				else if( pt.Equals( typeof( long )))
					return ( long ) 0 ;
				else if( pt.Equals( typeof( float )))
					return ( float ) 0 ;
				else if( pt.Equals( typeof( double )))
					return ( double ) 0 ;
				else if( pt.Equals( typeof( DateTime )))
					return DateTime.MinValue ;
				else if( pt.Equals( typeof( char )))
					return char.MinValue ;
				else
					return null;
			}
			else
			{
				System.ComponentModel.TypeConverter converter =
					System.ComponentModel.TypeDescriptor.GetConverter( pt );
				if( dva.Value != null )
				{
					Type t = dva.Value.GetType();
					if( t.Equals( pt ) || t.IsSubclassOf( pt ))
					{
						return dva.Value ;
					}
				}
				if( converter == null )
				{
					return dva.Value ;
				}
				else
				{
					return converter.ConvertFrom( dva.Value );
				}
			}
		}

		/// <summary>
		/// 数据表绑定信息对象
		/// </summary>
		private class TableBindInfo
		{
			/// <summary>
			/// 数据库表名
			/// </summary>
			public string TableName = null;
			/// <summary>
			/// 对象类型
			/// </summary>
			public Type ObjectType = null;
			/// <summary>
			/// 绑定信息对象
			/// </summary>
			public BindTableAttribute Attribute = null;
			/// <summary>
			/// 绑定的字段信息对象
			/// </summary>
			public FieldBindInfo[] Fields = null;
			/// <summary>
			/// 绑定的字段列表，格式为"字段1,字段2,字段3"
			/// </summary>
			public string FieldNameList = null;
		}

		/// <summary>
		/// 数据字段绑定信息对象
		/// </summary>
		private class FieldBindInfo
		{
			/// <summary>
			/// 绑定的字段名
			/// </summary>
			public string FieldName = null;
			/// <summary>
			/// 绑定的字段序号
			/// </summary>
			public int FieldIndex = - 1;
			/// <summary>
			/// 对象属性信息
			/// </summary>
			public System.Reflection.PropertyInfo Property = null;
			/// <summary>
			/// 数据类型
			/// </summary>
			public Type ValueType = null;
			/// <summary>
			/// 默认值
			/// </summary>
			public object DefaultValue = null;
			/// <summary>
			/// 绑定信息对象
			/// </summary>
			public BindFieldAttribute Attribute = null;

			/// <summary>
			/// 将对象数据转换为数据库中的数据
			/// </summary>
			/// <param name="v">对象数据</param>
			/// <returns>数据库数据</returns>
			public object ToDataBase( object v )
			{
				if( v == null || DBNull.Value.Equals( v ))
					return DBNull.Value ;
				string Format = Attribute.WriteFormat ;
				if( Format != null && Format.Trim().Length > 0 )
				{
					if( v is System.IFormattable )
					{
						v = ( ( System.IFormattable ) v ).ToString( Format , null );
					}
				}
				return v ;
			}

			/// <summary>
			/// 将从数据库中获得的数据转换为对象数据
			/// </summary>
			/// <param name="v">从数据库获得的原始数据</param>
			/// <returns>转化后的对象数据</returns>
			public object FromDataBase( object v )
			{
				// 若数据为空则返回默认值
				if( v == null || DBNull.Value.Equals( v ))
					return DefaultValue ;

				// 进行格式化解析
				string Format = Attribute.ReadFormat ;
				if( Format != null && Format.Trim().Length > 0 )
				{
					string Value = Convert.ToString( v );
					if( ValueType.Equals( typeof( DateTime )))
					{
						if( Format == null )
							return DateTime.Parse( Value );
						else
							return DateTime.ParseExact( Value , Format , null );
					}
					else if( ValueType.Equals( typeof(byte )))
					{
						return byte.Parse( Value );
					}
					else if( ValueType.Equals( typeof( short )))
					{
						return short.Parse( Value );
					}
					else if( ValueType.Equals( typeof( int )))
					{
						return int.Parse( Value );
					}
					else if( ValueType.Equals( typeof( float )))
					{
						return float.Parse( Value );
					}
					else if( ValueType.Equals( typeof( double )))
					{
						return double.Parse( Value );
					}
					return Convert.ChangeType( Value , ValueType );
				}
				if( v.GetType().Equals( ValueType ) || v.GetType().IsSubclassOf( ValueType ))
				{
					// 若数据类型匹配则直接返回数值
					return v ;
				}
				else
				{
					// 若读取的值和对象数据的类型不匹配则进行数据类型转换
					System.ComponentModel.TypeConverter converter =
						System.ComponentModel.TypeDescriptor.GetConverter( ValueType );
					if( converter != null && converter.CanConvertFrom( v.GetType()) )
					{
						return converter.ConvertFrom( v ) ;
					}
					return Convert.ChangeType( v , ValueType );
				}
			}//public object FromDataBase( object v )
		}

		#endregion

	}//public class MyORMFramework
}