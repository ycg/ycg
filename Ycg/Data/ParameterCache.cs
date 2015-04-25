using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using MySql.Data.MySqlClient;

namespace Ycg.Data
{
    public class ParameterCache
    {
        private readonly ParameterCacheMechanism _parameterCacheMechanism = new ParameterCacheMechanism();

        public void PrepareParameters(DbCommand command, IDataParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return;
            for (int index = 0; index < parameters.Length; index++)
            {
                parameters[index].Value = parameters[index].Value ?? DBNull.Value;
                command.Parameters.Add(this._parameterCacheMechanism.CloneParameter(parameters[index]));
            }
        }

        public void PrepareParameters(DbCommand command, IList<ParameterMappingInfo> parameterNamesAndValues)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (this._parameterCacheMechanism.HasParametersFromCache(command))
            {
                this._parameterCacheMechanism.AssignParameterValues(command, parameterNamesAndValues);
            }
            else
            {
                this.DeriveParameters(command);
                this._parameterCacheMechanism.SetParametersToCache(command);
                this._parameterCacheMechanism.AssignParameterValues(command, parameterNamesAndValues);
            }
        }

        private void DeriveParameters(DbCommand command)
        {
            if (command is SqlCommand)
            {
                SqlCommandBuilder.DeriveParameters(((SqlCommand)command));
            }
            else if (command is OleDbCommand)
            {
                OleDbCommandBuilder.DeriveParameters(((OleDbCommand)command));
            }
            else if (command is OdbcCommand)
            {
                OdbcCommandBuilder.DeriveParameters(((OdbcCommand)command));
            }
            else if (command is SQLiteCommand)
            {
                throw new NotImplementedException("The SQLite command builder no derive parameters method.");
            }
            else if (command is MySqlCommand)
            {
                MySqlCommandBuilder.DeriveParameters(((MySqlCommand)command));
            }
        }
    }
}
