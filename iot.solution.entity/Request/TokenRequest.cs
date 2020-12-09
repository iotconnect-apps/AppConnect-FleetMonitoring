using System;
using System.ComponentModel.DataAnnotations;

namespace iot.solution.entity
{
    public class TokenRequest
    {
        
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string UserName { get; set; }
        
    }
}
