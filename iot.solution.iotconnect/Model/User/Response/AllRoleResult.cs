using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// All Role Result.
    /// </summary>
    public class AllRoleResult
    {
        /// <summary>
        /// Returns Guid.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Returns Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Returns description.
        /// </summary>
        public object Description { get; set; }
        /// <summary>
        /// Returns Created Date.
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Returns Updated Date.
        /// </summary>
        public DateTime UpdatedDate { get; set; }
        /// <summary>
        /// Returns Created By.
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Returns Update By.
        /// </summary>
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Returns Updated by name.
        /// </summary>
        public object UpdatedByName { get; set; }
        /// <summary>
        /// Returns Status.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Returns User Count.
        /// </summary>
        public int UserCount { get; set; }
    } 
}
