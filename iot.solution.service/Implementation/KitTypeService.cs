using component.logger;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
namespace iot.solution.service.Implementation
{
    public class KitTypeService : IKitTypeService
    {
        private readonly IKitTypeRepository _kitTypeRepository;
        private readonly LogHandler.Logger _logger;

        public KitTypeService(IKitTypeRepository kitTypeRepository, LogHandler.Logger logManager)
        {
            _kitTypeRepository = kitTypeRepository;
            _logger = logManager;
        }

        public List<Entity.KitType> GetAllKitTypes()
        {
            return _kitTypeRepository.GetAllKitTypes().Select(k => Mapper.Configuration.Mapper.Map<Entity.KitType>(k)).ToList();
        }

        public Entity.KitType GetAllKitTypeDetail(Guid templateId)
        {
            return Mapper.Configuration.Mapper.Map<Entity.KitType>(_kitTypeRepository.GetAllKitTypeDetail(templateId));
        }

        public List<Entity.KitTypeAttribute> GetKitTypeAttributes(Guid templateId)
        {
            return _kitTypeRepository.GetKitTypeAttributes(templateId).Select(k => Mapper.Configuration.Mapper.Map<Entity.KitTypeAttribute>(k)).ToList();
        }

        public List<Entity.KitTypeCommand> GetKitTypeCommands(Guid templateId)
        {
            return _kitTypeRepository.GetKitTypeCommands(templateId).Select(k => Mapper.Configuration.Mapper.Map<Entity.KitTypeCommand>(k)).ToList();
        }
    }
}
