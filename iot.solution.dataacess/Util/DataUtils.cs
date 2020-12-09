using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace iot.solution.data
{
    public static class DataUtils
    {
        public const BindingFlags MemberAccess =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;

        public const BindingFlags MemberPublicInstanceAccess =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        #region Unique Ids and Random numbers
        public static string GenerateUniqueId(int stringSize = 8, string additionalCharacters = null)
        {
            string chars = "abcdefghijkmnopqrstuvwxyz1234567890" + (additionalCharacters ?? string.Empty);
            StringBuilder result = new StringBuilder(stringSize);
            int count = 0;


            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                result.Append(chars[b % (chars.Length)]);
                count++;
                if (count >= stringSize)
                    break;
            }
            return result.ToString();
        }
        public static long GenerateUniqueNumericId()
        {
            byte[] bytes = Guid.NewGuid().ToByteArray();
            return (long)BitConverter.ToUInt64(bytes, 0);
        }
        private static Random rnd = new Random();
        public static int GetRandomNumber(int min, int max)
        {
            return rnd.Next(min, max + 1);
        }
        #endregion

        #region Byte Data
        public static int IndexOfByteArray(byte[] buffer, byte[] bufferToFind)
        {
            if (buffer.Length == 0 || bufferToFind.Length == 0)
                return -1;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == bufferToFind[0])
                {
                    bool innerMatch = true;
                    for (int j = 1; j < bufferToFind.Length; j++)
                    {
                        if (buffer[i + j] != bufferToFind[j])
                        {
                            innerMatch = false;
                            break;
                        }
                    }
                    if (innerMatch)
                        return i;
                }
            }

            return -1;
        }
        public static int IndexOfByteArray(byte[] buffer, string stringToFind, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            if (buffer.Length == 0 || string.IsNullOrEmpty(stringToFind))
                return -1;

            var bytes = encoding.GetBytes(stringToFind);

            return IndexOfByteArray(buffer, bytes);
        }
        #endregion

        #region Copying Objects and Data
        public static bool CopyDataRow(DataRow source, DataRow target)
        {
            DataColumnCollection columns = target.Table.Columns;

            for (int x = 0; x < columns.Count; x++)
            {
                string fieldname = columns[x].ColumnName;

                try
                {
                    target[x] = source[fieldname];
                }
                catch {; }  // skip any errors
            }

            return true;
        }
        public static void CopyObjectFromDataRow(DataRow row, object targetObject, MemberInfo[] cachedMemberInfo = null)
        {
            if (cachedMemberInfo == null)
            {
                cachedMemberInfo = targetObject.GetType()
                    .FindMembers(MemberTypes.Field | MemberTypes.Property,
                        ReflectionUtils.MemberAccess, null, null);
            }
            foreach (MemberInfo Field in cachedMemberInfo)
            {
                string Name = Field.Name;
                if (!row.Table.Columns.Contains(Name))
                    continue;

                object value = row[Name];
                if (value == DBNull.Value)
                    value = null;

                if (Field.MemberType == MemberTypes.Field)
                {
                    ((FieldInfo)Field).SetValue(targetObject, value);
                }
                else if (Field.MemberType == MemberTypes.Property)
                {
                    ((PropertyInfo)Field).SetValue(targetObject, value, null);
                }
            }
        }
        public static bool CopyObjectToDataRow(DataRow row, object target)
        {
            bool result = true;

            MemberInfo[] miT = target.GetType().FindMembers(MemberTypes.Field | MemberTypes.Property, MemberAccess, null, null);
            foreach (MemberInfo Field in miT)
            {
                string name = Field.Name;
                if (!row.Table.Columns.Contains(name))
                    continue;

                try
                {
                    if (Field.MemberType == MemberTypes.Field)
                    {
                        row[name] = ((FieldInfo)Field).GetValue(target) ?? DBNull.Value;
                    }
                    else if (Field.MemberType == MemberTypes.Property)
                    {
                        row[name] = ((PropertyInfo)Field).GetValue(target, null) ?? DBNull.Value;
                    }
                }
                catch { result = false; }
            }

            return result;
        }
        public static List<T> DataTableToTypedList<T>(DataTable dsTable) where T : class, new()
        {
            var objectList = new List<T>();

            MemberInfo[] cachedMemberInfo = null;
            foreach (DataRow dr in dsTable.Rows)
            {
                var obj = default(T); // Activator.CreateInstance<T>();				
                CopyObjectFromDataRow(dr, obj, cachedMemberInfo);
                objectList.Add(obj);
            }

            return objectList;
        }
        public static void CopyObjectData(object source, Object target)
        {
            CopyObjectData(source, target, MemberAccess);
        }
        public static void CopyObjectData(object source, Object target, BindingFlags memberAccess)
        {
            CopyObjectData(source, target, null, memberAccess);
        }
        public static void CopyObjectData(object source, Object target, string excludedProperties)
        {
            CopyObjectData(source, target, excludedProperties, MemberAccess);
        }
        public static void CopyObjectData(object source, object target, string excludedProperties = null, BindingFlags memberAccess = MemberAccess)
        {
            string[] excluded = null;
            if (!string.IsNullOrEmpty(excludedProperties))
                excluded = excludedProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            MemberInfo[] miT = target.GetType().GetMembers(memberAccess);
            foreach (MemberInfo Field in miT)
            {
                string name = Field.Name;

                // Skip over any property exceptions
                if (!string.IsNullOrEmpty(excludedProperties) &&
                    excluded.Contains(name))
                    continue;

                if (Field.MemberType == MemberTypes.Field)
                {
                    FieldInfo SourceField = source.GetType().GetField(name);
                    if (SourceField == null)
                        continue;

                    object SourceValue = SourceField.GetValue(source);
                    ((FieldInfo)Field).SetValue(target, SourceValue);
                }
                else if (Field.MemberType == MemberTypes.Property)
                {
                    PropertyInfo piTarget = Field as PropertyInfo;
                    PropertyInfo SourceField = source.GetType().GetProperty(name, memberAccess);
                    if (SourceField == null)
                        continue;

                    if (piTarget.CanWrite && SourceField.CanRead)
                    {
                        object SourceValue = SourceField.GetValue(source, null);
                        piTarget.SetValue(target, SourceValue, null);
                    }
                }
            }
        }
        #endregion
        
        #region DataTable and DataReader
        public static List<T> DataTableToObjectList<T>(DataTable dsTable) where T : class, new()
        {
            var objectList = new List<T>();

            foreach (DataRow dr in dsTable.Rows)
            {
                var obj = Activator.CreateInstance<T>();
                CopyObjectFromDataRow(dr, obj);
                objectList.Add(obj);
            }

            return objectList;
        }
        public static List<T> DataReaderToObjectList<T>(IDataReader reader, string propertiesToSkip = null, Dictionary<string, PropertyInfo> piList = null)
            where T : new()
        {
            List<T> list = new List<T>();

            using (reader)
            {
                // Get a list of PropertyInfo objects we can cache for looping            
                if (piList == null)
                {
                    piList = new Dictionary<string, PropertyInfo>();
                    var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var prop in props)
                        piList.Add(prop.Name.ToLower(), prop);
                }

                while (reader.Read())
                {
                    T inst = new T();
                    DataReaderToObject(reader, inst, propertiesToSkip, piList);
                    list.Add(inst);
                }
            }

            return list;
        }
        public static IEnumerable<T> DataReaderToIEnumerable<T>(IDataReader reader, string propertiesToSkip = null, Dictionary<string, PropertyInfo> piList = null)
            where T : new()
        {
            if (reader != null)
            {
                using (reader)
                {
                    // Get a list of PropertyInfo objects we can cache for looping            
                    if (piList == null)
                    {
                        piList = new Dictionary<string, PropertyInfo>();
                        var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                        foreach (var prop in props)
                            piList.Add(prop.Name.ToLower(), prop);
                    }

                    while (reader.Read())
                    {
                        T inst = new T();
                        DataReaderToObject(reader, inst, propertiesToSkip, piList);
                        yield return inst;
                    }
                }
            }
        }
        public static List<T> DataReaderToList<T>(IDataReader reader, string propertiesToSkip = null, Dictionary<string, PropertyInfo> piList = null)
          where T : new()
        {
            var list = new List<T>();

            if (reader != null)
            {
                using (reader)
                {
                    // Get a list of PropertyInfo objects we can cache for looping            
                    if (piList == null)
                    {
                        piList = new Dictionary<string, PropertyInfo>();
                        var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                        foreach (var prop in props)
                            piList.Add(prop.Name.ToLower(), prop);
                    }

                    while (reader.Read())
                    {
                        T inst = new T();
                        DataReaderToObject(reader, inst, propertiesToSkip, piList);
                        list.Add(inst);
                    }
                }
            }
            return list;
        }
        public static void DataReaderToObject(IDataReader reader, object instance,
                                              string propertiesToSkip = null,
                                              Dictionary<string, PropertyInfo> piList = null)
        {
            if (reader.IsClosed)
                throw new InvalidOperationException("DataReaderPassedToDataReaderToObjectCannot");

            if (string.IsNullOrEmpty(propertiesToSkip))
                propertiesToSkip = string.Empty;
            else
                propertiesToSkip = "," + propertiesToSkip + ",";

            propertiesToSkip = propertiesToSkip.ToLower();

            // create a dictionary of properties to look up
            // we can pass this in so we can cache the list once 
            // for a list operation 
            if (piList == null || piList.Count < 1)
            {
                if (piList == null)
                    piList = new Dictionary<string, PropertyInfo>();

                var props = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                    piList.Add(prop.Name.ToLower(), prop);
            }

            for (int index = 0; index < reader.FieldCount; index++)
            {
                string name = reader.GetName(index).ToLower();
                if (piList.ContainsKey(name))
                {
                    var prop = piList[name];

                    // skip fields in skip list
                    if (!string.IsNullOrEmpty(propertiesToSkip) && propertiesToSkip.Contains("," + name + ","))
                        continue;

                    // find writable properties and assign
                    if ((prop != null) && prop.CanWrite)
                    {
                        var val = reader.GetValue(index);

                        if (val == DBNull.Value)
                            val = null;
                        // deal with data drivers return bit values as int64 or in
                        else if (prop.PropertyType == typeof(bool) && (val is long || val is int))
                            val = (long)val == 1 ? true : false;
                        // int conversions when the value is not different type of number
                        else if (prop.PropertyType == typeof(int) && (val is long || val is decimal))
                            val = Convert.ToInt32(val);
                        else if (prop.PropertyType == typeof(string) && (val is long || val is decimal || val is int || val is float))
                            val = Convert.ToString(val);

                        prop.SetValue(instance, val, null);
                    }
                }
            }

            return;
        }
        public static DateTime MinimumSqlDate = DateTime.Parse("01/01/1900");
        public static void InitializeDataRowWithBlanks(DataRow row)
        {
            DataColumnCollection loColumns = row.Table.Columns;

            for (int x = 0; x < loColumns.Count; x++)
            {
                if (!row.IsNull(x))
                    continue;

                string lcRowType = loColumns[x].DataType.Name;

                if (lcRowType == "String")
                    row[x] = string.Empty;
                else if (lcRowType.StartsWith("Int"))
                    row[x] = 0;
                else if (lcRowType == "Byte")
                    row[x] = 0;
                else if (lcRowType == "Decimal")
                    row[x] = 0.00M;
                else if (lcRowType == "Double")
                    row[x] = 0.00;
                else if (lcRowType == "Boolean")
                    row[x] = false;
                else if (lcRowType == "DateTime")
                    row[x] = DataUtils.MinimumSqlDate;

                // Everything else isn't handled explicitly and left alone
                // Byte[] most specifically

            }
        }
        #endregion

        

        #region Type Conversions
        public static Type SqlTypeToDotNetType(SqlDbType sqlType)
        {
            if (sqlType == SqlDbType.NText || sqlType == SqlDbType.Text ||
                sqlType == SqlDbType.VarChar || sqlType == SqlDbType.NVarChar)
                return typeof(string);
            else if (sqlType == SqlDbType.Int)
                return typeof(Int32);
            else if (sqlType == SqlDbType.Decimal || sqlType == SqlDbType.Money)
                return typeof(decimal);
            else if (sqlType == SqlDbType.Bit)
                return typeof(Boolean);
            else if (sqlType == SqlDbType.DateTime || sqlType == SqlDbType.DateTime2)
                return typeof(DateTime);
            else if (sqlType == SqlDbType.Char || sqlType == SqlDbType.NChar)
                return typeof(char);
            else if (sqlType == SqlDbType.Float)
                return typeof(Single);
            else if (sqlType == SqlDbType.Real)
                return typeof(Double);
            else if (sqlType == SqlDbType.BigInt)
                return typeof(Int64);
            else if (sqlType == SqlDbType.Image)
                return typeof(byte[]);
            else if (sqlType == SqlDbType.SmallInt)
                return typeof(byte);

            throw new InvalidCastException("Unable to convert " + sqlType.ToString() + " to .NET type.");
        }
        public static Type DbTypeToDotNetType(DbType sqlType)
        {
            if (sqlType == DbType.String || sqlType == DbType.StringFixedLength || sqlType == DbType.AnsiString)
                return typeof(string);
            else if (sqlType == DbType.Int16 || sqlType == DbType.Int32)
                return typeof(Int32);
            else if (sqlType == DbType.Int64)
                return typeof(Int64);
            else if (sqlType == DbType.Decimal || sqlType == DbType.Currency)
                return typeof(decimal);
            else if (sqlType == DbType.Boolean)
                return typeof(Boolean);
            else if (sqlType == DbType.DateTime || sqlType == DbType.DateTime2 || sqlType == DbType.Date)
                return typeof(DateTime);
            else if (sqlType == DbType.Single)
                return typeof(Single);
            else if (sqlType == DbType.Double)
                return typeof(Double);
            else if (sqlType == DbType.Binary)
                return typeof(byte[]);
            else if (sqlType == DbType.SByte || sqlType == DbType.Byte)
                return typeof(byte);
            else if (sqlType == DbType.Guid)
                return typeof(Guid);
            else if (sqlType == DbType.Binary)
                return typeof(byte[]);

            throw new InvalidCastException("Unable to convert " + sqlType.ToString() + " to .NET type.");
        }
        public static DbType DotNetTypeToDbType(Type type)
        {
            if (type == typeof(string))
                return DbType.String;
            else if (type == typeof(Int32))
                return DbType.Int32;
            else if (type == typeof(Int16))
                return DbType.Int16;
            else if (type == typeof(Int64))
                return DbType.Int64;
            else if (type == typeof(Guid))
                return DbType.Guid;
            else if (type == typeof(decimal))
                return DbType.Decimal;
            else if (type == typeof(double) || type == typeof(float))
                return DbType.Double;
            else if (type == typeof(Single))
                return DbType.Single;
            else if (type == typeof(bool) || type == typeof(Boolean))
                return DbType.Boolean;
            else if (type == typeof(DateTime))
                return DbType.DateTime;
            else if (type == typeof(DateTimeOffset))
                return DbType.DateTimeOffset;
            else if (type == typeof(byte))
                return DbType.Byte;
            else if (type == typeof(byte[]))
                return DbType.Binary;

            throw new InvalidCastException(string.Format("Unable to cast {0} to a DbType", type.Name));
        }
        public static SqlDbType DotNetTypeToSqlType(Type type)
        {
            if (type == typeof(string))
                return SqlDbType.NVarChar;
            else if (type == typeof(Int32))
                return SqlDbType.Int;
            else if (type == typeof(Int16))
                return SqlDbType.SmallInt;
            else if (type == typeof(Int64))
                return SqlDbType.BigInt;
            else if (type == typeof(Guid))
                return SqlDbType.UniqueIdentifier;
            else if (type == typeof(decimal))
                return SqlDbType.Decimal;
            else if (type == typeof(double) || type == typeof(float))
                return SqlDbType.Float;
            else if (type == typeof(Single))
                return SqlDbType.Float;
            else if (type == typeof(bool) || type == typeof(Boolean))
                return SqlDbType.Bit;
            else if (type == typeof(DateTime))
                return SqlDbType.DateTime;
            else if (type == typeof(DateTimeOffset))
                return SqlDbType.DateTimeOffset;
            else if (type == typeof(byte))
                return SqlDbType.SmallInt;
            else if (type == typeof(byte[]))
                return SqlDbType.Image;

            throw new InvalidCastException(string.Format("Unable to cast {0} to a DbType", type.Name));
        }

        #endregion

       

    }

    public enum DataAccessProviderTypes
    {
        SqlServer,
        SqLite,
        MySql,
        PostgreSql,
    }
}
