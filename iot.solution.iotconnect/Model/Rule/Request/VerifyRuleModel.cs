using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Verify Rule class
    /// </summary>
   public class VerifyRuleModel
    {
        /// <summary>
        /// Gets and sets the deviceTemplateGuid
        /// </summary>
        [Required (ErrorMessage = "DeviceTemplateGuid is required")]
        public string deviceTemplateGuid { get; set; }
        /// <summary>
        /// Gets and sets the expression
        /// </summary>
        [Required(ErrorMessage = "Expression is required")]
        public string expression { get; set; }
    }
}
