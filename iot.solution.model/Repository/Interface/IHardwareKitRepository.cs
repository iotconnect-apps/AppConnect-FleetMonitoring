using iot.solution.entity;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IHardwareKitRepository : IGenericRepository<Model.HardwareKit>
    {
        Entity.SearchResult<List<Entity.HardwareKitResponse>> List(Entity.SearchRequest request, bool isAssigned, string companyId);
        List<LookupItem> GetKitTypeLookup();
        ActionStatus VerifyHardwareKit(KitVerifyRequest request, bool isEdit);
        Entity.ActionStatus SaveHardwareKit(Entity.KitVerifyRequest requestData,bool isEdit);
        public ActionStatus UploadHardwareKit(List<Entity.HardwareKitDTO> requestData);
        

    }
}
