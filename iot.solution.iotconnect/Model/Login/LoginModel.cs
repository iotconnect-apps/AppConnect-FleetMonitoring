using Newtonsoft.Json;

namespace IoTConnect.Model
{
    /// <summary>
    /// Login Model.
    /// </summary>
    public class LoginModel 
    {
        /// <summary>
        /// IotConnect Username.
        /// </summary>
        [JsonProperty("username")]
        public string UserName { get; set; }

        /// <summary>
        /// IotConnect Password.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
