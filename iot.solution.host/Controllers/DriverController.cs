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
    [Route(DriverRoute.Route.Global)]
    [ApiController]
    public class DriverController : BaseController
    {
        private readonly IDriverService _service;
      

        public DriverController(IDriverService driverService)
        {
            _service = driverService;
        }

        [HttpGet]
        [Route(DriverRoute.Route.GetList, Name = DriverRoute.Name.GetList)]
        public Entity.BaseResponse<List<Entity.Driver>> Get()
        {
            Entity.BaseResponse<List<Entity.Driver>> response = new Entity.BaseResponse<List<Entity.Driver>>(true);
            try
            {
                response.Data = _service.Get();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.Driver>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(DriverRoute.Route.GetById, Name = DriverRoute.Name.GetById)]
        [EnsureGuidParameter("id", "Driver")]
        public Entity.BaseResponse<Entity.Driver> Get(string id)
        {
            if (id == null || id == string.Empty)
            {
                return new Entity.BaseResponse<Entity.Driver>(false, "Location id required!");
            }

            Entity.BaseResponse<Entity.Driver> response = new Entity.BaseResponse<Entity.Driver>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.Driver>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(DriverRoute.Route.Manage, Name = DriverRoute.Name.Manage)]
        public Entity.BaseResponse<Entity.Driver> Manage([FromForm]Entity.DriverModel request)
        {

            Entity.BaseResponse<Entity.Driver> response = new Entity.BaseResponse<Entity.Driver>(false);
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
                return new Entity.BaseResponse<Entity.Driver>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(DriverRoute.Route.Delete, Name = DriverRoute.Name.Delete)]
        [EnsureGuidParameter("id", "Driver")]
        public Entity.BaseResponse<bool> Delete(string id)
        {
            if (id == null || id == string.Empty)
            {
                return new Entity.BaseResponse<bool>(false, "Driver id required!");
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
        [Route(DriverRoute.Route.BySearch, Name = DriverRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.DriverDetail>>> GetBySearch([FromBody] Entity.DriverListRequest request)
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.DriverDetail>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.DriverDetail>>>(true);
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
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.DriverDetail>>>(false, ex.Message);
            }
            return response;
        }

        //Driver image
        //Licence image
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [HttpPut]
        [Route(DriverRoute.Route.DeleteImage, Name = DriverRoute.Route.DeleteImage)]
        [EnsureGuidParameterAttribute("driverId", "Driver")]
        public Entity.BaseResponse<bool> DeleteImage(string driverId)
        {
            if (driverId == null || driverId == string.Empty)
            {
                return new Entity.BaseResponse<bool>(false, "Driver id required!");
            }
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.DeleteImage(Guid.Parse(driverId));
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

        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [HttpPut]
        [Route(DriverRoute.Route.DeleteLicenceImage, Name = DriverRoute.Route.DeleteLicenceImage)]
        [EnsureGuidParameterAttribute("driverId", "Driver")]
        public Entity.BaseResponse<bool> DeleteLicenceImage(string driverId)
        {
            if (driverId == null || driverId == string.Empty)
            {
                return new Entity.BaseResponse<bool>(false, "Driver id required!");
            }
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.DeleteLicenceImage(Guid.Parse(driverId));
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

        [HttpPut]
        [Route(DriverRoute.Route.UpdateStatus, Name = DriverRoute.Route.UpdateStatus)]
        [EnsureGuidParameter("id", "Trip")]
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