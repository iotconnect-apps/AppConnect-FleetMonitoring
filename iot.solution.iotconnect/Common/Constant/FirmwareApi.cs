namespace IoTConnect.Common.Constant
{
   internal class FirmwareApi
    {
        internal const string All = "api/v{0}/Firmware";
        internal const string firmwareDetails = "api/v{0}/Firmware/{1}";
        internal const string firmwareLookup = "api/v{0}/Firmware/lookup";
        internal const string Add = "api/v{0}/Firmware";
        internal const string FirmwareValidate= "api/v{0}/firmware-validate";
        internal const string Update = "api/v{0}/Firmware/{1}";
        internal const string firmwareUpgradLookup = "api/v{0}/Firmware/{1}/lookup";
        internal const string firmwareUpgradLookupById = "api/v{0}/firmware-upgrade/lookup/{1}";
        internal const string firmwareUpgradByFirmwareUpgradeId = "api/v{0}/firmware-upgrade/{1}";
        internal const string firmwareUpgrads= "api/v{0}/firmware-upgrade";
        internal const string AddFirmware = "api/v{0}/firmware-upgrade";
        internal const string UpdateFirmwareUpgrade = "api/v{0}/firmware-upgrade/{1}";
        internal const string PublishFirmware = "api/v{0}/firmware-upgrade/{1}/publish";
        internal const string DeleteFirmware= "api/v{0}/firmware-upgrade/{1}";
        internal const string RecentOTA = "api/v{0}/ota-update/device/{1}/recent";
        internal const string AllRecentOTA = "api/v{0}/ota-update/recent";
        internal const string AllRecentOTAActivity = "api/v{0}/ota-update/recent-activity";
        internal const string OTAUpdateByDeviceGuid = "api/v{0}/device/{1}/ota-update";
        internal const string DeviceListByOTAUpdate = "api/v{0}/ota-update/{1}";
        internal const string DeviceListByOTAUpdateandStatus = "api/v{0}/ota-update/{1}/status/{2}";
        internal const string AllOTAUpdate = "api/v{0}/ota-update";
        internal const string SendOTAUpdate = "api/v{0}/ota-update";
        internal const string UpdateOTAStatus= "api/v{0}/ota-update/device/{1}/otaupdateitem/{2}/status/{3}";
        internal const string OTAUpdateHistory = "api/v{0}/ota-update/device-update-history";



    }
}
