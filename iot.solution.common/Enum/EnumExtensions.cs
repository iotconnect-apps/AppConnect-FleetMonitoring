using System;
using System.ComponentModel;

namespace iot.solution.common
{
    /// <summary>
    /// EnumExtensions
    /// </summary>
    static class EnumExtensions
    {
        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this System.Enum value) where T : Attribute
        {
            Type type = value.GetType();
            System.Reflection.MemberInfo[] memberInfo = type.GetMember(value.ToString());
            object[] attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes[0];
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        /// <summary>
        /// Converts to description.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToDescription(this System.Enum value)
        {
            DescriptionAttribute attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
