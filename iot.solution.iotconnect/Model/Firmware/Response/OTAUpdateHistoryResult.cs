using System;
using System.Collections.Generic;
using System.Text;

namespace IoTConnect.Model
{
    /// <summary>
    /// OTA Update History.
    /// </summary>
   public class OTAUpdateHistoryResult
    {
        /// <summary>
        /// Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// OTA Update Guid.
        /// </summary>
        public string otaGuid { get; set; }
        /// <summary>
        /// Device Guid.
        /// </summary>
        public string deviceGuid { get; set; }
        /// <summary>
        /// Hardware.
        /// </summary>
        public string hardware { get; set; }
        /// <summary>
        /// Software.
        /// </summary>
        public string software { get; set; }
        /// <summary>
        /// Updated Date.
        /// </summary>
        public DateTime updatedDate { get; set; }
    }
}
