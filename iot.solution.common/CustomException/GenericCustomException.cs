using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace iot.solution.common
{
    public class GenericCustomException : Exception
    {
        public string ErrorCode { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }

        public GenericCustomException(string message) : base(message)
        { }

        public GenericCustomException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

    }
}
