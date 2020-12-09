namespace iot.solution.entity
{
    public class KitVerifyResponse : KitDeviceRequest
    {
        public string KitTypeGuid { get; set; }
        public string KitCode { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }



}
