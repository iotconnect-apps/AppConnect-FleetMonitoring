using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
   /// <summary>
   /// Child Class.
   /// </summary>
    public class Child
    {
        /// <summary>
        /// Child Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Chaild Type.
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Child Unit.
        /// </summary>
        public string unit { get; set; }
    }

    /// <summary>
    /// Template Attribute Class.
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// Attribute Name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Attribute type.
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// attribute tag.
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// (Optional) Unit
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// (Optional) Tumbling Window.
        /// </summary>
        public string tumblingWindow { get; set; }
        /// <summary>
        /// (Optional) AggregateType. Pass Null if not add.
        /// </summary>
        public List<object> aggregateTypes { get; set; }
        /// <summary>
        /// (Optional) Childs. Pass Null if not add.
        /// </summary>
        public List<Child> childs { get; set; }
    }

    /// <summary>
    /// Add setting class
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Setting Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Setting Type.
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Setting Local Name.
        /// </summary>
        public string localName { get; set; }
        /// <summary>
        /// Setting Default Value.
        /// </summary>
        public string defaultValue { get; set; }
    }

    /// <summary>
    /// Add Commands Class for templates.
    /// </summary>
    public class Commands
    {
        /// <summary>
        /// Commands Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Command.
        /// </summary>
        public string command { get; set; }
        /// <summary>
        /// Command tag.
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// Is required parm. pass 1 or 0.
        /// </summary>
        public bool requiredParam { get; set; }
        /// <summary>
        /// Is Required ACK. pass 1 or 0.
        /// </summary>
        public bool requiredAck { get; set; }
        /// <summary>
        /// Is OTA Command pass 1 or 0.
        /// </summary>
        public bool isOTACommand { get; set; }
    }

    /// <summary>
    /// Template Json Class.
    /// </summary>
    public class TemplateJSON
    {
        /// <summary>
        /// Template Code. Not more than 10 charector.
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// Template Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Device Authentication Type(s).
        /// 1 = Key.
        /// 2 = CA Signed Certificate.
        /// 3 = Self Signed Certificate.
        /// 4 = TPM.
        /// </summary>
        public int authType { get; set; }
        /// <summary>
        /// Tag.
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// (Optional) Is Edge Support.
        /// </summary>
        public string isEdgeSupport { get; set; }
        /// <summary>
        /// add new device template attributes list.
        /// </summary>
        public List<Attribute> attributes { get; set; }
        /// <summary>
        /// (Optional) add new template setting list. pass null if not add.
        /// </summary>
        public List<Settings> settings { get; set; }
        /// <summary>
        /// (Optional) add new template command list pass null if not add.
        /// </summary>
        public List<Commands> commands { get; set; }
    }

    /// <summary>
    /// Add quick template with attribute class.
    /// </summary>
    public class AddQuickTemplateModel
    {
        /// <summary>
        /// Add Template json.
        /// </summary>
        [Required(ErrorMessage = "TemplateJSON is required")]
        public TemplateJSON templateJSON { get; set; }
        /// <summary>
        /// version
        /// </summary>
        public string version { get; set; }
    }
}
