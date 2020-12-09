
using System.ComponentModel.DataAnnotations;
namespace IoTConnect.Model
{
   public class UpdateCommandModel
    {
        /// <summary>
        /// Command Name.
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }
        /// <summary>
        /// Device Template Guid.
        /// </summary>
        [Required(ErrorMessage = "DeviceTemplateGuid is required")]
        public string deviceTemplateGuid { get; set; }
        /// <summary>
        /// Command.
        /// </summary>
        [Required(ErrorMessage = "Command is required")]
        public string command { get; set; }
        /// <summary>
        /// Is Required Param ?
        /// </summary>
        public bool requiredParam { get; set; }
        /// <summary>
        /// Required ACK ?
        /// </summary>
        public bool requiredAck { get; set; }
        /// <summary>
        /// Is OTA Command ?
        /// </summary>
        public bool isOTACommand { get; set; }
        /// <summary>
        /// Template Tag.
        /// </summary>
        public string tag { get; set; }
    }
}
