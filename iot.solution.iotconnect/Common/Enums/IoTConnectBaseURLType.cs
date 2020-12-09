using System;
using System.Collections.Generic;
using System.Text;

namespace IoTConnect.Common
{
    /// <summary>
    /// IotConnect Base URL Types(Auth, User, Device, Telemetry).
    /// </summary>
    internal enum IoTConnectBaseURLType
    {
        AuthBaseUrl,
        UserBaseUrl,
        DeviceBaseUrl,
        EventBaseUrl,
        TelemetryBaseUrl,
        MasterBaseUrl,
        FirmwareBaseUrl
    }
}
