using System;
using System.ComponentModel.DataAnnotations;

namespace iot.solution.entity
{
    public class TokenResponse
    {
        
        public string accessToken { get; set; }
        public string expiresIn { get; set; }


    }
}
