using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Add new user
    /// </summary>
    public class AddUserModel
    {
        /// <summary>
        /// First name.
        /// </summary>
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        /// <summary>
        /// Last name.
        /// </summary>
       [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }
        /// <summary>
        /// User email address.
        /// </summary>
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
        /// <summary>
        /// Timezone guid. Call User.TimeZones() method to get list of timezones. 
        /// </summary>
        [Required(ErrorMessage = "TimezoneGuid is required")]
        public string TimezoneGuid { get; set; }
        /// <summary>
        /// Role guid. Call User.AllRole() method to get list of all roles. 
        /// </summary>
        [Required(ErrorMessage = "RoleGuid is required")]
        public string RoleGuid { get; set; }
        /// <summary>
        /// User Phone number.
        /// </summary>
        [StringLength(14, ErrorMessage =
                      "ContactNumber length should be valid")]
        [Required(ErrorMessage = "Contact Number is required")]
        public string ContactNo { get; set; }
        /// <summary>
        /// Other properties
        /// </summary>
        internal List<Property> Properties { get; set; }
        /// <summary>
        /// User Status. Supply 0 if you wish to add user as a inactive user.
        /// </summary>
        public int IsActive { get; set; }
        /// <summary>
        /// Entity guid. Get Entity.All() method to get list of all entities.
        /// </summary>
        [Required(ErrorMessage = "EntityGuid is required")]
        public string EntityGuid { get; set; }
    }

    public class Property
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
