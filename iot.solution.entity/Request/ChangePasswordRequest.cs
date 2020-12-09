using System.ComponentModel.DataAnnotations;

namespace iot.solution.entity
{
    public class ChangePasswordRequest
    {
        [Required]
        [MaxLength(250)]
        public string Email { get; set; }

        [Required]
        [MaxLength(250)]
        public string OldPassword { get; set; }

        [MaxLength(250)]
        [Required]
        public string NewPassword { get; set; }
    }
}
