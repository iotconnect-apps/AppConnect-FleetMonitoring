using component.helper;
using component.logger;
using iot.solution.common;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using LogHandler = component.services.loghandler;
using System.Reflection;

namespace iot.solution.service.Implementation
{
    public class CompanyConfigService : ICompanyConfigService
    {
        private readonly ICompanyConfigRepository _companyConfigRepository;
        private readonly LogHandler.Logger _logger;

        public CompanyConfigService(ICompanyConfigRepository companyConfigRepository, LogHandler.Logger logManager)
        {
            _companyConfigRepository = companyConfigRepository;
            _logger = logManager;
        }

        public List<Entity.CompanyConfig> Get()
        {
            try
            {
                return _companyConfigRepository.GetAll().Where(e => !e.IsDeleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.CompanyConfig>(p)).ToList();
            }
            catch (Exception ex)
            {

                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        public Entity.CompanyConfig Get(Guid id)
        {
            try
            {
                return _companyConfigRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.CompanyConfig>(p)).FirstOrDefault();
            }
            catch (Exception ex)
            {

                _logger.ErrorLog( ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        public Entity.ActionStatus List(string searchContext)
        {
            throw new NotImplementedException();
        }
        public Entity.ActionStatus Manage(Entity.CompanyConfig companyConfig)
        {

            try
            {
                var dbCompanyConfig = Mapper.Configuration.Mapper.Map<Entity.CompanyConfig, Model.CompanyConfig>(companyConfig);
                Entity.ActionStatus actionStatus = null;
                if (dbCompanyConfig.Guid == Guid.Empty)
                {
                    dbCompanyConfig.CompanyGuid = SolutionConfiguration.CompanyId;
                    dbCompanyConfig.Guid = Guid.NewGuid();
                    dbCompanyConfig.CreatedDate = DateTime.Now;
                    dbCompanyConfig.CreatedBy = SolutionConfiguration.CurrentUserId;
                    actionStatus = _companyConfigRepository.Insert(dbCompanyConfig);
                }
                else
                {
                    var olddbCompanyConfig = _companyConfigRepository.GetByUniqueId(x => x.Guid == dbCompanyConfig.Guid);
                    if (olddbCompanyConfig == null)
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : CompanyConfig");

                    dbCompanyConfig.CreatedDate = olddbCompanyConfig.CreatedDate;
                    dbCompanyConfig.CreatedBy = olddbCompanyConfig.CreatedBy; //TODO: Need To change later with session userid
                    dbCompanyConfig.UpdatedDate = DateTime.Now;
                    dbCompanyConfig.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    actionStatus = _companyConfigRepository.Update(dbCompanyConfig);
                }
                return actionStatus;
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
        public Entity.ActionStatus Delete(Guid id)
        {
            try
            {
                //TODO: NEED TO IMPLEMENT RDK CALLS
                var dbCompanyConfig = _companyConfigRepository.GetByUniqueId(x => x.Guid == id);
                if (dbCompanyConfig == null)
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : CompanyConfig");
                return _companyConfigRepository.Delete(dbCompanyConfig);
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
    }
}
