using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Add Template Command model.
    /// </summary>
   public class AddCommandModel
    {
        /// <summary>
        /// Command Name.
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }
        /// <summary>
        /// Command Local Name.
        /// </summary>
        public string localName { get; set; }
        /// <summary>
        /// Device Template Guid. Call Template.All() method to get list of devicetemplate.
        /// </summary>
        [Required(ErrorMessage = "DeviceTemplateGuid is required")]
        public string deviceTemplateGuid { get; set; }
        /// <summary>
        /// Command
        /// </summary>
        [Required(ErrorMessage = "Command is required")]
        public string command { get; set; }
        /// <summary>
        /// Required Param ?
        /// </summary>
        public bool requiredParam { get; set; }
        /// <summary>
        /// Required Ack ?
        /// </summary>
        public bool requiredAck { get; set; }
        /// <summary>
        /// Is OTA Command ?
        /// </summary>
        public bool isOTACommand { get; set; }
        /// <summary>
        /// Tempalte Tag.
        /// </summary>
        public string tag { get; set; }
    }
}
