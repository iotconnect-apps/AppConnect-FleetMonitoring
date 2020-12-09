using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Update User Model.
    /// </summary>
    public class UpdateUserModel
    {
        /// <summary>
        /// First Name.
        /// </summary>
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        /// <summary>
        /// Last Name.
        /// </summary>
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [JsonIgnore]
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
        /// Entity guid. Get Entity.All() method to get list of all entities.
        /// </summary>
        [Required(ErrorMessage = "EntityGuid is required")]
        public string EntityGuid { get; set; }
        /// <summary>
        /// Other properties
        /// </summary>
        public List<Property> Properties { get; set; }
    }
}
