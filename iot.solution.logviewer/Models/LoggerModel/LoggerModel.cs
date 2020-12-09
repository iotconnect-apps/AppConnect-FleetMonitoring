using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace component.services.logger.viewer.Models.LoggerModel
{
    public class LoggerModel
    {
        public LoggerModel()
        {
            LoggerList = new List<Loggers>();
            SeverityList = new List<severities>();
            HoursList = new List<SelectListItem>();
            ConnectionList = new List<SelectListItem>();
            ApplicationList = new List<SelectListItem>();
        }
        public IList<Loggers> LoggerList { get; set; }
        public IList<severities> SeverityList { get; set; }
        public IList<SelectListItem> HoursList { get; set; }
        public int Hours { get; set; }
        public int ConStringId { get; set; }
        public IList<SelectListItem> ConnectionList { get; set; }
        public IList<SelectListItem> ApplicationList { get; set; }
        public string ApplicationCode { get; set; }
    }
}

public class Loggers
{
    public string Logger { get; set; }
    public int LoggerCount { get; set; }
}
public class severities
{
    public string Logger { get; set; }
    public string Severity { get; set; }
    public int SeverityCount { get; set; }
}
