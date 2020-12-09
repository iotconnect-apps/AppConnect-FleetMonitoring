

namespace IoTConnect.Model
{
    /// <summary>
    /// Update Device Twin Model.
    /// </summary>
   public class UpdateDeviceTwinModel
    {
        /// <summary>
        /// Twin Local Name. 
        /// </summary>
        public string localName { get; set; }
        /// <summary>
        /// Twin desired value.
        /// </summary>
        public string desiredValue { get; set; }
        /// <summary>
        /// Template Setting Guid.
        /// </summary>
        public string templateSettingGuid { get; set; }
    }
}
