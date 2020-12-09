using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;


namespace iot.solution.model.Repository.Interface
{
    public interface ITripRepository : IGenericRepository<Model.Trip>
    {
        Entity.SearchResult<List<Entity.TripListItem>> List(Entity.SearchRequest request);
        ActionStatus Manage(Model.TripModel request);
        Entity.ActionStatus Delete(Guid tripGuid);
        Entity.ActionStatus UploadFiles(string xmlString, string tripId);
        Entity.ActionStatus DeleteShipmentFile(Guid tripId, Guid? fileId);
        List<Entity.DeviceMediaFiles> GetShipmentFiles(Guid tripId);
        Entity.ActionStatus AddTripStops(string xmlString, string tripId);
        List<Entity.TripStops> GetTripStops(Guid tripId);
        Entity.ActionStatus StartTrip(Entity.StartTripModal request);
        bool GetTripStatus(Guid tripId);
        Entity.BaseResponse<List<Entity.TripDashboardOverviewResponse>> GetStatistics(Guid tripGuid, DateTime currentDate, string timeZone);

    }
}
