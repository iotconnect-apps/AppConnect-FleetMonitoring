using System.Collections.Generic;
using System.ComponentModel;

namespace IoTConnect.Model
{
    /// <summary>
    /// Search user Mdoel
    /// </summary>
    public class SearchUserModel : PagingModel
    {
        public SearchUserModel()
        {
            UserSearchRequest = new List<UserSearchRequest>();
        }
        /// <summary>
        /// Property.
        /// </summary>
        [DefaultValue(false)]
        public bool? Property { get; set; }
        /// <summary>
        /// User Status
        /// </summary>
        [DefaultValue(false)]
        public bool? Status { get; set; }
        
        /// <summary>
        /// User Search result.
        /// </summary>
        public List<UserSearchRequest> UserSearchRequest { get; set; }
    }

    /// <summary>
    /// User Search Request
    /// </summary>
    public class UserSearchRequest
    {
        public UserSearchRequest()
        {
            UserGuids = new List<Userguid>();
            RoleGuids = new List<Roleguid>();
        }
        /// <summary>
        /// User Guids
        /// </summary>
        public List<Userguid> UserGuids { get; set; }
        /// <summary>
        /// User role Guids.
        /// </summary>
        public List<Roleguid> RoleGuids { get; set; }
    }
    /// <summary>
    /// User Guid
    /// </summary>
    public class Userguid
    {
        /// <summary>
        /// Guid
        /// </summary>
        public string guid { get; set; }
    }
    /// <summary>
    /// Role Guid
    /// </summary>
    public class Roleguid
    {
        /// <summary>
        /// Guid
        /// </summary>
        public string guid { get; set; }
    }
}
