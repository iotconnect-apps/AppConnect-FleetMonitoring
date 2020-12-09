using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace iot.solution.data
{
    public class SqlDataAccess : DataAccessBase
    {
        public SqlDataAccess()
        {
            dbProvider = SqlClientFactory.Instance;
        }
        public SqlDataAccess(string connectionString)
        : base(connectionString, SqlClientFactory.Instance)
        { }
        public SqlDataAccess(string connectionString, DbProviderFactory provider)
            : base(connectionString, provider)
        { }
        public override DbCommand CreatePagingCommand(string sql, int pageSize, int page, string sortOrderFields, params object[] Parameters)
        {
            int pos = sql.IndexOf("select ", 0, StringComparison.OrdinalIgnoreCase);
            if (pos == -1)
            {
                SetError("Invalid Command for paging. Must start with select and followed by field list");
                return null;
            }
            sql = StringUtils.ReplaceStringInstance(sql, "select", string.Empty, 1, true);

            string NewSql = string.Format(@" select * FROM  (SELECT ROW_NUMBER() OVER (ORDER BY @OrderByFields) as __No,{0}) __TQuery where __No > (@Page-1) * @PageSize and __No < (@Page * @PageSize + 1) ", sql);

            if (!string.IsNullOrWhiteSpace(sortOrderFields))
            {
                NewSql = string.Format("{0} order by {1}", NewSql, sortOrderFields);
            }

            return CreateCommand(NewSql,
                            CreateParameter("@PageSize", pageSize),
                            CreateParameter("@Page", page));

        }
        public List<DbParameter> CreateParams(Guid? invokingUser, string version, bool includeOutput = true, bool includeFieldName = true)
        {
            var parameters = new List<DbParameter>();
            if (invokingUser.HasValue)
            {
                parameters.Add(base.CreateParameter("invokinguser", invokingUser, DbType.Guid, ParameterDirection.Input));
            }
            if (!string.IsNullOrWhiteSpace(version))
            {
                parameters.Add(base.CreateParameter("version", version, DbType.String, ParameterDirection.Input));
            }
            if (includeOutput)
            {
                parameters.Add(base.CreateParameter("output", version, DbType.String, ParameterDirection.Output, 16));
            }
            if (includeFieldName)
            {
                parameters.Add(base.CreateParameter("fieldname", version, DbType.String, ParameterDirection.Output, 1000));
            }


            return parameters;
        }
        public SqlParameter CreateParam(string parameterName, string value, SqlDbType sqlDbType, ParameterDirection direction)
        {
            return new SqlParameter { ParameterName = parameterName, SqlDbType = sqlDbType, Direction = direction, Value = value };
        }
    }

}

