using component.exception.Common;
using System.Net;

namespace iot.solution.common
{
    public static class Authentication
    {
        public static void RaiseAccessDeniedException()
        {
            string message = "Access Denied.";
            throw new GenericCustomException(message)
            {
                ErrorCode = AuthenticationErrorCode.AccessDenied.ToDescription(),
                HttpStatusCode = HttpStatusCode.Forbidden
            };
        }

        public static void RaiseUnauthorizedUserException()
        {
            string message = "Unauthorized user.";
            throw new GenericCustomException(message)
            {
                ErrorCode = AuthenticationErrorCode.UnauthorizedUser.ToDescription(),
                HttpStatusCode = HttpStatusCode.Unauthorized
            };
        }

        public static void RaiseInvalidAPIKeyException()
        {
            string message = "Invalid API Key.";
            throw new GenericCustomException(message)
            {
                ErrorCode = AuthenticationErrorCode.InvalidAPIKey.ToDescription(),
                HttpStatusCode = HttpStatusCode.Forbidden
            };
        }

        public static void RaiseTokenExpiredException()
        {
            string message = "Token expired.";
            throw new GenericCustomException(message)
            {
                ErrorCode = AuthenticationErrorCode.TokenExpired.ToDescription(),
                HttpStatusCode = HttpStatusCode.Unauthorized
            };
        }
    }
}
