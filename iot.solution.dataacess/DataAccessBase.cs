using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace iot.solution.data
{
    public class ConnectionStringInfo
    {
        public static string DefaultProviderName = "System.Data.SqlClient";
        public string ConnectionString { get; set; }
        public DbProviderFactory Provider { get; set; }
        public static ConnectionStringInfo GetConnectionStringInfo(string connectionString, string providerName = null, DbProviderFactory factory = null)
        {
            var info = new ConnectionStringInfo();

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("AConnectionStringMustBePassedToTheConstructor");

            if (!connectionString.Contains("="))
            {
                throw new ArgumentException("Connection string names are not supported with .NET Standard. Please use a full connectionstring.");
            }
            else
            {
                info.Provider = factory;

                if (factory == null)
                {
                    if (providerName == null)
                        providerName = DefaultProviderName;

                    info.Provider = SqlClientFactory.Instance;
                }
            }

            info.ConnectionString = connectionString;

            return info;
        }
    }

    [DebuggerDisplay("{ ErrorMessage } {ConnectionString} {LastSql}")]
    public abstract class DataAccessBase : IDisposable
    {

        protected DataAccessBase()
        {

        }
        protected DataAccessBase(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("AConnectionStringMustBePassedToTheConstructor");

            // sets dbProvider and ConnectionString properties
            // based on connectionString Name or full connection string
            GetConnectionInfo(connectionString, null);
        }
        protected DataAccessBase(string connectionString, DbProviderFactory provider)
        {
            ConnectionString = connectionString;
            dbProvider = provider;
        }
       

        public void GetConnectionInfo(string connectionString, string providerName = null)
        {
            // throws if connection string is invalid or missing
            var connInfo = ConnectionStringInfo.GetConnectionStringInfo(connectionString, providerName);

            ConnectionString = connInfo.ConnectionString;
            dbProvider = connInfo.Provider;
        }

        public DbProviderFactory dbProvider = null;
        public virtual string ErrorMessage { get; set; } = string.Empty;
        public int ErrorNumber { get; set; } = 0;
        public Exception ErrorException { get; set; }
        public bool ThrowExceptions { get; set; } = false;
        public string ParameterPrefix { get; set; } = "@";
        public bool UsePositionalParameters { get; set; } = false;
        public string LeftFieldBracket { get; set; } = "[";
        public string RightFieldBracket { get; set; } = "]";
        public virtual string ConnectionString { get; set; } = string.Empty;
        public virtual DbTransaction Transaction { get; set; }
        public virtual DbConnection Connection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }

        protected DbConnection _Connection = null;
        public int Timeout { get; set; } = -1;
        public virtual bool ExecuteWithSchema { get; set; } = false;
        public string LastSql { get; set; }

        #region Connection Operations
        public virtual bool OpenConnection()
        {
            try
            {
                if (_Connection == null)
                {
                    if (ConnectionString.Contains("="))
                    {
                        _Connection = dbProvider.CreateConnection();
                        _Connection.ConnectionString = ConnectionString;
                    }
                    else
                    {
                        var connInfo = ConnectionStringInfo.GetConnectionStringInfo(ConnectionString);
                        if (connInfo == null)
                        {
                            SetError("InvalidConnectionString");

                            if (ThrowExceptions)
                                throw new ApplicationException(ErrorMessage);

                            return false;
                        }

                        dbProvider = connInfo.Provider;
                        ConnectionString = connInfo.ConnectionString;

                        _Connection = dbProvider.CreateConnection();
                        _Connection.ConnectionString = ConnectionString;
                    }
                }

                if (_Connection.State != ConnectionState.Open)
                    _Connection.Open();
            }
            catch (DbException ex)
            {
                SetError(string.Format("ConnectionOpeningFailure", ex.Message));
                return false;
            }
            catch (Exception ex)
            {
                SetError(string.Format("ConnectionOpeningFailure", ex.GetBaseException().Message));
                return false;
            }

            return true;
        }
        public virtual void CloseConnection(DbCommand Command)
        {
            if (Transaction != null)
                return;

            if (Command.Connection != null &&
                Command.Connection.State != ConnectionState.Closed)
                Command.Connection.Close();

            _Connection = null;
        }
        public virtual void CloseConnection()
        {
            if (Transaction != null)
                return;

            if (_Connection != null &&
                _Connection.State != ConnectionState.Closed)
                _Connection.Close();

            _Connection = null;
        }

        #endregion

        #region Core Operations
        public virtual DbCommand CreateCommand(string sql, CommandType commandType, params object[] parameters)
        {
            SetError();

            DbCommand command = dbProvider.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = sql;
            if (Timeout > -1)
                command.CommandTimeout = Timeout;

            try
            {
                if (Transaction != null)
                {
                    command.Transaction = Transaction;
                    command.Connection = Transaction.Connection;
                }
                else
                {
                    if (!OpenConnection())
                        return null;

                    command.Connection = _Connection;
                }
            }
            catch (Exception ex)
            {
                SetError(ex);
                return null;
            }

            if (parameters != null)
                AddParameters(command, parameters);


            return command;
        }
        public virtual DbCommand CreateCommand(string sql, params object[] parameters)
        {
            return CreateCommand(sql, CommandType.Text, parameters);
        }
        protected void AddParameters(DbCommand command, object[] parameters)
        {
            if (parameters != null)
            {
                var parmCount = 0;
                foreach (var parameter in parameters)
                {
                    if (parameter is DbParameter)
                        command.Parameters.Add(parameter);
                    else
                    {
                        var parm = CreateParameter(ParameterPrefix + parmCount, parameter);
                        command.Parameters.Add(parm);
                        parmCount++;
                    }
                }
            }

        }
        public virtual DbParameter CreateParameter(string parameterName, object value)
        {
            DbParameter parm = dbProvider.CreateParameter();
            parm.ParameterName = parameterName;
            if (value == null)
                value = DBNull.Value;
            parm.Value = value;
            return parm;
        }
        public virtual DbParameter CreateParameter(string parameterName, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            DbParameter parm = CreateParameter(parameterName, value);
            parm.Direction = direction;
            return parm;
        }
        public virtual DbParameter CreateParameter(string parameterName, object value, int size)
        {
            DbParameter parm = CreateParameter(parameterName, value);
            parm.Size = size;
            return parm;
        }
        public virtual DbParameter CreateParameter(string parameterName, object value, DbType type)
        {
            DbParameter parm = CreateParameter(parameterName, value);
            parm.DbType = type;
            return parm;
        }
        internal DbParameter CreateParameter(string parameterName, object value, DbType type, ParameterDirection direction, int size)
        {
            DbParameter parm = CreateParameter(parameterName, value);
            parm.DbType = type;
            parm.Direction = direction;
            parm.Size = size;
            return parm;
        }
        public virtual DbParameter CreateParameter(string parameterName, object value, DbType type, ParameterDirection direction = ParameterDirection.Input)
        {
            DbParameter parm = CreateParameter(parameterName, value);
            parm.DbType = type;
            parm.Direction = direction;
            return parm;
        }
        public virtual DbParameter CreateParameter(string parameterName, DbType type, ParameterDirection direction, int size)
        {
            DbParameter parm = CreateParameter(parameterName, null);
            parm.DbType = type;
            parm.Size = size;
            parm.Direction = direction;
            return parm;
        }
        public virtual DbParameter CreateParameter(string parameterName, object value, DbType type, int size)
        {
            DbParameter parm = CreateParameter(parameterName, value);
            parm.DbType = type;
            parm.Size = size;
            return parm;
        }
        #endregion

        #region Transactions
        public virtual bool BeginTransaction()
        {
            if (_Connection == null)
            {
                if (!OpenConnection())
                    return false;
            }

            Transaction = _Connection.BeginTransaction();
            if (Transaction == null)
                return false;

            return true;
        }
        public virtual bool CommitTransaction()
        {
            if (Transaction == null)
            {
                SetError("NoActiveTransactionToCommit");
                if (ThrowExceptions)
                    new InvalidOperationException("NoActiveTransactionToCommit");
                return false;
            }

            Transaction.Commit();
            Transaction = null;

            CloseConnection();

            return true;
        }
        public virtual bool RollbackTransaction()
        {
            if (Transaction == null)
                return true;

            Transaction.Rollback();
            Transaction = null;

            CloseConnection();

            return true;
        }
        #endregion

        #region Non-list Sql Commands 
        public virtual int ExecuteNonQuery(DbCommand Command)
        {
            SetError();

            int RecordCount = 0;

            try
            {
                LastSql = Command.CommandText;

                RecordCount = Command.ExecuteNonQuery();
                if (RecordCount == -1)
                    RecordCount = 0;
            }
            catch (DbException ex)
            {
                RecordCount = -1;
                SetError(ex); ;
            }
            catch (Exception ex)
            {
                RecordCount = -1;
                SetError(ex);
            }
            finally
            {
                CloseConnection();
            }

            return RecordCount;
        }

        public virtual int ExecuteNonQuery(DbCommand Command, params object[] parameters)
        {
            SetError();

            int RecordCount = 0;

            try
            {
                AddParameters(Command, parameters);
                LastSql = Command.CommandText;

                RecordCount = Command.ExecuteNonQuery();
                if (RecordCount == -1)
                    RecordCount = 0;
            }
            catch (DbException ex)
            {
                RecordCount = -1;
                SetError(ex); ;
            }
            catch (Exception ex)
            {
                RecordCount = -1;
                SetError(ex);
            }
            finally
            {
                CloseConnection();
            }

            return RecordCount;
        }
        public virtual int ExecuteNonQuery(string sql, params object[] parameters)
        {
            DbCommand command = CreateCommand(sql, parameters);
            if (command == null)
                return -1;

            return ExecuteNonQuery(command);
        }
        public virtual object ExecuteScalar(DbCommand command, params object[] parameters)
        {
            SetError();

            AddParameters(command, parameters);

            object Result = null;
            try
            {
                LastSql = command.CommandText;
                Result = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                SetError(ex.GetBaseException());
            }
            finally
            {
                CloseConnection();
            }

            return Result;
        }
        public virtual object ExecuteScalar(string sql, params object[] parameters)
        {
            SetError();

            DbCommand command = CreateCommand(sql, parameters);
            if (command == null)
                return null;

            return ExecuteScalar(command, null);
        }
        public bool RunSqlScript(string script, bool continueOnError = false, bool scriptIsFile = false)
        {
            SetError();

            if (scriptIsFile)
            {
                try
                {
                    script = File.ReadAllText(script);
                }
                catch (Exception ex)
                {
                    SetError(ex.GetBaseException());
                    return false;
                }
            }

            // Normalize line endings to \n
            string scriptNormal = script.Replace("\r\n", "\n").Replace("\r", "\n");
            string[] scriptBlocks = Regex.Split(scriptNormal + "\n", "GO\n");

            string errors = "";

            if (!continueOnError)
                BeginTransaction();

            foreach (string block in scriptBlocks)
            {
                if (string.IsNullOrEmpty(block.TrimEnd()))
                    continue;

                if (ExecuteNonQuery(block) == -1)
                {
                    errors = ErrorMessage + block;
                    if (!continueOnError)
                    {
                        RollbackTransaction();
                        return false;
                    }
                }
            }

            if (!continueOnError)
                CommitTransaction();

            if (string.IsNullOrEmpty(errors))
                return true;

            ErrorMessage = errors;
            return false;
        }
        #endregion 

        #region Sql Execution

        public virtual DbDataReader ExecuteReader(ref DbCommand command)
        {
            SetError();
            if (command.Connection == null || command.Connection.State != ConnectionState.Open)
            {
                if (!OpenConnection())
                    return null;

                command.Connection = _Connection;
            }

            DbDataReader Reader = null;
            try
            {
                LastSql = command.CommandText;
                Reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                SetError(ex.GetBaseException());
                CloseConnection(command);
                return null;
            }

            return Reader;
        }
        public virtual DbDataReader ExecuteReader(DbCommand command, params object[] parameters)
        {
            SetError();

            if (command.Connection == null || command.Connection.State != ConnectionState.Open)
            {
                if (!OpenConnection())
                    return null;

                command.Connection = _Connection;
            }

            AddParameters(command, parameters);

            DbDataReader Reader = null;
            try
            {
                LastSql = command.CommandText;
                Reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                SetError(ex.GetBaseException());
                CloseConnection(command);
                return null;
            }

            return Reader;
        }
        public virtual DbDataReader ExecuteReader(string sql, params object[] parameters)
        {
            DbCommand command = CreateCommand(sql, parameters);
            if (command == null)
                return null;

            return ExecuteReader(command);
        }
       
        [Obsolete("Use the Query method instead with the same syntax")]
        public virtual IEnumerable<T> ExecuteReader<T>(string sql, params object[] parameters)
            where T : class, new()
        {
            return Query<T>(sql, null, parameters);
        }
        [Obsolete("Use the Query method instead with the same syntax")]
        public virtual IEnumerable<T> ExecuteReader<T>(DbCommand command, params object[] parameters)
            where T : class, new()
        {
            return Query<T>(command, parameters);
        }
        public virtual IEnumerable<T> Query<T>(string sql, params object[] parameters)
            where T : class, new()
        {
            var reader = ExecuteReader(sql, parameters);

            if (reader == null)
                return null;

            try
            {
                return DataUtils.DataReaderToIEnumerable<T>(reader, null);
            }
            catch (Exception ex)
            {
                SetError(ex);
                return null;
            }
        }
        public virtual List<T> QueryList<T>(string sql, params object[] parameters)
                where T : class, new()
        {
            var reader = ExecuteReader(sql, parameters);

            if (reader == null)
                return null;

            try
            {
                return DataUtils.DataReaderToObjectList<T>(reader, null);
            }
            catch (Exception ex)
            {
                SetError(ex);
                return null;
            }
        }
        public virtual List<T> QueryListWithExclusions<T>(string sql, string propertiesToSkip, params object[] parameters)
                where T : class, new()
        {
            var reader = ExecuteReader(sql, parameters);

            if (reader == null)
                return null;

            try
            {
                return DataUtils.DataReaderToObjectList<T>(reader, propertiesToSkip);
            }
            catch (Exception ex)
            {
                SetError(ex);
                return null;
            }
        }
        public virtual IEnumerable<T> Query<T>(DbCommand command, params object[] parameters)
            where T : class, new()
        {
            var reader = ExecuteReader(command, parameters);

            if (reader == null)
                return null;

            try
            {
                return DataUtils.DataReaderToIEnumerable<T>(reader, null);
            }
            catch (Exception ex)
            {
                SetError(ex);
                return null;
            }
        }
        public virtual IEnumerable<T> QueryWithExclusions<T>(string sql, string propertiesToExclude, params object[] parameters)
            where T : class, new()
        {
            IEnumerable<T> result;

            var reader = ExecuteReader(sql, parameters);

            if (reader == null)
                return null;

            try
            {
                result = DataUtils.DataReaderToIEnumerable<T>(reader, propertiesToExclude);
            }
            catch (Exception ex)
            {
                SetError(ex);
                return null;
            }

            return result;
        }
        public virtual IEnumerable<T> QueryWithExclusions<T>(DbCommand sqlCommand, string propertiesToExclude, params object[] parameters)
            where T : class, new()
        {
            var reader = ExecuteReader(sqlCommand, parameters);

            try
            {
                return DataUtils.DataReaderToIEnumerable<T>(reader, propertiesToExclude);
            }
            catch (Exception ex)
            {
                SetError(ex);
                return null;
            }
        }
        public virtual DbDataReader ExecuteStoredProcedureReader(string storedProc, params object[] parameters)
        {
            var command = CreateCommand(storedProc, parameters);
            if (command == null)
                return null;

            command.CommandType = CommandType.StoredProcedure;

            return ExecuteReader(command);
        }
        public virtual IEnumerable<T> ExecuteStoredProcedureReader<T>(string storedProc, List<DbParameter> parameters)
            where T : class, new()
        {
            var command = CreateCommand(storedProc, parameters.ToArray());
            if (command == null)
                return null;

            command.CommandType = CommandType.StoredProcedure;

            return Query<T>(command, null);
        }
        public virtual int ExecuteStoredProcedureNonQuery(string storedProc, params object[] parameters)
        {
            var command = CreateCommand(storedProc, parameters);
            if (command == null)
                return -1;

            command.CommandType = CommandType.StoredProcedure;

            return ExecuteNonQuery(command);
        }
        public virtual DataTable ExecuteTable(string tablename, DbCommand command, params object[] parameters)
        {
            SetError();

            AddParameters(command, parameters);

            DbDataAdapter Adapter = dbProvider.CreateDataAdapter();
            if (Adapter == null)
            {
                SetError("Failed to create data adapter.");
                return null;
            }
            Adapter.SelectCommand = command;

            LastSql = command.CommandText;

            DataTable dt = new DataTable(tablename);

            try
            {
                Adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                SetError(ex.GetBaseException());
                return null;
            }
            finally
            {
                CloseConnection(command);
            }

            return dt;
        }
        public virtual DataTable ExecuteTable(string Tablename, string Sql, params object[] Parameters)
        {
            SetError();

            DbCommand Command = CreateCommand(Sql, Parameters);
            if (Command == null)
                return null;

            return ExecuteTable(Tablename, Command);
        }
        public virtual DataSet ExecuteDataSet(string Tablename, DbCommand Command, params object[] Parameters)
        {
            return ExecuteDataSet(null, Tablename, Command, Parameters);
        }
        public virtual DataSet ExecuteDataSet(string tablename, string sql, params object[] parameters)
        {
            return ExecuteDataSet(tablename, CreateCommand(sql), parameters);
        }
        public virtual DataSet ExecuteDataSet(DataSet dataSet, string tableName, DbCommand command, params object[] parameters)
        {
            SetError();

            if (dataSet == null)
                dataSet = new DataSet();

            DbDataAdapter Adapter = dbProvider.CreateDataAdapter();
            Adapter.SelectCommand = command;
            LastSql = command.CommandText;


            if (ExecuteWithSchema)
                Adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            AddParameters(command, parameters);

            DataTable dt = new DataTable(tableName);

            if (dataSet.Tables.Contains(tableName))
                dataSet.Tables.Remove(tableName);

            try
            {
                Adapter.Fill(dataSet, tableName);
            }
            catch (Exception ex)
            {
                SetError(ex);
                return null;
            }
            finally
            {
                CloseConnection(command);
            }

            return dataSet;
        }
        public virtual DataSet ExecuteDataSet(DataSet dataSet, string tablename, string sql, params object[] parameters)
        {
            DbCommand Command = CreateCommand(sql, parameters);
            if (Command == null)
                return null;

            return ExecuteDataSet(dataSet, tablename, Command);
        }
        public virtual DbCommand CreatePagingCommand(string sql, int pageSize, int page, string sortOrderFields, params object[] Parameters)
        {
            int pos = sql.IndexOf("select ", 0, StringComparison.OrdinalIgnoreCase);
            if (pos == -1)
            {
                SetError("Invalid Command for paging. Must start with select and followed by field list");
                return null;
            }
            sql = StringUtils.ReplaceStringInstance(sql, "select", string.Empty, 1, true);

            string NewSql = string.Format(
            @"
select * FROM 
   (SELECT ROW_NUMBER() OVER (ORDER BY @OrderByFields) as __No,{0}) __TQuery
where __No > (@Page-1) * @PageSize and __No < (@Page * @PageSize + 1)
", sql);

            return CreateCommand(NewSql,
                            CreateParameter("@PageSize", pageSize),
                            CreateParameter("@Page", page),
                            CreateParameter("@OrderByFields", sortOrderFields));

        }
        #endregion

        #region Generic Entity features
        public virtual bool GetEntity(object entity, DbCommand command, string propertiesToSkip = null)
        {
            SetError();

            if (string.IsNullOrEmpty(propertiesToSkip))
                propertiesToSkip = string.Empty;

            DbDataReader reader = ExecuteReader(command);
            if (reader == null)
                return false;

            if (!reader.Read())
            {
                reader.Close();
                CloseConnection(command);
                return false;
            }

            DataUtils.DataReaderToObject(reader, entity, propertiesToSkip);

            reader.Close();
            CloseConnection();

            return true;
        }
        public bool GetEntity(object entity, string sql, object[] parameters)
        {
            return GetEntity(entity, CreateCommand(sql, parameters), null);
        }
        public virtual bool GetEntity(object entity, string table, string keyField, object keyValue, string propertiesToSkip = null)
        {
            SetError();

            DbCommand Command = CreateCommand("select * from " + table + " where " + LeftFieldBracket + keyField + RightFieldBracket + "=" + ParameterPrefix + "Key",
                                                    CreateParameter(ParameterPrefix + "Key", keyValue));
            if (Command == null)
                return false;

            return GetEntity(entity, Command, propertiesToSkip);
        }
        public virtual T Find<T>(object keyValue, string tableName, string keyField)
            where T : class, new()
        {
            T obj = new T();
            if (obj == null)
                return null;

            if (!GetEntity(obj, tableName, keyField, keyValue, null))
                return null;

            return obj;
        }
        public virtual T Find<T>(string sql, params object[] parameters)
            where T : class, new()
        {
            T obj = new T();
            if (!GetEntity(obj, sql, parameters))
                return null;

            return obj;
        }
        public virtual T FindEx<T>(string sql, string propertiesToSkip, params object[] parameters)
            where T : class, new()
        {
            T obj = new T();
            if (!GetEntity(obj, CreateCommand(sql, parameters), propertiesToSkip))
                return null;

            return obj;
        }
        public virtual bool UpdateEntity(object entity, string table, string keyField, string propertiesToSkip = null)
        {
            SetError();

            var Command = GetUpdateEntityCommand(entity, table, keyField, propertiesToSkip);
            if (Command == null)
                return false;

            bool result;
            using (Command)
            {
                result = ExecuteNonQuery(Command) > -1;
                CloseConnection(Command);
            }

            return result;
        }
        public virtual DbCommand GetUpdateEntityCommand(object entity, string table, string keyField, string propertiesToSkip = null)
        {
            SetError();

            if (string.IsNullOrEmpty(propertiesToSkip))
                propertiesToSkip = string.Empty;
            else
                propertiesToSkip = "," + propertiesToSkip.ToLower() + ",";

            DbCommand Command = CreateCommand(string.Empty);

            Type ObjType = entity.GetType();

            StringBuilder sb = new StringBuilder();
            sb.Append("update " + table + " set ");

            PropertyInfo[] Properties = ObjType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo Property in Properties)
            {
                if (!Property.CanRead)
                    continue;

                string Name = Property.Name;

                if (propertiesToSkip.IndexOf("," + Name.ToLower() + ",") > -1)
                    continue;

                object Value = Property.GetValue(entity, null);

                string parmString = UsePositionalParameters ? ParameterPrefix : ParameterPrefix + Name;
                sb.Append(" " + LeftFieldBracket + Name + RightFieldBracket + "=" + parmString + ",");

                if (Value == null && Property.PropertyType == typeof(byte[]))
                {
                    Command.Parameters.Add(
                        CreateParameter(ParameterPrefix + Name, DBNull.Value, DataUtils.DotNetTypeToDbType(Property.PropertyType))
                    );
                }
                else
                    Command.Parameters.Add(CreateParameter(ParameterPrefix + Name, Value ?? DBNull.Value));
            }

            object pkValue = ReflectionUtils.GetProperty(entity, keyField);

            String CommandText = sb.ToString().TrimEnd(',') + " where " + keyField + "=" + ParameterPrefix + "__PK";

            Command.Parameters.Add(CreateParameter(ParameterPrefix + "__PK", pkValue));
            Command.CommandText = CommandText;

            return Command;
        }
        public virtual bool UpdateEntity(object entity, string table, string keyField, string propertiesToSkip, string fieldsToUpdate)
        {
            SetError();

            var Command = GetUpdateEntityCommand(entity, table, keyField, propertiesToSkip, fieldsToUpdate);
            if (Command == null)
                return false;

            bool result;
            using (Command)
            {
                result = ExecuteNonQuery(Command) > -1;
                CloseConnection(Command);
            }

            return result;
        }
        public virtual DbCommand GetUpdateEntityCommand(object entity, string table,
            string keyField, string propertiesToSkip, string fieldsToUpdate)
        {
            SetError();

            if (propertiesToSkip == null)
                propertiesToSkip = string.Empty;
            else
                propertiesToSkip = "," + propertiesToSkip.ToLower() + ",";


            DbCommand Command = CreateCommand(string.Empty);
            if (Command == null)
            {
                SetError("Unable to create command.");
                return null;
            }

            Type ObjType = entity.GetType();

            StringBuilder sb = new StringBuilder();
            sb.Append("update " + table + " set ");

            string[] Fields = fieldsToUpdate.Split(',');
            foreach (string Name in Fields)
            {
                if (propertiesToSkip.IndexOf("," + Name.ToLower() + ",") > -1)
                    continue;

                PropertyInfo Property = ObjType.GetProperty(Name);
                if (Property == null)
                    continue;

                object Value = Property.GetValue(entity, null);

                string parmString = UsePositionalParameters ? ParameterPrefix : ParameterPrefix + Name;
                sb.Append(" " + LeftFieldBracket + Name + RightFieldBracket + "=" + parmString + ",");

                if (Value == null && Property.PropertyType == typeof(byte[]))
                    Command.Parameters.Add(CreateParameter(ParameterPrefix + Name, DBNull.Value,
                        DataUtils.DotNetTypeToDbType(Property.PropertyType)));
                else
                    Command.Parameters.Add(CreateParameter(ParameterPrefix + Name, Value ?? DBNull.Value));
            }

            object pkValue = ReflectionUtils.GetProperty(entity, keyField);

            // check to see if 
            string commandText = sb.ToString().TrimEnd(',') +
                                 " where " + LeftFieldBracket + keyField + RightFieldBracket + "=" + ParameterPrefix +
                                 (UsePositionalParameters ? "" : "__PK");
            Command.Parameters.Add(CreateParameter(ParameterPrefix + "__PK", pkValue));
            Command.CommandText = commandText;

            return Command;
        }
        public object InsertEntity(object entity, string table, string propertiesToSkip = null, bool returnIdentityKey = true)
        {
            SetError();
            DbCommand Command = GetInsertEntityCommand(entity, table, propertiesToSkip);

            using (Command)
            {
                if (returnIdentityKey)
                {
                    Command.CommandText += ";\r\n" + "select SCOPE_IDENTITY()";
                    return ExecuteScalar(Command);
                }

                int res = ExecuteNonQuery(Command);
                if (res < 0)
                    return null;

                return res;
            }
        }
        public DbCommand GetInsertEntityCommand(object entity, string table, string propertiesToSkip = null)
        {
            SetError();

            if (string.IsNullOrEmpty(propertiesToSkip))
                propertiesToSkip = string.Empty;
            else
                propertiesToSkip = "," + propertiesToSkip.ToLower() + ",";


            DbCommand Command = CreateCommand(string.Empty);
            if (Command == null)
            {
                SetError("Unable to create DbCommand instance");
                return null;
            }

            Type ObjType = entity.GetType();

            StringBuilder FieldList = new StringBuilder();
            StringBuilder DataList = new StringBuilder();
            FieldList.Append("insert into " + table + " (");
            DataList.Append(" values (");

            PropertyInfo[] Properties = ObjType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo Property in Properties)
            {
                if (!Property.CanRead)
                    continue;

                string Name = Property.Name;

                if (propertiesToSkip.IndexOf("," + Name.ToLower() + ",") > -1)
                    continue;

                object Value = Property.GetValue(entity, null);

                FieldList.Append(" " + LeftFieldBracket + Name + RightFieldBracket + ",");

                string parmString = ParameterPrefix;
                if (!UsePositionalParameters)
                    parmString += Name;

                DataList.Append(parmString + ",");

                if (Value == null && Property.PropertyType == typeof(byte[]))
                    Command.Parameters.Add(CreateParameter(ParameterPrefix + Name, DBNull.Value,
                        DataUtils.DotNetTypeToDbType(Property.PropertyType)));
                else
                    Command.Parameters.Add(CreateParameter(ParameterPrefix + Name, Value ?? DBNull.Value));
            }

            Command.CommandText = FieldList.ToString().TrimEnd(',') + ") " +
                                 DataList.ToString().TrimEnd(',') + ")";

            return Command;
        }
        public virtual bool SaveEntity(object entity, string table, string keyField, string propertiesToSkip = null)
        {
            object pkValue = ReflectionUtils.GetProperty(entity, keyField);
            object res = null;
            if (pkValue != null)
                res = ExecuteScalar("select " + LeftFieldBracket + keyField + RightFieldBracket + " from " +
                                    LeftFieldBracket + table + RightFieldBracket + "] " +
                                    "where " + LeftFieldBracket + keyField + RightFieldBracket + "=" + ParameterPrefix + "id",
                                         CreateParameter(ParameterPrefix + "id", pkValue));


            if (res == null)
            {
                InsertEntity(entity, table, propertiesToSkip);
                if (!string.IsNullOrEmpty(ErrorMessage))
                    return false;
            }
            else
                return UpdateEntity(entity, table, keyField, propertiesToSkip);

            return true;
        }
        #endregion

        #region Error Handling
        protected virtual void SetError(string Message, int errorNumber)
        {
            if (string.IsNullOrEmpty(Message))
            {
                ErrorMessage = string.Empty;
                ErrorNumber = 0;
                ErrorException = null;
                return;
            }

            ErrorMessage = Message;
            ErrorNumber = errorNumber;
        }
        protected virtual void SetError(string message)
        {
            SetError(message, 0);
        }
        protected virtual void SetError(DbException ex)
        {
            SetError(ex.Message, ex.ErrorCode);
            ErrorException = ex;

            if (ThrowExceptions)
                throw ex;
        }
        protected virtual void SetError(Exception ex)
        {
            if (ex is DbException)
                SetError(ex as DbException);
            else
                SetError(ex.Message, 0);

            ErrorException = ex;

            if (ThrowExceptions)
                throw ex;
        }
        protected virtual void SetError()
        {
            SetError(null, 0);
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if (_Connection != null)
                CloseConnection();
        }
        #endregion
    }



}