using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface IAdminUserService
    {
        Entity.ActionStatus AdminLogin(Entity.LoginRequest request);

        Entity.SearchResult<List<Entity.UserResponse>> GetAdminUserList(Entity.SearchRequest searchRequest);

        Entity.ActionStatus Manage(Entity.AddAdminUserRequest request);

        Entity.AdminUserResponse Get(Guid id);

        Entity.ActionStatus Delete(Guid id);

        Entity.ActionStatus UpdateStatus(Guid id,bool status);
        Entity.ActionStatus ChangePassword(Entity.ChangePasswordRequest request);

    }
}
