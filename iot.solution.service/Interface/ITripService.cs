using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Interface
{
    public interface ITripService
    {
       
        Entity.TripDetail Get(Guid id);
        Entity.ActionStatus Manage(Entity.TripModel request);
        Entity.ActionStatus Delete(Guid id);
        public Entity.ActionStatus DeleteShipmentFile(Guid tripId, Guid? fileId);
        Entity.SearchResult<List<Entity.TripListItem>> List(Entity.SearchRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
        Entity.ActionStatus UpdateTripStatus(Entity.TripUpdateStatus request);
        Entity.ActionStatus StartTrip(Entity.StartTripModal request);
        Entity.ActionStatus SaveStops(string tripStopsXml, string tripId);
        Entity.BaseResponse<Entity.TripDashboardOverviewResponse> GetTripDetail(Guid tripGuid, DateTime currentDate, string timeZone);
        //Entity.ActionStatus DeletePermissionFile(Guid fleetId, Guid? fileId);

    }
}
