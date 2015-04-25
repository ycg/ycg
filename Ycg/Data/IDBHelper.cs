using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Xml;

namespace Ycg.Data
{
    public interface IDBHelper
    {
        bool ExecuteNonQuery(string commandText);

        bool ExecuteNonQuery(string commandText, CommandType commandType);

        bool ExecuteNonQuery(string commandText, CommandType commandType, params IDataParameter[] parameters);

        object ExecuteScalar(string commandText);

        object ExecuteScalar(string commandText, CommandType commandType);

        object ExecuteScalar(string commandText, CommandType commandType, params IDataParameter[] parameters);

        T ExecuteScalar<T>(string commandText);

        T ExecuteScalar<T>(string commandText, CommandType commandType);

        T ExecuteScalar<T>(string commandText, CommandType commandType, params IDataParameter[] parameters);

        DataTable ExecuteDataTable(string commandText);

        DataTable ExecuteDataTable(string commandText, CommandType commandType);

        DataTable ExecuteDataTable(string commandText, CommandType commandType, params IDataParameter[] parameters);

        DataSet ExecuteDataSet(string commandText);

        DataSet ExecuteDataSet(string commandText, CommandType commandType);

        DataSet ExecuteDataSet(string commandText, CommandType commandType, params IDataParameter[] parameters);

        IDataReader ExecuteReader(string commandText);

        IDataReader ExecuteReader(string commandText, CommandType commandType);

        IDataReader ExecuteReader(string commandText, CommandType commandType, params IDataParameter[] parameters);

        XmlReader ExecuteXmlReader(string commandText);

        XmlReader ExecuteXmlReader(string commandText, CommandType commandType);

        XmlReader ExecuteXmlReader(string commandText, CommandType commandType, params IDataParameter[] parameters);

        DbConnection GetConnection();

        DbTransaction BeginTransaction();

        void CommitTransaction(DbTransaction transaction);

        void RollbackTransaction(DbTransaction transaction);

        bool ExecuteNonQuery(DbTransaction transaction, string commandText);

        bool ExecuteNonQuery(DbTransaction transaction, string commandText, CommandType commandType);

        bool ExecuteNonQuery(DbTransaction transaction, string commandText, CommandType commandType, params IDataParameter[] parameters);

        bool ExecuteNonQuery(DbConnection connection, string commandText);

        bool ExecuteNonQuery(DbConnection connection, string commandText, CommandType commandType);

        bool ExecuteNonQuery(DbConnection connection, string commandText, CommandType commandType, params IDataParameter[] parameters);

        IAsyncResult BeginExecuteNonQuery(string commandText, AsyncCallback asyncCallback, object state);

        IAsyncResult BeginExecuteNonQuery(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state);

        IAsyncResult BeginExecuteNonQuery(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state, params IDataParameter[] parameters);

        IAsyncResult BeginExecuteReader(string commandText, AsyncCallback asyncCallback, object state);

        IAsyncResult BeginExecuteReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state);

        IAsyncResult BeginExecuteReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state, params IDataParameter[] parameters);

        IAsyncResult BeginExecuteXmlReader(string commandText, AsyncCallback asyncCallback, object state);

        IAsyncResult BeginExecuteXmlReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state);

        IAsyncResult BeginExecuteXmlReader(string commandText, CommandType commandType, AsyncCallback asyncCallback, object state, params IDataParameter[] parameters);

        int UpdateDataSet(DataSet dataSet, string tableName);

        int UpdateDataSet(DataSet dataSet, string tableName, int updateBatchSize);

        int ExecuteNonQuery(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues);

        int ExecuteNonQuery(string storedProcedureName, ref object inputOutputValue, params ParameterMappingInfo[] parameterNamesAndValues);

        int ExecuteNonQuery(string storedProcedureName, ref object inputOutputValue, ref object returnValue, params ParameterMappingInfo[] parameterNamesAndValues);

        object ExecuteScalar(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues);

        T ExecuteScalar<T>(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues);

        XmlReader ExecuteXmlReader(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues);

        IDataReader ExecuteReader(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues);

        DataTable ExecuteDataTable(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues);

        DataTable ExecuteDataTable(string storedProcedureName, ref object inputOutputValue, params ParameterMappingInfo[] parameterNamesAndValues);

        DataTable ExecuteDataTable(string storedProcedureName, ref object inputOutputValue, ref object returnValue, params ParameterMappingInfo[] parameterNamesAndValues);

        DataSet ExecuteDataSet(string storedProcedureName, params ParameterMappingInfo[] parameterNamesAndValues);

        bool BatchInsert(DataTable dataTable, string destinationTableName);

        bool BatchInsert(DataTable dataTable, IList<BulkCopyColumnMapping> columnMappingCollection, string destinationTableName);
    }
}
