using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// Device Result.
    /// </summary>
    public class AllDeviceResult
    {
        /// <summary>
        /// Device guid.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Device UniqueId.
        /// </summary>
        public string UniqueId { get; set; }
        /// <summary>
        /// Device Display name.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Is Device Connected?
        /// </summary>
        public bool IsConnected { get; set; }
        /// <summary>
        /// Device Last Activity Date.
        /// </summary>
        public DateTime? LastActivityDate { get; set; }
        /// <summary>
        /// Device status(Active/Inactive).
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Firmware upgrade guid.
        /// </summary>
        public string FirmwareUpgradeGuid { get; set; }
        /// <summary>
        /// Device Temaplate guid.
        /// </summary>
        public string DeviceTemplateGuid { get; set; }
        /// <summary>
        /// Device Authentication Type(s).
        /// 1 = Key.
        /// 2 = CA Signed Certificate.
        /// 3 = Self Signed Certificate.
        /// 4 = TPM.
        /// </summary>
        public int AuthType { get; set; }
        /// <summary>
        /// Device Information.
        /// </summary>
        public object DeviceInfo { get; set; }
        /// <summary>
        /// Device Template name.
        /// </summary>
        public string DeviceTemplateName { get; set; }
        /// <summary>
        /// Reporting To entity name.
        /// </summary>
        public string ReportingTo { get; set; }
        /// <summary>
        /// Entity Name.
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// Is Device Acquired?
        /// </summary>
        public bool IsAcquired { get; set; }
        /// <summary>
        /// child Device count.
        /// </summary>
        public int ChildDeviceCount { get; set; }
        /// <summary>
        /// Is type2 supported Device?
        /// </summary>
        public bool IsType2Support { get; set; }
        /// <summary>
        /// Is Device Allotted?
        /// </summary>
        public int IsAllottedDevice { get; set; }
        /// <summary>
        /// Allotted Device Permission.
        /// </summary>
        public object AllottedDevicePermission { get; set; }
        /// <summary>
        /// Child Device count.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Is Device shared?
        /// </summary>
        public bool IsSharedDevice { get; set; }
    }
}
