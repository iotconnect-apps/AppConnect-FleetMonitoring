using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// Template Setting Response.
    /// </summary>
   public class AllTwinResult
    {
        /// <summary>
        /// Twin Guid.
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Data Type Guid.
        /// </summary>
        public string dataTypeGuid { get; set; }
        /// <summary>
        /// Created Date.
        /// </summary>
        public DateTime createdDate { get; set; }
        /// <summary>
        /// Created By.
        /// </summary>
        public string createdby { get; set; }
        /// <summary>
        /// Updated Date.
        /// </summary>
        public DateTime updatedDate { get; set; }
        /// <summary>
        /// Local Name.
        /// </summary>
        public string localName { get; set; }
        /// <summary>
        /// Default Value.
        /// </summary>
        public string defaultValue { get; set; }
        /// <summary>
        /// Data Validation.
        /// </summary>
        public string dataValidation { get; set; }
        /// <summary>
        /// Tag.
        /// </summary>
        public object tag { get; set; }
    }
}
