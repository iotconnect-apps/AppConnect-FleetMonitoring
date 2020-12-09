using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// All OTA Update Result.
    /// </summary>
   public class AllOTAUpdateResult
    {
        /// <summary>
        /// OTA Update Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Firmware Upgrade Guid.
        /// </summary>
        public string firmwareUpgradeGuid { get; set; }
        /// <summary>
        /// Firmware Guid.
        /// </summary>
        public string firmwareGuid { get; set; }
        /// <summary>
        /// Hardware.
        /// </summary>
        public string hardware { get; set; }
        /// <summary>
        /// Software.
        /// </summary>
        public string software { get; set; }
        /// <summary>
        /// isRecursive ?
        /// </summary>
        public bool isRecursive { get; set; }
        /// <summary>
        /// Schedule On
        /// </summary>
        public object scheduledOn { get; set; }
        /// <summary>
        /// Is Force Update.
        /// </summary>
        public bool isforceUpdate { get; set; }
        /// <summary>
        /// Created Date.
        /// </summary>
        public DateTime createdDate { get; set; }
        /// <summary>
        /// Created By.
        /// </summary>
        public string createdBy { get; set; }
        /// <summary>
        /// Updated Date.
        /// </summary>
        public DateTime updatedDate { get; set; }
        /// <summary>
        /// Updated By.
        /// </summary>
        public string updatedBy { get; set; }
        /// <summary>
        /// Entity Name.
        /// </summary>
        public string entityName { get; set; }
        /// <summary>
        /// Updated By Name.
        /// </summary>
        public string updatedByName { get; set; }
        /// <summary>
        /// Success.
        /// </summary>
        public string success { get; set; }
        /// <summary>
        /// Pending.
        /// </summary>
        public string pending { get; set; }
        /// <summary>
        /// Failed.
        /// </summary>
        public string failed { get; set; }
    }
}
