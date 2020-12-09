using System;
using System.Collections.Generic;
using System.Text;

namespace IoTConnect.Model
{
    public class AllAttributeResult
    {
        /// <summary>
        /// Is Validate template ?
        /// </summary>
        public bool isValidateTemplate { get; set; }
        /// <summary>
        /// List Of Attribute Result.
        /// </summary>
        public List<AttributeResult> data { get; set; }        
    }

    public class AttributeResult
    {
        /// <summary>
        /// Attribute Guid.
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
        /// Data Type guid.
        /// </summary>
        public string dataTypeGuid { get; set; }
        /// <summary>
        /// Data Type Name.
        /// </summary>
        public string dataTypeName { get; set; }
        /// <summary>
        /// Created Date.
        /// </summary>
        public DateTime createdDate { get; set; }
        /// <summary>
        /// Created by.
        /// </summary>
        public string createdby { get; set; }
        /// <summary>
        /// Updated Date.
        /// </summary>
        public DateTime updatedDate { get; set; }
        /// <summary>
        /// Data Validation.
        /// </summary>
        public object dataValidation { get; set; }
        /// <summary>
        /// Unit
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public object tag { get; set; }
        /// <summary>
        /// Sequence.
        /// </summary>
        public int sequence { get; set; }
        /// <summary>
        /// Parent Template Attribute Guid.
        /// </summary>
        public object parentTemplateAttributeGuid { get; set; }
        /// <summary>
        /// Is Attribute Used ?
        /// </summary>
        public bool isTemplateAttributeUsed { get; set; }
        /// <summary>
        /// Is tag used ?
        /// </summary>
        public bool isTagUsed { get; set; }
        /// <summary>
        /// Aggregate Type.
        /// </summary>
        public List<object> aggregateType { get; set; }
        /// <summary>
        /// Aggregate Type Count.
        /// </summary>
        public int? aggregateTypeCount { get; set; }
        /// <summary>
        /// Tumblain Window
        /// </summary>
        public object tumblingWindow { get; set; }
        /// <summary>
        /// Attributes.
        /// </summary>
        public List<object> attributes { get; set; }
    }
}
