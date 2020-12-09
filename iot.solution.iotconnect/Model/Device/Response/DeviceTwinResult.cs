using System;
namespace IoTConnect.Model
{
    /// <summary>
    /// Device Twin Result.
    /// </summary>
   public class DeviceTwinResult
    {
        /// <summary>
        /// twin guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Device Company Guid.
        /// </summary>
        public string companyGuid { get; set; }
        /// <summary>
        /// Template Setting Guid.
        /// </summary>
        public string templateSettingGuid { get; set; }
        /// <summary>
        /// Device Guid.
        /// </summary>
        public string deviceGuid { get; set; }
        /// <summary>
        /// Twin Created Date.
        /// </summary>
        public DateTime createdDate { get; set; }
        /// <summary>
        /// Twin SDK Date.
        /// </summary>
        public object sdkDate { get; set; }
        /// <summary>
        /// Twin gateway Date.
        /// </summary>
        public object gatewayDate { get; set; }
        /// <summary>
        /// Device Date.
        /// </summary>
        public object deviceDate { get; set; }
        /// <summary>
        /// Desired Value.
        /// </summary>
        public object desiredValue { get; set; }
        /// <summary>
        /// Reported Value.
        /// </summary>
        public object reportedValue { get; set; }
        /// <summary>
        /// Desired Update Date.
        /// </summary>
        public DateTime desiredUpdatedDate { get; set; }
        /// <summary>
        /// Reported Update Date.
        /// </summary>
        public DateTime reportedUpdatedDate { get; set; }
        /// <summary>
        /// Desired Version.
        /// </summary>
        public object desiredVersion { get; set; }
        /// <summary>
        /// Twin Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Local Name.
        /// </summary>
        public string localName { get; set; }
        /// <summary>
        /// Data Type Name.
        /// </summary>
        public string dataTypeName { get; set; }
        /// <summary>
        /// Data Type Guid.
        /// </summary>
        public string daTypeGuid { get; set; }
    }
}
