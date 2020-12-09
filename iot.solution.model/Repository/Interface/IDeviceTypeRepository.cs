using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IDeviceTypeRepository : IGenericRepository<Model.DeviceType>
    {
        Entity.SearchResult<List<Entity.DeviceTypeDetail>> List(Entity.SearchRequest request);
        List<Entity.LookupItem> GetLookup(Guid companyId);
       
        ActionStatus Manage(Model.DeviceType request);
        Entity.BaseResponse<List<Entity.DashboardOverviewResponse>> GetStatistics(Guid entityId);
    }
}
