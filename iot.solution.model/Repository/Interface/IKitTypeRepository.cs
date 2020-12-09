using System;
using System.Collections.Generic;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;

namespace iot.solution.model.Repository.Interface
{
    public interface IKitTypeRepository : IGenericRepository<Model.KitType>
    {
        List<Model.KitType> GetAllKitTypes();
        Model.KitType GetAllKitTypeDetail(Guid templateId);
        List<Model.KitTypeAttribute> GetKitTypeAttributes(Guid templateId);
        List<Model.KitTypeCommand> GetKitTypeCommands(Guid templateId);
    }
}
