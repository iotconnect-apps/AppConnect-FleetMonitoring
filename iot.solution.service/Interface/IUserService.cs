using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = iot.solution.entity;

namespace iot.solution.service.Interface
{
    public interface IUserService
    {
        List<Entity.User> Get();
        Entity.User Get(Guid id);
        Entity.ActionStatus Manage(Entity.AddUserRequest request);
        Entity.SearchResult<List<Entity.UserResponse>> List(Entity.SearchRequest request);
        Entity.ActionStatus Delete(Guid id);
        Entity.ActionStatus ChangePassword(Entity.ChangePasswordRequest request);
        Entity.ActionStatus Login(Entity.LoginRequest request);
        Entity.ActionStatus Identity(string token);
        Entity.ActionStatus IdentityLogin(string token);
        Entity.LoginResponse RefreshToken(Entity.RefreshTokenRequest request);
        Entity.ActionStatus UpdateStatus(Guid id, bool status);
    }
}