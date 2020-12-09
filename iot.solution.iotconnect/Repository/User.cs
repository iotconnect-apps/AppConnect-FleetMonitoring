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
using System.Net.Http;
using System.Threading.Tasks;

namespace IoTConnect.UserProvider
{
    /// <summary>
    /// IotConnect User Provider. Contains methods related to IotConnect User module.
    /// </summary>
    public class User : IUser
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
        public User(string token, string envCode, string solutionKey)
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

        #region User Crud + Reset Password.
        /// <summary>
        /// Add new User.
        /// </summary>
        /// <param name="model">Add User Model.</param>
        /// <param name="password">(Optional)Password. Default Empty.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddUserResult>> Add(AddUserModel model, string password = "")
        {
            try
            {
                var errorList = Helper.ValidateObject(model);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddUserResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                model.Properties = null;
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.Add);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PostJsonAsync(model).ReceiveJson<DataResponse<List<AddUserResult>>>();

                if (!string.IsNullOrWhiteSpace(password))
                {
                    var resetPaasword = await ResetPassword(new ResetPasswordModel() { Email = model.UserId, InvitationGuid = result.data.FirstOrDefault().invitationGuid, NewPassword = password });
                    return new DataResponse<AddUserResult>(null)
                    {
                        data = null,
                        message = "User registered successfully",
                        status = true,
                    };
                }
                return new DataResponse<AddUserResult>(null)
                {
                    data = result.data.FirstOrDefault(),
                    message = result.message,
                    status = true,
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddUserResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "Add()");
                throw ex;
            }
        }


        /// <summary>
        /// Update User. 
        /// </summary>
        /// <param name="userGuid">User identifier.</param>
        /// <param name="model">Update user model.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateUserResult>> Update(string userGuid, UpdateUserModel model)
        {
            try
            {
                var errorList = Helper.ValidateObject(model);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<UpdateUserResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.Update);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion, userGuid);
                var updateUser = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(model).ReceiveJson<DataResponse<List<UpdateUserResult>>>();

                return new DataResponse<UpdateUserResult>(null)
                {
                    data = updateUser.data.FirstOrDefault(),
                    message = updateUser.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateUserResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "Update()");
                throw ex;
            }
        }

        /// <summary>
        /// Delete user.
        /// </summary>
        /// <param name="UserGuid">User Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<DeleteUserResult>> Delete(string UserGuid)
        {
            try
            {

                if (string.IsNullOrEmpty(UserGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "UserGuid is required",
                        Param = "UserGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<DeleteUserResult>(null)
                    {
                        errorMessages = errorItemModels,
                        message = "Data Error",
                        status = false
                    };
                }
                DeleteUserModel request = new DeleteUserModel();
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.Delete);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion, UserGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SendJsonAsync(HttpMethod.Delete, request).ReceiveJson<DataResponse<List<DeleteUserResult>>>();

                return new DataResponse<DeleteUserResult>(null)
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
                return new DataResponse<DeleteUserResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "Delete()");
                return null;
                //throw ex;
            }
        }

        public async Task<DataResponse<SearchUserResult>> AllCompanyUser(SearchUserModel request)
        {
            var enitityportalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
            string enitityAccessTokenUrl = string.Concat(enitityportalApi, UserApi.CompanyUser);
            string enitityFormattedUrl = String.Format(enitityAccessTokenUrl, Constants.userVersion);

            var result = await enitityFormattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                .SetQueryParams(new
                {
                    pageSize = request.PageSize,
                    pageNumber = request.PageNo,
                    searchtext = request.SearchText,
                    sortBy = request.SortBy
                }).GetJsonAsync<SearchUserResult>();

            return new DataResponse<SearchUserResult>(null)
            {
                data = result,
                status = true
            };
        }

        /// <summary>
        /// Get User information by user id.
        /// </summary>
        /// <param name="userGuid">User id.</param>
        /// <param name="userInfoFlag">(Optional)User info. Default null.</param>
        /// <returns></returns>
        public async Task<DataResponse<ResponseGetUserDetailById>> Single(string userGuid, bool? userInfoFlag = null)
        {
            try
            {

                if (string.IsNullOrEmpty(userGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "UserGuid is required",
                        Param = "UserGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<ResponseGetUserDetailById>(null)
                    {
                        errorMessages = errorItemModels,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.Get);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion, userGuid);
                var user = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SetQueryParam("userInfoFlag", userInfoFlag)
                                                 .GetJsonAsync<DataResponse<List<ResponseGetUserDetailById>>>();

                return new DataResponse<ResponseGetUserDetailById>(null)
                {
                    data = user.data.FirstOrDefault(),
                    message = user.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {

                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<ResponseGetUserDetailById>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "Single()");
                throw ex;
            }
        }

        /// <summary>
        /// Search User.
        /// </summary>
        /// <param name="request">Search filter</param>
        /// <param name="roleGuid">(Optional)User role guid. Required for search by role. Default empty.</param>
        /// <param name="entityGuid">(Optional)Entity guid. Required for search by entity. Default empty.</param>
        /// <returns></returns>
        public async Task<DataResponse<SearchUserResult>> Search(SearchUserModel request, string roleGuid = "", string entityGuid = "")
        {

            try
            {
                if (!string.IsNullOrWhiteSpace(entityGuid))
                {
                    var entityResult = await AllUserEntity(entityGuid);
                    SearchUserResult roleResult = null;
                    if (!string.IsNullOrWhiteSpace(roleGuid))
                    {
                        roleResult = await AllUserRole(roleGuid);
                    }

                    if (roleResult != null)
                    {
                        entityResult.data.AddRange(roleResult.data);
                        entityResult.data = entityResult.data.GroupBy(x => x.UserId).Select(y => y.First()).ToList();
                        entityResult.invokingUser = roleResult.invokingUser;
                        return new DataResponse<SearchUserResult>(null)
                        {
                            data = entityResult,
                            status = true
                        };
                    }
                    else
                    {
                        return new DataResponse<SearchUserResult>(null)
                        {
                            data = entityResult,
                            status = true
                        };
                    }
                }

                else if (!string.IsNullOrWhiteSpace(roleGuid))
                {
                    var result = await AllUserRole(roleGuid);
                    return new DataResponse<SearchUserResult>(null)
                    {
                        data = result,
                        status = true
                    };
                }

                else
                {
                    var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                    string accessTokenUrl = string.Concat(portalApi, UserApi.Search);
                    string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                    var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                       .GetJsonAsync<SearchUserResult>();
                    return new DataResponse<SearchUserResult>(null)
                    {
                        data = result,
                        status = true
                    };
                }
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<SearchUserResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "Search()");
                throw ex;
            }

        }

        /// <summary>
        /// Get All user entity.
        /// </summary>
        /// <param name="entityGuid">(Optional)Entity guid. Required for single entity by entityguid. Default empty.</param>
        /// <returns></returns>
        private async Task<SearchUserResult> AllUserEntity(string entityGuid = "")
        {

            var enitityportalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
            string enitityAccessTokenUrl = string.Concat(enitityportalApi, UserApi.UserEntity);
            string enitityFormattedUrl = String.Format(enitityAccessTokenUrl, Constants.userVersion, entityGuid);
            return await enitityFormattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                             .GetJsonAsync<SearchUserResult>();
        }

        /// <summary>
        /// Get active or inactive User count. Supply Company information to get companywise user count.
        /// </summary>
        /// <param name="companyGuid">The company unique identifier(Company id is optional).</param>
        /// <param name="status">Status should be active/inactive or optional.</param>
        /// <returns></returns>
        public async Task<DataResponse<SingleUserCountResult>> UserCount(string status, string companyGuid = "")
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.Count);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);

                var userCount = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SetQueryParams(new { companyGuid = companyGuid, status = status })
                                                 .GetJsonAsync<DataResponse<List<SingleUserCountResult>>>();

                return new DataResponse<SingleUserCountResult>(null)
                {
                    data = userCount.data.FirstOrDefault(),
                    message = userCount.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<SingleUserCountResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "AllUserEntity()");
                throw ex;
            }
        }

        /// <summary>
        /// Forgot password.
        /// </summary>
        /// <param name="EmailId">Email Id.</param>
        /// <returns></returns>
        public async Task<DataResponse<ForgotPasswordResult>> ForgotPassword(string EmailId)
        {
            try
            {
                if (string.IsNullOrEmpty(EmailId))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "EmailId is required",
                        Param = "EmailId"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<ForgotPasswordResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                ForgotPasswordModel request = new ForgotPasswordModel { Email = EmailId };
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.ForgotPassword);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PostJsonAsync(request).ReceiveJson<DataResponse<List<ForgotPasswordResult>>>();

                return new DataResponse<ForgotPasswordResult>(null)
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
                return new DataResponse<ForgotPasswordResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "ForgotPassword()");
                throw ex;
            }
        }

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<DataResponse<ChangePasswordResult>> ChangePassword(ChangePasswordModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<ChangePasswordResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.ChangePassword);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PostJsonAsync(request).ReceiveJson<DataResponse<List<ChangePasswordResult>>>();
                return new DataResponse<ChangePasswordResult>(null)
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
                return new DataResponse<ChangePasswordResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "ChangePassword()");
                throw ex;
            }
        }

        /// <summary>
        /// Reset password.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<DataResponse<ResetPasswordResult>> ResetPassword(ResetPasswordModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<ResetPasswordResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                if (string.IsNullOrEmpty(request.InvitationGuid))
                {
                    var result = await ForgotPassword(request.Email);
                    if (result != null && result.data != null)
                    {
                        request.InvitationGuid = result?.data?.InvitationGuid;
                    }
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.ResetPassword);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                var response = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PostJsonAsync(request).ReceiveJson<DataResponse<List<ResetPasswordResult>>>();

                return new DataResponse<ResetPasswordResult>(null)
                {
                    data = response.data.FirstOrDefault(),
                    message = response.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<ResetPasswordResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "ResetPassword()");
                throw ex;
            }
        }

        /// <summary>
        /// Make User active or inactive.
        /// </summary>
        /// <param name="UserGuid">User Unique Identifier.</param>
        /// <param name="Status">Status.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateUserStatusResult>> UpdateUserStatus(string UserGuid, bool Status)
        {
            try
            {
                if (string.IsNullOrEmpty(UserGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "UserGuid is required",
                        Param = "UserGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<UpdateUserStatusResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                UpdateUserStatusModel request = new UpdateUserStatusModel { isActive = Status };
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.UpdateStatus);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion, UserGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(request).ReceiveJson<DataResponse<List<UpdateUserStatusResult>>>();

                return new DataResponse<UpdateUserStatusResult>(null)
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
                return new DataResponse<UpdateUserStatusResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "UpdateUserStatus()");
                throw ex;
            }
        }

        /// <summary>
        /// Get active or inactive User count. Supply Company information to get companywise user count.
        /// </summary>
        /// <param name="companyGuid">The company unique identifier(Company id is optional).</param>
        /// <param name="status">Status should be active/inactive or optional.</param>
        /// <returns></returns>
        public async Task<DataResponse<UserQuotaExhaustedNotificationResult>> GetQuotaExhaustedNotification(string token)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, UserApi.QuotaExhaustedNotification);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                var userQuotaResult = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + token })
                                                 .GetJsonAsync<DataResponse<List<UserQuotaExhaustedNotificationResult>>>();
                return new DataResponse<UserQuotaExhaustedNotificationResult>(null)
                {
                    data = userQuotaResult.data.FirstOrDefault(),
                    message = userQuotaResult.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UserQuotaExhaustedNotificationResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "User", "GetQuotaExhaustedNotification()");
                throw ex;
            }
        }
        #endregion

        #region Role Crud

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
        #endregion
    }
}
