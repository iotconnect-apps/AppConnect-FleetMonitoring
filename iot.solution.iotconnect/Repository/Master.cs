using Flurl.Http;
using IoTConnect.Common.Constant;
using IoTConnect.Common.Interface;
using IoTConnect.Common.Repository;
using IoTConnect.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IoTConnect.Common
{
    public class Master : IMaster
    {
        #region Private Properties
        private string _token = string.Empty;
        private IIoTConnectAPIDiscovery _ioTConnectAPIDiscovery;
        private string _envCode = string.Empty;
        private string _solutionKey = string.Empty;

        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the IOTConnect.Common.Auth class.
        /// </summary>
        /// <param name="environmentCode">IotConnect Environment Code.</param>
        /// <param name="solutionKey">IotConnect Solution Key. Should be a valid Solution Key.</param>
        public Master(string token, string environmentCode, string solutionKey)
        {
            _token = token;
            _envCode = environmentCode;
            _solutionKey = solutionKey;
            _ioTConnectAPIDiscovery = new IoTConnectAPIDiscovery();
            FlurlHttp.Configure(settings => settings.OnError = HandleFlurlErrorAsync);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles the flurl error asynchronous.
        /// </summary>
        /// <param name="call">Http call.</param>
        /// <returns></returns>
        private void HandleFlurlErrorAsync(HttpCall call)
        {
            call.ExceptionHandled = true;
            IoTConnectException ioTConnectErrorResponse = JsonConvert.DeserializeObject<IoTConnectException>(call.Response.Content.ReadAsStringAsync().Result);
            throw ioTConnectErrorResponse;
        }
        #endregion

        /// <summary>
        /// Get All IotConnnect Country.
        /// </summary>
        /// <returns></returns>
        public async Task<DataResponse<List<Conuntry>>> Countries()
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.MasterBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, MasterApi.Country);
                string formattedUrl = String.Format(accessTokenUrl, Constants.masterVersion);
                return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .GetJsonAsync<DataResponse<List<Conuntry>>>();
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<Conuntry>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch(Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode,ex,"Master", "Countries()");
                throw ex;
            }
        }

        /// <summary>
        /// Get IotConnect TimeZone list.
        /// </summary>
        /// <returns></returns>
        public async Task<DataResponse<List<AllTimeZoneResult>>> TimeZones()
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.MasterBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.TimeZones);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .GetJsonAsync<DataResponse<List<AllTimeZoneResult>>>();

                return new DataResponse<List<AllTimeZoneResult>>(null)
                {
                    data = result.data,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {

                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllTimeZoneResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Master", "TimeZones()");
                throw ex;
            }
        }

        /// <summary>
        /// Get IotConnect State list by Country.
        /// </summary>
        /// <returns></returns>
        public async Task<DataResponse<List<StateResult>>> States(string CountryGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(CountryGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "CountryGuid is required",
                        Param = "CountryGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<StateResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.MasterBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, MasterApi.State);
                string formattedUrl = String.Format(accessTokenUrl, Constants.masterVersion, CountryGuid);
                return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .GetJsonAsync<DataResponse<List<StateResult>>>();
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<StateResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Master", "States()");
                throw ex;
            }

        }

        /// <summary>
        /// Get all roles.
        /// </summary>
        /// <returns></returns>
        public async Task<DataResponse<List<AllRoleLookupResult>>> AllRoleLookup()
        {

            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, MasterApi.RoleLookup);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .GetJsonAsync<DataResponse<List<AllRoleLookupResult>>>();
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllRoleLookupResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message =ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Master", "AllRoleLookup()");
                throw ex;
            }
        }

        /// <summary>
        /// This API endpoint will also give you the list of available device templates in your company. However, it provides an option to filter the list based on attributes availability. If you pass true in hasAttribute parameter, it will give you the list of device templates with at least one attribute.
        /// </summary>
        /// <param name="hasAttribute">has Attributes</param>
        /// <param name="excludePushUrl">exclude push url</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllTemplateResult>>> AllTemplateLookup(bool hasAttribute, bool excludePushUrl)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, MasterApi.TemplatesLookup);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SetQueryParams(new { hasAttribute = hasAttribute, excludePushUrl = excludePushUrl })
                                                  .GetJsonAsync<DataResponse<List<AllTemplateResult>>>();
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllTemplateResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Master", "AllTemplateLookup()");
                throw ex;
            }


        }

        /// <summary>
        /// This Method provides you the lookup list of available template attributes in your device template. For that, you need to send deviceTemplateGuid in request url.
        /// </summary>
        /// <param name="deviceTemplateGuid">Device Template Guid</param>
        /// <returns></returns>
        public async Task<DataResponse<List<TemplateAttributeLookupResult>>> AllAttributeLookup(string deviceTemplateGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceTemplateGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "DeviceTemplateGuid is required",
                        Param = "DeviceTemplateGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<TemplateAttributeLookupResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, MasterApi.AllAttributeLookup);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, deviceTemplateGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .GetJsonAsync<DataResponse<List<TemplateAttributeLookupResult>>>();

                return result;
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<TemplateAttributeLookupResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Master", "AllAttributeLookup()");
                throw ex;
            }
        }


    }
}
