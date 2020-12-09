using IoTConnect.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iot.solution.iotconnect.Interface
{
    public interface IRole
    {
        /// <summary>
        /// Add new Role.
        /// </summary>
        /// <param name="model">Add role model</param>
        /// <returns></returns>
        Task<DataResponse<AddRoleResult>> AddRole(AddRoleModel model);

        /// <summary>
        /// Add new Role.
        /// </summary>
        /// <param name="model">Add role model</param>
        /// <returns></returns>
        Task<DataResponse<UpdateRoleResult>> UpdateRole(string roleGuid, UpdateRoleModel model);

        /// <summary>
        /// Get Role by Role Id.
        /// </summary>
        /// <param name="roleGuid">Role Id.</param>
        /// <returns></returns>
        Task<DataResponse<SingleRoleResult>> SingleRole(string roleGuid);

        /// <summary>
        /// Get all Roles.
        /// </summary>
        /// <returns></returns>
        Task<DataResponse<List<AllRoleLookupResult>>> AllRoleLookup();

        /// <summary>
        /// Update Role Status.
        /// </summary>
        /// <param name="userGuid">User Id.</param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<DataResponse<UpdateRoleStatusResult>> UpdateRoleStatus(string roleGuid, bool status);

        /// <summary>
        /// Get user roles.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<DataResponse<List<AllRoleResult>>> AllRole(AllRoleModel request);
        /// <summary>
        /// Delete role.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<DataResponse<DeleteRoleResult>> DeleteRole(string roleGuid);
    }
}
