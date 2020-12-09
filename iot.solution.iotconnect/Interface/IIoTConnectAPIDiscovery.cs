using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly : InternalsVisibleTo("IoTConnect.DeviceProvider"), InternalsVisibleTo("IoTConnect.EntityProvider"),InternalsVisibleTo("IoTConnect.UserProvider"), InternalsVisibleTo("IoTConnect.TemplateProvider"), InternalsVisibleTo("IoTConnect.RuleProvider"), InternalsVisibleTo("IoTConnect.FirmwareProvider")]
namespace IoTConnect.Common.Interface
{
    internal interface IIoTConnectAPIDiscovery
    {
        /// <summary>
        /// Get IotConnect Portal Url.
        /// </summary>
        /// <param name="environmentCode">IoTConnect Environment Code</param>
        /// <param name="solutionKey">IotConnect Solution Unique Key</param>
        /// <param name="baseUrlType">IotConnect Base URL Types (Auth, User, Device, Telemetry)</param>
        /// <returns></returns>
        Task<string> GetPortalUrl(string environmentCode, string solutionKey, IoTConnectBaseURLType baseUrlType);

        /// <summary>
        /// Get Logger Url.
        /// </summary>
        /// <param name="environmentCode">IoTConnect Environment Code</param>
       
        /// <returns></returns>
        Task LoggedException(string environmentCode,Exception ex, string rdkFileName, string MethodName);
        /// <summary>
        /// Get IotConnect Base URL.
        /// </summary>
        /// <param name="discoveryURL">IotConnect Discovery URL</param>
        /// <param name="environment">IotConnect Environment</param>
        /// <param name="solutionKey">IotConnect Solution Unique Key</param>
        /// <param name="baseUrlType">IotConnect Base URL Types (Auth, User, Device, Telemetry)</param>
        /// <returns></returns>
        Task<string> GetIoTConnectBaseURL(string discoveryURL, string environment, string solutionKey, IoTConnectBaseURLType baseUrlType);
    }
}
