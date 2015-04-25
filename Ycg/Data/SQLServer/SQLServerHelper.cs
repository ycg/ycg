using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;

/*
 * 2013.12.13
 * 
 *      1.添加批量插入的方法 - SqlBulkCopy类
 *      2.自定义数据源和目标数据源列的映射类 - BulkCopyColumnMapping
 * 
 *      3.★ - 这几天再想一下事务的处理方式
 *      4.Parameter缓存你就是把我相同的参数集合作为一个键值对存在字典中去
 *          当使用相同参数的时候，就不需要重新实例化Parameter对象了，只要对参数重新赋值就好了
 *          参考资料：http://blog.sina.com.cn/s/blog_668042360100w6ia.html
 *          
 * 2013.12.14
 * 
 *      1. 增加对Parameter中的DBNull进行处理下
 *      2. 增加对读取Xml功能的方法
 *      
 *      3. 完成对事务的支持
 *      
 * 2013.12.16
 * 
 *      1. 增加对TransactionScope事务的支持
 *      2. 正在研究Enterprise Library中关于数据库的Parameter的缓存技术 - 正在研究中......
 *          已经做完一部分 - 也就是对Parameter克隆的部分
 *          运用ICloneable接口来实现克隆，因为SqlParameter实现了ICloneable接口，具体的可以用过Reflector来看底层代码
 *
 * 2013.12.18
 * 
 *      1. 研究了一下Enterprise Library的参数缓存，大体上明白是什么意思了
 *      2. 对SqlBulkCopy批量插入进行了测试，也对这个类有了进一步的了解，性能上也在继续的优化中
 *      3. 接下来会继续编写异步查询的代码 - 以及数据层访问层配置文件的代码编写
 *      
 * 2013.12.19
 * 
 *      1. 今天晚上简单实现一下异步方式的数据库查询
 *      2. ADO.NET 的异步资料 ①http://www.cnblogs.com/Scarface/archive/2011/03/28/1997772.html
 *      3. 如果使用异步 - 必须在连接字符串上添加 → async=true
 *      
 * 2013.12.20
 * 
 *      1. 今天终于想通了关于SqlDataAdapter的工作原理了，晚上睡觉之前在好好想，脑子里面在细细的思考一下，巩固下。
 *      
 * 2013.12.22
 * 
 *      1. 今天终于也有点明白了所谓的Parameter缓存到底是怎么一回事了
 *      2. 首先如果要缓存Parameter，基本上要缓存存储过程的Parameter
 *      3. 因为存储过程的Parameter是不变的，唯一变的就是每个Parameter所传的值
 *      4. 这个才是关键的所在
 *      5. 而Parameter缓存里面所谓的Key就是 → ProcedureName + ConnectionString
 *      
 * 2013.12.23
 * 
 *      1. 重要的知识点 - SqlCommandBuilder中DeriveParameters方法
 *          注意：它会自动帮你填充对应存储过程的所有参数项
 *          这是设计参数缓存一个很重要的东西所在
 *          
 * 2013.12.24
 * 
 *      1. 今天终于完成了数据访问层的参数缓存的设计
 *      2. 参照了Enterprise Library和IConnect的源代码，也逐渐明白了很多设计的思想
 *      
 * 2013.12.25
 * 
 *      1. 对代码进行了一些优化和修改
 *      2. 感冒了，最近又没钱了，苦逼啊。
 *      3. 明天继续做DataSet更新那一块，也就是SqlDataAdapter这一个关键点
 *      
 * 2013.12.26
 * 
 *      1. 在设计SqlCommand的异步方法的时候，有一个注意点：
 *          ① Connection对象不能使用using，因为如果SqlCommand还没有执行好你就关闭了数据库连接，一定会发生错误的
 *          ② 在使用异步方法的时候，连接字符串一定要添加Asynchronous Processing属性，表示启用异步
 *          ③ 推荐文章：http://kb.cnblogs.com/kb/130640
 *          
 *      2. 今天又有了对SqlCommand异步的理解，再接再厉咯。
 *      
 * 2013.12.27
 * 
 *      1. 有一个疑问：SqlCommand异步执行怎么去关闭数据库连接？
 *      2. 晚上把SqlBullCopy代码迁出来放到一个专门的类中去了 - SQLBulkCopyHelper
 *      
 *      3. 最近要开始研究自定义配置文件了
 *      4. 尽量搞的灵活一点，不需要太死
 *      
 * 2013.12.29
 * 
 *      1. 自定义配置文件这一块已经搞定了
 *      2. 做的时候遇到了一些问题，还好，很快的解决了
 *      
 *      3. 问题：如果有一个存储过程有返回值，我改怎么获取这个返回值呢
 *      
 * 2013.12.31
 * 
 *      1. 存储过程的返回值问题已经搞定
 *      ★ 任何一个存储过程都会自带一个returnValue的返回值的类型
 *      我们自己在存储过程定义的返回值是inputOutput类型，这个要牢记
 *      
 *      2. 最后定义了两个方法，一个用于获取inputOutput的值，另一个用于获取returnValue和inputOutput的值
 *      
 * 2014.1.24 - 新的一年
 * 
 *      1. 有一段时间没碰这个东西
 *      2. 发现设计返回值方法时有点问题：
 *          如果我要返回一个查询结果，并且获取一个返回值，那么就没有方法能够实现
 *      3. 增加存储过程返回Table的方法支持
 *      
 * 2014-2-2 - 新增对SQLite的支持
 * 
 *      1. 添加SQLite开源类库，用来支持对SQLite数据库的访问
 *      2. 对代码进行了优化，重构了一点代码
 *      3. 把一些共用方法的代码移到了DataBase基类中去了
 *      4. 优化自动生成存储过程参数，使得适用各种数据库
 *      4. 添加了对MySQL数据库的支持，增加MySQL开源类库
 *      
 * 2014-2-4 - 再一次重构代码：把[Execute Stored Procedure For Parameter Cache]提取公共方法。
 * 
 *      1. 删除[ExecuteDataSet For TableNames]方法，进一步重构方法。
 *      
 * 2014-2-7: 调整架构布局，正式把类库命名为 - Ycg
 * 
 * 2014-2-15: 执行存储过程的发放参数尤集合(List)改为数组(Array)
 */

namespace Ycg.Data.SQLServer
{
    public sealed class SQLServerHelper : DataBase, IDBHelper
    {
        public SQLServerHelper(string connectionString)
            : base(connectionString, SqlClientFactory.Instance)
        {
        }

        #region ExecuteNonQuery

        public bool ExecuteNonQuery(string commandText)
        {
            return this.ExecuteNonQuery(commandText, CommandType.Text);
        }

        public bool ExecuteNonQuery(string commandText, CommandType commandType)
        {
            return this.ExecuteNonQuery(commandText, commandType, null);
        }

        public new bool ExecuteNonQuery(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            return base.ExecuteNonQuery(commandText, commandType, parameters);
        }

        #endregion

        #region ExecuteNonQuery For Transaction

        public DbTransaction BeginTransaction()
        {
            return base.CreateTransaction();
        }

        public void CommitTransaction(DbTransaction transaction)
        {
            base.CommitDbTransaction(transaction);
        }

        public void RollbackTransaction(DbTransaction transaction)
        {
            base.RollbackDbTransaction(transaction);
        }

        public bool ExecuteNonQuery(DbTransaction transaction, string commandText)
        {
            return this.ExecuteNonQuery(transaction, commandText, CommandType.Text);
        }

        public bool ExecuteNonQuery(DbTransaction transaction, string commandText, CommandType commandType)
        {
            return this.ExecuteNonQuery(transaction, commandText, commandType, null);
        }

        public bool ExecuteNonQuery(DbTransaction transaction, string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            using (DbCommand command = base.CreateCommand(transaction.Connection, commandText, commandType, parameters))
            {
                command.Transaction = transaction;
                return command.ExecuteNonQuery() > 0;
            }
        }

        #endregion

        #region ExecuteNonQuery For Transaction Scope

        public DbConnection GetConnection()
        {
            return base.GetNewOpenConnection();
        }

        public bool ExecuteNonQuery(DbConnection connection, string commandText)
        {
            return this.ExecuteNonQuery(connection, commandText, CommandType.Text);
        }

        public bool ExecuteNonQuery(DbConnection connection, string commandText, CommandType commandType)
        {
            return this.ExecuteNonQuery(connection, commandText, commandType, null);
        }

        public bool ExecuteNonQuery(DbConnection connection, string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            using (DbCommand command = base.CreateCommand(connection, commandText, commandType, parameters))
            {
                return command.ExecuteNonQuery() > 0;
            }
        }

        #endregion

        #region ExecuteScalar

        public object ExecuteScalar(string commandText)
        {
            return this.ExecuteScalar(commandText, CommandType.Text);
        }

        public object ExecuteScalar(string commandText, CommandType commandType)
        {
            return this.ExecuteScalar(commandText, commandType, null);
        }

        public new object ExecuteScalar(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            return base.ExecuteScalar(commandText, commandType, parameters);
        }

        #endregion

        #region ExecuteScaler For Generic

        public T ExecuteScalar<T>(string commandText)
        {
            return this.ExecuteScalar<T>(commandText, CommandType.Text);
        }

        public T ExecuteScalar<T>(string commandText, CommandType commandType)
        {
            return this.ExecuteScalar<T>(commandText, CommandType.Text, null);
        }

        public T ExecuteScalar<T>(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            return base.ConvertValue<T>(this.ExecuteScalar(commandText, commandType, parameters));
        }

        #endregion

        #region ExecuteReader

        public IDataReader ExecuteReader(string commandText)
        {
            return this.ExecuteReader(commandText, CommandType.Text);
        }

        public IDataReader ExecuteReader(string commandText, CommandType commandType)
        {
            return this.ExecuteReader(commandText, commandType, null);
        }

        public new IDataReader ExecuteReader(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            return base.ExecuteReader(commandText, commandType, parameters);
        }

        #endregion

        #region Execute DataSet

        public DataSet ExecuteDataSet(string commandText)
        {
            return this.ExecuteDataSet(commandText, CommandType.Text);
        }

        public DataSet ExecuteDataSet(string commandText, CommandType commandType)
        {
            return this.ExecuteDataSet(commandText, commandType, null);
        }

        public new DataSet ExecuteDataSet(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            return base.ExecuteDataSet(commandText, commandType, parameters);
        }

        #endregion

        #region ExecuteDataTable

        public DataTable ExecuteDataTable(string commandText)
        {
            return this.ExecuteDataTable(commandText, CommandType.Text);
        }

        public DataTable ExecuteDataTable(string commandText, CommandType commandType)
        {
            return this.ExecuteDataTable(commandText, commandType, null);
        }

        public new DataTable ExecuteDataTable(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            return base.ExecuteDataTable(commandText, commandType, parameters);
        }

        #endregion

        #region Execute XmlReader

        public XmlReader ExecuteXmlReader(string commandText)
        {
            return this.ExecuteXmlReader(commandText, CommandType.Text);
        }

        public XmlReader ExecuteXmlReader(string commandText, CommandType commandType)
        {
            return this.ExecuteXmlReader(commandText, commandType, null);
        }

        public XmlReader ExecuteXmlReader(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            using (DbConnection connection = base.GetNewOpenConnection())
            {
                using (DbCommand command = base.CreateCommand(connection, commandText, commandType, parameters))
                {
                    return ((SqlCommand)command).ExecuteXmlReader();
                }
            }
        }

        #endregion

        #region Async Execute Command

        /*
         * ★ - 下面是在使用异步的时候必须要注意的点
         * 
         *  ① 使用异步SqlCommand的时候，请注意把ConnectionString 的 Asynchronous Processing 设置为 true 。
         *  ② 注意：SqlCommand异步操作的特别之处在于线程并不依赖于CLR线程池，而是由Windows内部提供，这比使用异步委托更有效率。
         *  ③ 但如果需要使用回调函数的时候，回调函数的线程依然是来自于CLR线程池的工作者线程。 
         *  
         */

        public IAsyncResult BeginExecuteNonQuery(string commandText, AsyncCallback asyncCallback, object state)
        {
            return this.BeginExecuteNonQuery(commandText, CommandType.Text, asyncCallback, state);
        }

        public IAsyncResult BeginExecuteNonQuery(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state)
        {
            return this.BeginExecuteNonQuery(commandText, commandType, asyncCallback, state, null);
        }

        public IAsyncResult BeginExecuteNonQuery(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state, params IDataParameter[] parameters)
        {
            SqlCommand command = this.GetAsyncSqlCommand(commandText, commandType, parameters);
            return command.BeginExecuteNonQuery(asyncCallback, state);
        }

        public IAsyncResult BeginExecuteReader(string commandText, AsyncCallback asyncCallback, object state)
        {
            return this.BeginExecuteReader(commandText, CommandType.Text, asyncCallback, state);
        }

        public IAsyncResult BeginExecuteReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state)
        {
            return this.BeginExecuteReader(commandText, commandType, asyncCallback, state, null);
        }

        public IAsyncResult BeginExecuteReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state, params IDataParameter[] parameters)
        {
            SqlCommand command = this.GetAsyncSqlCommand(commandText, commandType, parameters);
            return command.BeginExecuteReader(asyncCallback, state);
        }

        public IAsyncResult BeginExecuteXmlReader(string commandText, AsyncCallback asyncCallback, object state)
        {
            return this.BeginExecuteXmlReader(commandText, CommandType.Text, asyncCallback, state);
        }

        public IAsyncResult BeginExecuteXmlReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state)
        {
            return this.BeginExecuteXmlReader(commandText, commandType, asyncCallback, state, null);
        }

        public IAsyncResult BeginExecuteXmlReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state, params IDataParameter[] parameters)
        {
            SqlCommand command = this.GetAsyncSqlCommand(commandText, commandType, parameters);
            return command.BeginExecuteXmlReader(asyncCallback, state);
        }

        private SqlCommand GetAsyncSqlCommand(string commandText, CommandType commandType, IDataParameter[] parameters)
        {
            DbConnection connection = base.GetNewOpenConnection();
            SqlCommand command = base.CreateCommand(connection, commandText, commandType, parameters) as SqlCommand;
            if (command == null) throw new NullReferenceException("The sql command is null.");
            return command;
        }

        #endregion

        #region UpdateDataSet - Not Implemented

        /*
         *2013.12.22
         *
         *      1. 使用SqlDataAdapter几个认识
         *          ①一定要有执行命令，也就是Command
         *          
         * ----------------------新的一年--------------------------
         * 2013.01.03
         * 
         *      刚刚看了一下IConnect关于Update DateSet的源码，发现是通过配置文件来组装SQL的
         *      
         *      Iconnect的内部通过构建存储过程来实现
         *      他为每个table里面的更新，插入，删除都创建了一个存储过程
         */

        public int UpdateDataSet(DataSet dataSet, string tableName)
        {
            throw new NotImplementedException();
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, int updateBatchSize)
        {
            using (DbConnection connection = base.GetNewOpenConnection())
            {
                SqlCommand insertCommand = base.CreateCommand(connection) as SqlCommand;

                //SqlCommand selectCommand = base.CreateCommand(connection) as SqlCommand;
                //selectCommand.CommandText = "SELECT * FROM Person";

                //SqlCommand insertCommand = new SqlCommand();
                insertCommand.CommandText = "INSERT Person VALUES (@ID,@Name,@Age,@Address,@Sex)";
                insertCommand.Parameters.Add("@ID", SqlDbType.Int, 4, "ID");
                insertCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 20, "Name");
                insertCommand.Parameters.Add("@Age", SqlDbType.Int, 4, "Age");
                insertCommand.Parameters.Add("@Address", SqlDbType.NVarChar, 200, "Address");
                insertCommand.Parameters.Add("@Sex", SqlDbType.Bit, 1, "Sex");

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
                {
                    //SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

                    //dataAdapter.SelectCommand = selectCommand;

                    dataAdapter.InsertCommand = insertCommand;
                    //dataAdapter.InsertCommand.Parameters.Add()
                    //dataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;

                    //dataSet.Tables[tableName].AcceptChanges();
                    return dataAdapter.Update(dataSet.Tables[tableName]);
                }
            }
            ////获取事务对象
            //DbTransaction transaction = base.CreateTransaction();


            ////分别获取添加，更新，删除三个命令执行对象
            //SqlCommand insertCommand = base.CreateCommand(transaction) as SqlCommand;
            //SqlCommand updateCommand = base.CreateCommand(transaction) as SqlCommand;
            //SqlCommand deleteCommand = base.CreateCommand(transaction) as SqlCommand;

            //using (SqlDataAdapter dataAdapter = base.CreateDataAdapter() as SqlDataAdapter)
            //{
            //    SqlCommandBuilder commandBuilder= new SqlCommandBuilder(dataAdapter);

            //    dataAdapter.InsertCommand = insertCommand;
            //    dataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;

            //    //dataAdapter.UpdateCommand = updateCommand;
            //    //dataAdapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;

            //    //dataAdapter.DeleteCommand = deleteCommand;
            //    //dataAdapter.DeleteCommand.UpdatedRowSource = UpdateRowSource.None;

            //    dataAdapter.UpdateBatchSize = updateBatchSize;
            //    dataSet.Tables[tableName].AcceptChanges();
            //    dataAdapter.Update(dataSet.Tables[tableName]);
            //}

            return 100;
            //throw new NotImplementedException();
        }

        #endregion

        #region Execute Stored Procedure For Parameter Cache

        public int ExecuteNonQuery(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            object inputOutputValue = SpecialValue, returnValue = SpecialValue;
            return this.ExecuteNonQuery(storedProcedureName, ref inputOutputValue, ref returnValue, parameterNamesAndValues);
        }

        public int ExecuteNonQuery(string storedProcedureName, ref object inputOutputValue, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            object returnValue = SpecialValue;
            return this.ExecuteNonQuery(storedProcedureName, ref inputOutputValue, ref returnValue, parameterNamesAndValues);
        }

        public new int ExecuteNonQuery(string storedProcedureName, ref object inputOutputValue, ref object returnValue, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            return base.ExecuteNonQuery(storedProcedureName, ref inputOutputValue, ref returnValue, parameterNamesAndValues);
        }

        public new DataSet ExecuteDataSet(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            return base.ExecuteDataSet(storedProcedureName, parameterNamesAndValues);
        }

        public DataTable ExecuteDataTable(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            object inputOutputValue = string.Empty, returnValue = string.Empty;
            return this.ExecuteDataTable(storedProcedureName, ref inputOutputValue, ref returnValue, parameterNamesAndValues);
        }

        public DataTable ExecuteDataTable(string storedProcedureName, ref object inputOutputValue, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            object returnValue = string.Empty;
            return this.ExecuteDataTable(storedProcedureName, ref inputOutputValue, ref returnValue, parameterNamesAndValues);
        }

        public new DataTable ExecuteDataTable(string storedProcedureName, ref object inputOutputValue, ref object returnValue, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            return base.ExecuteDataTable(storedProcedureName, ref inputOutputValue, ref returnValue, parameterNamesAndValues);
        }

        public new object ExecuteScalar(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            return base.ExecuteScalar(storedProcedureName, parameterNamesAndValues);
        }

        public T ExecuteScalar<T>(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            return base.ConvertValue<T>(this.ExecuteScalar(storedProcedureName, parameterNamesAndValues));
        }

        public new IDataReader ExecuteReader(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            return base.ExecuteReader(storedProcedureName, parameterNamesAndValues);
        }

        public XmlReader ExecuteXmlReader(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            using (DbConnection connection = base.GetNewOpenConnection())
            {
                using (DbCommand command = base.CreateCommand(connection, storedProcedureName, parameterNamesAndValues))
                {
                    return ((SqlCommand)command).ExecuteXmlReader();
                }
            }
        }

        #endregion

        #region Batch Insert Data

        public bool BatchInsert(DataTable dataTable, string destinationTableName)
        {
            return new SQLServerBulkCopyHelper().BatchInsertData(dataTable, destinationTableName);
        }

        public bool BatchInsert(DataTable dataTable, System.Collections.Generic.IList<BulkCopyColumnMapping> columnMappingCollection, string destinationTableName)
        {
            return new SQLServerBulkCopyHelper().BatchInsertData(dataTable, destinationTableName, columnMappingCollection);
        }

        #endregion
    }
}