using model.iot.solution.Request;
using model.iot.solution.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace engine.iot.solution.Interface
{
    public interface IRoleEngine
    {
        Task<Guid> AddRoleAsync(AddRoleModel addRoleModel);
        Task<Guid> EditRoleAsync(Guid id, AddRoleModel addRoleModel);
        Task<RoleModel> GetRoleAsync(string roleName);
    }
}
