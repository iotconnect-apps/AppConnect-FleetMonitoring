using engine.iot.solution.Interface;
using IOTConnect.Common.Auth;
using IOTConnect.Common.Model.Login;
using Microsoft.Extensions.Configuration;
using model.iot.solution.Request;
using model.iot.solution.Response;
using System;
using System.Threading.Tasks;

namespace engine.iot.solution.Data
{
    public class UserEngine : IUserEngine
    {
        IConfiguration _configuration;
        public UserEngine(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Guid> AddUserAsync(AddUserModel addUserModel)
        {
            return Guid.NewGuid();
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordModel changePasswordModel)
        {
            return true;
        }

        public async Task<Guid> EditUserAsync(Guid id, AddUserModel addUserModel)
        {
            return Guid.NewGuid();
        }

        public async Task<UserModel> GetUserAsync(string userName)
        {
            return new UserModel();
        }

        public async Task<bool> ValidateUserAsync(LoginUserModel loginUserModel)
        {
            string envCode = _configuration.GetSection("RDK:EnvCode").Value;
            string solutionKey = _configuration.GetSection("RDK:SolutionKey").Value;
            var obj = new Login(envCode, solutionKey)
                                            .LoginAccess(new LoginModel() { UserName = loginUserModel.Username, Password = loginUserModel.Password }).Result;

            return obj != null;

        }
    }
}
