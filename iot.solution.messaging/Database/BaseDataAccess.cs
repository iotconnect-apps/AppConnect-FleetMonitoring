using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace component.messaging.Database
{
    public class BaseDataAccess
    {
        protected readonly string _connectionString;

        public BaseDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlParameter AddInParameter(string parameter, object value)
        {
            var parameterObject = new SqlParameter(parameter, value ?? DBNull.Value)
            {
                Direction = ParameterDirection.Input
            };
            return parameterObject;
        }

        public void Execute(string procedureName, List<SqlParameter> parameters)
        {
            var sqlConnection = new SqlConnection(_connectionString);
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            var sqlCommand = new SqlCommand(procedureName, sqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            try
            {
                if (parameters != null && parameters.Count > 0)
                {
                    sqlCommand.Parameters.AddRange(parameters.ToArray());
                }
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlCommand.Dispose();
                sqlConnection.Dispose();
            }
        }
    }
}
