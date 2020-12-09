using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IAdminUserRepository : IGenericRepository<Model.AdminUser>
    {
        Model.AdminUser Get(string userName);
        Entity.SearchResult<List<Entity.UserResponse>> List(Entity.SearchRequest request);

        Model.AdminUser AdminLogin(Entity.LoginRequest request);
    }
}