using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Request = iot.solution.entity.Request;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface IAdminRuleService
    {        
        Entity.SearchResult<List<Entity.AdminRule>> List(Entity.SearchRequest request);
        Entity.ActionStatus Manage(Entity.AdminRule adminRule);
        Entity.ActionStatus Delete(Guid id);
        Entity.AdminRule Get(Guid id);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);

    }
}
