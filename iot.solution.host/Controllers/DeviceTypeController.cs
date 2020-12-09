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
    [Route(DeviceTypeRoute.Route.Global)]
    [ApiController]
    public class DeviceTypeController : BaseController
    {
        private readonly IDeviceTypeService _service;
       
        public DeviceTypeController(IDeviceTypeService deviceTypeService, IDeviceService deviceService, ILookupService lookupService)
        {
            _service = deviceTypeService;           
        }
        [HttpGet]
        [Route(DeviceTypeRoute.Route.GetById, Name = DeviceTypeRoute.Name.GetById)]
        [EnsureGuidParameterAttribute("id", "DeviceType")]
        public Entity.BaseResponse<Entity.DeviceType> Get(string id)
        {
            if (id == null || id == string.Empty)
            {
                return new Entity.BaseResponse<Entity.DeviceType>(false, "Device Type id required!");
            }

            Entity.BaseResponse<Entity.DeviceType> response = new Entity.BaseResponse<Entity.DeviceType>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
               
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.DeviceType>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DeviceTypeRoute.Route.Manage, Name = DeviceTypeRoute.Name.Add)]
        public Entity.BaseResponse<Entity.DeviceType> Manage([FromBody]Entity.DeviceType request)
        {

            Entity.BaseResponse<Entity.DeviceType> response = new Entity.BaseResponse<Entity.DeviceType>(false);
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
                return new Entity.BaseResponse<Entity.DeviceType>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(DeviceTypeRoute.Route.Delete, Name = DeviceTypeRoute.Name.Delete)]
        [EnsureGuidParameterAttribute("id", "DeviceType")]
        public Entity.BaseResponse<bool> Delete(string id)
        {
            if (id == null || id == string.Empty)
            {
                return new Entity.BaseResponse<bool>(false, "Device Type id required!");
            }

            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.Delete(Guid.Parse(id));
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

        
        [HttpPost]
        [Route(DeviceTypeRoute.Route.BySearch, Name = DeviceTypeRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceTypeDetail>>> GetBySearch([FromBody] Entity.ListRequest request)
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceTypeDetail>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceTypeDetail>>>(true);
            try
            {
                response.Data = _service.List(new Entity.SearchRequest()
                {
                    SearchText = request.searchText,
                    PageNumber = request.pageNo.Value,
                    PageSize = request.pageSize.Value,
                    OrderBy = request.orderBy
                });
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.DeviceTypeDetail>>>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(DeviceTypeRoute.Route.UpdateStatus, Name = DeviceTypeRoute.Name.Status)]
        [EnsureGuidParameter("id", "DeviceType")]
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