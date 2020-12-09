using component.helper;
using component.logger;
using iot.solution.common;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using IoTConnect.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Implementation
{
    public class DeviceTypeService : IDeviceTypeService
    {
        private readonly IDeviceTypeRepository _deviceTypeRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILogger _logger;
       

        public DeviceTypeService(IDeviceTypeRepository productRepository, ILogger logger, IDeviceRepository deviceRepository, IDeviceService deviceService)
        {
            _logger = logger;
            _deviceTypeRepository = productRepository;
            _deviceRepository = deviceRepository;
        }
        
        public Entity.DeviceType Get(Guid id)
        {
            try
            {
                Entity.DeviceType response = _deviceTypeRepository.FindBy(r => r.Guid == id).Select(p => Mapper.Configuration.Mapper.Map<Entity.DeviceType>(p)).FirstOrDefault();
               
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "DeviceTypeService.Get " + ex);
                return null;
            }
        }
        public Entity.ActionStatus Manage(Entity.DeviceType request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    //var checkExisting = _deviceTypeRepository.FindBy(x => x.Name.Equals(request.Name) && x.CompanyGuid.Equals(SolutionConfiguration.CompanyId) && x.IsActive == true && !x.IsDeleted).FirstOrDefault();
                    //if (checkExisting == null)
                    //{

                        var dbDeviceType = Mapper.Configuration.Mapper.Map<Entity.DeviceType, Model.DeviceType>(request);


                        dbDeviceType.CompanyGuid = SolutionConfiguration.CompanyId;
                        dbDeviceType.CreatedDate = DateTime.Now;
                        dbDeviceType.CreatedBy = SolutionConfiguration.CurrentUserId;
                        dbDeviceType.IsActive = true;
                        actionStatus = _deviceTypeRepository.Manage(dbDeviceType);
                        dbDeviceType.Guid = actionStatus.Data!=null ? Guid.Parse(actionStatus.Data) :Guid.Empty;
                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.DeviceType, Entity.DeviceType>(dbDeviceType);
                        if (!actionStatus.Success)
                        {
                            _logger.Error($"DeviceType is not added in solution database, Error: {actionStatus.Message}");
                            actionStatus.Success = false;
                            actionStatus.Message = "DeviceType is not added in solution database";
                        }

                    //}
                    //else
                    //{
                    //    _logger.Error($"Device Type name Already Exist !!");
                    //    actionStatus.Success = false;
                    //    actionStatus.Message = "Device Type Name Already Exists";
                    //}
                }
                else
                {
                    var olddbEntity = _deviceTypeRepository.FindBy(x => x.Guid.Equals(request.Guid)).FirstOrDefault();
                    if (olddbEntity == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Location");
                    }
                
                    var dbDeviceType = Mapper.Configuration.Mapper.Map(request, olddbEntity);
                    dbDeviceType.Guid = request.Guid;
                    dbDeviceType.UpdatedDate = DateTime.Now;
                    dbDeviceType.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    dbDeviceType.CompanyGuid = SolutionConfiguration.CompanyId;
                    dbDeviceType.Description = request.Description;
                    dbDeviceType.Make = request.Make;
                    dbDeviceType.Model = request.Model;
                    dbDeviceType.Manufacturer = request.Manufacturer;
                    dbDeviceType.EntityGuid = request.EntityGuid;
                    dbDeviceType.TemplateGuid = request.TemplateGuid;
                    actionStatus = _deviceTypeRepository.Manage(dbDeviceType);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.DeviceType, Entity.DeviceType>(dbDeviceType);
                    if (!actionStatus.Success)
                    {
                        _logger.Error($"Device Type is not updated in solution database, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "DeviceTypeService.Manage " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        
        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbDeviceType = _deviceTypeRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbDeviceType == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : DeviceType");
                }
                var dbDevice = _deviceRepository.FindBy(x => x.TypeGuid.Equals(id) && !x.IsDeleted).FirstOrDefault();
                if (dbDevice == null)
                {
                    dbDeviceType.IsDeleted = true;
                    dbDeviceType.UpdatedDate = DateTime.Now;
                    dbDeviceType.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    return _deviceTypeRepository.Update(dbDeviceType);
                }
                else
                {
                    _logger.Error($"Device Type is not updated in solution database.Device exists, Error: {actionStatus.Message}");
                    actionStatus.Success = false;
                    actionStatus.Message = "Asset Type status cannot be deleted because device exists";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, "DeviceType.Delete " + ex);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var dbDeviceType = _deviceTypeRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();
                if (dbDeviceType == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : DeviceType");
                }
                try
                {
                    
                    var dbDevice = _deviceRepository.FindBy(x => x.TypeGuid.Equals(id) && !x.IsDeleted).FirstOrDefault();
                    if (dbDevice == null)
                    {
                        dbDeviceType.IsActive = status;
                        dbDeviceType.UpdatedDate = DateTime.Now;
                        dbDeviceType.UpdatedBy = SolutionConfiguration.CurrentUserId;
                        return _deviceTypeRepository.Update(dbDeviceType);
                    }
                    else
                    {
                        _logger.Error($"Device Type is not updated in solution database.Device exists, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Asset Type status cannot be updated because device exists";
                    }
                }
                catch (IoTConnectException ex)
                {
                    _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    actionStatus.Success = false;
                    actionStatus.Message = ex.Message;
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
        public Entity.SearchResult<List<Entity.DeviceTypeDetail>> List(Entity.SearchRequest request)
        {
            try
            {
                var result = _deviceTypeRepository.List(request);
                Entity.SearchResult<List<Entity.DeviceTypeDetail>> response = new Entity.SearchResult<List<Entity.DeviceTypeDetail>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.DeviceTypeDetail>(p)).ToList(),
                    Count = result.Count
                };
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, $"DeviceTypeDetailService.List, Error: {ex.Message}");
                return new Entity.SearchResult<List<Entity.DeviceTypeDetail>>();
            }
        }
      
        public Entity.BaseResponse<Entity.DashboardOverviewResponse> GetDeviceTypeDetail(Guid deviceTypeId)
        {
            Entity.BaseResponse<List<Entity.DashboardOverviewResponse>> listResult = new Entity.BaseResponse<List<Entity.DashboardOverviewResponse>>();
            Entity.BaseResponse<Entity.DashboardOverviewResponse> result = new Entity.BaseResponse<Entity.DashboardOverviewResponse>(true);
            try
            {
                listResult = _deviceTypeRepository.GetStatistics(deviceTypeId);               
                if (listResult.Data.Count > 0)
                {
                    result.IsSuccess = true;
                    result.Data = listResult.Data[0];
                    result.LastSyncDate = listResult.LastSyncDate;                   
                }
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

    }
}
