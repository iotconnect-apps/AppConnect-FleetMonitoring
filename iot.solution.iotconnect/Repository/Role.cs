using Flurl.Http;
using iot.solution.iotconnect.Interface;
using IoTConnect.Common;
using IoTConnect.Common.Constant;
using IoTConnect.Common.Interface;
using IoTConnect.Common.Repository;
using IoTConnect.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace iot.solution.iotconnect.Repository
{
    public class Role : IRole
    {

        #region Private properties
        private string _token = string.Empty;
        private IIoTConnectAPIDiscovery _ioTConnectAPIDiscovery;
        private string _envCode = string.Empty;
        private string _solutionKey = string.Empty;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the IOTConnect.UserProvider.User class.
        /// </summary>
        /// <param name="token">IotConnect Bearer Token. Refer IOTConnect.Common.Auth.Login() method to generate bearer token. </param>
        /// <param name="envCode">IotConnect Environment Code.</param>
        /// <param name="solutionKey">IotConnect Solution Key. Should be a valid Solution Key.</param>
        public Role(string token, string envCode, string solutionKey)
        {
            _token = token;
            _envCode = envCode;
            _solutionKey = solutionKey;
            _ioTConnectAPIDiscovery = new IoTConnectAPIDiscovery();
            //FlurlHttp.Configure(settings => settings.OnError = HandleFlurlErrorAsync);
        }
        #endregion

        #region Private Method
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

        #region Role Crud
        /// <summary>
        /// Get role list with user count.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllRoleResult>>> AllRole(AllRoleModel request)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.Roles);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                var roleList = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SetQueryParams(new
                                                 {
                                                     PageSize = request.PageSize,
                                                     PageNo = request.PageNo,
                                                     SortBy = request.SortBy,
                                                     SearchText = request.SearchText
                                                 }).GetJsonAsync<BaseResponse<List<AllRoleResult>>>();

                return new DataResponse<List<AllRoleResult>>(null)
                {
                    data = roleList.Data,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllRoleResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "AllRole()");
                throw ex;
            }
        }

        /// <summary>
        /// Add new Role.
        /// </summary>
        /// <param name="model">Add Role Model.</param>        
        /// <returns></returns>
        public async Task<DataResponse<AddRoleResult>> AddRole(AddRoleModel model)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.AddRole);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                var addRole = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PostJsonAsync(model).ReceiveJson<DataResponse<List<AddRoleResult>>>();

                return new DataResponse<AddRoleResult>(null)
                {
                    data = addRole.data.FirstOrDefault(),
                    message = addRole.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddRoleResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "AddRole()");
                throw ex;
            }
        }

        /// <summary>
        /// Get All user role.
        /// </summary>
        /// <param name="roleGuid">(Optional)Role guid. Required for single user role by roleguid. Default empty.</param>
        /// <returns></returns>
        private async Task<SearchUserResult> AllUserRole(string roleGuid = "")
        {

            var roleportalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
            string roleAccessTokenUrl = string.Concat(roleportalApi, UserApi.UserRole);
            string roleFormattedUrl = String.Format(roleAccessTokenUrl, Constants.userVersion, roleGuid);
            return await roleFormattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                             .GetJsonAsync<SearchUserResult>();
        }

        /// <summary>
        /// Get role by role id.
        /// </summary>
        /// <param name="roleGuid">Role id.</param>
        /// <returns></returns>
        public async Task<DataResponse<SingleRoleResult>> SingleRole(string roleGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(roleGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    return new DataResponse<SingleRoleResult>(null)
                    {
                        errorMessages = errorItemModels,
                        message = "RoleGuid is required",
                        status = false
                    };
                }

                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.RoleDetail);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion, roleGuid);

                var role = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .GetJsonAsync<DataResponse<List<SingleRoleResult>>>();

                if (role == null)
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    return new DataResponse<SingleRoleResult>(null)
                    {
                        errorMessages = errorItemModels,
                        message = "Role not found",
                        status = false
                    };
                }

                return new DataResponse<SingleRoleResult>(null)
                {
                    data = role.data.FirstOrDefault(),
                    message = role.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<SingleRoleResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "SingleRole()");
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
                string accessTokenUrl = string.Concat(portalApi, UserApi.RoleLookup);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .GetJsonAsync<DataResponse<List<AllRoleLookupResult>>>();

                return new DataResponse<List<AllRoleLookupResult>>(null)
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
                return new DataResponse<List<AllRoleLookupResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "AllRoleLookup()");
                throw ex;
            }
        }

        /// <summary>
        /// Update Role
        /// </summary>
        /// <param name="roleGuid">Role Guid</param>
        /// <param name="model">Update Role Model</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateRoleResult>> UpdateRole(string roleGuid, UpdateRoleModel model)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.UpdateRole);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion, roleGuid);
                var updateRole = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(model).ReceiveJson<DataResponse<List<UpdateRoleResult>>>();

                return new DataResponse<UpdateRoleResult>(null)
                {
                    data = updateRole.data.FirstOrDefault(),
                    message = updateRole.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateRoleResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "UpdateRole()");
                throw ex;
            }
        }

        /// <summary>
        /// Delete Role.
        /// </summary>
        /// <param name="UserGuid">Role Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<DeleteRoleResult>> DeleteRole(string roleGuid)
        {
            try
            {
                DeleteRoleModel request = new DeleteRoleModel();
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.DeleteRole);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion, roleGuid);
                var deleteRole = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SendJsonAsync(HttpMethod.Delete, request).ReceiveJson<DataResponse<List<DeleteRoleResult>>>();

                return new DataResponse<DeleteRoleResult>(null)
                {
                    data = deleteRole.data.FirstOrDefault(),
                    message = deleteRole.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<DeleteRoleResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "DeleteRole()");
                throw ex;
            }
        }
        /// <summary>
        /// Make User active or inactive.
        /// </summary>
        /// <param name="UserGuid">User Unique Identifier.</param>
        /// <param name="Status">Status.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateRoleStatusResult>> UpdateRoleStatus(string RoleGuid, bool Status)
        {
            try
            {
                if (string.IsNullOrEmpty(RoleGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "RoleGuid is required",
                        Param = "RoleGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<UpdateRoleStatusResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                UpdateRoleStatusModel request = new UpdateRoleStatusModel { isActive = Status };
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.UpdateRoleStatus);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion, RoleGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(request).ReceiveJson<DataResponse<List<UpdateRoleStatusResult>>>();

                return new DataResponse<UpdateRoleStatusResult>(null)
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
                return new DataResponse<UpdateRoleStatusResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "UpdateRoleStatus()");
                throw ex;
            }
        }
        #endregion



    }
}
