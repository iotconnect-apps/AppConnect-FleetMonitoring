using System;
using System.Collections.Generic;

namespace component.services.loghandler.Common
{
    /// <summary>
    /// CommonMethods
    /// </summary>
    public static class CommonMethods
    {
        /// <summary>
        /// Gets the value from dynamic.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static string GetValueFromDynamic(dynamic requestModel, string propertyName)
        {
            try
            {
                if (requestModel != null)
                {
                    Type type = requestModel.GetType();
                    bool isDict = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
                    if (isDict)
                    {
                        foreach (dynamic item in requestModel)
                        {
                            try
                            {
                                if (item.Key.GetType() == typeof(string) && item.Value.GetType() == typeof(string))
                                {
                                    if (Convert.ToString(propertyName).ToLower() == "applicationcode")
                                    {
                                        if (Convert.ToString(item.Key).ToLower() == "code" || Convert.ToString(item.Key).ToLower() == "applicationcode")
                                        {
                                            return Convert.ToString(item.Value);
                                        }
                                    }
                                    else if (Convert.ToString(item.Key).ToLower() == Convert.ToString(propertyName).ToLower())
                                    {
                                        return Convert.ToString(item.Value);
                                    }
                                }
                                else if ((item.Key.GetType() == typeof(string) && item.Value.GetType() == typeof(Guid))
                                    && Convert.ToString(item.Key).ToLower() == Convert.ToString(propertyName).ToLower())
                                {
                                    return Convert.ToString(item.Value);
                                }
                                else
                                {
                                    string propertyValue = item.GetType().GetProperty(propertyName).GetValue(item, null);
                                    if (!String.IsNullOrEmpty(propertyValue))
                                    {
                                        return propertyValue;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                            }
                        }

                        return string.Empty;
                    }
                    else
                    {
                        return Convert.ToString(requestModel.GetType().GetProperty(propertyName).GetValue(requestModel, null));
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts to filename.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToFileName(this string value)
        {
            try
            {
                if (value.Contains(@"\"))
                {
                    return value.Substring(value.LastIndexOf(@"\") + 1);
                }              
            }
            catch (Exception ex)
            {
                return value;
            }

            return value;
        }

        /// <summary>
        /// Tries the parse boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryParseBoolean(this string value)
        {
            Boolean parsedValue = false;
            bool res = Boolean.TryParse(value, out parsedValue);
            return res;
        }

        /// <summary>
        /// Tries the parse numeric.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryParseNumeric(this string value)
        {
            int num1;
            bool res = int.TryParse(value, out num1);
            return res;
        }
    }
}
