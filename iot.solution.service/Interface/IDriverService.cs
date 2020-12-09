using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface IDriverService
    {
        List<Entity.Driver> Get();
        Entity.Driver Get(Guid id);
        Entity.ActionStatus Manage(Entity.DriverModel request);
        Entity.ActionStatus Delete(Guid id);
        Entity.ActionStatus DeleteImage(Guid driverId);
        Entity.ActionStatus DeleteLicenceImage(Guid driverId);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
        Entity.SearchResult<List<Entity.DriverDetail>> List(Entity.SearchRequest request);
        Entity.BaseResponse<Entity.DriverDashboardOverviewResponse> GetDriverDetail(Guid driverGuid, DateTime currentDate, string timeZone);
    }
}
