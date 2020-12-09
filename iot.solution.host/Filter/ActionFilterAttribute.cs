using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using component.services.loghandler;

namespace host.iot.solution.Filter
{
    public class ActionFilterAttribute : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute, IActionFilter
    {
        private ActionExecutingContext _request { get; set; }
        
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _request = context;
            return base.OnActionExecutionAsync(context, next);
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            //string operationId = Convert.ToString(actionContext.RouteData.Values["action"]);

            //if (!string.IsNullOrEmpty(operationId))
            //{
            //    actionContext.HttpContext.Response.Headers.Add("X-Operation-Id", operationId);
            //}

            //// Add debug logs for each action call start
            ControllerActionDescriptor controllerActionDescriptor = actionContext.ActionDescriptor as ControllerActionDescriptor;
            actionContext.HttpContext.Items.Add(new KeyValuePair<object, object>("ActionRquestData", actionContext.ActionArguments));
            actionContext.HttpContext.Items.Add(new KeyValuePair<object, object>("ControllerName", controllerActionDescriptor.ControllerName));
            actionContext.HttpContext.Items.Add(new KeyValuePair<object, object>("ActionName", controllerActionDescriptor.ActionName));
            if (!actionContext.ModelState.IsValid)
            {
                if (actionContext.ModelState.Keys.Where(t => !string.IsNullOrEmpty(t)).Count() > 0)
                {
                    actionContext.Result = new ValidationFailedResult(actionContext.ModelState);
                }
            }
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Task.Run(() =>
            {
                LogRequestAsDebugLog(context);
            });

            base.OnActionExecuted(context);
        }

        public void LogRequestAsDebugLog(ActionExecutedContext actionContext)
        {
            try
            {
                Logger _logger = actionContext.HttpContext.RequestServices.GetService<Logger>();
                if (_logger != null)
                {
                    object responseValue = null;
                    if (actionContext.Result != null && actionContext.Result is ObjectResult)
                    {
                        responseValue = ((ObjectResult)actionContext.Result).Value;
                    }

                    ControllerActionDescriptor controllerActionDescriptor = _request.ActionDescriptor as ControllerActionDescriptor;
                    Dictionary<string, object> requestResponseData = new Dictionary<string, object>
                    {
                        { "ApiCall", actionContext.HttpContext.Request.Path.ToString() },
                        { "Method", actionContext.HttpContext.Request.Method.ToString() },
                        { "Request", _request.ActionArguments },
                        { "Response", responseValue }
                    };

                    //ClaimsIdentity claimsIdentity = actionContext.HttpContext.User.Identity as ClaimsIdentity;
                    //string identityUserId = claimsIdentity.FindFirst(IdpClaim.UserId.ToDescription())?.Value;
                    //actionContext.HttpContext.Items.TryGetValue(nameof(IdpClaim.UserId), out object identityUserId);

                    _logger.DebugLog(null, requestResponseData, identity: null, fileName: $"{controllerActionDescriptor.ControllerTypeInfo.Name}.cs", methodName: controllerActionDescriptor.ActionName);
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}
