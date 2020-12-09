

using System;
using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// Returns Firmware Details.
    /// </summary>
    public class FirmwareDetailsResult
    {
        /// <summary>
        /// Returns Firmware Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Description.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Hardware.
        /// </summary>
        public string hardware { get; set; }
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
        /// Updated by.
        /// </summary>
        public string updatedBy { get; set; }
        /// <summary>
        /// Firmware Upgrads.
        /// </summary>
        public List<Upgrade> Upgrades { get; set; }
    }

    public class Upgrade
    {
        /// <summary>
        /// Software.
        /// </summary>
        public string software { get; set; }
        /// <summary>
        /// Firmware Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// FileName.
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// Description.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Is Draft ?
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
