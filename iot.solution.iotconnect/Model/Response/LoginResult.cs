using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// IotConnect Login Result class.
    /// </summary>
    public class LoginResult
    { 
        /// <summary>
        /// Http Status code.
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// Data.
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// IotConnect response message.
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// IotConnect Token Type (Basic/Bearer).
        /// </summary>
        public string token_type { get; set; }

        /// <summary>
        /// IotConnect Bearer token.
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// IotConnect Refresh Token.
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// Token expiry date time. 
        /// </summary>
        public long expires_in { get; set; }
    }

    /// <summary>
    /// Data Response class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataResponse<T>
    {
        /// <summary>
        /// Initializes new class of IoTConnect.CommonProvider.Model.Response.DataResponse<T> class
        /// </summary>
        /// <param name="value">object value.</param>
        public DataResponse(T value)
        {
            data = value;            
        }

        /// <summary>
        /// Http Status code.
        /// </summary>
        public bool status { get; set; }
        
        /// <summary>
        /// Response Data.
        /// </summary>
        public T data { get; set; }

        /// <summary>
        /// IotConnect response message.
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Error list. Contains one or more IotConnect error(s).
        /// </summary>
        public List<ErrorItemModel> errorMessages = new List<ErrorItemModel>();

    }

}
