using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/*
 
 * 2013.12.18 - 使用SqlBulkCopy应该要注意的几个问题
 * 
 *      1.源数据应该和数据库表中的列是一一对应的，因为底层是根据ColumnIndex来一次插入的
 *      
 *      2.如果遇到自动增长列，也必须创建一个对应的列
 *      
 *      3.如果使用了列名映射，那么在性能上会有一点点不稳定
 *      
 * 最佳实践：
 * 
 *      1.Table初始化的时候就把列名和数据库中的列名一一对应起来
 *      
 *      2.尽量不要使用列名映射就不要用
 *      
 *      3.最好使用有事务的批量插入
 *      
 * 2013.12.22
 * 
 *      1. 执行插入多少行后触发事件
 *      
 * 2014-2-7
 * 
 *      1. 重构SQLBulkCopyHelper代码，去除DbConnection参数。
 
 */

namespace Ycg.Data.SQLServer
{
    internal class SQLServerBulkCopy
    {
        internal static void BatchInsertData(SqlConnection connection, DataTable dataTable, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection)
        {
            using (SqlBulkCopy bulkCopy = CreateBulkCpy(connection, destinationTableName, columnMappingCollection))
            {
                bulkCopy.WriteToServer(dataTable);
            }
        }

        internal static void BatchInsertData(SqlConnection connection, DataRow[] dataRows, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection)
        {
            using (SqlBulkCopy bulkCopy = CreateBulkCpy(connection, destinationTableName, columnMappingCollection))
            {
                bulkCopy.WriteToServer(dataRows);
            }
        }

        internal static void BatchInsertData(SqlConnection connection, IDataReader dataReader, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection)
        {
            using (SqlBulkCopy bulkCopy = CreateBulkCpy(connection, destinationTableName, columnMappingCollection))
            {
                bulkCopy.WriteToServer(dataReader);
            }
        }

        private static SqlBulkCopy CreateBulkCpy(SqlConnection connection, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection)
        {
            SqlBulkCopy bulkCopy = new SqlBulkCopy(connection.ConnectionString, SqlBulkCopyOptions.UseInternalTransaction);
            if (columnMappingCollection != null && columnMappingCollection.Count > 0)
            {
                foreach (BulkCopyColumnMapping columnMapping in columnMappingCollection)
                {
                    SqlBulkCopyColumnMapping sqlColumnsMapping = new SqlBulkCopyColumnMapping
                    {
                        SourceColumn = columnMapping.SourceColumnName,
                        DestinationColumn = columnMapping.DestinationColumnName
                    };
                    bulkCopy.ColumnMappings.Add(sqlColumnsMapping);
                }
            }
            bulkCopy.DestinationTableName = destinationTableName;
            return bulkCopy;
        }
    }

    /*
     * 2013.12.22
     * 
     *      1. 添加委托，封装SqlBulkCopy中的事件方法     
     *      2. 如果这样做了，我还要在方法里添加两个参数，一个是批量插入的行数，还有一个插入多少条的时候触发事件
     *      3. 总感觉这样的过度封装有点过头了，不太好。
     *      4. 如果我不提供这些方法，那么本来我封装的意图就没有了，那还不如让用户直接使用SqlBulkCopy
     */

    public delegate void SQLBulkCopiedHandler(object sender, SqlRowUpdatedEventArgs eventArgs);
}
