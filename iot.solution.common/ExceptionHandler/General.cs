using FluentValidation.Results;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace iot.solution.common
{
    /// <summary>
    /// 
    /// </summary>
    public static class General
    {
        public static void RaiseHttpVersionNotSupportedException(string apiVersion)
        {
            string message = "Api version {0} does not exist. Please verify and try again.";
            throw new GenericCustomException(message, apiVersion)
            {
                ErrorCode = GeneralErrorCode.HttpVersionNotSupported.ToDescription(),
                HttpStatusCode = HttpStatusCode.HttpVersionNotSupported
            };
        }
        public static void RaiseRequestTimeoutException()
        {
            string message = "Request timeout.";
            throw new GenericCustomException(message)
            {
                ErrorCode = GeneralErrorCode.RequestTimeout.ToDescription(),
                HttpStatusCode = HttpStatusCode.RequestTimeout
            };
        }
        public static void RaiseBadRequestException()
        {
            string message = "Bad request.";
            throw new GenericCustomException(message)
            {
                ErrorCode = GeneralErrorCode.BadRequest.ToDescription(),
                HttpStatusCode = HttpStatusCode.BadRequest
            };
        }
        public static string ReturnCustomErrorResponse(GenericCustomException ex)
        {
            return JsonConvert.SerializeObject(new ComponentErrorResponse
            {
                Code = ex.ErrorCode.ToString(),
                Msg = ex.Message
            });
        }
        public static string ReturnSystemErrorResponse(System.Exception ex)
        {
            string message = "Operational error.";
            return JsonConvert.SerializeObject(new ComponentErrorResponse
            {
                Code = BusinessErrorCode.OperationalError.ToDescription(),
                Msg = message
            });
        }
        public static string ReturnImageResizeErrorResponse(string imageVesions)
        {
            string message = "Uploaded image is not valid for size {0}";
            throw new GenericCustomException(message, imageVesions)
            {
                ErrorCode = BusinessErrorCode.InvalidData.ToDescription(),
                HttpStatusCode = HttpStatusCode.PreconditionFailed
            };
        }
        public static string ReturnImagesizeErrorResponse(string imageVesions)
        {
            string message = "Uploaded image size is not valid.";
            throw new GenericCustomException(message, imageVesions)
            {
                ErrorCode = BusinessErrorCode.InvalidData.ToDescription(),
                HttpStatusCode = HttpStatusCode.PreconditionFailed
            };
        }
        public static string ReturnImageFormatNotValid(string extension)
        {
            string message = "Uploaded image extension {0} is not valid.";
            throw new GenericCustomException(message, extension)
            {
                ErrorCode = BusinessErrorCode.InvalidData.ToDescription(),
                HttpStatusCode = HttpStatusCode.PreconditionFailed
            };
        }
        public static void EmptyDateSpaceSchedule()
        {
            string message = "Invalid date.";
            throw new GenericCustomException(message)
            {
                ErrorCode = BusinessErrorCode.EntityCanNotBeEmpty.ToDescription(),
                HttpStatusCode = HttpStatusCode.PreconditionFailed
            };
        }
        public static string MissingFieldException()
        {
            string message = "Required column missing.";
            throw new GenericCustomException(message)
            {
                ErrorCode = BusinessErrorCode.InvalidData.ToDescription(),
                HttpStatusCode = HttpStatusCode.PreconditionFailed
            };
        }
        public static string CustomErrorMessage(string message, HttpStatusCode statusCode)
        {
            throw new GenericCustomException(message)
            {
                ErrorCode = GeneralErrorCode.BadRequest.ToDescription(),
                HttpStatusCode = statusCode
            };
        }
        public static string RaiseConfigurationMissingException(string entity)
        {
            string message = "Required Configuration Missing For {0}.";
            throw new GenericCustomException(message, entity)
            {
                ErrorCode = GeneralErrorCode.ConfigurationMissing.ToDescription(),
                HttpStatusCode = HttpStatusCode.InternalServerError
            };
        }
        public static void RaiseThirdPartyServiceNotImplemented(string entity)
        {
            string message = $"Third party {entity} service not implemented in system.";
            throw new GenericCustomException(message)
            {
                ErrorCode = GeneralErrorCode.ThirdPartyServiceNotImplemented.ToDescription(),
                HttpStatusCode = HttpStatusCode.NotFound
            };
        }
        public static void RaiseThirdPartyOperationFailError(string entity)
        {
            string message = $"Operation Failed for {entity}.";
            throw new GenericCustomException(message)
            {
                ErrorCode = GeneralErrorCode.ThirdPartyOperationFailError.ToDescription(),
                HttpStatusCode = HttpStatusCode.InternalServerError
            };
        }
        public static string ReturnSizeNotValidForOrignalImageErrorResponse()
        {
            string message = "Image size should not be multiple while uploading original image.";
            throw new GenericCustomException(message)
            {
                ErrorCode = BusinessErrorCode.InvalidData.ToDescription(),
                HttpStatusCode = HttpStatusCode.PreconditionFailed
            };
        }
        public static GenericCustomException RaiseCustomErrorMessage(int statusCode, string message)
        {
            throw new GenericCustomException(message)
            {
                ErrorCode = BusinessErrorCode.OperationalError.ToDescription(),
                HttpStatusCode = (HttpStatusCode)statusCode
            };
        }
        public static void RaiseThirdPartyServiceTokenNull(string entity)
        {
            string message = $"Third party {entity} service token is null.";
            throw new GenericCustomException(message)
            {
                ErrorCode = GeneralErrorCode.ThirdPartyServiceTokenNull.ToDescription(),
                HttpStatusCode = HttpStatusCode.NotFound
            };
        }
        public static void RaiseCannotEditArchiveDocumentException()
        {
            string message = "You can not update archived document";
            throw new GenericCustomException(message)
            {
                ErrorCode = BusinessErrorCode.EntityCanNotBeEmpty.ToDescription(),
                HttpStatusCode = HttpStatusCode.PreconditionFailed
            };
        }
        public static void RaiseCannotEditVersionOfArchiveDocumentException()
        {
            string message = "You can not update archived document version";
            throw new GenericCustomException(message)
            {
                ErrorCode = BusinessErrorCode.EntityCanNotBeEmpty.ToDescription(),
                HttpStatusCode = HttpStatusCode.PreconditionFailed
            };
        }
        public static string ReturnCanNotDeleteDefaultDocumentVersion()
        {
            string message = "You can not delete default document version.";
            throw new GenericCustomException(message)
            {
                ErrorCode = BusinessErrorCode.InvalidData.ToDescription(),
                HttpStatusCode = HttpStatusCode.PreconditionFailed
            };
        }
        public static void RaiseCustomErrors(HttpStatusCode httpStatusCode, List<ComponentErrorResponse> errors)
        {
            //var failures = errors.Select(x => new ValidationFailure(x.Code, x.Msg)).ToList();
            var failures = errors.Select(x => new ValidationFailure(x.Code, x.Msg) { ErrorCode = x.Code } ).ToList();
            throw new GenericCustomException(failures.FirstOrDefault().ErrorMessage)
            {
                HttpStatusCode = httpStatusCode
            };
        }
        public static void RaiseThirdPartyOperationFailError(string message, HttpStatusCode httpStatusCode)
        {
            throw new GenericCustomException(message)
            {
                ErrorCode = GeneralErrorCode.ThirdPartyOperationFailError.ToDescription(),
                HttpStatusCode = httpStatusCode
            };
        }
    }
}
