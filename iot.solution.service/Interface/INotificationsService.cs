using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.service.Interface
{
    public interface INotificationsService
    {
        Entity.SearchResult<List<Entity.AllRuleResponse>> List(Entity.SearchRequest request);
        Entity.ActionStatus Delete(Guid id);
        Entity.ActionStatus Manage(Entity.NotificationAddRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
        Entity.SingleRuleResponse Get(Guid id);

    }
}