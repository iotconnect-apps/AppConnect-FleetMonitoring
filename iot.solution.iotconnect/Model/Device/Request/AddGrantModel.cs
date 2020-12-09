

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Add Permission.
    /// </summary>
    public class AddGrantModel 
    {
        /// <summary>
        /// User (s) Mail.
        /// </summary>
        [Required(ErrorMessage = "Please enter email.")]
        public List<string> emails { get; set; }
        /// <summary>
        /// Permission.
        /// </summary>
        [Required(ErrorMessage = "Select at least one permission.")] 
        public Permission permission { get; set; }

    }

    /// <summary>
    /// Permission Class.
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Is View ?.
        /// </summary>
        public bool view { get; set; }
        /// <summary>
        /// Is Manage ?
        /// </summary>
        public bool operation { get; set; }
    }
}
