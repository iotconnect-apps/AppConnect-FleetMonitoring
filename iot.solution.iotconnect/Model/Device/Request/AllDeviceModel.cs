using Newtonsoft.Json;

namespace IoTConnect.Model
{
    /// <summary>
    /// All Devices.
    /// </summary>
    public class AllDeviceModel : PagingModel
    {
        /// <summary>
        /// Gets or sets the custom field.
        /// </summary>
        /// <value>
        /// The custom field.
        /// </value>
        public bool? CustomField { get; set; }
        /// <summary>
        /// Template guid.
        /// </summary>
        [JsonProperty("templateGuid")]
        public string templateGuid { get; set; }
        /// <summary>
        /// Entity guid.
        /// </summary>
        [JsonProperty("entityGuid")]
        public string entityGuid { get; set; }
        /// <summary>
        /// Company guid.
        /// </summary>
        [JsonProperty("companyGuid")]
        public string companyGuid { get; set; }
    }
}
