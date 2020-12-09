using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using component.services.loghandler;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.WebUtilities;
using Entity = iot.solution.entity;
using Newtonsoft.Json;

namespace host.iot.solution.Filter
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EnsureGuidParameterAttribute : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute,IActionFilter
    {
        public EnsureGuidParameterAttribute(string parameterName,string entityName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException($"'{nameof(parameterName)}' is required.");
            if (string.IsNullOrEmpty(entityName))
                throw new ArgumentException($"'{nameof(entityName)}' is required.");
            this.ParameterName = parameterName;
            this.EntityName = entityName;
        }

        public string ParameterName { get; set; }
        public string EntityName { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ControllerActionDescriptor actionDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;
            var attribute = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(EnsureGuidParameterAttribute), true).FirstOrDefault() as EnsureGuidParameterAttribute;

            // The attribute exists on the current action method
            if (attribute != null)
            {
                IDictionary<string, object> queryString = filterContext.ActionArguments;
                if (queryString != null && queryString.ContainsKey(attribute.ParameterName))
                {
                    Guid result;
                    // If the parameter is present and not guid
                    if (!string.IsNullOrEmpty(queryString[attribute.ParameterName].ToString()) && !Guid.TryParse(queryString[attribute.ParameterName].ToString(), out result))
                    {
                        var errorResult = new Entity.BaseResponse<bool>(false, "Invalid Guid for "+attribute.EntityName); 
                        ContentResult content = new ContentResult();
                        content.ContentType = "application/json";
                        content.Content = JsonConvert.SerializeObject(errorResult);
                        filterContext.Result = content;
                        //new  BadRequestObjectResult($"Invalid Guid for '{attribute.ParameterName}'.");
                        
                    }
                }
            }
        }
    }
   
}
