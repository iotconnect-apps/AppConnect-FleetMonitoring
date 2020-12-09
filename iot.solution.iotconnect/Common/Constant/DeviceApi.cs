
namespace IoTConnect.Common.Constant
{
    internal class DeviceApi
    {
        internal const string Get = "api/v{0}/Device";        
        internal const string UniqDevice = "api/v{0}/Device/uniqueId/{1}";
        internal const string DeviceEntity = "api/v{0}/entity/{1}/device";
        internal const string DeviceCounter = "api/v{0}/Device/counters";
        internal const string DeviceCompany = "api/v{0}/company/{1}/device";
        internal const string Add = "api/v{0}/Device";
        internal const string Update = "api/v{0}/Device/{1}";
        internal const string Delete = "api/v{0}/Device/{1}";
        internal const string Acquire = "api/v{0}/Device/{1}/acquire";
        internal const string Release = "api/v{0}/Device/{1}/release";
        internal const string DeviceStatus = "api/v{0}/Device/{1}/status";
        internal const string UpdatedeviceEntityBulk= "api/v{0}/entity/{1}/device";
        internal const string ChildDevice = "api/v{0}/Device/{1}/child-device";
        internal const string DeviceTwin = "api/v{0}/Device/{1}/twin";
        internal const string UpdateDeviceTwin = "api/v{0}/Device/{1}/twin";
        internal const string DeviceGrant = "api/v{0}/Device/{1}/grant";
        internal const string DeviceAddGrant = "api/v{0}/Device/{1}/grant";
        internal const string DeviceDeleteGrant = "api/v{0}/device/user-device-permission/{1}/revoke";
        internal const string AllottedDeviceUser = "api/v{0}/Device/user/{1}/grant";
        internal const string GetAllottedDeviceUser = "api/v{0}/Device/user";
        internal const string SingleAllottedDeviceUser = "api/v{0}/Device/user/{1}";
        internal const string TelemetryData = "api/v{0}/Telemetry/device/{1}";
        internal const string ConnectionStatus = "api/v{0}/Device/connection-status?uniqueId={1}";
        internal const string DeviceCounterByEntity = "api/v{0}/Device/statistic";

    }
}
