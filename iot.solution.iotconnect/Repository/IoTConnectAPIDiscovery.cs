using Flurl.Http;
using IoTConnect.Common.Constant;
using IoTConnect.Common.Interface;
using IoTConnect.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("IoTConnect.DeviceProvider"), InternalsVisibleTo("IoTConnect.EntityProvider"), InternalsVisibleTo("IoTConnect.UserProvider"), InternalsVisibleTo("IoTConnect.TemplateProvider"), InternalsVisibleTo("IoTConnect.RuleProvider"), InternalsVisibleTo("IoTConnect.FirmwareProvider")]
namespace IoTConnect.Common.Repository
{

    internal class IoTConnectAPIDiscovery : IIoTConnectAPIDiscovery
    {

        private static List<IoTConnectAPIEndPointResult> _ioTConnectAPIEndPoints { get; set; }

        private static IoTConnectAPIDiscovery instance = null;

        static HttpClient client = new HttpClient();

        private static readonly object _lock = new object();

        public IoTConnectAPIDiscovery()
        {
        }

        public static IoTConnectAPIDiscovery Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        // create the instance only if the instance is null
                        if (instance == null)
                        {
                            instance = new IoTConnectAPIDiscovery();
                        }
                    }
                }
                // Otherwise return the already existing instance
                return instance;
            }
        }

        /// <summary>
        /// Get IotConnect Portal Url.
        /// </summary>
        /// <param name="environmentCode">IoTConnect Environment Code</param>
        /// <param name="solutionKey">IotConnect Solution Unique Key</param>
        /// <param name="baseUrlType">IotConnect Base URL Types (Auth, User, Device, Telemetry)</param>
        /// <returns></returns>
        public async Task<string> GetPortalUrl(string environmentCode, string solutionKey, IoTConnectBaseURLType baseUrlType)
        {
            var url = await GetIoTConnectBaseURL(Constants.discoveryUrl, environmentCode, solutionKey, baseUrlType);
            return url?.Replace("api/v1.1", "");
        }

        /// <summary>
        /// Get IotConnect Base URL.
        /// </summary>
        /// <param name="discoveryURL">IotConnect Discovery URL</param>
        /// <param name="environment">IotConnect Environment</param>
        /// <param name="solutionKey">IotConnect Solution Unique Key</param>
        /// <param name="urlType">IotConnect Base URL Types (Auth, User, Device, Telemetry)</param>
        /// <returns></returns>
        public async Task<string> GetIoTConnectBaseURL(string discoveryURL, string environment, string solutionKey, IoTConnectBaseURLType urlType)
        {
            string response = string.Empty;

            if (_ioTConnectAPIEndPoints == null)
            {
                _ioTConnectAPIEndPoints = new List<IoTConnectAPIEndPointResult>();
            }

            var ioTConnectEndpoints = _ioTConnectAPIEndPoints
                .Where(x => string.Equals(x.SolutionKey, solutionKey, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Environment, environment, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.IoTConnectEndpoints).FirstOrDefault();
            if (ioTConnectEndpoints != null)
            {
                response = Convert.ToString(ioTConnectEndpoints.GetPropertyValue(urlType.ToString()));
            }

            if (string.IsNullOrWhiteSpace(response))
            {
                var discoveryDetail = await GetAPIEndPoints(discoveryURL, environment, solutionKey);
                if (discoveryDetail != null)
                {
                    IoTConnectAPIEndPointResult data = new IoTConnectAPIEndPointResult
                    {
                        SolutionKey = solutionKey,
                        Environment = environment,
                        IoTConnectEndpoints = discoveryDetail.IoTConnectEndpoints
                    };

                    _ioTConnectAPIEndPoints.Add(data);

                    response = Convert.ToString(discoveryDetail.IoTConnectEndpoints.GetPropertyValue(urlType.ToString()));
                }
            }

            if (!string.IsNullOrWhiteSpace(response))
            {
                return response;
            }
            else
            {
                //Business.CustomErrorMessage($"No IoTConnect API Discovery URL found for IoTConnectBaseURLType:{urlType}");
            }

            return null;
        }

        /// <summary>
        /// Get API endpoints.
        /// </summary>
        /// <param name="discoveryURL">IotConnect Discovery URL.</param>
        /// <param name="environment">IotConnect environment.</param>
        /// <param name="solutionKey">IotConnect Solution Unique Key.</param>
        /// <returns></returns>
        private async Task<ResponseIoTConnectAPIEndpoint> GetAPIEndPoints(string discoveryURL, string environment, string solutionKey)
        {
            ResponseIoTConnectAPIEndpoint response = new ResponseIoTConnectAPIEndpoint();
            try
            {
                string url = $"{discoveryURL}/api/uisdk/solutionkey/{solutionKey}/env/{environment}";
                response = await url.GetJsonAsync<ResponseIoTConnectAPIEndpoint>();
            }
            catch (FlurlHttpException ex)
            {
                throw ex;
            }

            return response;
        }

        /// <summary>
        /// Get IotConnect Portal Url.
        /// </summary>
        /// <param name="environmentCode">IoTConnect Environment Code</param>
        /// <param name="solutionKey">IotConnect Solution Unique Key</param>
        /// <param name="baseUrlType">IotConnect Base URL Types (Auth, User, Device, Telemetry)</param>
        /// <returns></returns>
        public async Task LoggedException(string environmentCode,Exception ex,string rdkFileName,string MethodName)
        {
            var url = string.Empty;
            if(environmentCode == EnviormentCode.qa.ToString())
            {
                url = Constants.qaLoggerUrl;
            }
            else if (environmentCode == EnviormentCode.dev.ToString())
            {
                url = Constants.devLoggerUrl;
            }
            else if(environmentCode == EnviormentCode.poc.ToString())
            {
                url = Constants.uatLoggerUrl;
            }
            else if(environmentCode == EnviormentCode.avnet.ToString())
            {
                url = Constants.prodLoggerUrl;
            }
            else if (environmentCode == EnviormentCode.prod.ToString())
            {
                url = Constants.prodLoggerUrl;
            }

            ErrorHandlerModel errorHandlerModel = new ErrorHandlerModel() {
                applicationCode = "Media",
                date = DateTime.UtcNow,
                errorCode = Convert.ToString(ex.HResult),
                componentStatus = "false",
                exception = ex.ToString(),
                file = rdkFileName,
                identity = string.Empty,
                logger = "General_MediaService",
                message = "Logged SuccessFull",
                messageData = ex.Message.ToString(),
                method = MethodName,
                severity = 1,
                stackTrace = ex.StackTrace.ToString()
            };

            try
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Clear();                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("General_MediaServiceKey");
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                var contentString = JsonConvert.SerializeObject(errorHandlerModel);
                request.Content = new StringContent(contentString,
                                    Encoding.UTF8,
                                    "application/json-patch+json");//CONTENT-TYPE header

               var da = await client.SendAsync(request);
                
            }                       
            catch (Exception ex2)
            {
                throw ex2;
            }
        }
    }
}

