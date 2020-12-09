using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// Base Response.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResponse<T>
    {
        /// <summary>
        /// Data. Type of T.
        /// </summary>
        public T Data { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }
        /// <summary>
        /// Parameters.
        /// </summary>
        public Dictionary<string, object> Params { get; set; }
    }
}
