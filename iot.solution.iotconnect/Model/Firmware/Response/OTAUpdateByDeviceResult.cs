
namespace IoTConnect.Model
{
    /// <summary>
    /// OTA update By Device Guid.
    /// </summary>
   public class OTAUpdateByDeviceResult
    {
        /// <summary>
        /// Ota update Guid.
        /// </summary>
        public string otaUpdateItemGuid { get; set; }
        /// <summary>
        /// Firmware Guid.
        /// </summary>
        public string firmwareGuid { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Hardware.
        /// </summary>
        public string hardware { get; set; }
        /// <summary>
        /// Software.
        /// </summary>
        public string software { get; set; }
        /// <summary>
        /// Firmware Upgrade Guid.
        /// </summary>
        public string firmwareUpgradeGuid { get; set; }
        /// <summary>
        /// File Name.
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// Status.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Device Guid.
        /// </summary>
        public string deviceGuid { get; set; }
        /// <summary>
        /// File URL.
        /// </summary>
        public string fileUrl { get; set; }
    }
}
