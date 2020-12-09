using component.helper;
using component.logger;
using iot.solution.common;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;
using LogHandler = component.services.loghandler;
namespace iot.solution.service.Data
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository _inotificationsrepository;
        private readonly IotConnectClient _iotConnectClient;
        private readonly IKitTypeService _kitTypeService;
        private readonly LogHandler.Logger _logger;

        public NotificationsService(INotificationsRepository notificationsRepository, IKitTypeService kitTypeService, LogHandler.Logger logger)
        {
            _logger = logger;
            _inotificationsrepository = notificationsRepository;
            _kitTypeService = kitTypeService;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }

        public Entity.SearchResult<List<Entity.AllRuleResponse>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.AllRuleResponse>> result = new SearchResult<List<AllRuleResponse>>();
            try
            {
                IOT.DataResponse<List<IOT.AllRuleResult>> rules = _iotConnectClient.Rule.All(new IoTConnect.Model.PagingModel() { PageNo = 1, PageSize = 1000 }).Result;
                if (rules.status && rules.data != null)
                {
                    result = new Entity.SearchResult<List<Entity.AllRuleResponse>>()
                    {
                        Items = rules.data.Select(p => Mapper.Configuration.Mapper.Map<Entity.AllRuleResponse>(p)).ToList(),
                        Count = rules.data.Count
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.SearchResult<List<Entity.AllRuleResponse>>();
            }
            return result;
        }

        public Entity.ActionStatus Manage(Entity.NotificationAddRequest request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            var _list = new List<string>();
            try
            {
                if (request.Guid == null || request.Guid == Guid.Empty)
                {                 

                    var addRuleResult = _iotConnectClient.Rule.Add(Mapper.Configuration.Mapper.Map<IOT.AddRuleModel>(request)).Result;

                    if (addRuleResult != null && !addRuleResult.status)
                    {
                        _logger.ErrorLog(new Exception($"Notification is not added in iotconnect, Error: {addRuleResult.message}")
                            , this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = new UtilityHelper().IOTResultMessage(addRuleResult.errorMessages);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(addRuleResult.data.ruleGuid))
                        {
                            actionStatus.Data = Get(Guid.Parse(addRuleResult.data.ruleGuid.ToUpper()));
                        }
                    }
                }
                else
                {
                    IOT.DataResponse<IOT.SingleRuleResult> singleRule = _iotConnectClient.Rule.Single(request.Guid.ToString()).Result;
                    if (singleRule.data == null)
                    {
                        _logger.ErrorLog(new Exception($"Notification is not added in iotconnect, Error: {singleRule.message}")
                            , this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = singleRule.errorMessages[0].Message;
                    }
                    else
                    {
                        request.IsActive = singleRule.data.isActive;
                        var updateRuleResult = _iotConnectClient.Rule.Update(request.Guid.ToString(), Mapper.Configuration.Mapper.Map<IOT.UpdateRuleModel>(request)).Result;
                        if (updateRuleResult != null && !updateRuleResult.status)
                        {
                            _logger.ErrorLog(new Exception($"Notification is not updated in iotconnect, Error: {updateRuleResult.message}")
                                , this.GetType().Name, MethodBase.GetCurrentMethod().Name);

                            actionStatus.Success = false;
                            actionStatus.Message = new UtilityHelper().IOTResultMessage(updateRuleResult.errorMessages);
                        }
                        else
                        {
                            actionStatus.Data = Get(request.Guid.Value);
                        }
                    }
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
        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                IOT.DataResponse<IOT.SingleRuleResult> singleRule = _iotConnectClient.Rule.Single(id.ToString()).Result;
                if (singleRule.data != null && singleRule.status)
                {
                    var deleteRule = _iotConnectClient.Rule.Delete(id.ToString()).Result;
                    if (deleteRule.status)
                    {
                        actionStatus.Success = deleteRule.status;
                        actionStatus.Message = deleteRule.message;
                    }
                    else
                        throw new NotFoundCustomException(deleteRule.errorMessages[0].Message);
                }
                else
                    throw new NotFoundCustomException(singleRule.errorMessages[0].Message);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return actionStatus;
        }
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                IOT.DataResponse<IOT.SingleRuleResult> singleRule = _iotConnectClient.Rule.Single(id.ToString()).Result;
                if (singleRule == null || singleRule.data == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : Notification");
                }
                else
                {
                    var iotRuleDetail = _iotConnectClient.Rule.UpdateRuleStatus(id.ToString(), status).Result;
                    actionStatus.Success = iotRuleDetail.status;
                    actionStatus.Message = iotRuleDetail.message;
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
        public Entity.SingleRuleResponse Get(Guid id)
        {
            Entity.SingleRuleResponse result = null;
            try
            {
                IOT.DataResponse<IOT.SingleRuleResult> response = _iotConnectClient.Rule.Single(Convert.ToString(id)).Result;
                if (response != null && response.data != null)
                {
                    result = Mapper.Configuration.Mapper.Map<IoTConnect.Model.SingleRuleResult, Entity.SingleRuleResponse>(response.data);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return result;
            }
            return result;
        }

    }
}