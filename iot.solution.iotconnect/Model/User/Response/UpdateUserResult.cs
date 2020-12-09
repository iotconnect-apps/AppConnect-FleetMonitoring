using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// Update User.
    /// </summary>
    public class UpdateUserResult
    {
        /// <summary>
        /// User Role Guid.
        /// </summary>
        public Guid roleUserGuid { get; set; }
        /// <summary>
        /// Status.
        /// </summary>
        public bool isActive { get; set; }
        /// <summary>
        /// Role Guid
        /// </summary>
        public Guid roleGuid { get; set; }
        /// <summary>
        /// Is update Role?
        /// </summary>
        public bool isUpdateRole { get; set; }
        /// <summary>
        /// Entity Guid.
        /// </summary>
        public Guid entityGuid { get; set; }
    }
}
