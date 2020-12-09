using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// Device Grant Response.
    /// </summary>
    public class AddDeviceGrantResult
    {

        public List<Success> success { get; set; }
        public List<object> error { get; set; }
        
        public class Success
        {
            /// <summary>
            /// User Id.
            /// </summary>
            public string userId { get; set; }
            /// <summary>
            /// User Guid.
            /// </summary>
            public string userGuid { get; set; }
            /// <summary>
            /// Error Log.
            /// </summary>
            public string errorLog { get; set; }
        }
    }
}
