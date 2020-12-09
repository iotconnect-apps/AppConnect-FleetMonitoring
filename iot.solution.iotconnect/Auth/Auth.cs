using Flurl.Http;

using IoTConnect.Common.Constant;
using IoTConnect.Common.Interface;
using IoTConnect.Common.Repository;
using IoTConnect.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTConnect.Common
{
    /// <summary>
    /// IotConnect Authentication Provider class. Contains methods required to login into IotConnect.
    /// </summary>
    public class Auth
    {
        private IIoTConnectAPIDiscovery _ioTConnectAPIDiscovery;
        private string _envCode = string.Empty;
        private string _solutionKey = string.Empty;

        #region ctor
        /// <summary>
        /// Initializes a new instance of the IOTConnect.Common.Auth class.
        /// </summary>
        /// <param name="environmentCode">IotConnect Environment Code.</param>
        /// <param name="solutionKey">IotConnect Solution Key. Should be a valid Solution Key.</param>
        public Auth(string environmentCode, string solutionKey)
        {
            _envCode = environmentCode;
            _solutionKey = solutionKey;
            _ioTConnectAPIDiscovery = new IoTConnectAPIDiscovery();
            FlurlHttp.Configure(settings => settings.OnError = HandleFlurlErrorAsync);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Login into IotConnect.
        /// </summary>
        /// <param name="loginModel">The login model.</param>
        /// <returns></returns>
        public async Task<DataResponse<LoginResult>> Login(LoginModel loginModel)
        {

            try
            {
                var portalAuthApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.AuthBaseUrl);
                string formattedUrl = String.Format(portalAuthApi, Constants.authVersion);
                string basicToken = await GetBasicToken(formattedUrl);

                if (!string.IsNullOrWhiteSpace(basicToken)) {

                    var dataResponse = await GetLoginAccess(loginModel.UserName, loginModel.Password, formattedUrl, basicToken, _solutionKey);
                    return new DataResponse<LoginResult>(null)
                    {
                        data = dataResponse,
                        status = true
                    };
                }
                else{
                    return new DataResponse<LoginResult>(null);
                }
                    
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);

                return new DataResponse<LoginResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }

        }
        public async Task<DataResponse<IdentityResult>> Identity(string token)
        {
            try
            {
                var portalAuthApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.UserBaseUrl);
                string formattedUrl = String.Format(portalAuthApi, Constants.authVersion);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var dataResponse = await GetIdentityAccess(formattedUrl, token, _solutionKey);
                    return new DataResponse<IdentityResult>(null)
                    {
                        data = dataResponse,
                        status = true
                    };
                }
                else
                {
                    return new DataResponse<IdentityResult>(null);
                }

            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);

                return new DataResponse<IdentityResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
        }
        /// <summary>
        /// Refresh IotConnect token. Requires bearer token and refresh token.
        /// </summary>
        /// <param name="token">Bearer token.</param>
        /// <param name="refreshTokenModel">The refresh token model.</param>
        /// <returns></returns>
        public async Task<DataResponse<LoginResult>> RefreshToken(string token, RefreshTokenModel refreshTokenModel)
        {
            try
            {
                var portalAuthApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.AuthBaseUrl);
                string accessTokenUrl = string.Concat(portalAuthApi, AuthApi.RefreshTokenUrl);
                string formattedUrl = String.Format(accessTokenUrl, Constants.authVersion);
                var data = await formattedUrl
                             .WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + token })
                             .PostJsonAsync(refreshTokenModel).ReceiveJson<LoginResult>();
                return new DataResponse<LoginResult>(null)
                {
                    data = data,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                return new DataResponse<LoginResult>(null)
                {
                    errorMessages = ex.error,
                    status = false
                };
            };
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

        /// <summary>
        /// Get Basic Token.
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        private async Task<string> GetBasicToken(string apiUrl)
        {
            string basicTokenUrl = string.Concat(apiUrl, AuthApi.BasicTokenUrl);
            string formattedUrl = String.Format(basicTokenUrl, Constants.authVersion);
            LoginResult data = await formattedUrl.GetAsync().ReceiveJson<LoginResult>();
            if (data.status == (int)System.Net.HttpStatusCode.OK)
                return data.data?.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Get Login access.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="authUrl"></param>
        /// <param name="basicToken"></param>
        /// <param name="solutionKey"></param>
        /// <returns></returns>
        private async Task<LoginResult> GetLoginAccess(string userName, string password, string authUrl, string basicToken, string solutionKey)
        {
            var model = new LoginModel { UserName = userName, Password = password };
            string accessTokenUrl = string.Concat(authUrl, AuthApi.LoginUrl);
            string formattedUrl = String.Format(accessTokenUrl, Constants.authVersion);
            return await formattedUrl
                         .WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.basicTokenType + basicToken, solution_key = solutionKey })
                         .PostJsonAsync(model).ReceiveJson<LoginResult>();
        }
        /// <summary>
        /// Get Identity access.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="authUrl"></param>
        /// <param name="basicToken"></param>
        /// <param name="solutionKey"></param>
        /// <returns></returns>
        private async Task<IdentityResult> GetIdentityAccess(string authUrl, string bearerToken, string solutionKey)
        {

            string accessTokenUrl = string.Concat(authUrl, AuthApi.IdentityUrl);
            string formattedUrl = String.Format(accessTokenUrl, Constants.authVersion);
            var data = await formattedUrl
                 .WithHeaders(new { Authorization = Constants.bearerTokenType + bearerToken })
                        .GetAsync().ReceiveJson<object>();
            IdentityResult result = JsonConvert.DeserializeObject<IdentityResult>(data.ToString());
            return result;
        }
        #endregion
    }
}
