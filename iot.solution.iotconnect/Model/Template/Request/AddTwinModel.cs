

using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    public class AddTwinModel
    {
        /// <summary>
        /// Twin Name.
        /// </summary>
        [Required(ErrorMessage ="Name is required")] 
        public string name { get; set; }

        /// <summary>
        /// Data Type Guid. Call Template.Datatype() method to get list of datatype.
        /// </summary>
        [Required(ErrorMessage = "DataTypeGuid is required")]
        public string dataTypeGuid { get; set; }

        /// <summary>
        /// Device Template Guid. Call Template.All() method to get list of devicetemplate.
        /// </summary>
        [Required(ErrorMessage = "DeviceTemplateGuid is required")]
        public string deviceTemplateGuid { get; set; }

        /// <summary>
        /// Local Name (Key in IotConnect Ui).
        /// </summary>
        [Required(ErrorMessage = "LocalName is required")]
        public string localName { get; set; }

        /// <summary>
        /// Default Value.
        /// </summary>
        [Required(ErrorMessage = "DefaultValue is required")]
        public string defaultValue { get; set; }

        /// <summary>
        /// Data Validation.
        /// </summary>
        public string dataValidation { get; set; }
    }
}
