using System.Collections.Generic;
namespace IoTConnect.Model
{
   public class AllottedDeviceToUserResult
    {
        /// <summary>
        /// Success
        /// </summary>
        public List<Success> success { get; set; }
        /// <summary>
        /// Error.
        /// </summary>
        public List<object> error { get; set; }

        public class Success
        {
            /// <summary>
            /// User Guid
            /// </summary>
            public string userId { get; set; }
            /// <summary>
            /// device Guid.
            /// </summary>
            public string deviceGuid { get; set; }
            /// <summary>
            /// Error Log.
            /// </summary>
            public string errorLog { get; set; }
        }
    }
}
