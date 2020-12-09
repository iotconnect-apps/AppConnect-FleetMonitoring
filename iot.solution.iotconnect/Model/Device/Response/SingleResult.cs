using System;
using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// Device result.
    /// </summary>
    public class SingleResult
    {
        /// <summary>
        /// Device guid.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Device Unique Id.
        /// </summary>
        public string UniqueId { get; set; }
        /// <summary>
        /// Display name.
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
        /// Device Status.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// firmware upgrade guid.
        /// </summary>
        public string FirmwareUpgradeGuid { get; set; }
        /// <summary>
        /// Device template guid.
        /// </summary>
        public string DeviceTemplateGuid { get; set; }
        /// <summary>
        /// Device entity guid.
        /// </summary>
        public string EntityGuid { get; set; }
        /// <summary>
        /// Certificate guid.
        /// </summary>
        public string CertificateGuid { get; set; }
        /// <summary>
        /// Certificate Name.
        /// </summary>
        public string CertificateName { get; set; }
        /// <summary>
        /// Certificate Type.
        /// </summary>
        public string CertificateType { get; set; }
        /// <summary>
        /// Device Template Auth Type.
        /// </summary>
        public int DeviceTemplateAuthType { get; set; }
        /// <summary>
        /// Entity Name.
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// Device Template Name.
        /// </summary>
        public string DeviceTemplateName { get; set; }
        /// <summary>
        /// Note.
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Device Image.
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// Company Id.
        /// </summary>
        public string CpId { get; set; }
        /// <summary>
        /// Is Device Acquired?
        /// </summary>
        public int IsAcquired { get; set; }
        /// <summary>
        /// Firmware Avail.
        /// </summary>
        public int FirmwareAvail { get; set; }
        /// <summary>
        /// Parent Device guid.
        /// </summary>
        public string ParentDeviceGuid { get; set; }
        /// <summary>
        /// Tag.
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// Endorsement Key.
        /// </summary>
        public string EndorsementKey { get; set; }
        /// <summary>
        /// Parent Device Unique Id.
        /// </summary>
        public string ParentDeviceUniqueId { get; set; }
        /// <summary>
        /// Is Parent Acquired?
        /// </summary>
        public int IsParentAcquired { get; set; }
        /// <summary>
        /// Is Device Simulator On?
        /// </summary>
        public int IsSimulatorOn { get; set; }
        /// <summary>
        /// Type 2 support available?
        /// </summary>
        public bool IsType2Support { get; set; }
        /// <summary>
        /// Edge Support available?
        /// </summary>
        public bool IsEdgeSupport { get; set; }
        /// <summary>
        /// Is allotted Device?
        /// </summary>
        public int IsAllottedDevice { get; set; }
        /// <summary>
        /// Allotted Device Permission.
        /// </summary>
        public AllottedDevicePermission AllottedDevicePermission { get; set; }
        /// <summary>
        /// Image URL.
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// Device Properties.
        /// </summary>
        public List<object> Properties { get; set; }
        /// <summary>
        /// Device Broker guid.
        /// </summary>
        public string BrokerGuid { get; set; }
    }

    public class AllottedDevicePermission
    {
        public bool? View { get; set; }
        public bool? Operation { get; set; }
    }
}
