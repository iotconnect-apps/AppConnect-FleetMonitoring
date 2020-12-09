using iot.solution.entity;
using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;


namespace iot.solution.model.Repository.Interface
{
    public interface IDriverRepository : IGenericRepository<Model.Driver>
    {
        Entity.SearchResult<List<Entity.DriverDetail>> List(Entity.SearchRequest request);
        ActionStatus Manage(Model.Driver request);
        Entity.BaseResponse<List<Entity.DriverDashboardOverviewResponse>> GetStatistics(Guid driverGuid, DateTime currentDate, string timeZone);
    }
}
