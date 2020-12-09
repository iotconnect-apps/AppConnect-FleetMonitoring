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
    [Route(EntityRoute.Route.Global)]
    [ApiController]
    public class EntityController : BaseController
    {
        private readonly IEntityService _service;
        private readonly IDeviceService _deviceService;
        private readonly ILookupService _lookupService;

        public EntityController(IEntityService entityService, IDeviceService deviceService, ILookupService lookupService)
        {
            _service = entityService;
            _deviceService = deviceService;
            _lookupService = lookupService;
        }

        [HttpGet]
        [Route(EntityRoute.Route.GetList, Name = EntityRoute.Name.GetList)]
        public Entity.BaseResponse<List<Entity.EntityWithCounts>> Get()
        {
            Entity.BaseResponse<List<Entity.EntityWithCounts>> response = new Entity.BaseResponse<List<Entity.EntityWithCounts>>(true);
            try
            {
                response.Data = _service.Get();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.EntityWithCounts>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(EntityRoute.Route.GetById, Name = EntityRoute.Name.GetById)]
        [EnsureGuidParameter("id", "Location")]
        public Entity.BaseResponse<Entity.EntityDetail> Get(string id)
        {
            if (id == null || id == string.Empty)
            {
                return new Entity.BaseResponse<Entity.EntityDetail>(false, "Location id required!");
            }

            Entity.BaseResponse<Entity.EntityDetail> response = new Entity.BaseResponse<Entity.EntityDetail>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
                response.Data.Devices = _deviceService.GetEntityWiseDevices(Guid.Parse(id));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.EntityDetail>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(EntityRoute.Route.Manage, Name = EntityRoute.Name.Add)]
        public Entity.BaseResponse<Entity.Entity> Manage([FromForm]Entity.EntityModel request)
        {

            Entity.BaseResponse<Entity.Entity> response = new Entity.BaseResponse<Entity.Entity>(false);
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
                return new Entity.BaseResponse<Entity.Entity>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(EntityRoute.Route.Delete, Name = EntityRoute.Name.Delete)]
        [EnsureGuidParameter("id", "Location")]
        public Entity.BaseResponse<bool> Delete(string id)
        {
            if (id == null || id == string.Empty)
            {
                return new Entity.BaseResponse<bool>(false, "Location id required!");
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

        [HttpPut]
        [Route(EntityRoute.Route.DeleteImage, Name = EntityRoute.Name.DeleteImage)]
        [EnsureGuidParameter("id", "Location")]
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
        [HttpPost]
        [Route(EntityRoute.Route.BySearch, Name = EntityRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityListItem>>> GetBySearch([FromBody]Entity.EntityListRequest request)
        {
            //string parentEntityGuid = "", string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "", DateTime? currentDate = null, string timeZone = ""
            Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityListItem>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityListItem>>>(true);
            try
            {
                //DateTime? myDate = currentDate;
                //if (!currentDate.HasValue && timestamp > 0)
                //{
                //     myDate = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(timestamp.ToString()));// new DateTime(1595940704153);
                //   // DateTime mycurrentDate = new DateTime(myDate.ToString("yyyy-MM-dd HH:mm:ss"));
                //}
                response.Data = _service.List(new Entity.SearchRequest()
                {
                    EntityId = string.IsNullOrEmpty(request.parentEntityGuid) ? Guid.Empty : new Guid(request.parentEntityGuid),
                    SearchText = request.searchText,
                    PageNumber = -1,//pageNo.Value,
                    PageSize = -1,//pageSize.Value,
                    OrderBy = request.orderBy,
                    CurrentDate = request.currentDate,
                    TimeZone= request.timeZone
                });
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityListItem>>>(false, ex.Message);
            }
            return response;
        }
        //[HttpGet]       
        //[Route(EntityRoute.Route.BySearch2, Name = EntityRoute.Name.BySearch2)]
        //public Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityListItem>>> GetBySearch(string parentEntityGuid = "", string searchText = "", int? pageNo = 1, int? pageSize = 10, string orderBy = "", string timeZone = "", long timestamp = 0)
        //{
        //    Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityListItem>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityListItem>>>(true);
        //    try
        //    {
        //        DateTime? myDate =null;
        //        if (timestamp > 0)
        //        {
        //            //myDate = new DateTime(timestamp);
        //            // myDate = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
        //            myDate = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(timestamp.ToString()));// new DateTime(1595940704153);
        //        }
        //        response.Data = _service.List(new Entity.SearchRequest()
        //        {
        //            EntityId = string.IsNullOrEmpty(parentEntityGuid) ? Guid.Empty : new Guid(parentEntityGuid),
        //            SearchText = searchText,
        //            PageNumber = -1,//pageNo.Value,
        //            PageSize = -1,//pageSize.Value,
        //            OrderBy = orderBy,
        //            CurrentDate = myDate,
        //            TimeZone = timeZone
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        base.LogException(ex);
        //        return new Entity.BaseResponse<Entity.SearchResult<List<Entity.EntityListItem>>>(false, ex.Message);
        //    }
        //    return response;
        //}
        [HttpPost]
        [Route(EntityRoute.Route.UpdateStatus, Name = EntityRoute.Name.UpdateStatus)]
        [EnsureGuidParameter("id", "Location")]
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