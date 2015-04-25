using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Ycg.Data.SQLServer
{
    public class SQLServerBulkCopyHelper : BulkCopyBase
    {
        #region DataTable

        public override bool BatchInsertData(DataTable dataTable, string destinationTableName)
        {
            return BatchInsertData(dataTable, destinationTableName, null);
        }

        public override bool BatchInsertData(DataTable dataTable, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection)
        {
            using (DbConnection connection = DataBaseContext.DBInstance.GetConnection())
            {
                SQLServerBulkCopy.BatchInsertData((SqlConnection)connection, dataTable, destinationTableName, columnMappingCollection);
                return true;
            }
        }

        #endregion

        #region DataRow

        public override bool BatchInsertData(DataRow[] dataRows, string destinationTableName)
        {
            return BatchInsertData(dataRows, destinationTableName, null);
        }

        public override bool BatchInsertData(DataRow[] dataRows, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection)
        {
            using (DbConnection connection = DataBaseContext.DBInstance.GetConnection())
            {
                SQLServerBulkCopy.BatchInsertData((SqlConnection)connection, dataRows, destinationTableName, columnMappingCollection);
                return true;
            }
        }

        #endregion

        #region DataReader

        public override bool BatchInsertData(IDataReader dataReader, string destinationTableName)
        {
            return BatchInsertData(dataReader, destinationTableName, null);
        }

        public override bool BatchInsertData(IDataReader dataReader, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection)
        {
            using (DbConnection connection = DataBaseContext.DBInstance.GetConnection())
            {
                SQLServerBulkCopy.BatchInsertData((SqlConnection)connection, dataReader, destinationTableName, columnMappingCollection);
                return true;
            }
        }

        #endregion
    }
}
