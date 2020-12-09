using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IFleetRepository : IGenericRepository<Model.Fleet>
    {
        Entity.SearchResult<List<Entity.FleetMapListItem>> MapList(Entity.SearchRequest request);
        Entity.SearchResult<List<Entity.FleetListItem>> List(Entity.SearchRequest request);
        List<Entity.LookupItem> GetLookup(Guid companyId);
        List<Entity.LookupItem> GetTypeLookup();
        List<Entity.LookupItem> GetMaterialTypeLookup();
        ActionStatus Manage(Model.FleetModel request);
        ActionStatus ValidateDevice(Entity.FleetDeleteModel request);
        ActionStatus Delete(Entity.FleetDeleteModel request);
        Entity.BaseResponse<List<Entity.FleetDashboardOverviewResponse>> GetStatistics(Guid fleetGuid, DateTime currentDate, string timeZone);
    }
}
