using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Ycg.Data
{
    public class DataBase
    {
        private readonly string _connectionString = string.Empty;
        protected const string SpecialValue = "@#@#@#";
        private readonly DbProviderFactory _providerFactory = null;
        private readonly ParameterCache _parameterCache = new ParameterCache();

        protected DataBase(string connectionString, DbProviderFactory dbProviderFactory)
        {
            this._connectionString = connectionString;
            this._providerFactory = dbProviderFactory;
        }

        protected T ConvertValue<T>(object value)
        {
            /*
             * 如果一个object类型的对要转化为sting或Int类型的时候，必须用强制转化
             * 如果采取隐式转换是会发生异常的，JIT是不允许这样的。
             * 例：
             *      object result = this.GetValue();
             *      string strValue = (string)result; //会发生异常
             */

            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }

            switch (typeof(T).Name)
            {
                case "Int32":
                    value = Convert.ToInt32(value);
                    break;
                case "String":
                    value = value.ToString();
                    break;
            }
            return (T)value;
        }

        #region Create Connection

        protected string ConnectionString
        {
            get { return this._connectionString; }
        }

        private DbConnection CreateConnection()
        {
            DbConnection connection = this._providerFactory.CreateConnection();
            connection.ConnectionString = this._connectionString;
            return connection;
        }

        protected DbConnection GetNewOpenConnection()
        {
            DbConnection connection = null;
            try
            {
                connection = this.CreateConnection();
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.CloseConnection(connection);
                throw;
            }
            catch (Exception execption)
            {
                this.CloseConnection(connection);
                throw;
            }
            return connection;
        }

        private void CloseConnection(DbConnection connection)
        {
            if (connection != null)
            {
                connection.Close();
            }
        }

        #endregion

        #region Load DataTable Or DataSet

        protected DataSet LoadDataSet(DbCommand command)
        {
            using (DbDataAdapter dataAdapter = _providerFactory.CreateDataAdapter())
            {
                dataAdapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                return dataSet;
            }
        }

        protected DataTable LoadDataTable(DbCommand command)
        {
            using (DbDataAdapter dataAdapter = _providerFactory.CreateDataAdapter())
            {
                dataAdapter.SelectCommand = command;
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                return table;
            }
        }

        #endregion

        #region Create Or Execute Transaction

        protected DbTransaction CreateTransaction()
        {
            return this.GetNewOpenConnection().BeginTransaction();
        }

        protected void CommitDbTransaction(DbTransaction transaction)
        {
            using (DbConnection connection = transaction.Connection)
            {
                transaction.Commit();
                transaction.Dispose();
            }
        }

        protected void RollbackDbTransaction(DbTransaction transaction)
        {
            using (DbConnection connection = transaction.Connection)
            {
                transaction.Rollback();
                transaction.Dispose();
            }
        }

        #endregion

        #region Create Command And Prepare Parameter

        protected DbCommand CreateCommand(DbConnection connection)
        {
            return connection.CreateCommand();
        }

        private DbCommand CreateCommand(DbConnection connection, string commandText, CommandType commandType)
        {
            DbCommand command = this.CreateCommand(connection);
            command.CommandText = commandText;
            command.CommandType = commandType;
            return command;
        }

        protected DbCommand CreateCommand(DbConnection connection, string commandText, CommandType commandType, IDataParameter[] parameters)
        {
            DbCommand command = this.CreateCommand(connection, commandText, commandType);
            this.PrepareParameters(command, parameters);
            return command;
        }

        protected DbCommand CreateCommand(DbConnection connection, string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            DbCommand command = this.CreateCommand(connection, storedProcedureName, CommandType.StoredProcedure);
            this.PrepareParameters(command, parameterNamesAndValues);
            return command;
        }

        protected void PrepareParameters(DbCommand command, params IDataParameter[] parameters)
        {
            this._parameterCache.PrepareParameters(command, parameters);
        }

        protected void PrepareParameters(DbCommand command, IList<ParameterMappingInfo> parameterNamesAndValues)
        {
            this._parameterCache.PrepareParameters(command, parameterNamesAndValues);
        }

        protected void GetInputOutputValueOrReturnValue(DbCommand command, ref object inputOutputValue, ref object returnValue)
        {
            if (inputOutputValue == SpecialValue && returnValue == SpecialValue) return;
            foreach (DbParameter parameter in command.Parameters)
            {
                if (parameter.Direction == ParameterDirection.InputOutput)
                {
                    inputOutputValue = parameter.Value;
                }
                if (parameter.Direction == ParameterDirection.ReturnValue)
                {
                    returnValue = parameter.Value;
                }
            }
        }

        #endregion

        #region Common Mothod：ExecuteNonQuery || ExecuteScalar || ExecuteReader || ExecuteDataTable

        protected bool ExecuteNonQuery(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            using (DbConnection connection = this.GetNewOpenConnection())
            {
                using (DbCommand command = this.CreateCommand(connection, commandText, commandType, parameters))
                {
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        protected int ExecuteNonQuery(string storedProcedureName, ref object inputOutputValue, ref object returnValue, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            using (DbConnection connection = this.GetNewOpenConnection())
            {
                using (DbCommand command = this.CreateCommand(connection, storedProcedureName, parameterNamesAndValues))
                {
                    int result = command.ExecuteNonQuery();
                    this.GetInputOutputValueOrReturnValue(command, ref inputOutputValue, ref returnValue);
                    return result;
                }
            }
        }

        protected object ExecuteScalar(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            using (DbConnection connection = this.GetNewOpenConnection())
            {
                using (DbCommand command = this.CreateCommand(connection, commandText, commandType, parameters))
                {
                    return command.ExecuteScalar();
                }
            }
        }

        protected object ExecuteScalar(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            using (DbConnection connection = this.GetNewOpenConnection())
            {
                using (DbCommand command = this.CreateCommand(connection, storedProcedureName, parameterNamesAndValues))
                {
                    return command.ExecuteScalar();
                }
            }
        }

        protected IDataReader ExecuteReader(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            //Bug
            //问题描述：每次获取的DataReader对象总是Closed的
            //原因：因为下面代码使用了using，导致方法执行结束后自动关闭了connection对象，从而导致DataReader对象是关闭的
            //using (DbConnection connection = this.GetNewOpenConnection())
            //{
            //    using (DbCommand command = this.CreateCommand(connection, commandText, commandType, parameters))
            //    {
            //        return command.ExecuteReader(CommandBehavior.CloseConnection);
            //    }
            //}

            DbConnection connection = this.GetNewOpenConnection();
            DbCommand command = this.CreateCommand(connection, commandText, commandType, parameters);
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        protected IDataReader ExecuteReader(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            //using (DbConnection connection = this.GetNewOpenConnection())
            //{
            //    using (DbCommand command = this.CreateCommand(connection, storedProcedureName, parameterNamesAndValues))
            //    {
            //        return command.ExecuteReader(CommandBehavior.CloseConnection);
            //    }
            //}

            DbConnection connection = this.GetNewOpenConnection();
            DbCommand command = this.CreateCommand(connection, storedProcedureName, parameterNamesAndValues);
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        protected DataTable ExecuteDataTable(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            using (DbConnection connection = this.GetNewOpenConnection())
            {
                using (DbCommand command = this.CreateCommand(connection, commandText, commandType, parameters))
                {
                    return this.LoadDataTable(command);
                }
            }
        }

        protected DataTable ExecuteDataTable(string storedProcedureName, ref object inputOutputValue, ref object returnValue, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            using (DbConnection connection = this.GetNewOpenConnection())
            {
                using (DbCommand command = this.CreateCommand(connection, storedProcedureName, parameterNamesAndValues))
                {
                    DataTable table = this.LoadDataTable(command);
                    this.GetInputOutputValueOrReturnValue(command, ref inputOutputValue, ref returnValue);
                    return table;
                }
            }
        }

        protected DataSet ExecuteDataSet(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            using (DbConnection connection = this.GetNewOpenConnection())
            {
                using (DbCommand command = this.CreateCommand(connection, commandText, commandType, parameters))
                {
                    return this.LoadDataSet(command);
                }
            }
        }

        protected DataSet ExecuteDataSet(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues)
        {
            using (DbConnection connection = this.GetNewOpenConnection())
            {
                using (DbCommand command = this.CreateCommand(connection, storedProcedureName, parameterNamesAndValues))
                {
                    return this.LoadDataSet(command);
                }
            }
        }

        #endregion
    }
}
