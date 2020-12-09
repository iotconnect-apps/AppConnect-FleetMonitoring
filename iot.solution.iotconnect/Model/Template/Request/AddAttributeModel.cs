

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Add Attribute Model.
    /// </summary>
   public class AddAttributeModel
    {
        /// <summary>
        /// Attribute Local Name.
        /// </summary>
        [Required(ErrorMessage = "Enter local name")]
        public string localName { get; set; }
        /// <summary>
        /// (Optional) Description. 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Device Template Guid. Call Template.All() method to get list of devicetemplate. 
        /// </summary>
        public string deviceTemplateGuid { get; set; }
        /// <summary>
        /// Data Type Guid. Call Template.Datatype() method to get list of Datatype.
        /// </summary>
        [Required(ErrorMessage = "Select data type")]
        public string dataTypeGuid { get; set; }
        /// <summary>
        /// (Optional) Data Validation.
        /// </summary>
        public string dataValidation { get; set; }
        
        /// <summary>
        /// Unit.
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// Tag.
        /// </summary>
        [Required(ErrorMessage = "Enter tag name")]
        public string tag { get; set; }
        /// <summary>
        /// (Optional) Aggregate Type (s).
        /// </summary>
        public List<string> aggregateType { get; set; }
        /// <summary>
        /// (Optional) tumbling Window.
        /// </summary>
        public string tumblingWindow { get; set; }
        /// <summary>
        /// Attribute (s).
        /// </summary>
        public List<AddAttribute> attributes { get; set; }
    }
    /// <summary>
    /// Add Attribute Class.
    /// </summary>
    public class AddAttribute
    {
        /// <summary>
        /// Local Name.
        /// </summary>
        public string localName { get; set; }
        /// <summary>
        /// description.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Data Validation
        /// </summary>
        public string dataValidation { get; set; }
        /// <summary>
        /// Data Type Guid.
        /// </summary>
        public string dataTypeGuid { get; set; }
        /// <summary>
        /// sqquence.
        /// </summary>
        public int sequence { get; set; }
        /// <summary>
        /// Unit.
        /// </summary>
        public string unit { get; set; }
    }

}
