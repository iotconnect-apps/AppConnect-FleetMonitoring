using Entity = iot.solution.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.service.Interface
{
    public interface IHardwareKitService
    {
        Entity.SearchResult<List<Entity.HardwareKitResponse>> List(Entity.SearchRequest request, bool isAssigned);
        Entity.HardwareKitDTO Get(Guid id);
        Entity.ActionStatus Manage(Entity.KitVerifyRequest hardwareKit,bool isEdit);
        Entity.ActionStatus Delete(Guid id);        
        Entity.ActionStatus UploadKit(Entity.KitVerifyRequest request);
        Entity.ActionStatus VerifyKit(Entity.KitVerifyRequest request, bool isEdit = false);
    }
}
