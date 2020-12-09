using System.Collections.Generic;

namespace IoTConnect.Model
{
    /// <summary>
    /// User Detail By Ids.
    /// </summary>
    public class ResponseGetUserDetailById
    {
        /// <summary>
        /// User Guid.
        /// </summary>
        public string UserGuid { get; set; }
        /// <summary>
        /// User Id.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Company guid.
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
        /// Time Zone Guid.
        /// </summary>
        public string TimezoneGuid { get; set; }
        /// <summary>
        /// Image Name.
        /// </summary>
        public object ImageName { get; set; }
        /// <summary>
        /// user Status.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Role Name.
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// Role Guid.
        /// </summary>
        public string RoleGuid { get; set; }
        /// <summary>
        /// Contact No.
        /// </summary>
        public object ContactNo { get; set; }
        /// <summary>
        /// Entity Name.
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// List Of user.
        /// </summary>
        public List<UserInfo> UserInfo { get; set; }
        /// <summary>
        /// Creted By.
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Updated By.
        /// </summary>
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Updated by Name.
        /// </summary>
        public string UpdatedByName { get; set; }
        /// <summary>
        /// Created Date.
        /// </summary>
        public string CreatedDate { get; set; }
        /// <summary>
        /// Updated Date.
        /// </summary>
        public string UpdatedDate { get; set; }
        /// <summary>
        /// Entity Guid.
        /// </summary>
        public string EntityGuid { get; set; }
        /// <summary>
        /// Parent Entity Guid.
        /// </summary>
        public object ParentEntityGuid { get; set; }
        /// <summary>
        /// Company logo.
        /// </summary>
        public object CompanyLogo { get; set; }
        /// <summary>
        /// Company Name.
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// Company CpId.
        /// </summary>
        public string CompanyCpid { get; set; }
        /// <summary>
        /// TimeZone Offset.
        /// </summary>
        public string TimezoneOffset { get; set; }
        /// <summary>
        /// Image Url.
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// Company Logo url.
        /// </summary>
        public string CompanyLogoUrl { get; set; }
        /// <summary>
        /// Dms_Version.
        /// </summary>
        public string Dms_version { get; set; }
    }

    public class UserInfo
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string FieldType { get; set; }
        public string Options { get; set; }
        public string Value { get; set; }
    }
}
