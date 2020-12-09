using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace component.helper.Interface
{
    public interface ISubscriberHelper
    {
        bool ValidateSubscriptionAccessToken();
        void InitSubscriptionToken();
        Response.TimezoneResponse GetTimezoneData();
        Response.CountryResponse GetCountryData();
        Response.StateResponse GetStateData(string countryID);
        Response.SubscriptionPlanResponse GetSubscriptionPlans(string solutionID);
        Entity.BaseResponse<bool> ValidateCompany(Entity.ValidateCompanyRequest requestData);
        Entity.SearchResult<List<Entity.SubscriberData>> SubscriberList(string solutionID, Entity.SearchRequest request);
        Entity.SaveCompanyResponse CreateCompany(Entity.SaveCompanyRequest requestData);      
       Entity.SubsciberCompanyDetails GetSubscriberDetails(string solutionCode, string userEmail);
        Entity.LastSyncResponse GetLastSyncDetails();

    }
}
