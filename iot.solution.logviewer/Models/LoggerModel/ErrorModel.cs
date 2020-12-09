using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace component.services.logger.viewer.Models.LoggerModel
{
    public class ErrorModel
    {
        public ErrorModel()
        {
            ErrorList = new List<Error>();
            HoursList = new List<SelectListItem>();
            SeverityList = new List<SelectListItem>();
            ApplicationList = new List<SelectListItem>();
        }
        public IList<Error> ErrorList { get; set; }
        public string Logger { get; set; }
        public string Severity { get; set; }
        public IList<SelectListItem> HoursList { get; set; }
        public IList<SelectListItem> SeverityList { get; set; }
        public IList<SelectListItem> ApplicationList { get; set; }
        public int Hours { get; set; }
        public int ConStringId { get; set; }
        public string ApplicationCode { get; set; }
    }

    public class Error
    {
        public string LogId { get; set; }
        public string ApplicationCode { get; set; }
        public string ComponentStatus { get; set; }
        public string ErrorCode { get; set; }
        public string Exception { get; set; }
        public string LogFile { get; set; }
        public string Identity { get; set; }
        public DateTime LogDate { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string MessageData { get; set; }
        public string Method { get; set; }
        public string Severity { get; set; }
        public string StackTrace { get; set; }
    }
}