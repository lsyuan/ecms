
using System;

namespace MyORM
{
	/// <summary>
	/// ���ݱ���������
	/// </summary>
	/// <remarks>
	/// ���������ڱ��ĳ������ӳ�䵽ָ�����Ƶ����ݱ��ϡ�����ָ�����ݱ�����
	/// ����ָ�����ݱ�������Ϊ���͵����ƾ��ǰ󶨵����ݱ�����
	/// ������ֻ������class �������棬���������������͡�
	/// </remarks>
	[System.AttributeUsage( System.AttributeTargets.Class , AllowMultiple = false ) ]
	public class BindTableAttribute : System.Attribute
	{
		/// <summary>
		/// ��ʼ������
		/// </summary>
		public BindTableAttribute( )
		{
		}

		/// <summary>
		/// ��ʼ������
		/// </summary>
		/// <param name="name">���ݱ���</param>
		public BindTableAttribute( string name )
		{
			strName = name ;
		}

		private string strName = null;
		/// <summary>
		/// ���ݱ���
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
	/// �����ֶ�������Ϣ
	/// </summary>
	/// <remarks>
	/// ���������ڱ��ĳ������ӳ�䵽ָ�����Ƶ��ֶ��ϣ�����ָ���ֶ�����
	/// ����ָ������Ϊ���Ե����ƾ���Ӱ����ֶ�����
	/// ������ֻ�����ڹ����������棬���������������ͳ�Ա�������������档
	/// </remarks>
	[System.AttributeUsage( System.AttributeTargets.Property , AllowMultiple = false ) ]
	public class BindFieldAttribute : System.Attribute
	{
		/// <summary>
		/// ��ʼ������
		/// </summary>
		public BindFieldAttribute( )
		{
		}

		/// <summary>
		/// ��ʼ������
		/// </summary>
		/// <param name="name">�ֶ���</param>
		public BindFieldAttribute( string name )
		{
			strName = name ;
		}

		private string strName = null;
		/// <summary>
		/// �����ֶ���
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
		/// ���ֶ�Ϊ�ؼ��ֶΣ���������ѯ����
		/// </summary>
		/// <remarks>
		/// ��ܳ������޸ĺ�ɾ������ʱ���������ͱ���������һ�����Ա��ΪKey = true .
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
		/// ���ݶ�ȡ��ʽ���ַ���
		/// </summary>
		/// <remarks>
		/// ����������ָ�����ݿ��е����ݴ洢��ʽ���������ݿ��б����������"20080603"
		/// �����ı�ʾ�������ݵĸ�ʽΪ"yyyyMMdd"���ַ�������ʱ���ö������Ե�ReadFormat
		/// ��ʽΪ"yyyyMMdd",���Ҷ������Ե���������ΪDateTime���͡����ܶ�ȡ����ʱ��
		/// ����ʹ��"yyyyMMdd"��ʽ������ȡ��ԭʼ����Ϊһ���������ݲ����õ���������ֵ��
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
		/// ���ݴ洢��ʽ���ַ���
		/// </summary>
		/// <remarks>
		/// ����������ָ����������ֵ���浽���ݿ��еĸ�ʽ���������������������ΪDateTime��
		/// �����ݿ��б����������"20080603"�����ı�ʾ�������ݵĸ�ʽΪ"yyyyMMdd"���ַ�����
		/// ��ʱ���ö������Ե�WriteFormat��ʽΪ"yyyyMMdd",���ܱ�������ʱ�᳢��ʹ��
		/// "yyyyMMdd"��ʽ����������ֵ���и�ʽ������ת��������浽���ݿ��С�
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
	/// �ҵĶ���-���ݿ�ӳ���ϵ������
	/// </summary>
	/// <remarks>
	/// ������Ϊһ���������Ķ���-���ݿ�ӳ���ϵ�����ܣ��ܸ��ݱ�̶�����
	/// ���ݿ���м򵥵Ĳ�ѯ���������޸ĺ�ɾ�����ݿ��¼������ܴ�������ݿ�
	/// ӳ�����Ͷ�����ʹ�� BindTableAttribute �� BindFieldAttribute ���б�ǡ�
	/// </remarks>
	public class MyORMFramework : System.IDisposable
	{
		/// <summary>
		/// ��ʼ������
		/// </summary>
		public MyORMFramework()
		{
		}

		/// <summary>
		/// ��ʼ������
		/// </summary>
		/// <param name="conn">���ݿ����Ӷ��󣬸ö�������Ѿ���</param>
		public MyORMFramework( System.Data.IDbConnection conn )
		{
			myConnection = conn ;
		}
		
		private System.Data.IDbConnection myConnection = null;
		/// <summary>
		/// ����ʹ�õ����ݿ����Ӷ��󣬸ö�������Ѿ��򿪡�
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
		/// �ж����ݿ����Ƿ����ָ���Ķ���
		/// </summary>
		/// <param name="obj">����</param>
		/// <returns>true:���ݿ��д���ָ���ؼ��ֵļ�¼ false:���ݿ��в�����ָ���ؼ��ֵļ�¼</returns>
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

		#region �������ݿ��¼�Ľӿ� ******************************************

		/// <summary>
		/// ����һ������
		/// </summary>
		/// <param name="obj">Ҫ���µĶ���</param>
		/// <returns>�����޸ĵ����ݿ��¼����</returns>
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
		/// ���¶������
		/// </summary>
		/// <param name="Objects">�����б�</param>
		/// <returns>�����޸ĵ����ݿ��¼����</returns>
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
					// ƴ������SQL�������
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
					// ����SQL�������������
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

		#region ɾ�����ݿ��¼�Ľӿ� ******************************************

		/// <summary>
		/// ɾ��һ�������¼
		/// </summary>
		/// <param name="obj">Ҫɾ���Ķ���</param>
		/// <returns>ɾ�������ݿ��¼����</returns>
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
		/// ɾ�����������������
		/// </summary>
		/// <param name="Objects">�����б�</param>
		/// <returns>ɾ���ļ�¼����</returns>
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
					// ƴ��SQL���
					System.Collections.ArrayList values = new System.Collections.ArrayList();
					string strSQL = BuildCondition( obj , values );
					strSQL = "Delete From " + FixTableName( table.TableName ) + " Where " + strSQL ;

					// ����SQL�������
					cmd.Parameters.Clear();
					cmd.CommandText = strSQL ;
					foreach( object v in values )
					{
						System.Data.IDbDataParameter p = cmd.CreateParameter();
						p.Value = v ;
						cmd.Parameters.Add( p );
					}
					// ִ��SQL��ɾ����¼
					RecordCount += cmd.ExecuteNonQuery();
				}
			}
			return RecordCount ;
		}

		#endregion

		#region ��ѯ���ݿ��¼�Ľӿ� ******************************************

		/// <summary>
		/// ��ȡָ�����͵����еĶ���
		/// </summary>
		/// <param name="ObjectType">��¼��������</param>
		/// <returns>��ȡ�ļ�¼��������</returns>
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
		/// ʹ��ָ����SQL��ѯ����ѯ���ݲ���ȡһ�����ݿ��¼����
		/// </summary>
		/// <param name="strSQL">SQL��ѯ���</param>
		/// <param name="ObjectType">Ҫ��ȡ�Ķ�������</param>
		/// <returns>��ȡ�Ķ���</returns>
		public object ReadObject( string strSQL , Type ObjectType )
		{
			// ������
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
		/// ʹ��ָ����SQL��ѯ����ѯ���ݿⲢ��ȡ�������ݿ��¼����
		/// </summary>
		/// <param name="strSQL">SQL��ѯ���</param>
		/// <param name="ObjectType">Ҫ��ȡ�Ķ�������</param>
		/// <returns>��ȡ�Ķ�����ɵ�����</returns>
		public object[] ReadObjects( string strSQL , Type ObjectType)
		{
			if( ObjectType == null )
			{
				throw new ArgumentNullException("ObjectType");
			}
			// ������ݿ�ӳ����Ϣ
			this.CheckBindInfo( ObjectType , false );
			return ReadObjects( strSQL , ObjectType , 0 );
		}

		/// <summary>
		/// ʹ��ָ����SQL��ѯ����ѯ���ݿⲢ��ȡ�������ݿ��¼����
		/// </summary>
		/// <param name="strSQL">SQL��ѯ���</param>
		/// <param name="ObjectType">Ҫ��ȡ�Ķ�������</param>
		/// <param name="MaxObjectCount">����ȡ�Ķ������</param>
		/// <returns>��ȡ�Ķ�����ɵ�����</returns>
		public object[] ReadObjects( string strSQL , Type ObjectType , int MaxObjectCount )
		{
			// ������
			if( strSQL == null )
			{
				throw new ArgumentNullException("strSQL");
			}
			if( ObjectType == null )
			{
				throw new ArgumentNullException("ObjectType");
			}
			// ������ݿ�ӳ����Ϣ
			this.CheckBindInfo( ObjectType , false );
			// ������ݿ�����
			this.CheckConnetion();
			// ����SQL�������
			using( System.Data.IDbCommand cmd = myConnection.CreateCommand())
			{
				// ִ��SQL��ѯ,���һ�����ݶ�ȡ��
				cmd.CommandText = strSQL ;
				System.Data.IDataReader reader = cmd.ExecuteReader(
					MaxObjectCount == 1 ?
					System.Data.CommandBehavior.SingleRow : 
					System.Data.CommandBehavior.SingleResult );

				System.Collections.ArrayList list = new System.Collections.ArrayList();
				TableBindInfo table = this.GetBindInfo( ObjectType );
				lock( table )
				{
					// �����ֶ���ţ��������
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
						// ���ݶ������ʹ�������ʵ��
						object obj = System.Activator.CreateInstance( ObjectType );
						// ��ȡ��������ֵ
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
				// ���ض�ȡ�Ķ�������
				return list.ToArray();
			}//using
		}

		#endregion

		#region  �������ݿ��¼�Ľӿ� *****************************************

		/// <summary>
		/// ��һ��������뵽���ݿ���
		/// </summary>
		/// <param name="obj">����</param>
		/// <returns>��������ݿ��¼����</returns>
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
		/// ��һ��������뵽ָ�������ݱ���
		/// </summary>
		/// <param name="obj">����</param>
		/// <param name="TableName">ָ�������ݱ���δָ����ΪĬ�����ݱ���</param>
		/// <returns>��������ݿ��¼�ĸ���</returns>
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
		/// �����ɸ�������뵽���ݿ���
		/// </summary>
		/// <param name="Objects">�����б�</param>
		/// <param name="TableName">�ƶ������ݱ���δָ����ʹ��Ĭ�ϵ����ݱ���</param>
		/// <returns>��������ݿ��¼�ĸ���</returns>
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
			// ��һ��ִ�е�SQL���
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
					// ƴ��SQL���
					System.Text.StringBuilder myStr = new System.Text.StringBuilder();
					System.Text.StringBuilder myFields = new System.Text.StringBuilder();
					foreach( FieldBindInfo field in table.Fields )
					{
						if( field.Property.CanRead == false )
						{
							throw new Exception("���� " + field.Property.Name + " �ǲ���д��");
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
						// ��������SQL�������
						strLastSQL = strSQL ;
						cmd.Parameters.Clear();
						cmd.CommandText = strSQL ;
						for( int iCount = 0 ; iCount < values.Count ; iCount ++ )
						{
							cmd.Parameters.Add( cmd.CreateParameter());
						}
					}

					// ���SQL�������ֵ
					for( int iCount = 0 ; iCount < values.Count ; iCount ++ )
					{
						( ( System.Data.IDbDataParameter ) cmd.Parameters[ iCount ]).Value = values[ iCount ] ;
					}

					// ִ��SQL���������ݱ�������¼
					InsertCount += cmd.ExecuteNonQuery();
				}//foreach
			}//using
			return InsertCount ;
		}

		#endregion

		/// <summary>
		/// �������ݱ���
		/// </summary>
		/// <param name="TableName">�ɵ����ݱ���</param>
		/// <returns>����������ݱ���</returns>
		/// <remarks>
		/// ��ĳЩ����£�����ָ�������ݱ����ǲ��Ϸ��ģ���Ҫ��������������������Access���ڱ�����߼��Ϸ����š�
		/// </remarks>
		protected virtual string FixTableName( string TableName )
		{
			return TableName ;
		}

		/// <summary>
		/// ���������ֶ���
		/// </summary>
		/// <param name="FieldName">�ɵ������ֶ���</param>
		/// <returns>��������ֶ���</returns>
		/// <remarks>
		/// ��ĳЩ����£�����ָ�����ֶ������Ϸ�����Ҫ��������������������Access���ڱ�����߼��Ϸ����š�
		/// </remarks>
		protected virtual string FixFieldName( string FieldName )
		{
			return FieldName ;
		}
		
		/// <summary>
		/// ���ٶ���
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

		#region �ڲ�˽�г�Ա **************************************************

		/// <summary>
		/// ������ݿ������Ƿ����
		/// </summary>
		private void CheckConnetion()
		{
			if( myConnection == null || myConnection.State != System.Data.ConnectionState.Open )
			{
				throw new InvalidOperationException("���ݿ�������Ч");
			}
		}
		
		/// <summary>
		/// ���ݶ�����ֵ������ѯ������SQL���
		/// </summary>
		/// <param name="obj">����</param>
		/// <param name="values">SQL����ֵ�б�</param>
		/// <returns>������SQL����ַ���</returns>
		private string BuildCondition( object obj , System.Collections.ArrayList values )
		{
			TableBindInfo table = this.GetBindInfo( obj.GetType() );
			// ƴ�ղ�ѯ����SQL���
			System.Text.StringBuilder mySQL = new System.Text.StringBuilder();
			foreach( FieldBindInfo field in table.Fields )
			{
				if( field.Attribute.Key )
				{
					object v = field.Property.GetValue( obj , null );
					if( v == null || DBNull.Value.Equals( v ))
					{
						throw new Exception("�ؼ��ֶ����� " + field.Property.Name + " δָ��ֵ" ) ;
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
				throw new Exception("���� " + obj.GetType().FullName + " δ�����ɲ�ѯ����");
			}
			return mySQL.ToString();
		}

		/// <summary>
		/// �����ݶ�ȡ���ж�ȡ���ݲ���䵽һ��������
		/// </summary>
		/// <param name="ObjInstance">����ʵ��</param>
		/// <param name="info">���ݿ����Ϣ����</param>
		/// <param name="reader">���ݶ�ȡ��</param>
		private int InnerReadValues( object ObjInstance , TableBindInfo info , System.Data.IDataReader reader )
		{
			// ������
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
			// ���ζ�ȡ��������ֵ
			foreach( FieldBindInfo field in info.Fields )
			{
				// �󶨵�������ֻ����
				if( field.Property.CanWrite == false )
				{
					continue ;
				}
				// û���ҵ��󶨵��ֶ�
				if( field.FieldIndex < 0 )
				{
					continue ;
				}
				// ��ȡ�����ֶ�ֵ
				object v = reader.GetValue( field.FieldIndex );
				v = field.FromDataBase( v );
				// ���ö�������ֵ
				field.Property.SetValue( ObjInstance , v , null );
				FieldCount ++ ;
			}//foreach
			return FieldCount ;
		}

		/// <summary>
		/// ������ɸ���������ݿ��״̬������鲻ͨ�����׳��쳣��
		/// </summary>
		/// <param name="Objects">�����б�</param>
		/// <param name="CheckKey">�Ƿ��鶨���˹ؼ��ֶ�����</param>
		private void CheckBindInfo( System.Collections.IEnumerable Objects , bool CheckKey )
		{
			Type LastType = null ;
			foreach( object obj in Objects )
			{
				if( obj == null )
				{
					throw new Exception("���󼯺��г��ֿ�����");
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
		/// ���һ���������͵����ݿ��״̬������鲻ͨ�����׳��쳣��
		/// </summary>
		/// <param name="t">��������</param>
		/// <param name="CheckKey">�Ƿ��鶨���˹ؼ��ֶ�����</param>
		private void CheckBindInfo( Type t , bool CheckKey )
		{
			TableBindInfo table = this.GetBindInfo( t );
			if( table == null )
			{
				throw new Exception("���� " + t.FullName + " δӳ�䵽���ݿ�");
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
				throw new Exception("���� " + t.FullName + " δ����ؼ��ֶ�");
			}
		}

		/// <summary>
		/// ���ڲ������ӳ����Ϣ�б��˴�Ϊ������ٶȡ�
		/// </summary>
		private static System.Collections.Hashtable myBufferedInfos = new System.Collections.Hashtable();

		/// <summary>
		/// ���ָ�����͵����ݱ�ӳ����Ϣ����
		/// </summary>
		/// <param name="ObjectType">��������</param>
		/// <returns>��õ�ӳ����Ϣ����</returns>
		/// <remarks>
		/// �������ڲ�ʹ���� myBufferedInfos ��������Ϣ��������ܡ�
		/// </remarks>
		private TableBindInfo GetBindInfo( Type ObjectType )
		{
			if( ObjectType == null )
			{
				throw new ArgumentNullException("OjbectType");
			}
			// �����ѻ����ӳ����Ϣ����
			TableBindInfo info = ( TableBindInfo ) myBufferedInfos[ ObjectType ] ;
			if( info != null )
			{
				return info ;
			}

			// ��δ�ҵ��򴴽��µ�ӳ����Ϣ����
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
				// ����������û��ָ���󶨵ı�����ʹ��Ĭ�ϵĶ�����������
				NewInfo.TableName = ObjectType.Name ;
			}
			System.Text.StringBuilder myFieldList = new System.Text.StringBuilder();
			System.Collections.ArrayList fields = new System.Collections.ArrayList();
			// �������еĹ�����ʵ������������ֶΰ���Ϣ
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
						// ����������û��ָ���󶨵��ֶ�����ʹ��Ĭ�ϵ���������
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
			// �������Ϣ����
			myBufferedInfos[ ObjectType ] = NewInfo ;
			return NewInfo ;
		}
 

		/// <summary>
		/// �ж������ֶ����Ƿ�ȼ�
		/// </summary>
		/// <param name="name1">�ֶ���1</param>
		/// <param name="name2">�ֶ���2</param>
		/// <returns>true:�����ֶ����ȼ� false:�ֶ�������ͬ</returns>
		private bool EqualsFieldName( string name1 , string name2 )
		{
			if( name1 == null || name2 == null )
			{
				throw new ArgumentNullException("name1 or name2");
			}
			name1 = name1.Trim();
			name2 = name2.Trim();
			// ���в����ִ�Сд�ıȽ�
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
		/// ���ָ�����Ե�Ĭ��ֵ
		/// </summary>
		/// <param name="p">���Զ���</param>
		/// <returns>��õ�Ĭ��ֵ</returns>
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
		/// ���ݱ����Ϣ����
		/// </summary>
		private class TableBindInfo
		{
			/// <summary>
			/// ���ݿ����
			/// </summary>
			public string TableName = null;
			/// <summary>
			/// ��������
			/// </summary>
			public Type ObjectType = null;
			/// <summary>
			/// ����Ϣ����
			/// </summary>
			public BindTableAttribute Attribute = null;
			/// <summary>
			/// �󶨵��ֶ���Ϣ����
			/// </summary>
			public FieldBindInfo[] Fields = null;
			/// <summary>
			/// �󶨵��ֶ��б���ʽΪ"�ֶ�1,�ֶ�2,�ֶ�3"
			/// </summary>
			public string FieldNameList = null;
		}

		/// <summary>
		/// �����ֶΰ���Ϣ����
		/// </summary>
		private class FieldBindInfo
		{
			/// <summary>
			/// �󶨵��ֶ���
			/// </summary>
			public string FieldName = null;
			/// <summary>
			/// �󶨵��ֶ����
			/// </summary>
			public int FieldIndex = - 1;
			/// <summary>
			/// ����������Ϣ
			/// </summary>
			public System.Reflection.PropertyInfo Property = null;
			/// <summary>
			/// ��������
			/// </summary>
			public Type ValueType = null;
			/// <summary>
			/// Ĭ��ֵ
			/// </summary>
			public object DefaultValue = null;
			/// <summary>
			/// ����Ϣ����
			/// </summary>
			public BindFieldAttribute Attribute = null;

			/// <summary>
			/// ����������ת��Ϊ���ݿ��е�����
			/// </summary>
			/// <param name="v">��������</param>
			/// <returns>���ݿ�����</returns>
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
			/// �������ݿ��л�õ�����ת��Ϊ��������
			/// </summary>
			/// <param name="v">�����ݿ��õ�ԭʼ����</param>
			/// <returns>ת����Ķ�������</returns>
			public object FromDataBase( object v )
			{
				// ������Ϊ���򷵻�Ĭ��ֵ
				if( v == null || DBNull.Value.Equals( v ))
					return DefaultValue ;

				// ���и�ʽ������
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
					// ����������ƥ����ֱ�ӷ�����ֵ
					return v ;
				}
				else
				{
					// ����ȡ��ֵ�Ͷ������ݵ����Ͳ�ƥ���������������ת��
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