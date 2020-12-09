using Flurl.Http;
using Flurl.Http.Content;
using IoTConnect.Common;
using IoTConnect.Common.Constant;
using IoTConnect.Common.Interface;
using IoTConnect.Common.Repository;
using IoTConnect.Interface;
using IoTConnect.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IoTConnect.FirmwareProvider
{
    public class Firmware : IFirmware
    {
        #region Private properties
        private string _token = string.Empty;
        private IIoTConnectAPIDiscovery _ioTConnectAPIDiscovery;
        private string _envCode = string.Empty;
        private string _solutionKey = string.Empty;
        #endregion

        #region Ctor
        public Firmware(string token, string environmentCode, string solutionKey)
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
        /// <param name="call">The call.</param>
        /// <returns></returns>
        private void HandleFlurlErrorAsync(HttpCall call)
        {
            call.ExceptionHandled = true;
            IoTConnectException ioTConnectErrorResponse = JsonConvert.DeserializeObject<IoTConnectException>(call.Response.Content.ReadAsStringAsync().Result);
            throw ioTConnectErrorResponse;
        }
        #endregion

        #region Firmware Crud
        /// <summary>
        /// Get the list of available Firmware. Though filters are optional, you can add them as given in the parameters.To apply the filters, you need to pass these filters Method.
        /// </summary>
        /// <param name="request">Paging Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllFirmwareResult>>> All(PagingModel pagingModel)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.All);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             pageSize = pagingModel.PageSize,
                                             pageNumber = pagingModel.PageNo,
                                             searchtext = pagingModel.SearchText,
                                             sortBy = pagingModel.SortBy
                                         })
                                         .GetJsonAsync<BaseResponse<List<AllFirmwareResult>>>();

                return new DataResponse<List<AllFirmwareResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllFirmwareResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "All()");
                throw ex;
            }
        }

        /// <summary>
        /// Firmware Details. Get result off passing firmware guid. user able to pass multiple firmwareGuid with comma seperate.
        /// </summary>
        /// <param name="firmwareGuid">Firmware Guid (s) : user able to pass multiple firmwareGuid with comma seperate.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<FirmwareDetailsResult>>> FirmwareDetails(string firmwareGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(firmwareGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Firmware Guid is required",
                        Param = "Firmware"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<FirmwareDetailsResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.firmwareDetails);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, firmwareGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .GetJsonAsync<BaseResponse<List<FirmwareDetailsResult>>>();

                return new DataResponse<List<FirmwareDetailsResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<FirmwareDetailsResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "FirmwareDetails()");
                throw ex;
            }
        }

        /// <summary>
        /// Returns Firmware Look Up (s).
        /// </summary>
        /// <returns></returns>
        public async Task<DataResponse<List<FirmwareLookupResult>>> LookUp()
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.firmwareLookup);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .GetJsonAsync<BaseResponse<List<FirmwareLookupResult>>>();

                return new DataResponse<List<FirmwareLookupResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<FirmwareLookupResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "LookUp()");
                throw ex;
            }
        }

        /// <summary>
        /// Add firmware 
        /// </summary>
        /// <param name="createRequest">Add Firmware Model</param>
        /// <returns></returns>
        public async Task<DataResponse<AddFirmwareResult>> Add(AddFirmwareModel createRequest)
        {
            try
            {
                var errorList = Helper.ValidateObject(createRequest);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddFirmwareResult>(null)
                    {
                        errorMessages = errorList,
                        status = false
                    };
                }

                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.Add);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion);

                var addFirmware = await formattedUrl.WithHeaders(new { Authorization = Constants.bearerTokenType + _token })
                                                 .PostMultipartAsync(mp => mp.AddStringParts(new AddFirmwareModel { firmwareName = createRequest.firmwareName, hardware = createRequest.hardware, software = createRequest.software, firmwareDescription = createRequest.firmwareTag, firmwarefile = createRequest.firmwarefile, firmwareTag = createRequest.firmwareTag })).ReceiveJson<DataResponse<List<AddFirmwareResult>>>();

                return new DataResponse<AddFirmwareResult>(null)
                {
                    data = addFirmware.data.FirstOrDefault(),
                    message = addFirmware.message,
                    status = addFirmware.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddFirmwareResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "Add()");
                throw ex;
            }
        }

        /// <summary>
        /// Firmware Validate.
        /// </summary>
        /// <param name="request">Firmware Validate Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddFirmwareResult>> FirmwareValidate(FirmwareValidateModel request)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.FirmwareValidate);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion);
                var firmwarevalidate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AddFirmwareResult>>>();

                return new DataResponse<AddFirmwareResult>(null)
                {
                    data = firmwarevalidate.data.FirstOrDefault(),
                    message = firmwarevalidate.message,
                    status = firmwarevalidate.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddFirmwareResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "FirmwareValidate()");
                throw ex;
            }
        }

        /// <summary>
        /// Update Firmware.
        /// </summary>
        /// <param name="updateRequest">Update Firmware Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddFirmwareResult>> Update(UpdateFirmwareModel updateRequest)
        {
            try
            {
                var errorList = Helper.ValidateObject(updateRequest);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddFirmwareResult>(null)
                    {
                        errorMessages = errorList,
                        status = false
                    };
                }

                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.Update);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, updateRequest.FirmwareGuid);
                var content = new Flurl.Http.Content.CapturedMultipartContent();
                content.AddStringParts(new UpdateFirmwareModel { firmwareName = updateRequest.firmwareName, hardware = updateRequest.hardware, software = updateRequest.software, firmwareDescription = updateRequest.firmwareDescription, status = updateRequest.status });
                var updateFirmware = await formattedUrl.WithHeaders(new { Authorization = Constants.bearerTokenType + _token })
                                                  .SendAsync(System.Net.Http.HttpMethod.Put, content).ReceiveJson<DataResponse<List<AddFirmwareResult>>>();

                return new DataResponse<AddFirmwareResult>(null)
                {
                    data = updateFirmware.data?.FirstOrDefault(),
                    message = updateFirmware.message,
                    status = updateFirmware.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddFirmwareResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "Update()");
                throw ex;
            }
        }

        /// <summary>
        /// Firmware Lookup by Firmware Id.
        /// </summary>
        /// <param name="firmwareGuid">Firmware Guid.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<FirmwareUpgradeLookupResult>>> FirmwareLookUpById(string firmwareGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(firmwareGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Firmware Guid is required",
                        Param = "Firmware"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<FirmwareUpgradeLookupResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.firmwareUpgradLookup);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, firmwareGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .GetJsonAsync<BaseResponse<List<FirmwareUpgradeLookupResult>>>();

                return new DataResponse<List<FirmwareUpgradeLookupResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<FirmwareUpgradeLookupResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "FirmwareLookUpById()");
                throw ex;
            }
        }

        /// <summary>
        /// Get Firmware Upgrade lookup by Firmware Guid.
        /// </summary>
        /// <param name="firmwareUpgradeGuid">Firmware Guid.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<FirmwareUpgradeLookupResult>>> FirmwareUpgradeLookUpById(string firmwareGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(firmwareGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Firmware Guid is required",
                        Param = "Firmware"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<FirmwareUpgradeLookupResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.firmwareUpgradLookupById);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, firmwareGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .GetJsonAsync<BaseResponse<List<FirmwareUpgradeLookupResult>>>();

                return new DataResponse<List<FirmwareUpgradeLookupResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<FirmwareUpgradeLookupResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "FirmwareUpgradeLookUpById()");
                throw ex;
            }
        }

        /// <summary>
        /// Get Firmware Upgrade by Firmware Upgrade Guid.
        /// </summary>
        /// <param name="firmwareupgradeGuid">Firmware upgrade Guid.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<FirmwareUpgradeByUpgradeGuidResult>>> FirmwareUpgradeByFirmwareUpgradeGuid(string firmwareupgradeGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(firmwareupgradeGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Firmware Upgrade Guid is required",
                        Param = "Firmware"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<FirmwareUpgradeByUpgradeGuidResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.firmwareUpgradByFirmwareUpgradeId);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, firmwareupgradeGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .GetJsonAsync<BaseResponse<List<FirmwareUpgradeByUpgradeGuidResult>>>();

                return new DataResponse<List<FirmwareUpgradeByUpgradeGuidResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<FirmwareUpgradeByUpgradeGuidResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "FirmwareUpgradeByFirmwareUpgradeGuid()");
                throw ex;
            }
        }

        /// <summary>
        /// All Firmware upgrade.
        /// </summary>
        /// <param name="firmwareGuid">(Optional). Firmware Guid. Pass null if not required.</param>
        /// <param name="type">Firmware type</param>
        /// <param name="pagingModel">List Paging Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<FirmwareUpgradeByUpgradeGuidResult>>> AllFirmwareUpgrade(string firmwareGuid, string type, PagingModel pagingModel)
        {
            try
            {
                if (string.IsNullOrEmpty(type))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "type is required",
                        Param = "Firmware"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<FirmwareUpgradeByUpgradeGuidResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.firmwareUpgrads);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             firmwareGuid = firmwareGuid,
                                             type = type,
                                             pageSize = pagingModel.PageSize,
                                             pageNumber = pagingModel.PageNo,
                                             searchtext = pagingModel.SearchText,
                                             sortBy = pagingModel.SortBy
                                         })
                                         .GetJsonAsync<BaseResponse<List<FirmwareUpgradeByUpgradeGuidResult>>>();
                return new DataResponse<List<FirmwareUpgradeByUpgradeGuidResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<FirmwareUpgradeByUpgradeGuidResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "AllFirmwareUpgrade()");
                throw ex;
            }
        }

        /// <summary>
        /// Add firmware upgrade.
        /// </summary>
        /// <param name="createRequest">Add Firmware upgrade Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddFirmwareResult>> AddFirmwareUpgrade(AddFirmwareUpgradeModel createRequest)
        {
            try
            {
                var errorList = Helper.ValidateObject(createRequest);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddFirmwareResult>(null)
                    {
                        errorMessages = errorList,
                        status = false
                    };
                }

                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.AddFirmware);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion);

                var addFirmware = await formattedUrl.WithHeaders(new { Authorization = Constants.bearerTokenType + _token })
                                                 .PostMultipartAsync(mp => mp.AddStringParts(new AddFirmwareUpgradeModel { firmwareGuid = createRequest.firmwareGuid, software = createRequest.software, description = createRequest.description, firmwarefile = createRequest.firmwarefile })).ReceiveJson<DataResponse<List<AddFirmwareResult>>>();

                return new DataResponse<AddFirmwareResult>(null)
                {
                    data = addFirmware.data.FirstOrDefault(),
                    message = addFirmware.message,
                    status = addFirmware.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddFirmwareResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "AddFirmwareUpgrade()");
                throw ex;
            }
        }


        /// <summary>
        /// Update Firmware upgrade.
        /// </summary>
        /// <param name="updateRequest">Update Firmware upgrade Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddFirmwareResult>> UpdateFirmwareUpgrade(UpdateFirmwareUpgradeModel updateRequest)
        {
            try
            {
                var errorList = Helper.ValidateObject(updateRequest);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddFirmwareResult>(null)
                    {
                        errorMessages = errorList,
                        status = false
                    };
                }

                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.UpdateFirmwareUpgrade);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, updateRequest.firmwareUpgradeGuid);
                var content = new Flurl.Http.Content.CapturedMultipartContent();
                content.AddStringParts(new UpdateFirmwareUpgradeModel { Description = updateRequest.Description, FirmwareGuid = updateRequest.FirmwareGuid, Software = updateRequest.Software, FirmwareFile = updateRequest.FirmwareFile });
                var updateFirmware = await formattedUrl.WithHeaders(new { Authorization = Constants.bearerTokenType + _token })
                                                  .SendAsync(System.Net.Http.HttpMethod.Put, content).ReceiveJson<DataResponse<List<AddFirmwareResult>>>();

                return new DataResponse<AddFirmwareResult>(null)
                {
                    data = updateFirmware.data?.FirstOrDefault(),
                    message = updateFirmware.message,
                    status = updateFirmware.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddFirmwareResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "UpdateFirmwareUpgrade()");
                throw ex;
            }
        }

        /// <summary>
        /// Publish firmware 
        /// </summary>
        /// <param name="updateRequest">Firmware upgrade Guid.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddFirmwareResult>> PublishFirmwareUpgrade(string firmwareUpgradeGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(firmwareUpgradeGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Firmware Upgrade Guid is required",
                        Param = "Firmware"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<AddFirmwareResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.PublishFirmware);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, firmwareUpgradeGuid);
                var content = new Flurl.Http.Content.CapturedMultipartContent();
                var publishFirmware = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(new object()).ReceiveJson<DataResponse<List<AddFirmwareResult>>>();

                return new DataResponse<AddFirmwareResult>(null)
                {
                    data = publishFirmware.data?.FirstOrDefault(),
                    message = publishFirmware.message,
                    status = publishFirmware.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddFirmwareResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "PublishFirmwareUpgrade()");
                throw ex;
            }
        }

        /// <summary>
        /// Delete Firmware        
        /// </summary>
        /// <param name="firmwareUpgradeGuid">Firmware Upgrade Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<DeleteAttributeResult>> DeleteFirmwareUpgrade(string firmwareUpgradeGuid, bool isDeleteSingleFirmware)
        {
            try
            {
                if (string.IsNullOrEmpty(firmwareUpgradeGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "AttributeGuid is required",
                        Param = "AttributeGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<DeleteAttributeResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                DeleteAttributeResult deleteTemplateAttribute = new DeleteAttributeResult();
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.DeleteFirmware);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, firmwareUpgradeGuid);
                var templatedelte = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token }).SetQueryParams(new
                {
                    isDeleteSingleFirmware = isDeleteSingleFirmware
                }).SendJsonAsync(HttpMethod.Delete, deleteTemplateAttribute).ReceiveJson<DataResponse<List<DeleteAttributeResult>>>();

                return new DataResponse<DeleteAttributeResult>(null)
                {
                    data = templatedelte.data.FirstOrDefault(),
                    message = templatedelte.message,
                    status = templatedelte.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<DeleteAttributeResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "DeleteFirmwareUpgrade()");
                throw ex;
            }
        }

        /// <summary>
        /// Get Recent OTA Update List for Single Device. For that you need to pass device guid.
        /// </summary>
        /// <param name="deviceGuid">device Guid. </param>
        /// <param name="pagingModel">paging Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<RecentOTAResult>>> SingleOTA(string deviceGuid, PagingModel pagingModel)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Device Guid is required",
                        Param = "Device Guid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<RecentOTAResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.RecentOTA);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, deviceGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             pageSize = pagingModel.PageSize,
                                             pageNumber = pagingModel.PageNo,
                                             searchtext = pagingModel.SearchText,
                                             sortBy = pagingModel.SortBy
                                         })
                                         .GetJsonAsync<BaseResponse<List<RecentOTAResult>>>();

                return new DataResponse<List<RecentOTAResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<RecentOTAResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "SingleOTA()");
                throw ex;
            }
        }

        /// <summary>
        /// Returns All Recent OTA Update For All Device. Filter It Out With Passing Entity Guid, Entries and if you want to get ota for child entities pass isRecursive as true in All Recent Model.
        /// </summary>
        /// <param name="allRecentOTAModel">All Recent Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<RecentOTAResult>>> RecentOTA(AllRecentOTAModel allRecentOTAModel)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.AllRecentOTA);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             isRecursive = allRecentOTAModel.isRecursive,
                                             entityGuid = allRecentOTAModel.entityGuid,
                                             pageSize = allRecentOTAModel.pagingModel.PageSize,
                                             pageNumber = allRecentOTAModel.pagingModel.PageNo,
                                             searchtext = allRecentOTAModel.pagingModel.SearchText,
                                             sortBy = allRecentOTAModel.pagingModel.SortBy,
                                             Entries = allRecentOTAModel.Entries
                                         })
                                         .GetJsonAsync<BaseResponse<List<RecentOTAResult>>>();

                return new DataResponse<List<RecentOTAResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<RecentOTAResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "RecentOTA()");
                throw ex;
            }
        }

        /// <summary>
        /// Returns List Of Recent Update OTA.
        /// </summary>
        /// <param name="allRecentActivityModel">Recent Activity Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllRecentActivityResult>>> RecentActivity(RecentActivityModel allRecentActivityModel)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.AllRecentOTAActivity);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             status = allRecentActivityModel.status,
                                             count = allRecentActivityModel.count,
                                         })
                                         .GetJsonAsync<BaseResponse<List<AllRecentActivityResult>>>();

                return new DataResponse<List<AllRecentActivityResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllRecentActivityResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "RecentActivity()");
                throw ex;
            }
        }

        /// <summary>
        /// Get Recent OTA Update List for Single Device. For that you need to pass device guid.
        /// </summary>
        /// <param name="deviceGuid">device Guid. </param>        
        /// <returns></returns>
        public async Task<DataResponse<List<AllDeviceByOtaUpdateResult>>> OTAUpdateByDeviceGuid(string deviceGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Device Guid is required",
                        Param = "Device Guid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<AllDeviceByOtaUpdateResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.OTAUpdateByDeviceGuid);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, deviceGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })

                                         .GetJsonAsync<BaseResponse<List<AllDeviceByOtaUpdateResult>>>();

                return new DataResponse<List<AllDeviceByOtaUpdateResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllDeviceByOtaUpdateResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "OTAUpdateByDeviceGuid()");
                throw ex;
            }
        }

        /// <summary>
        /// Get Device List.Which part off ota update.
        /// </summary>
        /// <param name="allDeviceOtaUpdateModel">All Device OTA Update Model.<param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllDeviceByOtaUpdateResult>>> AllDeviceByOTAUpdate(AllDeviceOtaUpdateModel allDeviceOtaUpdateModel)
        {
            try
            {
                var errorList = Helper.ValidateObject(allDeviceOtaUpdateModel);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<List<AllDeviceByOtaUpdateResult>>(null)
                    {
                        errorMessages = errorList,
                        status = false
                    };
                }

                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.DeviceListByOTAUpdate);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, allDeviceOtaUpdateModel.otaUpdateGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                    .SetQueryParams(new
                    {
                        otaUpdateGuid = allDeviceOtaUpdateModel.otaUpdateGuid,
                        status = allDeviceOtaUpdateModel.status,
                        pageSize = allDeviceOtaUpdateModel.pagingModel.PageSize,
                        pageNumber = allDeviceOtaUpdateModel.pagingModel.PageNo,
                        searchtext = allDeviceOtaUpdateModel.pagingModel.SearchText,
                        sortBy = allDeviceOtaUpdateModel.pagingModel.SortBy,
                    })
                                         .GetJsonAsync<BaseResponse<List<AllDeviceByOtaUpdateResult>>>();
                return new DataResponse<List<AllDeviceByOtaUpdateResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllDeviceByOtaUpdateResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "AllDeviceByOTAUpdate()");
                throw ex;
            }
        }

        /// <summary>
        /// Get OTA Update Details Based On otaUpdateGuid and status.
        /// </summary>
        /// <param name="allDeviceOtaUpdateModel">All Device OTA Update Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllDeviceByOtaUpdateResult>>> AllOTAUpdateByGuidAndStatus(AllDeviceOtaUpdateModel allDeviceOtaUpdateModel)
        {
            try
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                if (string.IsNullOrEmpty(allDeviceOtaUpdateModel.status))
                {

                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Status is required",
                        Param = "status"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<AllDeviceByOtaUpdateResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                if (string.IsNullOrEmpty(allDeviceOtaUpdateModel.otaUpdateGuid))
                {

                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "OTA UpdateGuid is required",
                        Param = "otaUpdateGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<AllDeviceByOtaUpdateResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.DeviceListByOTAUpdateandStatus);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, allDeviceOtaUpdateModel.otaUpdateGuid, allDeviceOtaUpdateModel.status);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                    .SetQueryParams(new
                    {
                        pageSize = allDeviceOtaUpdateModel.pagingModel.PageSize,
                        pageNumber = allDeviceOtaUpdateModel.pagingModel.PageNo,
                        searchtext = allDeviceOtaUpdateModel.pagingModel.SearchText,
                        sortBy = allDeviceOtaUpdateModel.pagingModel.SortBy,
                    })
                                         .GetJsonAsync<BaseResponse<List<AllDeviceByOtaUpdateResult>>>();
                return new DataResponse<List<AllDeviceByOtaUpdateResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllDeviceByOtaUpdateResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "AllOTAUpdateByGuidAndStatus()");
                throw ex;
            }
        }

        /// <summary>
        /// List Of OTA Update.
        /// </summary>
        /// <param name="pagingModel">Paging Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllOTAUpdateResult>>> AllOTAUpdate(PagingModel pagingModel)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.AllOTAUpdate);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                    .SetQueryParams(new
                    {
                        pageSize = pagingModel.PageSize,
                        pageNumber = pagingModel.PageNo,
                        searchtext = pagingModel.SearchText,
                        sortBy = pagingModel.SortBy,
                    })
                                         .GetJsonAsync<BaseResponse<List<AllOTAUpdateResult>>>();
                return new DataResponse<List<AllOTAUpdateResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllOTAUpdateResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "AllOTAUpdate()");
                throw ex;
            }
        }

        /// <summary>
        /// Send OTA Update.
        /// </summary>
        /// <param name="request">Send OTA Update Model</param>
        /// <returns></returns>
        public async Task<DataResponse<SendOTAUpdateResult>> SendOTAUpdate(SendOTAUpdateModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<SendOTAUpdateResult>(null)
                    {
                        errorMessages = errorList,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.SendOTAUpdate);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion);
                var addTempalte = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<SendOTAUpdateResult>>>();

                return new DataResponse<SendOTAUpdateResult>(null)
                {
                    data = addTempalte.data.FirstOrDefault(),
                    message = addTempalte.message,
                    status = addTempalte.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<SendOTAUpdateResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "SendOTAUpdate()");
                throw ex;
            }
        }

        /// <summary>
        /// This used for updating ota update status for this you need to pass deviceGuid where you want to update firmware,otaUpdateItemGuid which firmware you want to update,status which are (pending/sent/success/failed/skippe
        /// </summary>
        /// <param name="request">Update OTA Status Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateOTAStatusResult>> UpdateOTAStatus(UpdateOTAStatusModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<UpdateOTAStatusResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.UpdateOTAStatus);
                string formattedUrl = string.Empty;
                if (string.IsNullOrEmpty(request.deviceGuid))
                {
                    formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, request.uniqueId, request.otaUpdateItemGuid, request.status);
                }
                else
                    formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion, request.deviceGuid, request.otaUpdateItemGuid, request.status);

                var updateTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(request).ReceiveJson<DataResponse<List<UpdateOTAStatusResult>>>();

                return new DataResponse<UpdateOTAStatusResult>(null)
                {
                    data = updateTemplate.data?.FirstOrDefault(),
                    message = updateTemplate.message,
                    status = updateTemplate.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateOTAStatusResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "UpdateOTAStatus()");
                throw ex;
            }
        }

        /// <summary>
        /// Get OTA Update History data.Pass Multiple Device (s).
        /// </summary>
        /// <param name="createRequest">OTA Update History Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<OTAUpdateHistoryResult>>> OTAUpdateHistory(OTAUpdateHistoryModel createRequest)
        {
            try
            {
                var errorList = Helper.ValidateObject(createRequest);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<List<OTAUpdateHistoryResult>>(null)
                    {
                        errorMessages = errorList,
                        status = false
                    };
                }

                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.FirmwareBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, FirmwareApi.OTAUpdateHistory);
                string formattedUrl = String.Format(accessTokenUrl, Constants.firmwareVersion);

                var addFirmware = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(createRequest).ReceiveJson<BaseResponse<List<OTAUpdateHistoryResult>>>();

                return new DataResponse<List<OTAUpdateHistoryResult>>(null)
                {
                    data = addFirmware.Data,
                    message = addFirmware.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<OTAUpdateHistoryResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Firmware", "OTAUpdateHistory()");
                throw ex;
            }
        }


        #endregion
    }
}
