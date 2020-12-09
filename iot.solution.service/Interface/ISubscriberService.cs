using IoTConnect.EntityProvider;
using System;
using System.Collections.Generic;
using System.Text;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;


namespace iot.solution.service.Interface
{
    public interface ISubscriberService
    {
        Response.CountryResponse GetCountryLookUp();
        Response.StateResponse GetStateLookUp(string countryID);
        Response.TimezoneResponse GetTimezoneLookUp();
        Response.SubscriptionPlanResponse GetSubscriptionPlans(string solutionID);
        Entity.SearchResult<List<Entity.SubscriberData>> SubscriberList(string solutionID, Entity.SearchRequest request);
        Entity.ActionStatus ValidateCompany(Entity.ValidateCompanyRequest requestData);
        Entity.ActionStatus SaveCompany(Entity.SaveCompanyRequest requestData);
        Entity.SubsciberCompanyDetails GetSubscriberDetails(string solutionCode, string userEmail);
        Entity.SearchResult<List<Entity.HardwareKitResponse>> GetSubscriberKitDetails(string companyID,Entity.SearchRequest request, bool isAssigned);
        Entity.LastSyncResponse GetLastSyncDetails();
    }
}
