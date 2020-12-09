using Flurl.Http;
using IoTConnect.Common;
using IoTConnect.Common.Constant;
using IoTConnect.Common.Interface;
using IoTConnect.Common.Repository;
using IoTConnect.DeviceProvider.Interface;
using IoTConnect.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IoTConnect.DeviceProvider
{
    /// <summary>
    /// IotConnect Device Provider class. Contains methods to manage Iot Devices. 
    /// </summary>
    public class Device : IDevice
    {
        #region private Properties
        private string _token = string.Empty;
        private IIoTConnectAPIDiscovery _ioTConnectAPIDiscovery;
        private string _envCode = string.Empty;
        private string _solutionKey = string.Empty;
        #endregion

        /// <summary>
        /// Initializes a new instance of the IOTConnect.DeviceProvider.Device class.
        /// </summary>
        /// <param name="token">Bearer Token.</param>
        /// <param name="environmentCode">IotConnect Environment Code.</param>
        /// <param name="solutionKey">IotConnect Solution Key. Should be a valid Solution Key.</param>
        #region ctor
        public Device(string token, string environmentCode, string solutionKey)
        {
            _token = token;
            _envCode = environmentCode;
            _solutionKey = solutionKey;
            _ioTConnectAPIDiscovery = new IoTConnectAPIDiscovery();
            FlurlHttp.Configure(settings => settings.OnError = HandleFlurlErrorAsync);
        }
        #endregion

        #region Device Crud
        /// <summary>
        /// Get all device by unique identifier.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <param name="parentDeviceUniqueId">The parent device unique identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<SingleResult>>> AllUniqueDevice(string uniqueId, string parentDeviceUniqueId = "")
        {
            try
            {
                if (string.IsNullOrEmpty(uniqueId))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "UniqueId is required",
                        Param = "UniqueId"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<SingleResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.UniqDevice);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, uniqueId);
                return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             parentDeviceUniqueId = parentDeviceUniqueId,
                                         })
                                         .GetJsonAsync<DataResponse<List<SingleResult>>>();

            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                ErrorItemModel errorItemModel = new ErrorItemModel()
                {
                    Message = ex.message
                };
                errorItemModels.Add(errorItemModel);
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<SingleResult>>(null)
                {
                    errorMessages = errorItemModels,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "AllUniqueDevice()");
                throw ex;
            }
        }

        /// <summary>
        /// Get all Device(s) based on Parameters supplied. This method will get all devices if no parameter is supplied.
        /// </summary>
        /// <param name="pageNumber">Device Model.</param>
        /// <returns></returns>
        public async Task<BaseResponse<List<AllDeviceResult>>> AllDevice(AllDeviceModel deviceModel)
        {

            try
            {
                BaseResponse<List<AllDeviceResult>> resultDevices = new BaseResponse<List<AllDeviceResult>>();
                BaseResponse<List<AllDeviceResult>> resultTemplateDevices = new BaseResponse<List<AllDeviceResult>>();
                BaseResponse<List<AllDeviceResult>> resultEntityDevices = new BaseResponse<List<AllDeviceResult>>();
                BaseResponse<List<AllDeviceResult>> resultCompanyDevices = new BaseResponse<List<AllDeviceResult>>();

                if (!string.IsNullOrWhiteSpace(deviceModel.templateGuid))
                {
                    resultTemplateDevices = await AllDeviceTemplate(deviceModel.templateGuid);
                }

                if (!string.IsNullOrWhiteSpace(deviceModel.entityGuid))
                {
                    resultEntityDevices = await AllDeviceEntity(deviceModel.entityGuid,deviceModel.companyGuid);
                }

                if (!string.IsNullOrWhiteSpace(deviceModel.companyGuid))
                {
                    resultCompanyDevices = await AllDeviceCompany(deviceModel.companyGuid);
                }

                if (!string.IsNullOrWhiteSpace(deviceModel.templateGuid) || !string.IsNullOrWhiteSpace(deviceModel.entityGuid) || !string.IsNullOrWhiteSpace(deviceModel.companyGuid))
                {
                    if (resultTemplateDevices != null && resultTemplateDevices.Data?.Any() == true)
                    {
                        if (resultDevices.Data == null)
                            resultDevices.Data = new List<AllDeviceResult>();

                        resultDevices.Data.AddRange(resultTemplateDevices.Data);
                    }
                    if (resultEntityDevices != null && resultEntityDevices.Data?.Any() == true)
                    {
                        if (resultDevices.Data == null)
                            resultDevices.Data = new List<AllDeviceResult>();

                        resultDevices.Data.AddRange(resultEntityDevices.Data);
                    }
                    if (resultCompanyDevices != null && resultCompanyDevices.Data?.Any() == true)
                    {
                        if (resultDevices.Data == null)
                            resultDevices.Data = new List<AllDeviceResult>();

                        resultDevices.Data.AddRange(resultCompanyDevices.Data);
                    }

                    if (resultDevices != null && resultDevices.Data?.Any() == true)
                        resultDevices.Data = resultDevices.Data.GroupBy(x => x.Guid).Select(y => y.First()).ToList();

                    return new BaseResponse<List<AllDeviceResult>>()
                    {
                        Data = resultDevices.Data,
                        Params = new Dictionary<string, object>() { { "count", resultDevices.Data.Count } }
                    };
                }

                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.Get);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             pageSize = -1,//deviceModel.PageSize,
                                             pageNumber =-1,// deviceModel.PageNo,
                                             sortBy = deviceModel.SortBy,
                                             searchText = deviceModel.SearchText,
                                             CustomField = deviceModel.CustomField
                                         })
                                         .GetJsonAsync<BaseResponse<List<AllDeviceResult>>>();

                return new BaseResponse<List<AllDeviceResult>>()
                {
                    Data = result!=null ? result.Data:null,
                    Params = new Dictionary<string, object>() { { "count", result!=null && result.Data !=null?result.Data.Count:0 } }
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "AllDevice()");
                throw ex;
            }
        }

        /// <summary>
        /// Create new Device.
        /// </summary>
        /// <param name="request">Add Device model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddDeviceResult>> Add(AddDeviceModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddDeviceResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                request.firmwareUpgradeGuid = null;
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.Add);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var addDevice = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AddDeviceResult>>>();

                return new DataResponse<AddDeviceResult>(null)
                {
                    data = addDevice.data.FirstOrDefault(),
                    message = addDevice.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddDeviceResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "Add()");
                throw ex;
            }

        }

        /// <summary>
        /// Update Device.
        /// </summary>
        /// <param name="deviceGuid">Device Guid.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateDeviceResult>> Update(string deviceGuid, UpdateDeviceModel updateDevice)
        {
            try
            {

                var errorList = Helper.ValidateObject(updateDevice);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<UpdateDeviceResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.Update);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, deviceGuid);
                var updateTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(updateDevice).ReceiveJson<DataResponse<List<UpdateDeviceResult>>>();

                return new DataResponse<UpdateDeviceResult>(null)
                {
                    data = updateTemplate.data.FirstOrDefault(),
                    message = updateTemplate.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateDeviceResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "Update()");
                throw ex;
            }
        }

        /// <summary>
        /// Delete Device.
        /// </summary>
        /// <param name="deviceGuid">Device Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<DeleteDeviceResult>> Delete(string deviceGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "DeviceGuid is required",
                        Param = "DeviceGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<DeleteDeviceResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                DeleteDeviceModel deleteDevice = new DeleteDeviceModel();
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.Delete);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion, deviceGuid);
                var deletedevice = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SendJsonAsync(HttpMethod.Delete, deleteDevice).ReceiveJson<DataResponse<List<DeleteDeviceResult>>>();

                return new DataResponse<DeleteDeviceResult>(null)
                {
                    data = deletedevice.data.FirstOrDefault(),
                    message = deletedevice.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<DeleteDeviceResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "Delete()");
                throw ex;
            }
        }

        /// <summary>
        /// In simple words, acquire a device means registering your device on Azure IoT Hub. When you add a device, it adds in IoTConnect only. However, to transmit data from one device to the other, you need to register a device with Azure IoT Hub.
        /// To acquire your device, use this Method.To do that, send device uniqueID in Method.
        /// </summary>
        /// <param name="uniqeid">Device Uniqe Id.</param>
        /// <param name="acquireDevice">Device acquire model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AcquireDeviceResult>> AcquireDevice(string uniqeid, AcquireDeviceModel acquireDevice)
        {
            try
            {
                if (string.IsNullOrEmpty(uniqeid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "UniqueId is required",
                        Param = "UniqueId"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<AcquireDeviceResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.Acquire);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, uniqeid);
                var updateTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(acquireDevice).ReceiveJson<DataResponse<List<AcquireDeviceResult>>>();

                return new DataResponse<AcquireDeviceResult>(null)
                {
                    data = updateTemplate.data.FirstOrDefault(),
                    message = updateTemplate.message,
                    status = updateTemplate.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AcquireDeviceResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "AcquireDevice()");
                return null;
            }
        }

        /// <summary>
        //This Method is used to release or remove your IoT device from Azure IoT Hub.For that, you need to send device Unique ID in method.
        /// </summary>
        /// <param name="UniqueId">Device Uniqe Id.</param>
        /// <returns></returns>
        public async Task<DataResponse<ReleaseDeviceResult>> ReleaseDevice(string UniqueId)
        {
            try
            {
                if (string.IsNullOrEmpty(UniqueId))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "UniqueId is required",
                        Param = "UniqueId"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<ReleaseDeviceResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                ReleaseDeviceModel releaseDevice = new ReleaseDeviceModel();
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.Release);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, UniqueId);
                var updateTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(releaseDevice).ReceiveJson<DataResponse<List<ReleaseDeviceResult>>>();

                return new DataResponse<ReleaseDeviceResult>(null)
                {
                    data = updateTemplate.data.FirstOrDefault(),
                    message = updateTemplate.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<ReleaseDeviceResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "ReleaseDevice()");
                throw ex;
            }
        }

        
        /// <summary>
        //This Method allows you to change the status of your device. For that, you need to pass deviceGuid in method and isActive in the request body. In isActive, send true to activate your device and false to deactivate your device. When you deactivate your device, this will also release your device from Azure IoT Hub, even if the device is acquired.
        /// </summary>
        /// <param name="uniqeid">Device Guid.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateDeviceResult>> UpdateDeviceStatus(string deviceGuid, UpdateDeviceStatusModel updateDeviceStatusModel)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "DeviceGuid is required",
                        Param = "DeviceGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<UpdateDeviceResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.DeviceStatus);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, deviceGuid);
                var updateTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(updateDeviceStatusModel).ReceiveJson<DataResponse<List<UpdateDeviceResult>>>();

                return new DataResponse<UpdateDeviceResult>(null)
                {
                    data = updateTemplate.data.FirstOrDefault(),
                    message = updateTemplate.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateDeviceResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "UpdateDeviceStatus()");
                throw ex;
            }
        }

        /// <summary>
        //This method helps you to update an entity of multiple devices in a single call. For that, send the list of deviceGuids in the model class along with new entityGuid.
        /// </summary>
        /// <param name="entityGuid">Entity Guid.</param>
        /// <param name="UpdateDeviceEntityBulkModel">Update Device Entity Bulk Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateDeviceResult>> UpdateDeviceEntityBulk(string entityGuid, List<UpdateDeviceEntityBulkModel> updatedeviceEntityModel)
        {
            try
            {
                if (string.IsNullOrEmpty(entityGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Entity Guid is required",
                        Param = "Entity Guid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<UpdateDeviceResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.UpdatedeviceEntityBulk);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, entityGuid);
                var updateTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(updatedeviceEntityModel).ReceiveJson<DataResponse<List<UpdateDeviceResult>>>();

                return new DataResponse<UpdateDeviceResult>(null)
                {
                    data = updateTemplate.data.FirstOrDefault(),
                    message = updateTemplate.message,
                    status = updateTemplate.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateDeviceResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "UpdateDeviceEntityBulk()");
                throw ex;
            }
        }


        /// <summary>
        /// This method provides you the list of available child devices in a gateway device. For that, you need to send gateway deviceGuid in request url.
        /// You can also add the filters as given in the parameters.Though the filters are optional, to apply them, you need to send these filter parameters in a query string.        
        /// </summary>
        ///  <param name="parentDeviceGuid">Parent Device Identifier.</param>
        /// <param name="request">Paging Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllChildDeviceResult>>> AllChildDevice(string parentDeviceGuid, PagingModel pagingModel)
        {
            try
            {
                if (string.IsNullOrEmpty(parentDeviceGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Parent Device Guid is required",
                        Param = "Device Guid"   
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<AllChildDeviceResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.ChildDevice);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, parentDeviceGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             pageSize = pagingModel.PageSize,
                                             pageNumber = pagingModel.PageNo,
                                             searchtext = pagingModel.SearchText,
                                             sortBy = pagingModel.SortBy,
                                         })
                                         .GetJsonAsync<BaseResponse<List<AllChildDeviceResult>>>();

                return new DataResponse<List<AllChildDeviceResult>>(null)
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

                return new DataResponse<List<AllChildDeviceResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "AllChildDevice()");
                throw ex;
            }
        }

        /// <summary>
        ///Get a twin property list of any device using this method.For that, you need to send device uniqueId in method of which you want to get the twin property list.
        /// </summary>
        ///  <param name="uniqueid">Unique Identifier.</param>        
        /// <returns></returns>
        public async Task<DataResponse<List<DeviceTwinResult>>> DeviceTwinByUniqueId(string uniqueid)
        {
            try
            {
                if (string.IsNullOrEmpty(uniqueid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "UniqueId is required",
                        Param = "UniqueId"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<DeviceTwinResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.DeviceTwin);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, uniqueid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                                                  .GetJsonAsync<BaseResponse<List<DeviceTwinResult>>>();

                return new DataResponse<List<DeviceTwinResult>>(null)
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

                return new DataResponse<List<DeviceTwinResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "DeviceTwinByUniqueId()");
                throw ex;
            }
        }

        /// <summary>
        /// This method allows you to get the list of users with their device permissions. For that, you need to send deviceGuid method of which you want the device permissions list.        
        /// </summary>
        ///  <param name="deviceGuid">Device Identifier.</param>
        /// <param name="request">Paging Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<DeviceGrantResult>>> DeviceGrant(string deviceGuid,PagingModel pagingModel)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Device guid is required",
                        Param = "Device Guid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<DeviceGrantResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.DeviceGrant);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, deviceGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                                                  .GetJsonAsync<BaseResponse<List<DeviceGrantResult>>>();

                return new DataResponse<List<DeviceGrantResult>>(null)
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

                return new DataResponse<List<DeviceGrantResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "DeviceGrant()");
                throw ex;
            }
        }

        /// <summary>
        ///This Method allows you to update the twin property of a device. For that, you need to send localName, desiredValue and templateSettingGuid in updatedeviceenitymodel and uniqueId in method of which you look to update the twin property.
        /// </summary>
        /// <param name="uniqueid">unique Guid.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateDeviceResult>> UpdateDeviceTwin(string uniqueid, UpdateDeviceTwinModel updatedeviceEntityModel)
        {
            try
            {
                if (string.IsNullOrEmpty(uniqueid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "UniqueId is required",
                        Param = "UniqueId"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<UpdateDeviceResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.UpdateDeviceTwin);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, uniqueid);
                var updateTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(updatedeviceEntityModel).ReceiveJson<DataResponse<List<UpdateDeviceResult>>>();

                return new DataResponse<UpdateDeviceResult>(null)
                {
                    data = updateTemplate.data.FirstOrDefault(),
                    message = updateTemplate.message,
                    status = updateTemplate.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateDeviceResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "UpdateDeviceTwin()");
                throw ex;
            }
        }

        /// <summary>
        /// Create new Grant Permmision.
        /// </summary>
        /// <param name="request">Add Grant model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddDeviceGrantResult>> AddGrant(string deviceGuid,AddGrantModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddDeviceGrantResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.DeviceAddGrant);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, deviceGuid);
                var addDevice = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AddDeviceGrantResult>>>();

                return new DataResponse<AddDeviceGrantResult>(null)
                {
                    data = addDevice.data.FirstOrDefault(),
                    message = addDevice.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddDeviceGrantResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "AddGrant()");
                throw ex;
            }
        }

        /// <summary>
        /// Delete Grant.
        /// </summary>
        /// <param name="UserDevicePermissionGuid">User Device permission Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<DeleteDevicePermissionResult>> DeleteGrant(string UserDevicePermissionGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(UserDevicePermissionGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "UserDevicePermissionGuid is required",
                        Param = "UserDevicePermissionGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<DeleteDevicePermissionResult>(null)
                    {
                        errorMessages = errorItemModels,
                        message = "Data Error",
                        status = false
                    };
                }
                DeleteDeviceModel deleteDevice = new DeleteDeviceModel();
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.DeviceDeleteGrant);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion, UserDevicePermissionGuid);
                var deletedevice = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SendJsonAsync(HttpMethod.Delete, deleteDevice).ReceiveJson<DataResponse<List<DeleteDevicePermissionResult>>>();

                return new DataResponse<DeleteDevicePermissionResult>(null)
                {
                    data = deletedevice.data.FirstOrDefault(),
                    message = deletedevice.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<DeleteDevicePermissionResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "DeleteGrant()");
                throw ex;
            }
        }

        /// <summary>
        /// Allow different users to view or manage devices by allotting their devices using this API. For that, you need to send deviceGuid with device permissions and userGuid in request url to whom you want to allot these devices.
        /// </summary>
        /// <param name="request">Allotted Device User Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AllottedDeviceToUserResult>> AllottedDeviceToUser(string userId,AllottedDeviceUserModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AllottedDeviceToUserResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.AllottedDeviceUser);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, userId);
                var addDevice = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AllottedDeviceToUserResult>>>();

                return new DataResponse<AllottedDeviceToUserResult>(null)
                {
                    data = addDevice.data.FirstOrDefault(),
                    message = addDevice.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AllottedDeviceToUserResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "AllottedDeviceToUser()");
                throw ex;
            }
        }

        /// <summary>
        /// This Method lets you get the currently logged in user’s allotted device list.
        /// </summary>
        /// <param name="pagingModel">Paging model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllotedDeviceResult>>> GetAllAllottedDevice(PagingModel pagingModel)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.GetAllottedDeviceUser);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                          .SetQueryParams(new
                                          {
                                              pageNumber = pagingModel.PageNo,
                                              pageSize = pagingModel.PageSize,
                                              searchtext = pagingModel.SearchText,
                                              sortBy = pagingModel.SortBy,
                                          })
                                          .GetJsonAsync<DataResponse<List<AllotedDeviceResult>>>();

                return new DataResponse<List<AllotedDeviceResult>>(null)
                {
                    data = result.data,
                    message = result.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllotedDeviceResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "GetAllAllottedDevice()");
                throw ex;
            }
        }

        /// <summary>
        /// This Method lets you get the single user allotted device list.
        /// </summary>
        /// <param name="pagingModel">paging model.</param>
        /// <param name="userGuid">User Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<AllotedDeviceResult>> GetSingleAllottedDevice(string userGuid,PagingModel pagingModel)
        {
            try
            {
                if (string.IsNullOrEmpty(userGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "User guid is required",
                        Param = "User"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<AllotedDeviceResult>(null)
                    {
                        errorMessages = errorItemModels,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.SingleAllottedDeviceUser);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, userGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                          .SetQueryParams(new
                                          {
                                              pageSize = pagingModel.PageSize,
                                              pageNumber = pagingModel.PageNo,
                                              searchtext = pagingModel.SearchText,
                                              sortBy = pagingModel.SortBy,
                                          })
                                          .GetJsonAsync<DataResponse<List<AllotedDeviceResult>>>();

                return new DataResponse<AllotedDeviceResult>(null)
                {
                    data = result.data.FirstOrDefault(),
                    message = result.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AllotedDeviceResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Device", "GetSingleAllottedDevice()");
                throw ex;
            }
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

        private async Task<BaseResponse<List<AllDeviceResult>>> AllDeviceEntity(string entityGuid = "",string companyGuid="")
        {
            var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
            string accessTokenUrl = string.Concat(portalApi, DeviceApi.DeviceEntity);
            string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, entityGuid);
            return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                     .SetQueryParams(new
                                     {
                                         companyGuid = companyGuid
                                     })
                                     .GetJsonAsync<BaseResponse<List<AllDeviceResult>>>();
        }

        private async Task<BaseResponse<List<AllDeviceResult>>> AllDeviceCompany(string companyGuid = "")
        {
            var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
            string accessTokenUrl = string.Concat(portalApi, DeviceApi.DeviceCompany);
            string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, companyGuid);
            return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                     .SetQueryParams(new
                                     {
                                         companyGuid = companyGuid,
                                     })
                                     .GetJsonAsync<BaseResponse<List<AllDeviceResult>>>();
        }

        private async Task<BaseResponse<List<AllDeviceResult>>> AllDeviceTemplate(string templateGuid = "")
        {
            var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
            string accessTokenUrl = string.Concat(portalApi, TemplateApi.Template);
            string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, templateGuid);
            return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                     .SetQueryParams(new
                                     {
                                         templateGuid = templateGuid,
                                     })
                                     .GetJsonAsync<BaseResponse<List<AllDeviceResult>>>();
        }

        public async Task<DataResponse<List<DeviceCounterResult>>> GetDeviceCounters(string companyGuid)
        {
            var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
            string accessTokenUrl = string.Concat(portalApi, DeviceApi.DeviceCounter);
            string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
            return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                     .SetQueryParams(new { companyGuid = companyGuid })
                                     .GetJsonAsync<DataResponse<List<DeviceCounterResult>>>();
        }
        public async Task<DataResponse<List<DeviceCounterByEntityResult>>> GetDeviceCounterByEntity(string entityGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(entityGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Entity Guid is required",
                        Param = "EntityGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<DeviceCounterByEntityResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.DeviceCounterByEntity);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new { entity = entityGuid, recursive = true })
                                         .GetJsonAsync<DataResponse<List<DeviceCounterByEntityResult>>>();
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<DeviceCounterByEntityResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }

        }
        public async Task<DataResponse<List<DeviceTelemetryData>>> GetTelemetryData(string deviceGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Device Guid is required",
                        Param = "DeviceGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<DeviceTelemetryData>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.TelemetryBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.TelemetryData);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, deviceGuid);
                return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new { companyGuid = deviceGuid })
                                         .GetJsonAsync<DataResponse<List<DeviceTelemetryData>>>();
            }
            catch (IoTConnectException ex)
            {

                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<DeviceTelemetryData>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
           
           
        }

        public async Task<DataResponse<List<DeviceConnectionStatus>>> GetConnectionStatus(string uniqueId)
        {


            try
            {
                if (string.IsNullOrEmpty(uniqueId))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "UniqueId required",
                        Param = "UniqueId"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<DeviceConnectionStatus>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, DeviceApi.ConnectionStatus);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, uniqueId);
                return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .GetJsonAsync<DataResponse<List<DeviceConnectionStatus>>>();
            }
            catch (IoTConnectException ex)
            {

                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<DeviceConnectionStatus>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
        }
        #endregion
    }
}
