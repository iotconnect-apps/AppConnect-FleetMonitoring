

namespace IoTConnect.Model
{
    /// <summary>
    /// All Child Device Result.
    /// </summary>
    public class AllChildDeviceResult
    {
        /// <summary>
        /// Device Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Device Unique Id.
        /// </summary>
        public string uniqueId { get; set; }
        /// <summary>
        /// Device Display Name.
        /// </summary>
        public string displayName { get; set; }
        /// <summary>
        /// Device Tag.
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// Is Device Connected ?
        /// </summary>
        public bool isConnected { get; set; }
        /// <summary>
        /// Device last Activity Date.
        /// </summary>
        public object lastactivityDate { get; set; }
        /// <summary>
        /// Is Active Device ?
        /// </summary>
        public bool isActive { get; set; }
        /// <summary>
        /// Firmware Guid.
        /// </summary>
        public object firmwareupgradeGuid { get; set; }
        /// <summary>
        /// Device Template Guid.
        /// </summary>
        public string deviceTemplateGuid { get; set; }
        /// <summary>
        /// Device Template name.
        /// </summary>
        public string deviceTemplateName { get; set; }
        /// <summary>
        /// Reporting To.
        /// </summary>
        public string reportingTo { get; set; }
        /// <summary>
        /// Device Entity Name.
        /// </summary>
        public string entityName { get; set; }
        /// <summary>
        /// Is Device Acquired ?
        /// </summary>
        public bool isAcquired { get; set; }
    }
}
