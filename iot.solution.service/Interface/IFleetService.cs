using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface IFleetService
    {
       
        Entity.FleetDetail Get(Guid id);
        Entity.FleetDetail GetByDevice(Guid deviceId);
        Entity.ActionStatus Manage(Entity.FleetModel request);
        Entity.ActionStatus Delete(DateTime? currentDate, string timezone, Guid id);
        Entity.ActionStatus DeleteImage(Guid id);
        Entity.SearchResult<List<Entity.FleetListItem>> List(Entity.SearchRequest request);
        Entity.SearchResult<List<Entity.FleetMapListItem>> MapList(Entity.SearchRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
        Entity.ActionStatus DeletePermissionFile(Guid fleetId, Guid? fileId);
        Entity.BaseResponse<Entity.FleetDashboardOverviewResponse> GetFleetDetail(Guid fleetGuid, DateTime currentDate, string timeZone);


    }
}
