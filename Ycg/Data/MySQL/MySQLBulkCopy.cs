using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;

namespace Ycg.Data.MySQL
{
    public class MySQLBulkCopy : BulkCopyBase
    {
        public bool BatchInsertDataForINNODB(DataTable dataTable, string destinationTableName)
        {

            return true;
        }

        public bool BatchInsertDataForMyISAM(DataTable dataTable, string destinationTableName)
        {
            using (MySqlConnection mySqlConnection = (MySqlConnection)DataBaseContext.DBInstance.GetConnection())
            {
                using (MySqlTransaction transaction = mySqlConnection.BeginTransaction())
                {
                    try
                    {
                        MySqlCommand command = mySqlConnection.CreateCommand();
                        command.CommandText = string.Empty;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                return true;
            }
        }

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