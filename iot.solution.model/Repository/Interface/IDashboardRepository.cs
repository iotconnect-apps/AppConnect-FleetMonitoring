using System;
using System.Collections.Generic;
using System.Text;
using Model = iot.solution.model.Models;
using Entity = iot.solution.entity;

namespace iot.solution.model.Repository.Interface
{
    public interface IDashboardRepository:IGenericRepository<Model.Device>
    {
        public Entity.BaseResponse<List<Entity.DashboardOverviewResponse>> GetStatistics(DateTime currentDate, string timeZone);
        public Entity.BaseResponse<List<Entity.DashboardOverviewByEntityResponse>> GetStatisticsByEntity(DateTime currentDate, string timeZone);
    }
}
