
using System;

namespace IoTConnect.Common
{
   public class ErrorHandlerModel
    {
        public DateTime date { get; set; }
        public int severity { get; set; }
        public string logger { get; set; }
        public string file { get; set; }
        public string method { get; set; }
        public string identity { get; set; }
        public string message { get; set; }
        public string stackTrace { get; set; }
        public string exception { get; set; }
        public string applicationCode { get; set; }
        public string errorCode { get; set; }
        public string messageData { get; set; }
        public string componentStatus { get; set; }
    }
}
