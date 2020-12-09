using iot.solution.entity.Structs.Routes;
using Microsoft.AspNetCore.Authorization;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Entity = iot.solution.entity;
using Microsoft.Extensions.Configuration;

namespace host.iot.solution.Controllers
{
    [Route("api/account")]
    public class AccountController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IAdminUserService _adminUserService;

        public AccountController(IUserService userService, IAdminUserService adminUserService)
        {
            _userService = userService;
            _adminUserService = adminUserService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(UserRoute.Route.Validate, Name = UserRoute.Name.Validate)]
        public Entity.BaseResponse<Entity.LoginResponse> Login(Entity.LoginRequest request)
        {
            Entity.BaseResponse<Entity.LoginResponse> response = new Entity.BaseResponse<Entity.LoginResponse>(true, "");
            try
            {
                Entity.ActionStatus loginResponse = _userService.Login(request);
                if (loginResponse!= null && loginResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Data = GetApiResponse(loginResponse.Data);
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = loginResponse.Message;
                }
            }
           
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.LoginResponse>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route(UserRoute.Route.Identity, Name = UserRoute.Name.Identity)]
        public Entity.BaseResponse<object> Identity(Entity.ValidateRequest request)
        {
            Entity.BaseResponse<object> response = new Entity.BaseResponse<object>(true, "");
            try
            {
                Entity.ActionStatus loginResponse = _userService.IdentityLogin(request.token);
                if (loginResponse != null && loginResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Data = loginResponse.Data;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = loginResponse.Message;
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<object>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route(UserRoute.Route.RefreshToken, Name = UserRoute.Name.RefreshToken)]
        public Entity.BaseResponse<Entity.LoginResponse> RefreshToken(Entity.RefreshTokenRequest request)
        {
            Entity.BaseResponse<Entity.LoginResponse> response = new Entity.BaseResponse<Entity.LoginResponse>(true, "");
            try
            {
                response.Data = GetApiResponse(_userService.RefreshToken(request));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.LoginResponse>(false, ex.Message);
            }
            return response;
        }

        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [HttpPost]
        [Route(UserRoute.Route.ChangePassword, Name = UserRoute.Name.ChangePassword)]
        public Entity.BaseResponse<bool> ChangePassword(Entity.ChangePasswordRequest request)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true, "");
            try
            {
                var result = _userService.ChangePassword(request);
                response.Data = result.Success;
                response.IsSuccess = result.Success;
                response.Message = result.Message;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        /// <summary>
        /// Login For Admin User 
        /// </summary>
        /// <param name="request">Request contains Username and Password</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route(UserRoute.Route.AdminLogin, Name = UserRoute.Name.AdminLogin)]
        public Entity.BaseResponse<Entity.LoginResponse> AdminLogin(Entity.LoginRequest request)
        {
            Entity.BaseResponse<Entity.LoginResponse> response = new Entity.BaseResponse<Entity.LoginResponse>(true);
            try
            {
                var loginResult = _adminUserService.AdminLogin(request);
                if (loginResult != null && loginResult.Success)
                {
                    var secretKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(component.helper.SolutionConfiguration.Configuration.Token.SecurityKey));
                    List<System.Security.Claims.Claim> clm = new List<System.Security.Claims.Claim>();

                    var propertyInfoCompanyId = loginResult.Data.GetType().GetProperty("CompanyGuid");
                    var companyId = propertyInfoCompanyId.GetValue(loginResult.Data, null);
                    var propertyInfoUserId = loginResult.Data.GetType().GetProperty("Guid");
                    var currentUserId = propertyInfoUserId.GetValue(loginResult.Data, null);

                    var firstName = loginResult.Data.GetType().GetProperty("FirstName");
                    var fisrtNameValue = firstName.GetValue(loginResult.Data, null);

                    var lastName = loginResult.Data.GetType().GetProperty("LastName");
                    var lastNameValue = lastName.GetValue(loginResult.Data, null);

                    string fullName = fisrtNameValue + " " + lastNameValue;

                    clm.Add(new System.Security.Claims.Claim("IOT_CONNECT", "AdminUser"));
                    clm.Add(new System.Security.Claims.Claim("CURRENT_USERID", currentUserId.ToString().ToUpper()));
                    var tokeOptions = new JwtSecurityToken(
                        issuer: component.helper.SolutionConfiguration.Configuration.Token.Issuer.ToLower(),
                        audience: component.helper.SolutionConfiguration.Configuration.Token.Audience.ToLower(),
                        claims: clm,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(secretKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)
                    );
                    response.IsSuccess = true;
                    response.Data = new Entity.LoginResponse()
                    {                        
                        access_token = new JwtSecurityTokenHandler().WriteToken(tokeOptions),
                        expires_in = DateTime.Now.AddMinutes(30).Ticks,
                        UserDetail = new Entity.UserDetail
                        {
                            Id = currentUserId.ToString().ToUpper(),
                            CompanyId = companyId.ToString().ToUpper(),
                            IsAdmin = true,
                            FullName = fullName
                        }
                    };
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = loginResult.Message; //"Invalid Credentials.";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.LoginResponse>(false, ex.Message);
            }
            return response;
        }

        #region Private Methods
        private Entity.LoginResponse GetApiResponse(Entity.LoginResponse response)
        {
            if (response != null)
            {
                var secretKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(component.helper.SolutionConfiguration.Configuration.Token.SecurityKey));
                List<System.Security.Claims.Claim> clm = new List<System.Security.Claims.Claim>();
                clm.Add(new System.Security.Claims.Claim("IOT_CONNECT", response.access_token));
                var tokeOptions = new JwtSecurityToken(
                    issuer: component.helper.SolutionConfiguration.Configuration.Token.Issuer.ToLower(),
                    audience: component.helper.SolutionConfiguration.Configuration.Token.Audience.ToLower(),
                    claims: clm,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(secretKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)
                );
                response.access_token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            }
            return response;
        }
        #endregion
    }
}