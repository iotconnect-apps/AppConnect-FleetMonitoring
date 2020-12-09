using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface IDeviceTypeService
    {
      
        Entity.DeviceType Get(Guid id);
        Entity.ActionStatus Manage(Entity.DeviceType request);
        Entity.ActionStatus Delete(Guid id);
        
        Entity.SearchResult<List<Entity.DeviceTypeDetail>> List(Entity.SearchRequest request);       
        Entity.BaseResponse<Entity.DashboardOverviewResponse> GetDeviceTypeDetail(Guid deviceTypeId);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);

    }
}
