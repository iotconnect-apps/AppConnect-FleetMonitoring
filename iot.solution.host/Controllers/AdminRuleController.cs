using host.iot.solution.Filter;
using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;

namespace host.iot.solution.Controllers
{
    [Route(AdminRuleRoute.Route.Global)]
    [ApiController]
    public class AdminRuleController : BaseController
    {
        private readonly IAdminRuleService _service;

        public AdminRuleController(IAdminRuleService adminRuleService)
        {
            _service = adminRuleService;
        }
        [HttpPost]
        [Route(AdminRuleRoute.Route.BySearch, Name = AdminRuleRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.AdminRule>>> GetBySearch([FromBody]string searchText = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.AdminRule>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.AdminRule>>>(true);
            try
            {
                response.Data = _service.List(new Entity.SearchRequest()
                {
                    SearchText = searchText
                });
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.AdminRule>>>(false, ex.Message);
            }
            return response;
        }
        [HttpGet]
        [Route(AdminRuleRoute.Route.GetById, Name = AdminRuleRoute.Name.GetById)]
        [EnsureGuidParameter("id", "Admin Rule")]
        public Entity.BaseResponse<Entity.AdminRule> Get(string id)
        {
            Entity.BaseResponse<Entity.AdminRule> response = new Entity.BaseResponse<Entity.AdminRule>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.AdminRule>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(AdminRuleRoute.Route.Manage, Name = AdminRuleRoute.Name.Add)]
        public Entity.BaseResponse<Entity.AdminRule> Manage([FromBody]Entity.AdminRule request)
        {
            Entity.BaseResponse<Entity.AdminRule> response = new Entity.BaseResponse<Entity.AdminRule>(true);
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
                return new Entity.BaseResponse<Entity.AdminRule>(false, ex.Message);
            }
            return response;
        }
        [HttpPut]
        [Route(AdminRuleRoute.Route.Delete, Name = AdminRuleRoute.Name.Delete)]
        [EnsureGuidParameter("id", "Admin Rule")]
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
        [Route(AdminRuleRoute.Route.UpdateStatus, Name = AdminRuleRoute.Name.UpdateStatus)]
        [EnsureGuidParameter("id", "Admin Rule")]
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
