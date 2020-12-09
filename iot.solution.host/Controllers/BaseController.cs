using iot.solution.common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using LogHandler = component.services.loghandler;
namespace host.iot.solution.Controllers
{
    [ProducesResponseType(typeof(UnauthorizeError), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ModelStateError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(GenericError), (int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType(typeof(NotFoundError), (int)HttpStatusCode.NotFound)]
    [ApiController]
    [ApiVersionNeutral]
    [Authorize(AuthenticationSchemes = "CompanyUser,Admin")]
    public class BaseController : ControllerBase
    {
        public BaseController()
        {

        }

        public string CurrentUserId
        {
            get
            {
                return User.Identity.Name;
            }
        }
        protected void LogException(Exception ex)
        {
            object actionArguments = null, userId = null;

            HttpContext.Items.TryGetValue("ActionRquestData", out actionArguments);
            HttpContext.Items.TryGetValue("ControllerName", out object controllerName);
            HttpContext.Items.TryGetValue("ActionName", out object actionName);

            LogHandler.Logger _logger = HttpContext.RequestServices.GetService<LogHandler.Logger>();

            if (ex is GenericCustomException || ex is NotFoundCustomException || ex is UnauthorizedCustomException)
            {
                _logger.WarningLog(ex.Message, actionArguments, errorCode: ErrorCode(ex), identity: Convert.ToString(userId), fileName: $"{controllerName}.cs", methodName: Convert.ToString(actionName));
            }
            else
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.GetType() == typeof(GenericCustomException) || ex.InnerException.GetType() == typeof(NotFoundCustomException) || ex.InnerException.GetType() == typeof(UnauthorizedCustomException))
                    {
                        _logger.WarningLog(ex.Message, actionArguments, errorCode: ErrorCode(ex), identity: Convert.ToString(userId), fileName: $"{controllerName}.cs", methodName: Convert.ToString(actionName));
                    }
                    else
                    {
                        _logger.ErrorLog(ex.InnerException.Message, ex, actionArguments, errorCode: ErrorCode(ex), identity: Convert.ToString(userId), fileName: $"{controllerName}.cs", methodName: Convert.ToString(actionName));
                    }
                }
                else
                {
                    _logger.ErrorLog(ex.Message, ex, actionArguments, errorCode: ErrorCode(ex), identity: Convert.ToString(userId), fileName: $"{controllerName}.cs", methodName: Convert.ToString(actionName));
                }
            }
        }

        private string ErrorCode(Exception ex)
        {
            switch (ex)
            {
                case GenericCustomException genericException:
                    return genericException.ErrorCode;
                case NotFoundCustomException notFoundException:
                    return notFoundException.ErrorCode;
                case UnauthorizedCustomException unauthorizedCustomException:
                    return unauthorizedCustomException.ErrorCode;
                case Exception systemException:
                    return "B0x005";
                default:
                    break;
            }
            return "";
        }
    }
}
