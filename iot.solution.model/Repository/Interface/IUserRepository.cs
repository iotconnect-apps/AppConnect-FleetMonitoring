using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IUserRepository : IGenericRepository<Model.User>
    {
        Model.User Get(string userName);
        Entity.ActionStatus GetSubscriptionStatus(string userId);
        Entity.SearchResult<List<Entity.UserResponse>> List(Entity.SearchRequest request);
    }
}