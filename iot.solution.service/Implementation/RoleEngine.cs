using engine.iot.solution.Interface;
using model.iot.solution.Request;
using model.iot.solution.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace engine.iot.solution.Data
{
    public class RoleEngine : IRoleEngine
    {
        public RoleEngine()
        {
        }
        public async Task<Guid> AddRoleAsync(AddRoleModel addRoleModel)
        {
            return Guid.NewGuid();
        }

        public async Task<Guid> EditRoleAsync(Guid id, AddRoleModel addRoleModel)
        {
            return Guid.NewGuid();
        }

        public async Task<RoleModel> GetRoleAsync(string roleName)
        {
            return new RoleModel();
        }
    }
}
