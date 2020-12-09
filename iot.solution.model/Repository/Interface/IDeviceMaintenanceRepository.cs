using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IDeviceMaintenanceRepository : IGenericRepository<Model.DeviceMaintenance>
    {
        Entity.SearchResult<List<Entity.DeviceMaintenanceDetail>> List(Entity.SearchRequest request);
        Entity.ActionStatus Manage(Model.DeviceMaintenance request);
        List<Entity.DeviceMaintenanceResponse> GetUpComingList(Entity.DeviceMaintenanceRequest request);
        
        Entity.BaseResponse<Entity.DeviceSceduledMaintenanceResponse> GetDeviceScheduledMaintenance(Entity.DeviceMaintenanceRequest request);

        //List<Entity.DeviceMaintenance> Get();
        Entity.DeviceMaintenance Get(Guid id,DateTime currentDate,string timeZone);
    }
}
