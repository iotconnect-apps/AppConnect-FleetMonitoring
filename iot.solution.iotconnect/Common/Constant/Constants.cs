using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IoTConnect.DeviceProvider"), InternalsVisibleTo("IoTConnect.EntityProvider"), InternalsVisibleTo("IoTConnect.UserProvider")]
namespace IoTConnect.Common.Constant 
{
    internal class Constants
    {
        internal const string discoveryUrl = "https://discovery.iotconnect.io/";
        internal const string qaLoggerUrl = "http://192.168.3.42:8082/api/v1/event";
        internal const string devLoggerUrl = "https://qa-atune-api-log-wi.azurewebsites.net/api/v1/event";
        internal const string uatLoggerUrl = "https://qa-atune-api-log-wi.azurewebsites.net/api/v1/event";
        internal const string prodLoggerUrl = "https://qa-atune-api-log-wi.azurewebsites.net/api/v1/event";
        internal const string userVersion = "1.1";
        internal const string masterVersion = "1.1";
        internal const string authVersion = "2";
        internal const string entityVersion = "1.1";
        internal const string deviceVersion = "1.1";
        internal const string firmwareVersion = "1.1";
        internal const string eventVersion = "1.1";
        internal const string contentType = "application/json";
        internal const string bearerTokenType = "Bearer ";
        internal const string basicTokenType = "Basic ";
    }
}
