
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Update Attirbute Model.
    /// </summary>
   public class UpdateAttributeModel
    {
        /// <summary>
        /// attributes.
        /// </summary>
        public List<UpdateAttribute> attributes { get; set; }
        /// <summary>
        /// Local Name.
        /// </summary>
        [Required(ErrorMessage = "Enter local name")]
        public string localName { get; set; }
        /// <summary>
        /// Description.
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
        /// Data validation.
        /// </summary>
        public string dataValidation { get; set; }
        /// <summary>
        /// Unit.
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        [Required(ErrorMessage = "Enter Tag.")]
        public string tag { get; set; }
        /// <summary>
        /// Aggregate Type.
        /// </summary>
        public List<string> aggregateType { get; set; }
        /// <summary>
        /// tumbling Window.
        /// </summary>
        public string tumblingWindow { get; set; }
    }
    /// <summary>
    /// Update attribute
    /// </summary>
    public class UpdateAttribute
    {
        /// <summary>
        /// Attr Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Local Name.
        /// </summary>
        public string localName { get; set; }
        /// <summary>
        /// Description.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Data Validation.
        /// </summary>
        public string dataValidation { get; set; }
        /// <summary>
        /// Data Type Guid.
        /// </summary>
        public string dataTypeGuid { get; set; }
        /// <summary>
        /// Sequence.
        /// </summary>
        public int sequence { get; set; }
        /// <summary>
        /// Unit.
        /// </summary>
        public string unit { get; set; }
    }
}
