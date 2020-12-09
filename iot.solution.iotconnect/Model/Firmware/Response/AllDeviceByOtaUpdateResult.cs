using System;

namespace IoTConnect.Model
{
   public class AllDeviceByOtaUpdateResult
    {
        /// <summary>
        /// OTA Update Item Guid.
        /// </summary>
        public string otaUpdateItemGuid { get; set; }
        /// <summary>
        /// Updated Date.
        /// </summary>
        public DateTime updatedDate { get; set; }
        /// <summary>
        /// Device Guid.
        /// </summary>
        public string deviceGuid { get; set; }
        /// <summary>
        /// Device uniqueId.
        /// </summary>
        public string uniqueId { get; set; }
        /// <summary>
        /// Device Entity Name.
        /// </summary>
        public string entityName { get; set; }
    }
}
