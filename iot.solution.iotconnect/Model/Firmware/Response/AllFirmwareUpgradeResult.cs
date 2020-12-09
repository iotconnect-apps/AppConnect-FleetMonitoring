using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// All Firmware.
    /// </summary>
   public class AllFirmwareUpgradeResult
    {
        /// <summary>
        /// Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Firmware Guid.
        /// </summary>
        public string firmwareGuid { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string firmwareName { get; set; }
        /// <summary>
        /// Description.
        /// </summary>
        public string firmwareDescription { get; set; }
        /// <summary>
        /// File Name.
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// Is Draft ?.
        /// </summary>
        public string isDraft { get; set; }
        /// <summary>
        /// Hardware.
        /// </summary>
        public string hardware { get; set; }
        /// <summary>
        /// Software.
        /// </summary>
        public string software { get; set; }
        /// <summary>
        /// Created Date.
        /// </summary>
        public DateTime createdDate { get; set; }
        /// <summary>
        /// Created by.
        /// </summary>
        public string createdby { get; set; }
        /// <summary>
        /// Updated Date.
        /// </summary>
        public DateTime updatedDate { get; set; }
        /// <summary>
        /// Updated By Name.
        /// </summary>
        public string updatedByName { get; set; }
        /// <summary>
        /// File Url.
        /// </summary>
        public string fileUrl { get; set; }
    }
}
