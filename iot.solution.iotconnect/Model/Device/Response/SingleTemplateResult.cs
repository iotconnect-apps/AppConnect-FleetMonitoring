using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// Single Template.
    /// </summary>
    public class SingleTemplateResult
    {
        /// <summary>
        /// All attributes.
        /// </summary>
        public List<Attributes> Attributes { get; set; }
        /// <summary>
        /// All Settings.
        /// </summary>
        public List<Setting> Settings { get; set; }
        /// <summary>
        /// All Commands.
        /// </summary>
        public List<Command> Commands { get; set; }
    }

    /// <summary>
    /// Key
    /// </summary>
    public class Key
    {
        /// <summary>
        /// Local Name.
        /// </summary>
        public string LocalName { get; set; }
        /// <summary>
        /// Data Type guid.
        /// </summary>
        public string DatatypeGuid { get; set; }
        /// <summary>
        /// Data Type.
        /// </summary>
        public string DataType { get; set; }
    }

    /// <summary>
    /// Template Attribute.
    /// </summary>
    public class Attributes
    {
        /// <summary>
        /// Attribute guid.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Local Name.
        /// </summary>
        public string LocalName { get; set; }
        /// <summary>
        /// Tag.
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// Attribute Keys.
        /// </summary>
        public List<Key> Keys { get; set; }
        /// <summary>
        /// Parent Template Attribute guid 
        /// </summary>
        public string ParentTemplateAttributeGuid { get; set; }
    }

    /// <summary>
    /// Template Setting.
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// Setting guid.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Data Type guid.
        /// </summary>
        public string DatatypeGuid { get; set; }
        /// <summary>
        /// Data Type.
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// Local Name.
        /// </summary>
        public string LocalName { get; set; }
        /// <summary>
        /// Default Value.
        /// </summary>
        public string DefaultValue { get; set; }
    }

    public class Command
    {
        /// <summary>
        /// Command guid.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Command.
        /// </summary>
        public string command { get; set; }
        /// <summary>
        /// Required parameters.
        /// </summary>
        public string RequiredParam { get; set; }
    }
}
