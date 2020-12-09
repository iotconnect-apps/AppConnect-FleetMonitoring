using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;
using host.iot.solution.Filter;

namespace host.iot.solution.Controllers
{
    [Route(RoleRoute.Route.Global)]
    public class RoleController : BaseController
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService roleEngine)
        {
            _service = roleEngine;
        }

        [HttpGet]
        [Route(RoleRoute.Route.GetList, Name = RoleRoute.Name.GetList)]
        public Entity.BaseResponse<List<Entity.Role>> Get()
        {
            Entity.BaseResponse<List<Entity.Role>> response = new Entity.BaseResponse<List<Entity.Role>>(true);
            try
            {
                response.Data = _service.Get();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.Role>>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(RoleRoute.Route.GetByCompany, Name = RoleRoute.Name.GetByCompany)]
        public Entity.BaseResponse<List<Entity.Role>> GetByCompany()
        {
            Entity.BaseResponse<List<Entity.Role>> response = new Entity.BaseResponse<List<Entity.Role>>(true);
            try
            {
                response.Data = _service.Get().Where(t => t.CompanyGuid == component.helper.SolutionConfiguration.CompanyId && t.IsActive).ToList();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.Role>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(RoleRoute.Route.GetById, Name = RoleRoute.Name.GetById)]
        [EnsureGuidParameter("id", "Role")]
        public Entity.BaseResponse<Entity.Role> Get(string id)
        {
            Entity.BaseResponse<Entity.Role> response = new Entity.BaseResponse<Entity.Role>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.Role>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(RoleRoute.Route.Manage, Name = RoleRoute.Name.Add)]
        public Entity.BaseResponse<Entity.Role> Manage([FromBody]Entity.Role request)
        {
            Entity.BaseResponse<Entity.Role> response = new Entity.BaseResponse<Entity.Role>(true);
            try
            {
                var status = _service.Manage(request);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Data;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.Role>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(RoleRoute.Route.Delete, Name = RoleRoute.Name.Delete)]
        [EnsureGuidParameter("id", "Role")]
        public Entity.BaseResponse<bool> Delete(string id)
        {
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
        [Route(RoleRoute.Route.BySearch, Name = RoleRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.Role>>> GetBySearch([FromBody] Entity.ListRequest request)
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.Role>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.Role>>>(true);
            try
            {
                response.Data = _service.List(new Entity.SearchRequest()
                {
                    SearchText = request.searchText,
                    PageNumber = request.pageNo.Value,
                    PageSize = request.pageSize.Value,
                    OrderBy = request.orderBy
                });
                if (response.Data.Count < 0)
                {
                    response.Data.Count = 0;
                    response.IsSuccess = false;
                    response.Message = "User not authorized!";
                }
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.Role>>>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(RoleRoute.Route.UpdateStatus, Name = RoleRoute.Name.UpdateStatus)]
        [EnsureGuidParameter("id", "Role")]
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
