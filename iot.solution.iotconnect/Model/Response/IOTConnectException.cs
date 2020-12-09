using System.Collections.Generic;
using System.Net;

namespace IoTConnect.Model
{
    /// <summary>
    /// IotConnect Exception class.
    /// </summary>
    public class IoTConnectException :  System.Exception
    {
        /// <summary>
        /// Http Status code.
        /// </summary>
        public HttpStatusCode status { get; set; }
        /// <summary>
        /// IotConnect error message.
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Error list. Contains one or more IotConnect error(s).
        /// </summary>
        public List<ErrorItemModel> error = new List<ErrorItemModel>();
    } 
}
