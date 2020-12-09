

namespace IoTConnect.Model
{
    public class TemplateAttributeLookupResult
    {
        public string guid { get; set; }
        public string localname { get; set; }
        public bool isTemplateAttributeUsed { get; set; }
        public string parentTemplateAttributeGuid { get; set; }
    }
}
