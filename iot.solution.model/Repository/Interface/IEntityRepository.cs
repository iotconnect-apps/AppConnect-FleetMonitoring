using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IEntityRepository : IGenericRepository<Model.Entity>
    {
        Entity.SearchResult<List<Entity.EntityListItem>> List(Entity.SearchRequest request);
        List<Entity.LookupItem> GetLookup(Guid companyId);
        List<Entity.LookupItem> GetZoneLookup(Guid companyId);
        ActionStatus Manage(Model.Entity request);
        
    }
}
