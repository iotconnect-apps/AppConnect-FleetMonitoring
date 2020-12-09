using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace component.services.logger.viewer.Common.Extension
{
    public static class ExtentionMethods
    {
        /// <summary>
        /// Tries the get method information.
        /// </summary>
        /// <param name="apiDescription">The API description.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <returns></returns>
        public static bool TryGetMethodInfo(ApiDescription apiDescription, out MethodInfo methodInfo)
        {
            ControllerActionDescriptor controllerActionDescriptor = apiDescription.ActionDescriptor as ControllerActionDescriptor;

            methodInfo = controllerActionDescriptor?.MethodInfo;

            return (methodInfo != null);
        }

        public static string ToDescription(this Enum value)
        {
            DescriptionAttribute attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            Type type = value.GetType();
            MemberInfo[] memberInfo = type.GetMember(value.ToString());
            object[] attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes[0];
        }
    }


}
