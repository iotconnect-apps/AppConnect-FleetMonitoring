using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface IRoleService
    {
        List<Entity.Role> Get();

        Entity.Role Get(Guid id);

        Entity.ActionStatus Manage(Entity.Role role);

        Entity.ActionStatus Delete(Guid id);
        Entity.SearchResult<List<Entity.Role>> List(Entity.SearchRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
    }
}