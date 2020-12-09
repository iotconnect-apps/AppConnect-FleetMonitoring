using System;
namespace IoTConnect.Model
{
    /// <summary>
    /// Returns All Firmware Results.
    /// </summary>
   public class AllFirmwareResult
    {
        /// <summary>
        /// Firmware Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string name { get; set; }
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
        public string createdby { get; set; }
        /// <summary>
        /// Updated Date.
        /// </summary>
        public DateTime updatedDate { get; set; }
        /// <summary>
        /// Release Count.
        /// </summary>
        public int releaseCount { get; set; }
        /// <summary>
        /// Draft Count.
        /// </summary>
        public int draftCount { get; set; }
    }
}
