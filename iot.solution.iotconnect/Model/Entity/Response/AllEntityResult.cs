using System;
using System.Collections.Generic;
using System.Text;

namespace IoTConnect.Model
{
    /// <summary>
    /// All Entity List
    /// </summary>
    public class AllEntityResult
    {
        /// <summary>
        /// Returns Guid.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Returns Entity Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Returns Entity parent Entity Guid.
        /// </summary>
        public string ParentEntityGuid { get; set; }
        /// <summary>
        /// Returns Parent Name
        /// </summary>
        public string ParentName { get; set; }
        /// <summary>
        /// Returns Description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Returns Child Entity Lable.
        /// </summary>
        public string ChildEntityLable { get; set; }
        /// <summary>
        /// Returns Created Date.
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Returns Updated Date
        /// </summary>
        public DateTime UpdatedDate { get; set; }
        /// <summary>
        /// Returns Active user Count.
        /// </summary>
        public int ActiveUserCount { get; set; }
        /// <summary>
        /// Returns In Active Count.
        /// </summary>
        public int InActiveUserCount { get; set; }
        /// <summary>
        /// Returns Device Count.
        /// </summary>
        public int DeviceCount { get; set; }
    }
}
