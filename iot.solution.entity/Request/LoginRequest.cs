using System.ComponentModel.DataAnnotations;

namespace iot.solution.entity
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MaxLength(250)]
        public string Password { get; set; }
    }
    public class ValidateRequest
    {
        [Required]
        public string token { get; set; }
    }
    public class RefreshTokenRequest 
    {
        public string Token { get; set; }
    }
}
