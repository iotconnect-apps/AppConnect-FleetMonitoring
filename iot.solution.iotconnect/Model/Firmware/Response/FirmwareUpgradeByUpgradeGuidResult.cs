using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// Firmware Upgrade
    /// </summary>
    public class FirmwareUpgradeByUpgradeGuidResult
    {
        /// <summary>
        /// Firmware Guid.
        /// </summary>
        public string firmwareguid { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Hardware.
        /// </summary>
        public string hardware { get; set; }
        /// <summary>
        /// Firmware Upgrade Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Software.
        /// </summary>
        public string software { get; set; }
        /// <summary>
        /// Description.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// File Name.
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// Is Draft ?.
        /// </summary>
        public string isDraft { get; set; }
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
        /// File Url.
        /// </summary>
        public string fileUrl { get; set; }
    }
}
