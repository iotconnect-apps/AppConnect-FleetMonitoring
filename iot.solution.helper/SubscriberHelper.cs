using component.helper.Interface;
using component.logger;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Response = iot.solution.entity.Response;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
using System.Reflection;
using System.Net.Http;
using System.Net;
using System.Linq;
using Newtonsoft.Json;

namespace component.helper
{
    public class SubscriberHelper : ISubscriberHelper
    {
        private readonly IHttpClientHelper _httpClientHelper;
        private static string _subcriptionAccessToken;
        
        private readonly LogHandler.Logger _logger;
        private readonly string apiBaseURL = String.Empty;
        
        public SubscriberHelper(LogHandler.Logger logger)
        {
            _logger = logger;
            _httpClientHelper = new HttpClientHelper(logger);
            apiBaseURL = SolutionConfiguration.Configuration.SubscriptionAPI.BaseUrl;
            InitSubscriptionToken();
        }
        public bool ValidateSubscriptionAccessToken()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_subcriptionAccessToken))
                {
                    JwtSecurityTokenHandler jwthandler = new JwtSecurityTokenHandler();
                    Microsoft.IdentityModel.Tokens.SecurityToken jwttoken = jwthandler.ReadToken(_subcriptionAccessToken);

                    if (jwttoken.ValidTo < DateTime.UtcNow.AddMinutes(5))
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return false;
        }
        public void InitSubscriptionToken()
        {
            try
            {
                if (!ValidateSubscriptionAccessToken())
                {
                    Entity.TokenRequest request = new Entity.TokenRequest
                    {
                        ClientID = SolutionConfiguration.Configuration.SubscriptionAPI.ClientID,
                        ClientSecret = SolutionConfiguration.Configuration.SubscriptionAPI.ClientSecret,
                        UserName = SolutionConfiguration.Configuration.SubscriptionAPI.UserName
                    };
                    Entity.TokenResponse response = new Entity.TokenResponse();
                    response = _httpClientHelper.Post<Entity.TokenRequest, Entity.TokenResponse>(apiBaseURL + "subscription/token", request);
                    _subcriptionAccessToken = response.accessToken;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
        }
        public Response.TimezoneResponse GetTimezoneData()
        {
            Response.TimezoneResponse result = new Response.TimezoneResponse();
            try
            {
                result = _httpClientHelper.Get<Response.TimezoneResponse>(apiBaseURL + "timezone?pageNo=1&pageSize=200", _subcriptionAccessToken);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }

            return result;
        }
        public Response.CountryResponse GetCountryData()
        {
            Response.CountryResponse result= new Response.CountryResponse();
            try
            {
                result = _httpClientHelper.Get<Response.CountryResponse>(apiBaseURL + "country?pageNo=1&pageSize=1000", _subcriptionAccessToken);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public Response.StateResponse GetStateData(string countryID)
        {
            Response.StateResponse result = new Response.StateResponse();
            try
            {
                result = _httpClientHelper.Get<Response.StateResponse>(apiBaseURL + "state?pageNo=1&pageSize=1000" + "&countryID=" + countryID, _subcriptionAccessToken);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public bool GetCompanyDetails(string solutionCode, string companyName)
        {
            Entity.CompanyList result = new Entity.CompanyList();
            try
            {
                ///api/v1/consumers?displayDataOf=0&search=jessica.ref&solutionIdOrProductCode=PRD0000065&isActive=true

                Entity.Subscriber response = _httpClientHelper.Get<Entity.Subscriber>(string.Format("{0}solution/subscriber?displayDataOf=0&productCode={1}&search={2}", apiBaseURL, solutionCode, companyName), _subcriptionAccessToken);
                if (response.data != null && response.data.Count > 0)
                {
                    if (response.data.Where(t => t.companyName.Equals(companyName)).FirstOrDefault() != null)
                    {
                        return false;
                    }
                    else { return true; }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return false;
        }
        public Entity.BaseResponse<bool> ValidateCompany(Entity.ValidateCompanyRequest requestData)
        {
            InitSubscriptionToken();
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(false);
            //bool IsCompanyExists = false;
            bool IsEmailExists = false;
            if (!string.IsNullOrEmpty(requestData.Email))
            {
                Entity.CompanyUserDetails existingConsumer = GetCompanyUserDetails(requestData.CompanyName,SolutionConfiguration.Configuration.SubscriptionAPI.SolutionId.ToString(), requestData.Email);
                if (existingConsumer != null && existingConsumer.isExist)//Check email exists or not
                {
                    IsEmailExists = true;
                    response.Message = existingConsumer.errorMessage;
                }
                else
                {
                    IsEmailExists =false;
                }
            }
            //if (!string.IsNullOrEmpty(requestData.CompanyName))
            //{
            //    if (GetCompanyDetails(SolutionConfiguration.Configuration.SubscriptionAPI.SolutionCode, requestData.CompanyName))
            //    {
            //        IsCompanyExists = false;
            //    }
            //    else
            //    {
            //        IsCompanyExists = true;
            //    }
            //}
            if (IsEmailExists)
            {
                response.IsSuccess = false;
                
            }
            //else if (IsCompanyExists)
            //{
            //    response.IsSuccess = false;
            //    response.Message = "Company name already registered!";
            //}
            //else if (IsEmailExists)
            //{
            //    response.IsSuccess = false;
            //    response.Message = "Email address already registered!";
            //}
            else
            {
                response.IsSuccess = true;
                response.Message = "";
            }
            return response;
        }
        public Entity.SaveCompanyResponse CreateCompany(Entity.SaveCompanyRequest requestData)
        {
            InitSubscriptionToken();
            //Entity.SubsciberCompanyDetails existingConsumer = GetSubscriberDetails(requestData.SolutionCode, requestData.User.Email);
            //if (existingConsumer != null && string.IsNullOrEmpty(existingConsumer.email))//Check email exists or not
            //{
            Entity.CompanyUserDetails existingConsumer = GetCompanyUserDetails(requestData.User.CompanyName, SolutionConfiguration.Configuration.SubscriptionAPI.SolutionId.ToString(), requestData.User.Email);
            if (existingConsumer != null && existingConsumer.isExist)//Check email exists or not
            {
                throw new Exception(existingConsumer.errorMessage);
            }
            else 
            { 
                requestData.SolutionCode = SolutionConfiguration.Configuration.SubscriptionAPI.SolutionId.ToString();
                using (var response = _httpClientHelper.Post<Entity.SaveCompanyRequest>(string.Concat(apiBaseURL, "solution/company"), requestData, _subcriptionAccessToken))
                {
                    if ((int)response.StatusCode == (int)HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<Entity.SaveCompanyResponse>(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        List<Entity.ErrorMessageResponse> errorMessage = JsonConvert.DeserializeObject<List<Entity.ErrorMessageResponse>>(response.Content.ReadAsStringAsync().Result);
                        if (errorMessage != null && errorMessage.Any())
                        {
                            throw new Exception(string.Format("{0}", errorMessage.FirstOrDefault().msg));
                        }
                        else
                        {
                            throw new Exception("Please try again. Something went wrong!!!");
                        }
                    }
                }
            }
          
        }
        public Response.SubscriptionPlanResponse GetSubscriptionPlans(string solutionID)
        {
            Response.SubscriptionPlanResponse result = new Response.SubscriptionPlanResponse();
            try
            {
                result = _httpClientHelper.Get<Response.SubscriptionPlanResponse>(string.Format("{0}solution/{1}/plans?pageNo=1&pageSize=1000", apiBaseURL, solutionID), _subcriptionAccessToken);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.SubscriberData>> SubscriberList(string solutionID, Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.SubscriberData>> result = new Entity.SearchResult<List<Entity.SubscriberData>>();
            try
            {
                //Entity.Subscriber response = _httpClientHelper.Get<Entity.Subscriber>(string.Format("{0}solution/subscriber?pageNo={2}&pageSize={3}&displayDataOf=0&productCode={1}", apiBaseURL, solutionID, request.PageNumber, request.PageSize), _subcriptionAccessToken);
                Entity.Subscriber response = _httpClientHelper.Get<Entity.Subscriber>(string.Format("{0}solution/subscriber?displayDataOf=0&productCode={1}&pageNo={2}&pageSize={3}&search={4}&orderBy={5}", apiBaseURL, solutionID, request.PageNumber, request.PageSize, request.SearchText, request.OrderBy), _subcriptionAccessToken);
                result.Items = response.data;
                result.Count = response.@params.count;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public Entity.SubsciberCompanyDetails GetSubscriberDetails(string solutionCode, string userEmail)
        {
            Entity.SubsciberCompanyDetails result = new Entity.SubsciberCompanyDetails();
            try
            {
                result = _httpClientHelper.Get<Entity.SubsciberCompanyDetails>(string.Format("{0}subscriber/{1}/{2}/consumption/active", apiBaseURL, solutionCode, userEmail), _subcriptionAccessToken);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

        private Entity.CompanyUserDetails GetCompanyUserDetails(string companyName,string solutionCode, string userEmail)
        {
            Entity.CompanyUserDetails result = new Entity.CompanyUserDetails();
            try
            {

                Entity.ValidateUserRequest request = new Entity.ValidateUserRequest
                {
                    solutionId = solutionCode,
                    email = userEmail,
                    companyName = companyName
                };

                using (var response = _httpClientHelper.PUT<Entity.ValidateUserRequest>(string.Concat(apiBaseURL, "company/user/exist"), request, _subcriptionAccessToken))
                {
                    if ((int)response.StatusCode == (int)HttpStatusCode.OK)
                    {
                        result= JsonConvert.DeserializeObject<Entity.CompanyUserDetails>(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        List<Entity.ErrorMessageResponse> errorMessage = JsonConvert.DeserializeObject<List<Entity.ErrorMessageResponse>>(response.Content.ReadAsStringAsync().Result);
                        if (errorMessage != null && errorMessage.Any())
                        {
                            throw new Exception(string.Format("{0}", errorMessage.FirstOrDefault().msg));
                        }
                        else
                        {
                            throw new Exception("Please try again. Something went wrong!!!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public Entity.LastSyncResponse GetLastSyncDetails()
        {
            Entity.LastSyncResponse result = new Entity.LastSyncResponse();
            try
            {
                result = _httpClientHelper.Get<Entity.LastSyncResponse>(apiBaseURL + "consumption/lastsync", _subcriptionAccessToken);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
    }
}
