using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface IEntityService
    {
        List<Entity.EntityWithCounts> Get();
        Entity.EntityDetail Get(Guid id);
        Entity.ActionStatus Manage(Entity.EntityModel request);
        Entity.ActionStatus Delete(Guid id);
        Entity.ActionStatus DeleteImage(Guid id);
        Entity.SearchResult<List<Entity.EntityListItem>> List(Entity.SearchRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);

    }
}
