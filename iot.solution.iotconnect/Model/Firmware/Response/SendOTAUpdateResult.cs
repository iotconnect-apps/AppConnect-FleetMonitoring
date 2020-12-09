using System;
using System.Collections.Generic;
using System.Text;

namespace IoTConnect.Model
{
    /// <summary>
    /// Send OTA Update Result
    /// </summary>
   public class SendOTAUpdateResult
    {
        /// <summary>
        /// Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Device Guid.
        /// </summary>
        public string deviceGuid { get; set; }
        /// <summary>
        /// Available Date.
        /// </summary>
        public string availableDate { get; set; }
        /// <summary>
        /// Firmware upgrade Guid.
        /// </summary>
        public string firmwareUpgradeGuid { get; set; }
        /// <summary>
        /// Event Place Holders.
        /// </summary>
        public string eventPlaceHolders { get; set; }
    }
}
