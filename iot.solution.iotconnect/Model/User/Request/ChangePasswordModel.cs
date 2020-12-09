using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Change Password Model.
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// Existing Email Id.
        /// </summary>
        [Required(ErrorMessage = "Please enter a email address")]
        public string Email { get; set; }
        /// <summary>
        /// Old Password.
        /// </summary>
        [Required(ErrorMessage = "Please enter a oldpassword")]
        public string OldPassword { get; set; }
        /// <summary>
        /// New Password.
        /// </summary>
        [Required(ErrorMessage = "Please enter a newpassword")]
        public string NewPassword { get; set; }

        internal CustomETPlaceHolders customETPlaceHolders { get; set; }
    }
}
