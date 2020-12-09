using component.helper;
using component.logger;
using iot.solution.common;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
namespace iot.solution.service.Implementation
{
    public class HardwareKitService : IHardwareKitService
    {
        private readonly IHardwareKitRepository _hardwareKitRepository;
        private readonly IKitTypeRepository _kitTypeRepository;
        private readonly LogHandler.Logger _logger;
        private readonly IKitTypeAttributeRepository _kitTypeAttributeRepository;
        public HardwareKitService(IHardwareKitRepository hardwareKitRepository, IKitTypeAttributeRepository kitTypeAttributeRepository,IKitTypeRepository kitTypeRepository, LogHandler.Logger logManager)
        {
            _hardwareKitRepository = hardwareKitRepository;
            _kitTypeAttributeRepository = kitTypeAttributeRepository;
            _kitTypeRepository = kitTypeRepository;
            _logger = logManager;
        }
        public Entity.SearchResult<List<Entity.HardwareKitResponse>> List(Entity.SearchRequest request, bool isAssigned)
        {
            try
            {
                var result = _hardwareKitRepository.List(request, isAssigned, null);
                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.SearchResult<List<Entity.HardwareKitResponse>>();
            }
        }
        public Entity.HardwareKitDTO Get(Guid id)
        {       
            return _hardwareKitRepository.FindBy(r => r.Guid == id).Select(p => new Entity.HardwareKitDTO()
            {
                Guid = p.Guid,
                KitTypeGuid = p.KitTypeGuid,
                CompanyGuid = p.CompanyGuid,
                KitGuid = p.Guid,
                Name = p.Name,
                IsProvisioned = p.IsProvisioned,
                Note = p.Note,
                Tag = p.TagGuid,
                KitCode = p.KitCode,
                UniqueId = p.UniqueId
            }).FirstOrDefault();
        }
        public Entity.ActionStatus Manage(Entity.KitVerifyRequest hardwareKit, bool isEdit = false)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                //foreach (var kit in hardwareKit.HardwareKits)
                //{
                //    if (string.IsNullOrWhiteSpace(kit.Tag)) continue;
                //    var tagGuid = Guid.Parse(kit.Tag);
                //    var kitAttribute = _kitTypeAttributeRepository.FindBy(t => t.Guid.Equals(tagGuid)).FirstOrDefault();
                //    if (kitAttribute != null)
                //    {
                //        kit.Tag = kitAttribute.Tag;
                //        kit.AttributeName = kitAttribute.LocalName;
                //    }
                //}

                var verifyResult = _hardwareKitRepository.VerifyHardwareKit(hardwareKit, isEdit);

                if (verifyResult.Success)
                {
                    actionStatus = _hardwareKitRepository.SaveHardwareKit(hardwareKit, isEdit);
                }
                else
                {
                    List<Entity.BulkUploadResponse> errorResult = verifyResult.Data;
                    //if (errorResult != null)
                    //{
                    //    foreach (var error in errorResult)
                    //    {
                    //        if (string.IsNullOrWhiteSpace(error.tag)) continue;

                    //        var tag = error.tag;
                    //        var kitAttribute = _kitTypeAttributeRepository.FindBy(t => t.Tag.Equals(error.tag) && t.LocalName.Equals(error.attributename)).FirstOrDefault();
                    //        if (kitAttribute != null)
                    //        {
                    //            error.tag = kitAttribute.Guid.ToString();
                    //        }
                    //    }
                    //}

                    actionStatus.Data = errorResult;// verifyResult.Data;
                    actionStatus.Success = verifyResult.Success;
                    actionStatus.Message = verifyResult.Message;// "H/W Kit ID / Unique ID already exists";
                }
              
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.ActionStatus Delete(Guid id)
        {
            try
            {
                var dbHardwareKit = _hardwareKitRepository.GetByUniqueId(x => x.Guid == id);
                if (dbHardwareKit == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : HardwareKit");
                }
                dbHardwareKit.IsDeleted = true;
                dbHardwareKit.UpdatedDate = DateTime.Now;
                dbHardwareKit.UpdatedBy = SolutionConfiguration.CurrentUserId;
                return _hardwareKitRepository.Update(dbHardwareKit);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public Entity.ActionStatus UploadKit(Entity.KitVerifyRequest request)
        {

            var actionStatus = new Entity.ActionStatus();
            try
            {
                var verifyResult = _hardwareKitRepository.VerifyHardwareKit(request, false);

                if (verifyResult.Success)
                {
                    actionStatus = _hardwareKitRepository.SaveHardwareKit(request, false);
                }
                else
                {
                    actionStatus.Data = verifyResult.Data;
                    actionStatus.Success = verifyResult.Success;
                    actionStatus.Message = verifyResult.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return actionStatus;
        }
        public Entity.ActionStatus VerifyKit(Entity.KitVerifyRequest request, bool isEdit = false)
        {
            var result = new Entity.ActionStatus();
            try
            {
                result = _hardwareKitRepository.VerifyHardwareKit(request, isEdit);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return result;
        }

    }
}



