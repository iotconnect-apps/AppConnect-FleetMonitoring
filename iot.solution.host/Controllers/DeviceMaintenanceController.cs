using host.iot.solution.Filter;
using iot.solution.entity;
using iot.solution.entity.Structs.Routes;

using iot.solution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;

namespace host.iot.solution.Controllers
{
    [Route(DeviceMaintenanceRoute.Route.Global)]
    [ApiController]
    public class DeviceMaintenanceController : BaseController
    {

        private readonly IDeviceMaintenanceService _maintenanceService;
        public DeviceMaintenanceController(IDeviceMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpGet]
        [Route(DeviceMaintenanceRoute.Route.GetList, Name = DeviceMaintenanceRoute.Name.GetList)]
        public Entity.BaseResponse<List<Entity.DeviceMaintenance>> Get()
        {
            Entity.BaseResponse<List<Entity.DeviceMaintenance>> response = new Entity.BaseResponse<List<Entity.DeviceMaintenance>>(true);
            try
            {
                response.Data = _maintenanceService.Get();
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.DeviceMaintenance>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DeviceMaintenanceRoute.Route.GetById, Name = DeviceMaintenanceRoute.Name.GetById)]
        [EnsureGuidParameter("id", "Device Maintenance")]
        public Entity.BaseResponse<Entity.DeviceMaintenance> Get(string id, DateTime currentDate, string timeZone)
        {
            if (id == null || Guid.Parse(id) == Guid.Empty)
            {
                return new Entity.BaseResponse<Entity.DeviceMaintenance>(false, "Maintenance id required!");
            }

            Entity.BaseResponse<Entity.DeviceMaintenance> response = new Entity.BaseResponse<Entity.DeviceMaintenance>(true);
            try
            {

                response.Data = _maintenanceService.Get(Guid.Parse(id),currentDate,timeZone);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.DeviceMaintenance>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DeviceMaintenanceRoute.Route.Manage, Name = DeviceMaintenanceRoute.Name.Add)]
        public Entity.BaseResponse<Entity.DeviceMaintenance> Manage([FromBody]Entity.DeviceMaintenance request)
        {

            Entity.BaseResponse<Entity.DeviceMaintenance> response = new Entity.BaseResponse<Entity.DeviceMaintenance>(true);
            try
            {
                var status = _maintenanceService.Manage(request);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Data;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.DeviceMaintenance>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(DeviceMaintenanceRoute.Route.Delete, Name = DeviceMaintenanceRoute.Name.Delete)]
        [EnsureGuidParameter("id", "Device Maintenance")]
        public Entity.BaseResponse<bool> Delete(string id)
        {
            if (id == null || Guid.Parse(id) == Guid.Empty)
            {
                return new Entity.BaseResponse<bool>(false, "Maintenance is required!");
            }

            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _maintenanceService.Delete(Guid.Parse(id));
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DeviceMaintenanceRoute.Route.BySearch, Name = DeviceMaintenanceRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceMaintenanceDetail>>> GetBySearch([FromBody]Entity.DeviceMaintenanceListRequest request)
        {
            //string deviceId = "", string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "", DateTime? currentDate = null, string timeZone = ""
            Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceMaintenanceDetail>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceMaintenanceDetail>>>(true);
            try
            {
                if (!request.currentDate.HasValue)
                {
                    return new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceMaintenanceDetail>>>(false, "Current Date is required");
                }
                else if (string.IsNullOrEmpty(request.timeZone))
                {
                    return new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceMaintenanceDetail>>>(false, "Time Zone is required"); 
                }
                else
                {
                    response.Data = _maintenanceService.List(new Entity.SearchRequest()
                    {
                       // EntityId = string.IsNullOrEmpty(entityGuid) ? Guid.Empty : new Guid(entityGuid),
                        Guid = string.IsNullOrEmpty(request.deviceId) ? null : request.deviceId,
                        SearchText = request.searchText,
                        PageNumber = request.pageNo.Value,
                        PageSize = request.pageSize.Value,
                        OrderBy = request.orderBy,
                        CurrentDate = request.currentDate,
                        TimeZone = request.timeZone
                    });
                }
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceMaintenanceDetail>>>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(DeviceMaintenanceRoute.Route.Complete, Name = DeviceMaintenanceRoute.Name.Complete)]
        [EnsureGuidParameter("guid", "Maintenance")]
        public Entity.BaseResponse<bool> UpdateStatusCompleted(Entity.CompleteMaintenanceRequest request)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                if (request.guid == null || request.guid == string.Empty)
                {
                    return new Entity.BaseResponse<bool>(false, "Maintenance Guid is required");
                }                
                else
                {
                    Entity.ActionStatus result = _maintenanceService.CompleteMaintenance(request);
                    response.IsSuccess = result.Success;
                    response.Message = result.Message;
                    response.Data = result.Success;
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(DeviceMaintenanceRoute.Route.UpdateStatus, Name = DeviceMaintenanceRoute.Name.UpdateStatus)]
        [EnsureGuidParameter("id", "Device Maintenance")]
        public Entity.BaseResponse<bool> UpdateStatus(string id, string status)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {

                Entity.ActionStatus result = _maintenanceService.UpdateStatus(Guid.Parse(id), status);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Success;

            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DeviceMaintenanceRoute.Route.UpComingList, Name = DeviceMaintenanceRoute.Name.UpComingList)]
        public Entity.BaseResponse<List<Entity.DeviceMaintenanceResponse>> UpcomingList(DeviceMaintenanceRequest request)
        {
            Entity.BaseResponse<List<Entity.DeviceMaintenanceResponse>> response = new Entity.BaseResponse<List<Entity.DeviceMaintenanceResponse>>(true);
            try
            {
                response.Data = _maintenanceService.GetUpComingList(request);
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<List<Entity.DeviceMaintenanceResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DeviceMaintenanceRoute.Route.GetScheduledMaintenenceDate, Name = DeviceMaintenanceRoute.Name.GetScheduledMaintenenceDate)]
        public Entity.BaseResponse<Entity.DeviceSceduledMaintenanceResponse> GetDeviceScheduledMaintenence(DeviceMaintenanceRequest request)
        {
            Entity.BaseResponse<Entity.DeviceSceduledMaintenanceResponse> response = new Entity.BaseResponse<Entity.DeviceSceduledMaintenanceResponse>(true);
            try
            {
                if (request.DeviceGuid.HasValue || request.EntityGuid.HasValue)
                {
                    response = _maintenanceService.GetDeviceScheduledMaintenance(request);
                }
               
                else
                {
                    response.Message = "DeviceGuid or EntityGuid is missing!";
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                return new Entity.BaseResponse<Entity.DeviceSceduledMaintenanceResponse>(false, ex.Message);
            }
            return response;
        }

    }
}