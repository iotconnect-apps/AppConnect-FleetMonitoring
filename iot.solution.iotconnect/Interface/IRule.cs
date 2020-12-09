using IoTConnect.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTConnect.RuleProvider
{
   internal interface IRule
    {


        Task<DataResponse<List<AllRuleResult>>> All(PagingModel pagingModel);
        Task<DataResponse<AddRuleResult>> Add(AddRuleModel model);
        Task<DataResponse<SingleRuleResult>> Single(string ruleGuid);
        Task<DataResponse<UpdateRuleResult>> Update(string ruleGuid, UpdateRuleModel model);
        Task<DataResponse<DeleteRuleResult>> Delete(string ruleGuid);
        Task<DataResponse<VerifyRuleResult>> RuleVerify(VerifyRuleModel request);
        Task<DataResponse<UpdateRuleStatusResult>> UpdateRuleStatus(string ruleGuid, bool Status);
        Task<DataResponse<RuleCountResult>> RuleCount();
        Task<DataResponse<List<EventResult>>> Event(EventModel eventModel);
        Task<DataResponse<List<EventDeliveryMethod>>> EventDeliveryMethod(string eventId);
        Task<DataResponse<List<SeverityLevelLookupResult>>> SeverityLevelLookup();

    }
}
