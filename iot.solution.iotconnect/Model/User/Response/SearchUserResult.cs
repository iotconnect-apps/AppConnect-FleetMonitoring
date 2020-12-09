using System;
using System.Collections.Generic; 

namespace IoTConnect.Model
{
    /// <summary>
    /// User Search response
    /// </summary>
    public class SearchUserResult 
    { 
        /// <summary>
        /// invoking user.
        /// </summary>
        public string invokingUser { get; set; }
        /// <summary>
        /// user Data.
        /// </summary>
        public List<SearchUser> data { get; set; }
        /// <summary>
        /// Returns Status
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// Return Message
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Returns Count.
        /// </summary>
        public int count { get; set; }
    }
    /// <summary>
    /// Search user
    /// </summary>
    public class SearchUser
    {
        /// <summary>
        /// Guid
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Comapany Guid.
        /// </summary>
        public string CompanyGuid { get; set; }
        /// <summary>
        /// First Name.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last Name.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Contact No.
        /// </summary>
        public string ContactNo { get; set; }
        /// <summary>
        /// TimeZone Guid.
        /// </summary>
        public string TimezoneGuid { get; set; }
        /// <summary>
        /// Image Name
        /// </summary>
        public string ImageName { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Is verified?
        /// </summary>
        public bool IsVerified { get; set; }
        /// <summary>
        /// User info
        /// </summary>
        public object UserInfo { get; set; }
        /// <summary>
        /// Role Guid.
        /// </summary>
        public string RoleGuid { get; set; }
        /// <summary>
        /// Role Name.
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// Created By.
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Updated By.
        /// </summary>
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Created date.
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Updated Date.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }
        /// <summary>
        /// Entity Guid.
        /// </summary>
        public string EntityGuid { get; set; }
        /// <summary>
        /// Entity Name.
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// Image url.
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
