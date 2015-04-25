using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

/*
 * MySQL批量插入的改进(2014-4-21 17:35:00)：
 * 
 *  1.一条一条记录的插入 - 速度很慢
 *  
 *  2.使用事务进行一条条的插入 - 显然速度变快了，但还是不够快 - 速度发生质的的飞跃
 *  
 *  3.在使用事务的情况下分批插入 - 比如500条进行一次插入，也就是所谓的拼接，格式如下(速度提高一点)：
 *      INSERT INTO table_name(`name`) values('123');
 *      INSERT INTO table_name(`name`) values('456');
 *      INSERT INTO table_name(`name`) values('789');
 *      INSERT INTO table_name(`name`) values('101');
 *     
 *  4.结合事务，结合拼接，使用新的插入语法，如下(速度提高几倍)：
 *      INSERT INTO table_name(`name`) values('123'), values(`456`), values('789');
 *      
 *      设置最大运行执行的包：SET global max_allowed_packet = 2*1024*1024*10
 *      
 *  注意事项：
 *  1.SQL语句是有长度限制，在进行数据合并在同一SQL中务必不能超过SQL长度限制.
 *      通过max_allowed_packet配置可以修改，默认是1M，测试时修改为8M.
 *  2.事务需要控制大小，事务太大可能会影响执行的效率.
 *      MySQL有innodb_log_buffer_size配置项，超过这个值会把innodb的数据刷到磁盘中，这时，效率会有所下降.
 *      所以比较好的做法是，在数据达到这个这个值前进行事务提交。
 */

namespace Ycg.Data.MySQL
{
    public class MySQLBulkCopy : BulkCopyBase
    {
        public override bool BatchInsertData(DataTable dataTable, string destinationTableName)
        {
            if (dataTable == null || dataTable.Rows.Count <= 0) return false;

            const int batchInsertCount = 1000;
            int rowsCount = dataTable.Rows.Count;
            string columnNamesSQL = this.GetColumnNamesSQL(dataTable, destinationTableName);
            using (var transaction = DataBaseContext.DBInstance.BeginTransaction())
            {
                try
                {
                    if (rowsCount <= batchInsertCount)
                    {
                        StringBuilder sqlStringBuilder = new StringBuilder(columnNamesSQL);
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            sqlStringBuilder.Append(this.GetInsertSQL(dataRow));
                        }
                        DataBaseContext.DBInstance.ExecuteNonQuery(transaction, sqlStringBuilder.ToString().Remove(sqlStringBuilder.Length - 1, 1));
                    }
                    else
                    {
                        int times = rowsCount / batchInsertCount + (rowsCount % batchInsertCount > 0 ? 1 : 0);
                        for (int i = 0; i < times; i++)
                        {
                            StringBuilder sqlStringBuilder = new StringBuilder(columnNamesSQL);
                            int numbers = (i + 1) == times ? rowsCount : (i + 1) * batchInsertCount;
                            for (int rowIndex = i * batchInsertCount; rowIndex < numbers; rowIndex++)
                            {
                                sqlStringBuilder.Append(this.GetInsertSQL(dataTable.Rows[rowIndex]));
                            }
                            DataBaseContext.DBInstance.ExecuteNonQuery(transaction, sqlStringBuilder.ToString().Remove(sqlStringBuilder.Length - 1, 1));
                        }
                    }

                    DataBaseContext.DBInstance.CommitTransaction(transaction);
                }
                catch (Exception)
                {
                    DataBaseContext.DBInstance.RollbackTransaction(transaction);
                    throw;
                }
            }

            return true;
        }

        private string GetInsertSQL(DataRow dataRow)
        {
            StringBuilder stringBuilder = new StringBuilder("(");
            for (int i = 0; i < dataRow.Table.Columns.Count; i++)
            {
                stringBuilder.Append("'" + dataRow[i] + "',");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("),");
            return stringBuilder.ToString();
        }

        private string GetColumnNamesSQL(DataTable dataTable, string destinationTableName)
        {
            StringBuilder stringBuilder = new StringBuilder("INSERT INTO " + destinationTableName + "(");
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                stringBuilder.Append("`" + dataColumn.ColumnName + "`,");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append(") VALUES");
            return stringBuilder.ToString();
        }

        public override bool BatchInsertData(DataTable dataTable, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection)
        {
            throw new NotImplementedException();
        }

        public override bool BatchInsertData(DataRow[] dataRows, string destinationTableName)
        {
            throw new NotImplementedException();
        }

        public override bool BatchInsertData(DataRow[] dataRows, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection)
        {
            throw new NotImplementedException();
        }

        public override bool BatchInsertData(IDataReader dataReader, string destinationTableName)
        {
            throw new NotImplementedException();
        }

        public override bool BatchInsertData(IDataReader dataReader, string destinationTableName, IList<BulkCopyColumnMapping> columnMappingCollection)
        {
            throw new NotImplementedException();
        }
    }
}
