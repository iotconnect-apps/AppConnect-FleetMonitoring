using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace iot.solution.data
{
    public static class ReflectionUtils
    {
        public const BindingFlags MemberAccess =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;

        public const BindingFlags MemberAccessCom =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        #region Type and Assembly Creation
        public static object CreateInstanceFromType(Type typeToCreate, params object[] args)
        {
            if (args == null)
            {
                Type[] Parms = Type.EmptyTypes;
                return typeToCreate.GetConstructor(Parms).Invoke(null);
            }

            return Activator.CreateInstance(typeToCreate, args);
        }
        public static object CreateInstanceFromString(string typeName, params object[] args)
        {
            object instance = null;

            try
            {
                var type = GetTypeFromName(typeName);
                if (type == null)
                    return null;

                instance = Activator.CreateInstance(type, args);
            }
            catch
            {
                return null;
            }

            return instance;
        }
        public static Type GetTypeFromName(string typeName, string assemblyName )
        {
            var type = Type.GetType(typeName, false);
            if (type != null)
                return type;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // try to find manually
            foreach (Assembly asm in assemblies)
            {
                type = asm.GetType(typeName, false);

                if (type != null)
                    break;
            }
            if (type != null)
                return type;

            // see if we can load the assembly
            if (!string.IsNullOrEmpty(assemblyName))
            {
                var a = LoadAssembly(assemblyName);
                if (a != null)
                {
                    type = Type.GetType(typeName, false);
                    if (type != null)
                        return type;
                }
            }

            return null;
        }
        public static Type GetTypeFromName(string typeName)
        {
            return GetTypeFromName(typeName, null);
        }
        public static object CreateComInstance(string progId)
        {
            Type type = Type.GetTypeFromProgID(progId);
            if (type == null)
                return null;

            return Activator.CreateInstance(type);
        }
        public static Assembly LoadAssembly(string assemblyName)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch { }

            if (assembly != null)
                return assembly;

            if (File.Exists(assemblyName))
            {
                assembly = Assembly.LoadFrom(assemblyName);
                if (assembly != null)
                    return assembly;
            }
            return null;
        }
        #endregion

        #region Conversions
        public static object StringToTypedValue(string sourceString, Type targetType, CultureInfo culture = null)
        {
            object result = null;

            bool isEmpty = string.IsNullOrEmpty(sourceString);

            if (culture == null)
                culture = CultureInfo.CurrentCulture;

            if (targetType == typeof(string))
                result = sourceString;
            else if (targetType == typeof(Int32) || targetType == typeof(int))
            {
                if (isEmpty)
                    result = 0;
                else
                    result = Int32.Parse(sourceString, NumberStyles.Any, culture.NumberFormat);
            }
            else if (targetType == typeof(Int64))
            {
                if (isEmpty)
                    result = (Int64)0;
                else
                    result = Int64.Parse(sourceString, NumberStyles.Any, culture.NumberFormat);
            }
            else if (targetType == typeof(Int16))
            {
                if (isEmpty)
                    result = (Int16)0;
                else
                    result = Int16.Parse(sourceString, NumberStyles.Any, culture.NumberFormat);
            }
            else if (targetType == typeof(decimal))
            {
                if (isEmpty)
                    result = 0M;
                else
                    result = decimal.Parse(sourceString, NumberStyles.Any, culture.NumberFormat);
            }
            else if (targetType == typeof(DateTime))
            {
                if (isEmpty)
                    result = DateTime.MinValue;
                else
                    result = Convert.ToDateTime(sourceString, culture.DateTimeFormat);
            }
            else if (targetType == typeof(byte))
            {
                if (isEmpty)
                    result = 0;
                else
                    result = Convert.ToByte(sourceString);
            }
            else if (targetType == typeof(double))
            {
                if (isEmpty)
                    result = 0F;
                else
                    result = Double.Parse(sourceString, NumberStyles.Any, culture.NumberFormat);
            }
            else if (targetType == typeof(Single))
            {
                if (isEmpty)
                    result = 0F;
                else
                    result = Single.Parse(sourceString, NumberStyles.Any, culture.NumberFormat);
            }
            else if (targetType == typeof(bool))
            {
                sourceString = sourceString.ToLower();
                if (!isEmpty &&
                    sourceString == "true" || sourceString == "on" ||
                    sourceString == "1" || sourceString == "yes")
                    result = true;
                else
                    result = false;
            }
            else if (targetType == typeof(Guid))
            {
                if (isEmpty)
                    result = Guid.Empty;
                else
                    result = new Guid(sourceString);
            }
            else if (targetType.IsEnum)
                result = Enum.Parse(targetType, sourceString);
            else if (targetType == typeof(byte[]))
            {
                // TODO: Convert HexBinary string to byte array
                result = null;
            }

            // Handle nullables explicitly since type converter won't handle conversions
            // properly for things like decimal separators currency formats etc.
            // Grab underlying type and pass value to that
            else if (targetType.Name.StartsWith("Nullable`"))
            {
                if (sourceString.ToLower() == "null" || sourceString == string.Empty)
                    result = null;
                else
                {
                    targetType = Nullable.GetUnderlyingType(targetType);
                    result = StringToTypedValue(sourceString, targetType);
                }
            }
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(targetType);
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                    result = converter.ConvertFromString(null, culture, sourceString);
                else
                {
                    Debug.Assert(false, string.Format("Type Conversion not handled in StringToTypedValue for {0} {1}",
                        targetType.Name, sourceString));
                    throw (new InvalidCastException("StringToTypedValueValueTypeConversionFailed" + targetType.Name));
                }
            }

            return result;
        }
        public static T StringToTypedValue<T>(string sourceString, CultureInfo culture = null)
        {
            return (T)StringToTypedValue(sourceString, typeof(T), culture);
        }
        public static string TypedValueToString(object rawValue, CultureInfo culture = null, string unsupportedReturn = null)
        {
            if (rawValue == null)
                return string.Empty;

            if (culture == null)
                culture = CultureInfo.CurrentCulture;

            Type valueType = rawValue.GetType();
            string returnValue = null;

            if (valueType == typeof(string))
                returnValue = rawValue as string;
            else if (valueType == typeof(int) || valueType == typeof(decimal) ||
                     valueType == typeof(double) || valueType == typeof(float) || valueType == typeof(Single))
                returnValue = string.Format(culture.NumberFormat, "{0}", rawValue);
            else if (valueType == typeof(DateTime))
                returnValue = string.Format(culture.DateTimeFormat, "{0}", rawValue);
            else if (valueType == typeof(bool) || valueType == typeof(Byte) || valueType.IsEnum)
                returnValue = rawValue.ToString();
            else if (valueType == typeof(Guid?))
            {
                if (rawValue == null)
                    returnValue = string.Empty;
                else
                    return rawValue.ToString();
            }
            else
            {
                // Any type that supports a type converter
                TypeConverter converter = TypeDescriptor.GetConverter(valueType);
                if (converter != null && converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
                    returnValue = converter.ConvertToString(null, culture, rawValue);
                else
                {
                    // Last resort - just call ToString() on unknown type
                    if (!string.IsNullOrEmpty(unsupportedReturn))
                        returnValue = unsupportedReturn;
                    else
                        returnValue = rawValue.ToString();
                }
            }

            return returnValue;
        }

        #endregion

        #region Member Access
        public static object GetField(object Object, string Property)
        {
            return Object.GetType().GetField(Property, ReflectionUtils.MemberAccess | BindingFlags.GetField).GetValue(Object);
        }
        private static object GetPropertyInternal(object Parent, string Property)
        {
            if (Property == "this" || Property == "me")
                return Parent;

            object result = null;
            string pureProperty = Property;
            string indexes = null;
            bool isArrayOrCollection = false;

            // Deal with Array Property
            if (Property.IndexOf("[") > -1)
            {
                pureProperty = Property.Substring(0, Property.IndexOf("["));
                indexes = Property.Substring(Property.IndexOf("["));
                isArrayOrCollection = true;
            }

            // Get the member
            MemberInfo member = Parent.GetType().GetMember(pureProperty, ReflectionUtils.MemberAccess)[0];
            if (member.MemberType == MemberTypes.Property)
                result = ((PropertyInfo)member).GetValue(Parent, null);
            else
                result = ((FieldInfo)member).GetValue(Parent);

            if (isArrayOrCollection)
            {
                indexes = indexes.Replace("[", string.Empty).Replace("]", string.Empty);

                if (result is Array)
                {
                    int Index = -1;
                    int.TryParse(indexes, out Index);
                    result = CallMethod(result, "GetValue", Index);
                }
                else if (result is ICollection)
                {
                    if (indexes.StartsWith("\""))
                    {
                        // String Index
                        indexes = indexes.Trim('\"');
                        result = CallMethod(result, "get_Item", indexes);
                    }
                    else
                    {
                        // assume numeric index
                        int index = -1;
                        int.TryParse(indexes, out index);
                        result = CallMethod(result, "get_Item", index);
                    }
                }

            }

            return result;
        }
        private static object SetPropertyInternal(object Parent, string Property, object Value)
        {
            if (Property == "this" || Property == "me")
                return Parent;

            object Result = null;
            string PureProperty = Property;
            string Indexes = null;
            bool IsArrayOrCollection = false;

            // Deal with Array Property
            if (Property.IndexOf("[") > -1)
            {
                PureProperty = Property.Substring(0, Property.IndexOf("["));
                Indexes = Property.Substring(Property.IndexOf("["));
                IsArrayOrCollection = true;
            }

            if (!IsArrayOrCollection)
            {
                // Get the member
                MemberInfo Member = Parent.GetType().GetMember(PureProperty, ReflectionUtils.MemberAccess)[0];
                if (Member.MemberType == MemberTypes.Property)
                    ((PropertyInfo)Member).SetValue(Parent, Value, null);
                else
                    ((FieldInfo)Member).SetValue(Parent, Value);
                return null;
            }
            else
            {
                // Get the member
                MemberInfo Member = Parent.GetType().GetMember(PureProperty, ReflectionUtils.MemberAccess)[0];
                if (Member.MemberType == MemberTypes.Property)
                    Result = ((PropertyInfo)Member).GetValue(Parent, null);
                else
                    Result = ((FieldInfo)Member).GetValue(Parent);
            }
            if (IsArrayOrCollection)
            {
                Indexes = Indexes.Replace("[", string.Empty).Replace("]", string.Empty);

                if (Result is Array)
                {
                    int Index = -1;
                    int.TryParse(Indexes, out Index);
                    Result = CallMethod(Result, "SetValue", Value, Index);
                }
                else if (Result is ICollection)
                {
                    if (Indexes.StartsWith("\""))
                    {
                        // String Index
                        Indexes = Indexes.Trim('\"');
                        Result = CallMethod(Result, "set_Item", Indexes, Value);
                    }
                    else
                    {
                        // assume numeric index
                        int Index = -1;
                        int.TryParse(Indexes, out Index);
                        Result = CallMethod(Result, "set_Item", Index, Value);
                    }
                }
            }

            return Result;
        }
        public static object GetPropertyEx(object Parent, string Property)
        {
            Type type = Parent.GetType();

            int at = Property.IndexOf(".");
            if (at < 0)
            {
                // Complex parse of the property    
                return GetPropertyInternal(Parent, Property);
            }

            // Walk the . syntax - split into current object (Main) and further parsed objects (Subs)
            string main = Property.Substring(0, at);
            string subs = Property.Substring(at + 1);

            // Retrieve the next . section of the property
            object sub = GetPropertyInternal(Parent, main);

            // Now go parse the left over sections
            return GetPropertyEx(sub, subs);
        }
        public static PropertyInfo GetPropertyInfoEx(object Parent, string Property)
        {
            Type type = Parent.GetType();

            int at = Property.IndexOf(".");
            if (at < 0)
            {
                // Complex parse of the property    
                return GetPropertyInfoInternal(Parent, Property);
            }

            // Walk the . syntax - split into current object (Main) and further parsed objects (Subs)
            string main = Property.Substring(0, at);
            string subs = Property.Substring(at + 1);

            // Retrieve the next . section of the property
            object sub = GetPropertyInternal(Parent, main);

            // Now go parse the left over sections
            return GetPropertyInfoEx(sub, subs);
        }
        public static PropertyInfo GetPropertyInfoInternal(object Parent, string Property)
        {
            if (Property == "this" || Property == "me")
                return null;

            string propertyName = Property;

            // Deal with Array Property - strip off array indexer
            if (Property.IndexOf("[") > -1)
                propertyName = Property.Substring(0, Property.IndexOf("["));

            // Get the member
            return Parent.GetType().GetProperty(propertyName, ReflectionUtils.MemberAccess);
        }
        public static void SetProperty(object obj, string property, object value)
        {
            obj.GetType().GetProperty(property, ReflectionUtils.MemberAccess).SetValue(obj, value, null);
        }
        public static void SetField(object obj, string property, object value)
        {
            obj.GetType().GetField(property, ReflectionUtils.MemberAccess).SetValue(obj, value);
        }
        public static object SetPropertyEx(object parent, string property, object value)
        {
            Type Type = parent.GetType();

            // no more .s - we got our final object
            int lnAt = property.IndexOf(".");
            if (lnAt < 0)
            {
                SetPropertyInternal(parent, property, value);
                return null;
            }

            // Walk the . syntax
            string Main = property.Substring(0, lnAt);
            string Subs = property.Substring(lnAt + 1);

            object Sub = GetPropertyInternal(parent, Main);

            SetPropertyEx(Sub, Subs, value);

            return null;
        }
        public static object CallMethod(object instance, string method, Type[] parameterTypes, params object[] parms)
        {
            if (parameterTypes == null && parms.Length > 0)
                // Call without explicit parameter types - might cause problems with overloads    
                // occurs when null parameters were passed and we couldn't figure out the parm type
                return instance.GetType().GetMethod(method, ReflectionUtils.MemberAccess | BindingFlags.InvokeMethod).Invoke(instance, parms);
            else
                // Call with parameter types - works only if no null values were passed
                return instance.GetType().GetMethod(method, ReflectionUtils.MemberAccess | BindingFlags.InvokeMethod, null, parameterTypes, null).Invoke(instance, parms);
        }
        public static object CallMethod(object instance, string method, params object[] parms)
        {
            // Pick up parameter types so we can match the method properly
            Type[] parameterTypes = null;
            if (parms != null)
            {
                parameterTypes = new Type[parms.Length];
                for (int x = 0; x < parms.Length; x++)
                {
                    // if we have null parameters we can't determine parameter types - exit
                    if (parms[x] == null)
                    {
                        parameterTypes = null;  // clear out - don't use types        
                        break;
                    }
                    parameterTypes[x] = parms[x].GetType();
                }
            }
            return CallMethod(instance, method, parameterTypes, parms);
        }
        public static object CallMethodEx(object parent, string method, params object[] parms)
        {
            Type Type = parent.GetType();

            // no more .s - we got our final object
            int lnAt = method.IndexOf(".");
            if (lnAt < 0)
            {
                return ReflectionUtils.CallMethod(parent, method, parms);
            }

            // Walk the . syntax
            string Main = method.Substring(0, lnAt);
            string Subs = method.Substring(lnAt + 1);

            object Sub = GetPropertyInternal(parent, Main);

            // Recurse until we get the lowest ref
            return CallMethodEx(Sub, Subs, parms);
        }
        public static object GetStaticProperty(string typeName, string property)
        {
            Type type = GetTypeFromName(typeName);
            if (type == null)
                return null;

            return GetStaticProperty(type, property);
        }
        public static object GetStaticProperty(Type type, string property)
        {
            object result = null;
            try
            {
                result = type.InvokeMember(property, BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, type, null);
            }
            catch
            {
                return null;
            }

            return result;
        }
        public static object GetProperty(object instance, string property)
        {
            return instance.GetType().GetProperty(property, ReflectionUtils.MemberAccess).GetValue(instance, null);
        }
        //public static List<KeyValuePair<string, string>> GetEnumList(Type enumType, bool valueAsFieldValueNumber = false)
        //{
        //    //string[] enumStrings = Enum.GetNames(enumType);
        //    Array enumValues = Enum.GetValues(enumType);
        //    List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

        //    foreach (var enumValue in enumValues)
        //    {
        //        var strValue = enumValue.ToString();

        //        if (!valueAsFieldValueNumber)
        //            items.Add(new KeyValuePair<string, string>(enumValue.ToString(), StringUtils.FromCamelCase(strValue)));
        //        else
        //            items.Add(new KeyValuePair<string, string>(((int)enumValue).ToString(),
        //                StringUtils.FromCamelCase(strValue)
        //            ));
        //    }
        //    return items;
        //}
        #endregion

        //#region COM Reflection Routines
        //public static object GetPropertyCom(object instance, string property)
        //{
        //        return instance.GetType().InvokeMember(property, ReflectionUtils.MemberAccessCom | BindingFlags.GetProperty, null,
        //                                            instance, null);
        //}
        //public static object GetPropertyExCom(object parent, string property)
        //{

        //    Type Type = parent.GetType();

        //    int lnAt = property.IndexOf(".");
        //    if (lnAt < 0)
        //    {
        //        if (property == "this" || property == "me")
        //            return parent;

        //        // Get the member
        //        return parent.GetType().InvokeMember(property, ReflectionUtils.MemberAccessCom | BindingFlags.GetProperty , null,
        //            parent, null);
        //    }

        //    // Walk the . syntax - split into current object (Main) and further parsed objects (Subs)
        //    string Main = property.Substring(0, lnAt);
        //    string Subs = property.Substring(lnAt + 1);

        //    object Sub = parent.GetType().InvokeMember(Main, ReflectionUtils.MemberAccessCom | BindingFlags.GetProperty , null,
        //        parent, null);

        //    // Recurse further into the sub-properties (Subs)
        //    return ReflectionUtils.GetPropertyExCom(Sub, Subs);
        //}
        //public static void SetPropertyCom(object inst, string Property, object Value)
        //{
        //    inst.GetType().InvokeMember(Property, ReflectionUtils.MemberAccessCom | BindingFlags.SetProperty, null, inst, new object[1] { Value });
        //}
        //public static object SetPropertyExCom(object parent, string property, object value)
        //{
        //    Type Type = parent.GetType();

        //    int lnAt = property.IndexOf(".");
        //    if (lnAt < 0)
        //    {
        //        // Set the member
        //        parent.GetType().InvokeMember(property, ReflectionUtils.MemberAccessCom | BindingFlags.SetProperty , null,
        //            parent, new object[1] { value });

        //        return null;
        //    }

        //    // Walk the . syntax - split into current object (Main) and further parsed objects (Subs)
        //    string Main = property.Substring(0, lnAt);
        //    string Subs = property.Substring(lnAt + 1);


        //    object Sub = parent.GetType().InvokeMember(Main, ReflectionUtils.MemberAccessCom | BindingFlags.GetProperty , null,
        //        parent, null);

        //    return SetPropertyExCom(Sub, Subs, value);
        //}
        //public static object CallMethodCom(object instance, string method, params object[] parms)
        //{
        //    return instance.GetType().InvokeMember(method, ReflectionUtils.MemberAccessCom | BindingFlags.InvokeMethod, null, instance, parms);
        //}
        //public static object CallMethodExCom(object parent, string method, params object[] parms)
        //{
        //    Type Type = parent.GetType();

        //    // no more .s - we got our final object
        //    int at = method.IndexOf(".");
        //    if (at < 0)
        //    {
        //        return ReflectionUtils.CallMethodCom(parent, method, parms);
        //    }

        //    // Walk the . syntax - split into current object (Main) and further parsed objects (Subs)
        //    string Main = method.Substring(0, at);
        //    string Subs = method.Substring(at + 1);

        //    object Sub = parent.GetType().InvokeMember(Main, ReflectionUtils.MemberAccessCom | BindingFlags.GetProperty, null,
        //        parent, null);

        //    // Recurse until we get the lowest ref
        //    return CallMethodExCom(Sub, Subs, parms);
        //}
        //#endregion

    }


}


