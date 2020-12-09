using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.model.Repository.Interface
{
    public interface IAdminRuleRepository : IGenericRepository<Model.AdminRule>
    {
        Entity.SearchResult<List<Entity.AdminRule>> List(Entity.SearchRequest request);        
    }
}
