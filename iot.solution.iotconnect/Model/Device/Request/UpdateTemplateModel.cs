using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Update Device Template Model.
    /// </summary>
    public class UpdateTemplateModel
    {
        /// <summary>
        /// Template Name.
        /// </summary>
        [Required(ErrorMessage = "Template name is required")]
        public string Name { get; set; }
        /// <summary>
        /// (Optional) Description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// (Optional) Firmware guid.
        /// </summary>
        public string FirmwareGuid { get; set; }
    }
}
