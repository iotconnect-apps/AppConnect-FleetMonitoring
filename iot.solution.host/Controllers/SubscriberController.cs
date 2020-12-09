using component.helper;
using host.iot.solution.Controllers;
using iot.solution.entity;
using iot.solution.entity.Response;
using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace iot.solution.host.Controllers
{
    [Route(SubscriberRoute.Route.Global)]
    [AllowAnonymous]
    //[ApiController]
    public class SubscriberController : BaseController
    {
        private readonly ISubscriberService _service;

        public SubscriberController(ISubscriberService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(SubscriberRoute.Route.GetStripeKey, Name = SubscriberRoute.Name.GetStripeKey)]
        public Entity.BaseResponse<string> GetStripeKey()
        {
            Entity.BaseResponse<string> response = new Entity.BaseResponse<string>(true);
            try
            {
                response.Data = component.helper.SolutionConfiguration.Configuration.SubscriptionAPI.StripeAPIKey;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<string>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(SubscriberRoute.Route.GetCountryLookup, Name = SubscriberRoute.Name.GetCountryLookup)]
        public Entity.BaseResponse<Response.CountryResponse> GetCountryLookUp()
        {
            Entity.BaseResponse<Response.CountryResponse> response = new Entity.BaseResponse<Response.CountryResponse>(true);
            try
            {
                response.Data = _service.GetCountryLookUp();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Response.CountryResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(SubscriberRoute.Route.GetStateLookup, Name = SubscriberRoute.Name.GetStateLookup)]
        public Entity.BaseResponse<Response.StateResponse> GetStateLookUp(string countryId)
        {
            BaseResponse<Response.StateResponse> response = new BaseResponse<Response.StateResponse>(true);
            try
            {
                response.Data = _service.GetStateLookUp(countryId);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new BaseResponse<Response.StateResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(SubscriberRoute.Route.GetTimeZoneLookup, Name = SubscriberRoute.Name.GetTimeZoneLookup)]
        public Entity.BaseResponse<Entity.Response.TimezoneResponse> GetTimezoneLookUp()
        {
            BaseResponse<Response.TimezoneResponse> response = new BaseResponse<Response.TimezoneResponse>(true);
            try
            {
                response.Data = _service.GetTimezoneLookUp();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new BaseResponse<Response.TimezoneResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(SubscriberRoute.Route.GetSubscriptionPlan, Name = SubscriberRoute.Name.GetSubscriptionPlan)]
        public BaseResponse<SubscriptionPlanResponse> GetSubscriptionPlans()
        {
            BaseResponse<Response.SubscriptionPlanResponse> response = new BaseResponse<Response.SubscriptionPlanResponse>(true);
            try
            {
                response.Data = _service.GetSubscriptionPlans(component.helper.SolutionConfiguration.Configuration.SubscriptionAPI.SolutionCode);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new BaseResponse<Response.SubscriptionPlanResponse>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(SubscriberRoute.Route.ValidateCompany, Name = SubscriberRoute.Name.ValidateCompany)]
        public BaseResponse<bool> ValidateCompany(ValidateCompanyRequest requestData)
        {
            var response = new BaseResponse<bool>(true);
            try
            {
                var apiResponse = _service.ValidateCompany(requestData);
                     response.IsSuccess = apiResponse.Success;
                    response.Message = apiResponse.Message;                
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new BaseResponse<bool>(false, ex.Message);
            }

            return response;
        }
        [HttpPost]
        [Route(SubscriberRoute.Route.SaveCompany, Name = SubscriberRoute.Name.SaveCompany)]
        public BaseResponse<Entity.SaveCompanyResponse> SaveCompany(SaveCompanyRequest requestData)
        {
            var response = new BaseResponse<Entity.SaveCompanyResponse>(true);
            try
            {
                var apiResponse = _service.SaveCompany(requestData);
                response.Data = apiResponse.Data;
                if (!apiResponse.Success)
                {
                    response.IsSuccess = false;
                    response.Message = apiResponse.Message;
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new BaseResponse<Entity.SaveCompanyResponse>(false, ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route(SubscriberRoute.Route.BySearch, Name = SubscriberRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.SubscriberData>>> GetBySearch([FromBody] Entity.ListRequest request)
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.SubscriberData>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.SubscriberData>>>(true);
            try
            {
                response.Data = _service.SubscriberList(component.helper.SolutionConfiguration.Configuration.SubscriptionAPI.SolutionCode, new Entity.SearchRequest()
                {
                    SearchText = request.searchText,
                    PageNumber = request.pageNo.Value,
                    PageSize = request.pageSize.Value,
                    OrderBy = request.orderBy,

                });
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.SubscriberData>>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(SubscriberRoute.Route.GetSubscriberDetails, Name = SubscriberRoute.Name.GetSubscriberDetails)]
        public BaseResponse<Entity.SubsciberCompanyDetails> GetSubscriberDetails(string userEmail)
        {
            BaseResponse<Entity.SubsciberCompanyDetails> response = new BaseResponse<Entity.SubsciberCompanyDetails>(true);
            try
            {
                response.Data = _service.GetSubscriberDetails(component.helper.SolutionConfiguration.Configuration.SubscriptionAPI.SolutionCode, userEmail);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new BaseResponse<Entity.SubsciberCompanyDetails>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(SubscriberRoute.Route.GetSubscriberKitDetails, Name = SubscriberRoute.Name.GetSubscriberKitDetails)]
        public BaseResponse<Entity.SearchResult<List<Entity.HardwareKitResponse>>> GetSubscriberKitDetails(string companyID="",string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "")
        {
            BaseResponse<Entity.SearchResult<List<Entity.HardwareKitResponse>>> response = new BaseResponse<Entity.SearchResult<List<Entity.HardwareKitResponse>>>(true);
            if(string.IsNullOrEmpty(companyID))
            companyID = SolutionConfiguration.CompanyId.ToString();

            try
            {
                response.Data = _service.GetSubscriberKitDetails(companyID, new Entity.SearchRequest()
                {
                    SearchText = searchText,
                    PageNumber = pageNo.Value,
                    PageSize = pageSize.Value,
                    OrderBy = orderBy,

                },true);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new BaseResponse<Entity.SearchResult<List<Entity.HardwareKitResponse>>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(SubscriberRoute.Route.GetLastSyncDetails, Name = SubscriberRoute.Name.GetLastSyncDetails)]
        public BaseResponse<Entity.LastSyncResponse> GetLastSyncDetails()
        {
            BaseResponse<Entity.LastSyncResponse> response = new BaseResponse<Entity.LastSyncResponse>(true);
            try
            {
                response.Data = _service.GetLastSyncDetails();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new BaseResponse<Entity.LastSyncResponse>(false, ex.Message);
            }
            return response;
        }

    }
}
