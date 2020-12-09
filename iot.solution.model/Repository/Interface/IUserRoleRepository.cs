using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IRoleRepository : IGenericRepository<Model.Role>
    {
        Entity.SearchResult<List<Model.Role>> List(Entity.SearchRequest request);
        ActionStatus Manage(Model.Role request);
    }
}