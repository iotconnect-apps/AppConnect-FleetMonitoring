using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Update Device Request Class.
    /// </summary>
    public class UpdateDeviceModel
    {
        /// <summary>
        /// Device display name.
        /// </summary>
        [Required(ErrorMessage = "DisplayName is required")]
        public string displayName { get; set; }
        /// <summary>
        /// (Optional)Note.
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// Entity Guid. Refer IotConnect.EntityProvider.All() to get list of entities.
        /// </summary>
        [Required(ErrorMessage = "EntityGuid is required")]
        public string entityGuid { get; set; }
        /// <summary>
        /// List Of Properties.
        /// </summary>
        public List<UpdateProperties> properties { get; set; }
        /// <summary>
        /// Device template guid. Refer IotConnect.DeviceProvider.AllTemplate() to get list of all templates.
        /// </summary>
        [Required(ErrorMessage = "DeviceTemplateGuid is required")]
        public string deviceTemplateGuid { get; set; }        
        /// <summary>
        /// (Optional) Device Tag. Required if Parent Device guid is available.
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// (Optional) Parent Device guid. Set value of Gateway device if available.
        /// </summary>
        public string parentDeviceGuid { get; set; }

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

    public class UpdateProperties
    {
        public string guid { get; set; }
        public string value { get; set; }
    }
}
