using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    /// <summary>
    /// Reset Password Model.
    /// </summary>
    public class ResetPasswordModel
    {
        /// <summary>
        /// Existing Email Id.
        /// </summary>
        [Required(ErrorMessage = "Please enter a email address")]
        public string Email { get; set; }
        /// <summary>
        /// (Optional) Invitation guid.
        /// </summary>
        [Required(ErrorMessage = "Please enter invitationguid")]
        public string InvitationGuid { get; set; }
        /// <summary>
        /// New Password.
        /// </summary>
        [Required(ErrorMessage = "Please enter a newpassword")]
        public string NewPassword { get; set; }
    }
}
