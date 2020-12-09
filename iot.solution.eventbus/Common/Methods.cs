using component.eventbus.CustomAttribute;
using component.eventbus.Model.Base;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace component.eventbus.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class Methods
    {
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static string GetPropValue(object source, string propertyName)
        {
            PropertyInfo property = source.GetType().GetRuntimeProperties().FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
            if (property != null)
            {
                return Convert.ToString(property.GetValue(source));
            }
            return null;
        }

        /// <summary>
        /// Converts to base64encode.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        public static string ToBase64Encode(this string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Converts to base64decode.
        /// </summary>
        /// <param name="base64EncodedData">The base64 encoded data.</param>
        /// <returns></returns>
        public static string ToBase64Decode(this string base64EncodedData)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Extracts the json object.
        /// </summary>
        /// <param name="mixedString">The mixed string.</param>
        /// <returns></returns>
        public static object ExtractJsonObject(string mixedString)
        {
            for (int i = mixedString.IndexOf('{'); i > -1; i = mixedString.IndexOf('{', i + 1))
            {
                for (int j = mixedString.LastIndexOf('}'); j > -1; j = mixedString.LastIndexOf("}", j - 1))
                {
                    string jsonProbe = mixedString.Substring(i, j - i + 1);
                    try
                    {
                        return JsonConvert.DeserializeObject(jsonProbe);
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }
        public static ServiceBusConnectionData GetConnectionDetailFromAttribute(object currentClass)
        {
            ServiceBusConnectionData serviceBusAttr = new ServiceBusConnectionData();

            object[] data = currentClass.GetType().GetCustomAttributes(true);
            if (data != null)
            {
                serviceBusAttr.Connection = ((ConfigureAttribute)currentClass.GetType().GetCustomAttributes(true)[0]).Connection;
                serviceBusAttr.TopicName = ((ConfigureAttribute)currentClass.GetType().GetCustomAttributes(true)[0]).TopicName;
                serviceBusAttr.EventId = ((ConfigureAttribute)currentClass.GetType().GetCustomAttributes(true)[0]).EventId;
                serviceBusAttr.QueueName = ((ConfigureAttribute)currentClass.GetType().GetCustomAttributes(true)[0]).QueueName;
            }

            return serviceBusAttr;
        }
    }
}
