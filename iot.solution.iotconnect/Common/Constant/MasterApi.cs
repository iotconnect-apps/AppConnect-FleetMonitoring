namespace IoTConnect.Common.Constant
{
    internal class MasterApi
    {
        
        internal const string Country = "api/v{0}/Master/country";

        internal const string State = "api/v{0}/Master/country/{1}/state";

        internal const string TimeZones = "api/v{0}/Master/timezone";

        internal const string TemplatesLookup = "api/v{0}/device-template/lookup";

        internal const string RoleLookup = "api/v{0}/Role/lookup";

        internal const string AllAttributeLookup = "api/v{0}/template-attribute/{1}/lookup";
    }
}
