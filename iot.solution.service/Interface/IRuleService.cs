using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface IRuleService
    {
        Entity.SingleRuleResponse Get(Guid id);
        Entity.ActionStatus Manage(Entity.Rule request);
        Entity.ActionStatus Delete(Guid id);
        Entity.SearchResult<List<Entity.AllRuleResponse>> List(Entity.SearchRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
        Entity.ActionStatus Verify(Entity.VerifyRuleRequest request);
        Entity.ActionStatus ManageWebHook(string xml);
        Entity.SearchResult<List<Entity.AlertResponse>> AlertList(Entity.AlertRequest request);
    }
}
