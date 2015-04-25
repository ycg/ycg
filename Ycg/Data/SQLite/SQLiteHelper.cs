using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Xml;

namespace Ycg.Data.SQLite
{
    public class SQLiteHelper : DataBase, IDBHelper
    {
        public SQLiteHelper(string connectionString)
            : base(connectionString, SQLiteFactory.Instance)
        { }

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
            return this.ExecuteNonQuery(connection, commandText, CommandType.Text, null);
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
            return this.ExecuteScalar<T>(commandText, commandType, null);
        }

        public T ExecuteScalar<T>(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            return base.ConvertValue<T>(this.ExecuteScalar(commandText, commandType, parameters));
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

        #region ExecuteXmlReader -Not Implemented

        public XmlReader ExecuteXmlReader(string commandText)
        {
            throw new NotImplementedException();
        }

        public XmlReader ExecuteXmlReader(string commandText, CommandType commandType)
        {
            throw new NotImplementedException();
        }

        public XmlReader ExecuteXmlReader(string commandText, CommandType commandType, params IDataParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Execute Stroed Procedure For Use Parameter Cache

        #region Use The [params]

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
            throw new NotImplementedException();
        }

        #endregion

        #region Use The List

        //public int ExecuteNonQuery(string storedProcedureName, IList<ParameterMappingInfo> parameterNamesAndValues)
        //{
        //    object inputOutputValue = string.Empty, returnValue = string.Empty;
        //    return this.ExecuteNonQuery(storedProcedureName, parameterNamesAndValues, ref inputOutputValue, ref returnValue);
        //}

        //public int ExecuteNonQuery(string storedProcedureName, IList<ParameterMappingInfo> parameterNamesAndValues, ref object inputOutputValue)
        //{
        //    object returnValue = string.Empty;
        //    return this.ExecuteNonQuery(storedProcedureName, parameterNamesAndValues, ref inputOutputValue, ref returnValue);
        //}

        //public new int ExecuteNonQuery(string storedProcedureName, IList<ParameterMappingInfo> parameterNamesAndValues, ref object inputOutputValue, ref object returnValue)
        //{
        //    return base.ExecuteNonQuery(storedProcedureName, parameterNamesAndValues, ref inputOutputValue, ref returnValue);
        //}

        //public new object ExecuteScalar(string storedProcedureName, IList<ParameterMappingInfo> parameterNamesAndValues)
        //{
        //    return base.ExecuteScalar(storedProcedureName, parameterNamesAndValues);
        //}

        //public T ExecuteScalar<T>(string storedProcedureName, IList<ParameterMappingInfo> parameterNamesAndValues)
        //{
        //    return base.ConvertValue<T>(this.ExecuteScalar(storedProcedureName, parameterNamesAndValues));
        //}

        //public XmlReader ExecuteXmlReader(string storedProcedureName, IList<ParameterMappingInfo> parameterNamesAndValues)
        //{
        //    throw new NotImplementedException();
        //}

        //public new IDataReader ExecuteReader(string storedProcedureName, IList<ParameterMappingInfo> parameterNamesAndValues)
        //{
        //    return base.ExecuteReader(storedProcedureName, parameterNamesAndValues);
        //}

        //public new DataSet ExecuteDataSet(string storedProcedureName, IList<ParameterMappingInfo> parameterNamesAndValues)
        //{
        //    return base.ExecuteDataSet(storedProcedureName, parameterNamesAndValues);
        //}

        //public DataTable ExecuteDataTable(string storedProcedureName, IList<ParameterMappingInfo> parameterNamesAndValues)
        //{
        //    object inputOutputValue = string.Empty, returnValue = string.Empty;
        //    return this.ExecuteDataTable(storedProcedureName, parameterNamesAndValues, ref inputOutputValue, ref returnValue);
        //}

        //public DataTable ExecuteDataTable(string storedProcedureName, IList<ParameterMappingInfo> parameterNamesAndValues, ref object inputOutputValue)
        //{
        //    object returnValue = string.Empty;
        //    return this.ExecuteDataTable(storedProcedureName, parameterNamesAndValues, ref inputOutputValue, ref returnValue);
        //}

        //public new DataTable ExecuteDataTable(string storedProcedureName, IList<ParameterMappingInfo> parameterNamesAndValues, ref object inputOutputValue, ref object returnValue)
        //{
        //    return base.ExecuteDataTable(storedProcedureName, parameterNamesAndValues, ref inputOutputValue, ref returnValue);
        //} 

        #endregion

        #endregion

        #region IDBHelper Members  -- Not Implemented

        public IAsyncResult BeginExecuteNonQuery(string commandText, AsyncCallback asyncCallback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteNonQuery(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteNonQuery(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state, params IDataParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteReader(string commandText, AsyncCallback asyncCallback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state, params IDataParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteXmlReader(string commandText, AsyncCallback asyncCallback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteXmlReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginExecuteXmlReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state, params IDataParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public int UpdateDataSet(DataSet dataSet, string tableName)
        {
            throw new NotImplementedException();
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, int updateBatchSize)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Batch Insert Data

        public bool BatchInsert(DataTable dataTable, string destinationTableName)
        {
            throw new NotImplementedException();
        }

        public bool BatchInsert(DataTable dataTable, System.Collections.Generic.IList<BulkCopyColumnMapping> columnMappingCollection, string destinationTableName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
