using host.iot.solution.Filter;
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
    [Route(FleetRoute.Route.Global)]
    [ApiController]
    public class FleetController : BaseController
    {
        private readonly IFleetService _service;
        private readonly IDeviceService _deviceService;
        private readonly ILookupService _lookupService;

        public FleetController(IFleetService entityService, IDeviceService deviceService, ILookupService lookupService)
        {
            _service = entityService;
            _deviceService = deviceService;
            _lookupService = lookupService;
        }


        [HttpGet]
        [Route(FleetRoute.Route.GetById, Name = FleetRoute.Name.GetById)]
        [EnsureGuidParameter("id", "Fleet")]
        public Entity.BaseResponse<Entity.FleetDetail> Get(string id)
        {
            if (id == null || id == string.Empty)
            {
                return new Entity.BaseResponse<Entity.FleetDetail>(false, "Fleet id required!");
            }

            Entity.BaseResponse<Entity.FleetDetail> response = new Entity.BaseResponse<Entity.FleetDetail>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
             
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.FleetDetail>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(FleetRoute.Route.Manage, Name = FleetRoute.Name.Add)]
        public Entity.BaseResponse<Entity.Fleet> Manage([FromForm] Entity.FleetModel request)
        {
            Entity.BaseResponse<Entity.Fleet> response = new Entity.BaseResponse<Entity.Fleet>(false);
            try
            {
                var status = _service.Manage(request);
                if (status.Success)
                {
                    response.IsSuccess = status.Success;
                    response.Message = status.Message;
                    response.Data = status.Data;
                }
                else
                {
                    response.IsSuccess = status.Success;
                    response.Message = status.Message;
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.Fleet>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(FleetRoute.Route.Delete, Name = FleetRoute.Name.Delete)]
        
        public Entity.BaseResponse<bool> Delete([FromBody] Entity.FleetDeleteModel request)
        {
            if (request.guid== null || request.guid == Guid.Empty)
            {
                return new Entity.BaseResponse<bool>(false, "Fleet id required!");
            }

            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.Delete(request.currentDate,request.timeZone,request.guid);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(FleetRoute.Route.DeleteImage, Name = FleetRoute.Name.DeleteImage)]
        [EnsureGuidParameter("id", "Fleet")]
        public Entity.BaseResponse<bool> DeleteImage(string id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.DeleteImage(Guid.Parse(id));
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [HttpPut]
        [Route(FleetRoute.Route.DeletePermissionFile, Name = FleetRoute.Name.DeletePermissionFile)]
        [EnsureGuidParameterAttribute("fleetId", "Fleet")]
        public Entity.BaseResponse<bool> DeletePermissionFile(string fleetId, Guid? fileId)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.DeletePermissionFile(Guid.Parse(fleetId), fileId);
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
        [Route(FleetRoute.Route.BySearch, Name = FleetRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.FleetListItem>>> GetBySearch([FromBody]Entity.FleetListRequest request)
        {
            //string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "", DateTime? currentDate = null, string timeZone = ""
            Entity.BaseResponse<Entity.SearchResult<List<Entity.FleetListItem>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.FleetListItem>>>(true);
            try
            {

                response.Data = _service.List(new Entity.SearchRequest()
                {                    
                    SearchText = request.searchText,
                    PageNumber = request.pageNo.Value,
                    PageSize = request.pageSize.Value,
                    OrderBy = request.orderBy,
                    CurrentDate = request.currentDate,
                    TimeZone = request.timeZone
                });
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.FleetListItem>>>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(FleetRoute.Route.ByMapSearch, Name = FleetRoute.Name.ByMapSearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.FleetMapListItem>>> GetByMapSearch([FromBody] Entity.FleetListRequest request)
        {
            //string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "", DateTime? currentDate = null, string timeZone = ""
            Entity.BaseResponse<Entity.SearchResult<List<Entity.FleetMapListItem>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.FleetMapListItem>>>(true);
            try
            {

                response.Data = _service.MapList(new Entity.SearchRequest()
                {
                    SearchText = request.searchText,
                    PageNumber = request.pageNo.Value,
                    PageSize = request.pageSize.Value,
                    OrderBy = request.orderBy,
                    CurrentDate = request.currentDate,
                    TimeZone = request.timeZone
                });
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.FleetMapListItem>>>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(FleetRoute.Route.UpdateStatus, Name = FleetRoute.Name.UpdateStatus)]
        [EnsureGuidParameter("id", "Fleet")]
        public Entity.BaseResponse<bool> UpdateStatus(string id, bool status)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                Entity.ActionStatus result = _service.UpdateStatus(Guid.Parse(id), status);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }
    }
}