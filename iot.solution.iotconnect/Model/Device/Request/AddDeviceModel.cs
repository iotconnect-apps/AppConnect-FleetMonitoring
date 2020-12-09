using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Add Device.
    /// </summary>
    public class AddDeviceModel
    {
        /// <summary>
        /// Device Display Name.
        /// </summary>
        [JsonProperty("displayName")]
        [Required(ErrorMessage = "DisplayName is required")]
        public string DisplayName { get; set; }
        /// <summary>
        /// Device Unique Id. IotConnect will throw an error if Unique Id is already exist.
        /// </summary>
        [Required(ErrorMessage = "UniqueId is required")]
        public string uniqueId { get; set; }
        
        internal string firmwareUpgradeGuid { get; set; }
        /// <summary>
        /// Entity guid. Refer IotConnect.EntityProvider.All() to get list of entities.
        /// </summary>
        [Required(ErrorMessage = "EntityGuid is required")]
        public string entityGuid { get; set; }

        /// Device template guid. Refer IotConnect.TemplateProvider.AllTemplate() to get list of all templates.
        /// <summary>
        /// </summary>
        [Required(ErrorMessage = "DeviceTemplateGuid is required")]
        public string deviceTemplateGuid { get; set; }
        /// <summary>
        /// (Optional) Note.
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// (Optional) Device Tag. Required if Parent Device guid is available.
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// (Optional) Parent Device guid. Set value of Gateway device if available.
        /// </summary>
        public string parentDeviceGuid { get; set; }
        /// <summary>
        /// (Optional) Properties
        /// </summary>
        public List<AddProperties> properties { get; set; }
        /// <summary>
        /// (Optional) Primary Thumbprint.
        /// </summary>
        public object primaryThumbprint { get; set; }
        /// <summary>
        /// (Optional) Secondary Thumbprint.
        /// </summary>
        public object secondaryThumbprint { get; set; }
        /// <summary>
        /// (Optional) Endorsement Key.
        /// </summary>
        public object endorsementKey { get; set; }
    }

    public class AddProperties
    {
        public string guid { get; set; }
        public string value { get; set; }
    }
}
