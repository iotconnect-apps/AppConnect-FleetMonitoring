

using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// Returns OTA update results.
    /// </summary>
    public class RecentOTAResult
    {
        /// <summary>
        /// OTA Update Item Guid.
        /// </summary>
        public string otaUpdateItemGuid { get; set; }
        /// <summary>
        /// Firmware Guid.
        /// </summary>
        public string firmwareGuid { get; set; }
        /// <summary>
        /// Hardware.
        /// </summary>
        public string hardware { get; set; }
        /// <summary>
        /// Status.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Upadted Date.
        /// </summary>
        public DateTime updatedDate { get; set; }
        /// <summary>
        /// Entity Name.
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// isRecursive.
        /// </summary>
        public bool isRecursive { get; set; }
        /// <summary>
        /// Update By Name.
        /// </summary>
        public string updatedByName { get; set; }
    }
}
