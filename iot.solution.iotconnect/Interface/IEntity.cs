using IoTConnect.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTConnect.EntityProvider
{
    internal interface IEntity
    {
        /// <summary>
        /// Create new entity.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<DataResponse<AddEntityResult>> Add(AddEntityModel request);

        /// <summary>
        /// Update entity.
        /// </summary>
        /// <param name="entityGuid">The entity unique identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<DataResponse<UpdateEntityResult>> Update(string entityGuid, UpdateEntityModel request);

        /// <summary>
        /// Delete entity.
        /// </summary>
        /// <param name="entityGuid">The entity unique identifier.</param>
        /// <returns></returns>
        Task<DataResponse<DeleteEntityResult>> Delete(string entityGuid);

        /// <summary>
        /// Get entity list.
        /// </summary>
        /// <returns></returns>
        Task<DataResponse<List<AllEntityResult>>> All();

        /// <summary>
        /// Get entity detail.
        /// </summary>
        /// <param name="entityGuid">The entity unique identifier.</param>
        /// <returns></returns>
        Task<DataResponse<SingleEntityResult>> Single(string entityGuid);
    }
}
