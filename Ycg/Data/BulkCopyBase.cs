using System.Collections.Generic;
using System.Data;

namespace Ycg.Data
{
    public abstract class BulkCopyBase
    {
        public abstract bool BatchInsertData(DataTable dataTable, string destinationTableName);

        public abstract bool BatchInsertData(DataTable dataTable, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection);

        public abstract bool BatchInsertData(DataRow[] dataRows, string destinationTableName);

        public abstract bool BatchInsertData(DataRow[] dataRows, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection);

        public abstract bool BatchInsertData(IDataReader dataReader, string destinationTableName);

        public abstract bool BatchInsertData(IDataReader dataReader, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection);
    }
}
