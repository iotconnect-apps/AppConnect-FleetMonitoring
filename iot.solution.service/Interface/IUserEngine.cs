using model.iot.solution.Request;
using model.iot.solution.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace engine.iot.solution.Interface
{
    public interface IUserEngine
    {
        Task<Guid> AddUserAsync(AddUserModel addUserModel);
        Task<Guid> EditUserAsync(Guid id, AddUserModel addUserModel);
        Task<UserModel> GetUserAsync(string userName);
        Task<bool> ValidateUserAsync(LoginUserModel loginUserModel);
        Task<bool> ChangePasswordAsync(ChangePasswordModel changePasswordModel);

    }
}
