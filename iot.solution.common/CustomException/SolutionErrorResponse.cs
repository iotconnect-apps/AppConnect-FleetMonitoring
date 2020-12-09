using Newtonsoft.Json;

namespace iot.solution.common
{
    public class ComponentErrorResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }
}
