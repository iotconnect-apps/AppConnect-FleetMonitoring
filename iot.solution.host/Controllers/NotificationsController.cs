using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;
using host.iot.solution.Controllers;
using component.helper;
using host.iot.solution.Filter;

namespace iot.solution.host.Controllers
{
    [Route(NotificationsRoute.Route.Global)]
    [ApiController]
    public class NotificationsController : BaseController
    {
        private readonly INotificationsService _service;
        private readonly IRuleService _ruleService;

        public NotificationsController(INotificationsService notificationsService, IRuleService ruleService)
        {
            _service = notificationsService;
            _ruleService = ruleService;
        }

        [HttpPost]
        [Route(NotificationsRoute.Route.Manage, Name = NotificationsRoute.Name.Add)]
        public Entity.BaseResponse<Entity.SingleRuleResponse> Manage(Entity.NotificationAddRequest request)
        {
            Entity.BaseResponse<Entity.SingleRuleResponse> response = new Entity.BaseResponse<Entity.SingleRuleResponse>(true);
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
                return new Entity.BaseResponse<Entity.SingleRuleResponse>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(NotificationsRoute.Route.Verify, Name = NotificationsRoute.Name.Verify)]
        public Entity.BaseResponse<Entity.VerifyRuleResult> Verify([FromBody]Entity.VerifyRuleRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.deviceTemplateGuid) || string.IsNullOrWhiteSpace(request.expression))
            {
                return new Entity.BaseResponse<Entity.VerifyRuleResult>(false, "Invalid Request");
            }

            Entity.BaseResponse<Entity.VerifyRuleResult> response = new Entity.BaseResponse<Entity.VerifyRuleResult>(true);
            try
            {
                var verifySatus = _ruleService.Verify(request);
                response.IsSuccess = verifySatus.Success;
                response.Message = verifySatus.Message;
                response.Data = verifySatus.Data;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.VerifyRuleResult>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(NotificationsRoute.Route.Delete, Name = NotificationsRoute.Name.Delete)]
        [EnsureGuidParameter("id", "Notification")]
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
        [Route(NotificationsRoute.Route.BySearch, Name = NotificationsRoute.Name.BySearch)]
        public Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>> GetBySearch([FromBody]string searchText = "")
        {
            Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>> response = new Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>>(true);
            try
            {
                response.Data = _service.List(new Entity.SearchRequest()
                {
                    SearchText = searchText
                });

               // var result = TemplateHelper.GetTemplateDetailsFromIoT("Default");

            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SearchResult<List<Entity.AllRuleResponse>>>(false, ex.Message);
            }
            return response;
        }
        [HttpPost]
        [Route(NotificationsRoute.Route.UpdateStatus, Name = NotificationsRoute.Name.UpdateStatus)]
        [EnsureGuidParameter("id", "Notification")]
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
        [HttpGet]
        [Route(NotificationsRoute.Route.GetById, Name = NotificationsRoute.Name.GetById)]
        [EnsureGuidParameter("id", "Notification")]
        public Entity.BaseResponse<Entity.SingleRuleResponse> Get(string id)
        {
            Entity.BaseResponse<Entity.SingleRuleResponse> response = new Entity.BaseResponse<Entity.SingleRuleResponse>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.SingleRuleResponse>(false, ex.Message);
            }
            return response;
        }
    }
}