

namespace IoTConnect.Model
{
    /// <summary>
    /// Device Permission Result.
    /// </summary>
   public class DeviceGrantResult
    {
        /// <summary>
        /// Device Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// User Email.
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// User Guid.
        /// </summary>
        public string userGuid { get; set; }
        /// <summary>
        /// Reporting Group Guid.
        /// </summary>
        public object reportingGroupGuid { get; set; }
        /// <summary>
        /// User Device Permission Guid.
        /// </summary>
        public string userDevicePermissionGuid { get; set; }
        /// <summary>
        /// Permission.
        /// </summary>
        public Permission permission { get; set; }
    }

    
}
