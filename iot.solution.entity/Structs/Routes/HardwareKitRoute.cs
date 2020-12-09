namespace iot.solution.entity.Structs.Routes
{
    public struct HardwareKitRoute
    {
        public struct Name
        {
            public const string GetById = "hardwarekit.getbyid";
            public const string Delete = "hardwarekit.delete";
            public const string BySearch = "hardwarekit.search";
            public const string Manage = "hardwarekit.manage";
            public const string GetKitTypes = "hardwarekit.getkittypes";
            public const string DownloadSampleJson = "hardwarekit.samplejson";
            public const string VerifyKit = "hardwarekit.verifykit";
            public const string UploadKit = "hardwarekit.uploadkit";
        }

        public struct Route
        {
            public const string Global = "api/hardwarekit";
            public const string GetById = "{id}";
            public const string Delete = "delete";
            public const string BySearch = "search";
            public const string Manage = "manage";
            public const string GetKitTypes = "getkittypes";
            public const string DownloadSampleJson = "download";
            public const string VerifyKit = "verifykit";
            public const string UploadKit = "uploadkit";
        }
    }
}
