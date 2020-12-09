using Flurl.Http;
using IoTConnect.Common;
using IoTConnect.Common.Constant;
using IoTConnect.Common.Interface;
using IoTConnect.Common.Repository;
using IoTConnect.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTConnect.EntityProvider
{
    public class Entity : IEntity
    {
        private string _token = string.Empty;
        private IIoTConnectAPIDiscovery _ioTConnectAPIDiscovery;
        private string _envCode = string.Empty;
        private string _solutionKey = string.Empty;

        public Entity(string token, string environmentCode, string solutionKey)
        {
            _token = token;
            _envCode = environmentCode;
            _solutionKey = solutionKey;
            _ioTConnectAPIDiscovery = new IoTConnectAPIDiscovery();
            FlurlHttp.Configure(settings => settings.OnError = HandleFlurlErrorAsync);
        }

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

        /// <summary>
        /// Create new entity.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddEntityResult>> Add(AddEntityModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddEntityResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, EntityApi.Add);
                string formattedUrl = String.Format(accessTokenUrl, Constants.entityVersion);
                var addEntity = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AddEntityResult>>>();

                return new DataResponse<AddEntityResult>(null)
                {
                    data = addEntity.data.FirstOrDefault(),
                    message = addEntity.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddEntityResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Entity", "Add()");
                throw ex;
            }
        }

        /// <summary>
        /// Update entity.
        /// </summary>
        /// <param name="entityGuid">The entity unique identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateEntityResult>> Update(string entityGuid, UpdateEntityModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<UpdateEntityResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, EntityApi.Update);
                string formattedUrl = String.Format(accessTokenUrl, Constants.entityVersion, entityGuid);
                var updateEntity = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(request).ReceiveJson<DataResponse<List<UpdateEntityResult>>>();

                return new DataResponse<UpdateEntityResult>(null)
                {
                    data = updateEntity.data.FirstOrDefault(),
                    message = updateEntity.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateEntityResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Entity", "Update()");
                throw ex;
            }
        }

        /// <summary>
        /// Delete entity.
        /// </summary>
        /// <param name="entityGuid">The entity unique identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<DeleteEntityResult>> Delete(string entityGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(entityGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "EntityGuid is required",
                        Param = "EntityGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<DeleteEntityResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, EntityApi.Delete);
                string formattedUrl = String.Format(accessTokenUrl, Constants.entityVersion, entityGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .DeleteAsync().ReceiveJson<DataResponse<List<DeleteEntityResult>>>();

                return new DataResponse<DeleteEntityResult>(null)
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
                return new DataResponse<DeleteEntityResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Entity", "Delete()");
                throw ex;
            }
        }

        /// <summary>
        /// Get entity list.
        /// </summary>
        /// <returns></returns>
        public async Task<DataResponse<List<AllEntityResult>>> All()
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, EntityApi.All);
                string formattedUrl = String.Format(accessTokenUrl, Constants.entityVersion);
                return await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .GetJsonAsync<DataResponse<List<AllEntityResult>>>();
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllEntityResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Entity", "All()");
                throw ex;
            }
        }

        /// <summary>
        /// Get entity detail.
        /// </summary>
        /// <param name="entityGuid">The entity unique identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<SingleEntityResult>> Single(string entityGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(entityGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "EntityGuid is required",
                        Param = "EntityGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<SingleEntityResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, EntityApi.Single);
                string formattedUrl = String.Format(accessTokenUrl, Constants.entityVersion, entityGuid);

                var entity = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .GetJsonAsync<DataResponse<List<SingleEntityResult>>>();

                return new DataResponse<SingleEntityResult>(null)
                {
                    data = entity.data.FirstOrDefault(),
                    message = entity.message,
                    status = entity.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<SingleEntityResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Entity", "Single()");
                throw ex;
            }
        }
    }
}
