using System;
using System.Collections.Generic;
using System.Text;
using Model = iot.solution.model.Models;
using Entity = iot.solution.entity;

namespace iot.solution.model.Repository.Interface
{
    public interface INotificationsRepository : IGenericRepository<Model.AdminRule>
    {
        Entity.SearchResult<List<Entity.AlertResponse>> GetAlertList(Entity.AlertRequest request);
    }
}
