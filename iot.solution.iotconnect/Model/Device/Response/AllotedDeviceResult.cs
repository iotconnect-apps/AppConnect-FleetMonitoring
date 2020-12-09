

using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// Alloted device Result.
    /// </summary>
    public class AllotedDeviceResult
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
        /// Is Connected device ?.
        /// </summary>
        public bool isConnected { get; set; }
        /// <summary>
        /// Last Activity Date.
        /// </summary>
        public DateTime? lastactivityDate { get; set; }
        /// <summary>
        /// Is Active ?
        /// </summary>
        public bool isActive { get; set; }
        /// <summary>
        /// Firmware upgrade Guid.
        /// </summary>
        public string firmwareupgradeGuid { get; set; }
        /// <summary>
        /// Device Template Guid.
        /// </summary>
        public string deviceTemplateGuid { get; set; }
        /// <summary>
        /// Auth Type.
        /// </summary>
        public int authType { get; set; }
        /// <summary>
        /// Device Info.
        /// </summary>
        public string deviceInfo { get; set; }
        /// <summary>
        /// Device Template Name.
        /// </summary>
        public string deviceTemplateName { get; set; }
        /// <summary>
        /// Reporting To.
        /// </summary>
        public string reportingTo { get; set; }
        /// <summary>
        /// Is Acquired ?
        /// </summary>
        public bool isAcquired { get; set; }
        /// <summary>
        /// Child device count.
        /// </summary>
        public int childDeviceCount { get; set; }
        /// <summary>
        /// Is type 2 Support ?
        /// </summary>
        public bool isType2Support { get; set; }
        /// <summary>
        /// User Device Permission Guid.
        /// </summary>
        public string userDevicePermissionGuid { get; set; }
        /// <summary>
        /// Device Permission.
        /// </summary>
        public AllotedPermission permission { get; set; }
    }

    public class AllotedPermission {
        /// <summary>
        /// Is View ?.
        /// </summary>
        public bool View { get; set; }
        /// <summary>
        /// Is Manage ?
        /// </summary>
        public bool Operation { get; set; }
    }

    public class DeviceCounterResult
    {
        public int active { get; set; }
        public int inActive { get; set; }
        public int connected { get; set; }
        public int disConnected { get; set; }
        public int acquired { get; set; }
        public int available { get; set; }
        public int total { get; set; }
    }

    public class DeviceTelemetryData
    {
        public string templateAttributeGuid { get; set; }
        public string attributeName { get; set; }
        public string attributeValue { get; set; }
        public DateTime deviceUpdatedDate { get; set; }
        public int notificationCount { get; set; }
        public object aggregateType { get; set; }
        public string DataType { get; set; }
        public object aggregateTypeValues { get; set; }
    }
    public class DeviceConnectionStatus
    {
        public bool IsConnected { get; set; }
    }
    public class DeviceCounterByEntityResult
    {
        public DeviceCounterResult counters { get; set; }
    }
}
