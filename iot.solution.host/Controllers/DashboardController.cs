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
using Response = iot.solution.entity.Response;

namespace host.iot.solution.Controllers
{
    [Route(DashboardRoute.Route.Global)]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _service;
        private readonly IFleetService _fleetService;
        private readonly IDriverService _driverService;
        private readonly IDeviceService _deviceService;
        private readonly ITripService _tripService;
        public DashboardController(IDashboardService dashboardService, IFleetService fleetService, IDriverService driverService, IDeviceService deviceService, ITripService tripService)
        {
            _service = dashboardService;
            _fleetService = fleetService;
            _deviceService = deviceService;
            _driverService = driverService;
            _tripService = tripService;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetEntity, Name = DashboardRoute.Name.GetEntity)]
        [EnsureGuidParameter("companyId", "Company")]
        public Entity.BaseResponse<List<Entity.LookupItem>> GetEntities(string companyId)
        {
            Entity.BaseResponse<List<Entity.LookupItem>> response = new Entity.BaseResponse<List<Entity.LookupItem>>(true);
            try
            {
                response.Data = _service.GetEntityLookup(Guid.Parse(companyId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.LookupItem>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetOverview, Name = DashboardRoute.Name.GetOverview)]
        [EnsureGuidParameter("companyId", "Company")]
        public Entity.BaseResponse<Entity.DashboardOverviewResponse> GetOverview([FromQuery]OverviewCompanyRequest overviewRequest)
        {
            Entity.BaseResponse<Entity.DashboardOverviewResponse> response = new Entity.BaseResponse<Entity.DashboardOverviewResponse>(true);
            try
            {
                response = _service.GetOverview(overviewRequest.currentDate.Value, overviewRequest.timeZone);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.DashboardOverviewResponse>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(DashboardRoute.Route.GetOverviewByEntity, Name = DashboardRoute.Name.GetOverviewByEntity)]        
        public Entity.BaseResponse<List<Entity.DashboardOverviewByEntityResponse>> GetOverviewByEntity(DateTime currentDate, string timeZone)
        {
            Entity.BaseResponse<List<Entity.DashboardOverviewByEntityResponse>> response = new Entity.BaseResponse<List<Entity.DashboardOverviewByEntityResponse>>(true);
            try
            {
                response = _service.GetOverviewByEntity(currentDate, timeZone);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.DashboardOverviewByEntityResponse>>(false, ex.Message);
            }
            return response;
        }
       
        [HttpPost]
        [Route(DashboardRoute.Route.GetFleetDetail, Name = DashboardRoute.Name.GetFleetDetail)]
        [EnsureGuidParameter("fleetGuid", "Fleet")]
        public Entity.BaseResponse<Entity.FleetDashboardOverviewResponse> PostEntityDetail([FromBody] OverviewEntityRequest overviewRequest)
        {
            if (overviewRequest.fleetGuid == null || overviewRequest.fleetGuid == string.Empty)
            {
                return new Entity.BaseResponse<Entity.FleetDashboardOverviewResponse>(false, "Invalid Request");
            }

            Entity.BaseResponse<Entity.FleetDashboardOverviewResponse> response = new Entity.BaseResponse<Entity.FleetDashboardOverviewResponse>(true);
            try
            {
                response = _fleetService.GetFleetDetail(Guid.Parse(overviewRequest.fleetGuid), overviewRequest.currentDate.Value, overviewRequest.timeZone);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.FleetDashboardOverviewResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DashboardRoute.Route.GetTripDetail, Name = DashboardRoute.Name.GetTripDetail)]
        [EnsureGuidParameter("tripGuid", "Trip")]
        public Entity.BaseResponse<Entity.TripDashboardOverviewResponse> PostTripDetail([FromBody] OverviewTripRequest overviewRequest)
        {
            if (overviewRequest.tripGuid == null || overviewRequest.tripGuid == string.Empty)
            {
                return new Entity.BaseResponse<Entity.TripDashboardOverviewResponse>(false, "Invalid Request");
            }

            Entity.BaseResponse<Entity.TripDashboardOverviewResponse> response = new Entity.BaseResponse<Entity.TripDashboardOverviewResponse>(true);
            try
            {
                response = _tripService.GetTripDetail(Guid.Parse(overviewRequest.tripGuid), overviewRequest.currentDate.Value, overviewRequest.timeZone);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.TripDashboardOverviewResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DashboardRoute.Route.GetDriverDetail, Name = DashboardRoute.Name.GetDriverDetail)]
        [EnsureGuidParameter("driverGuid", "Driver")]
        public Entity.BaseResponse<Entity.DriverDashboardOverviewResponse> PostDriverDetail([FromBody] OverviewDriverRequest overviewRequest)
        {
            if (overviewRequest.driverGuid == null || overviewRequest.driverGuid == string.Empty)
            {
                return new Entity.BaseResponse<Entity.DriverDashboardOverviewResponse>(false, "Invalid Request");
            }

            Entity.BaseResponse<Entity.DriverDashboardOverviewResponse> response = new Entity.BaseResponse<Entity.DriverDashboardOverviewResponse>(true);
            try
            {
                response = _driverService.GetDriverDetail(Guid.Parse(overviewRequest.driverGuid), overviewRequest.currentDate.Value, overviewRequest.timeZone);
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.DriverDashboardOverviewResponse>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(DashboardRoute.Route.GetEntityDevices, Name = DashboardRoute.Name.GetEntityDevices)]
        [EnsureGuidParameter("entityId", "Location")]
        public Entity.BaseResponse<List<Response.EntityWiseDeviceResponse>> GetEntityDevices(string entityId)
        {
            if (entityId == null || entityId == string.Empty)
            {
                return new Entity.BaseResponse<List<Response.EntityWiseDeviceResponse>>(false, "Location id required!");
            }

            Entity.BaseResponse<List<Response.EntityWiseDeviceResponse>> response = new Entity.BaseResponse<List<Response.EntityWiseDeviceResponse>>(true);
            try
            {
                response.Data = _deviceService.GetEntityWiseDevices(Guid.Parse(entityId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.EntityWiseDeviceResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetEntityChildDevices, Name = DashboardRoute.Name.GetEntityChildDevices)]
        [EnsureGuidParameter("deviceId", "Asset")]
        public Entity.BaseResponse<List<Response.EntityWiseDeviceResponse>> GetEntityChildDevices(string deviceId)
        {
            if (deviceId == null || deviceId == string.Empty)
            {
                return new Entity.BaseResponse<List<Response.EntityWiseDeviceResponse>>(false, "Asset id required!");
            }

            Entity.BaseResponse<List<Response.EntityWiseDeviceResponse>> response = new Entity.BaseResponse<List<Response.EntityWiseDeviceResponse>>(true);
            try
            {
                response.Data = _deviceService.GetEntityChildDevices(Guid.Parse(deviceId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Response.EntityWiseDeviceResponse>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetDeviceDetail, Name = DashboardRoute.Name.GetDeviceDetail)]
        [EnsureGuidParameter("deviceId", "Device")]
        public Entity.BaseResponse<Response.DeviceDetailResponse> GetDeviceDetail(string deviceId)
        {
            if (deviceId == null || deviceId == string.Empty)
            {
                return new Entity.BaseResponse<Response.DeviceDetailResponse>(false, "Asset id required!");
            }

            Entity.BaseResponse<Response.DeviceDetailResponse> response = new Entity.BaseResponse<Response.DeviceDetailResponse>(true);
            try
            {
                response.Data = _deviceService.GetDeviceDetail(Guid.Parse(deviceId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Response.DeviceDetailResponse>(false, ex.Message);
            }
            return response;
        }
        #region Dynamic Dashboard API

        [HttpGet]
        [Route(DashboardRoute.Route.GetMasterWidget, Name = DashboardRoute.Name.GetMasterWidget)]
        public Entity.BaseResponse<List<Entity.MasterWidget>> GetMasterWidget()
        {
            Entity.BaseResponse<List<Entity.MasterWidget>> response = new Entity.BaseResponse<List<Entity.MasterWidget>>(true);
            try
            {
                response.Data = _service.GetMasterWidget();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.MasterWidget>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetMasterWidgetById, Name = DashboardRoute.Name.GetMasterWidgetById)]
        [EnsureGuidParameter("widgetId", "MasterWidget")]
        public Entity.BaseResponse<Entity.MasterWidget> GetMasterWidgetById(string widgetId)
        {
            Entity.BaseResponse<Entity.MasterWidget> response = new Entity.BaseResponse<Entity.MasterWidget>(true);
            try
            {
                response.Data = _service.GetMasterWidgetById(Guid.Parse(widgetId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.MasterWidget>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DashboardRoute.Route.Manage, Name = DashboardRoute.Name.Manage)]
        public Entity.BaseResponse<Entity.MasterWidget> Manage(Entity.MasterWidget request)
        {
            Entity.BaseResponse<Entity.MasterWidget> response = new Entity.BaseResponse<Entity.MasterWidget>(true);
            try
            {
                var result = _service.ManageMasterWidget(request);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.MasterWidget>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(DashboardRoute.Route.DeleteMasterWidget, Name = DashboardRoute.Name.DeleteMasterWidget)]
        [EnsureGuidParameter("id", "MasterWidget")]
        public Entity.BaseResponse<bool> DeleteMasterWidget(string id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.DeleteMasterWidget(Guid.Parse(id));
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

        [HttpGet]
        [Route(DashboardRoute.Route.GetUserWidget, Name = DashboardRoute.Name.GetUserWidget)]
        public Entity.BaseResponse<List<Entity.UserDasboardWidget>> GetUserWidget()
        {
            Entity.BaseResponse<List<Entity.UserDasboardWidget>> response = new Entity.BaseResponse<List<Entity.UserDasboardWidget>>(true);
            try
            {
                response.Data = _service.GetUserWidget();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.UserDasboardWidget>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DashboardRoute.Route.GetUserWidgetById, Name = DashboardRoute.Name.GetUserWidgetById)]
        [EnsureGuidParameter("widgetId", "UserWidget")]
        public Entity.BaseResponse<Entity.UserDasboardWidget> GetUserWidgetById(string widgetId)
        {
            Entity.BaseResponse<Entity.UserDasboardWidget> response = new Entity.BaseResponse<Entity.UserDasboardWidget>(true);
            try
            {
                response.Data = _service.GetUserWidgetById(Guid.Parse(widgetId));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.UserDasboardWidget>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DashboardRoute.Route.ManageUserWidget, Name = DashboardRoute.Name.ManageUserWidget)]
        public Entity.BaseResponse<Entity.UserDasboardWidget> ManageUserWidget(Entity.UserDasboardWidget request)
        {
            Entity.BaseResponse<Entity.UserDasboardWidget> response = new Entity.BaseResponse<Entity.UserDasboardWidget>(true);
            try
            {
                var result = _service.ManageUserWidget(request);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.UserDasboardWidget>(false, ex.Message);
            }
            return response;
        }
        [HttpPut]
        [Route(DashboardRoute.Route.DeleteUserWidget, Name = DashboardRoute.Name.DeleteUserWidget)]
        [EnsureGuidParameter("id", "UserWidget")]
        public Entity.BaseResponse<bool> DeleteUserWidget(string id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.DeleteUserWidget(Guid.Parse(id));
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
        #endregion
    }
}