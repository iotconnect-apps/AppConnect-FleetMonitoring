using component.helper;
using component.logger;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;
using LogHandler = component.services.loghandler;
namespace iot.solution.service.Implementation
{
    public class RuleService : IRuleService
    {
        private readonly IotConnectClient _iotConnectClient;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly LogHandler.Logger _logger;

        public RuleService(INotificationsRepository notificationsRepository, LogHandler.Logger logger)
        {
            _notificationsRepository = notificationsRepository;
            _logger = logger;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }

        public Entity.SearchResult<List<Entity.AlertResponse>> AlertList(Entity.AlertRequest request)
        {
            Entity.SearchResult<List<Entity.AlertResponse>> result = new Entity.SearchResult<List<Entity.AlertResponse>>();
            try
            {
                result = _notificationsRepository.GetAlertList(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return result;
            }
            return result;
        }
        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus status = new Entity.ActionStatus();
            try
            {
                var result = _iotConnectClient.Rule.Delete(id.ToString()).Result;
                if (result != null && !result.status)
                {
                    _logger.ErrorLog(new Exception($"Rule is not deleted from iotconnect"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    status.Success = false;
                    status.Message = "Something Went Wrong!";
                }
                else
                {
                    status.Success = true;
                    status.Message = "Rule is deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                status.Message = ex.Message;
                return status;
            }
            return status;
        }
        public Entity.SingleRuleResponse Get(Guid id)
        {
            Entity.SingleRuleResponse result = null;
            try
            {
                IOT.DataResponse<IOT.SingleRuleResult> rule = _iotConnectClient.Rule.Single(Convert.ToString(id)).Result;
                if (rule != null && rule.data != null)
                {
                    result = Mapper.Configuration.Mapper.Map<IoTConnect.Model.SingleRuleResult, Entity.SingleRuleResponse>(rule.data);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return result;
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.AllRuleResponse>> List(Entity.SearchRequest request)
        {
            Entity.SearchResult<List<Entity.AllRuleResponse>> result = new Entity.SearchResult<List<Entity.AllRuleResponse>>();
            try
            {
                var rules = _iotConnectClient.Rule.All(new IOT.PagingModel() { PageNo = request.PageNumber, PageSize = request.PageSize, SearchText = request.SearchText, SortBy = request.OrderBy }).Result;
                if (rules != null && rules.data != null && rules.data.Any())
                {
                    result = new Entity.SearchResult<List<Entity.AllRuleResponse>>()
                    {
                        Items = rules.data.Select(r => Mapper.Configuration.Mapper.Map<Entity.AllRuleResponse>(r)).ToList(),
                        Count = rules.data.Count
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return result;
            }
            return result;
        }
        public Entity.ActionStatus Manage(Entity.Rule request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                if (request.Guid == null || request.Guid == Guid.Empty)
                {
                    var addRuleResult = _iotConnectClient.Rule.Add(Mapper.Configuration.Mapper.Map<IOT.AddRuleModel>(request)).Result;

                    if (addRuleResult != null && !addRuleResult.status)
                    {
                        _logger.ErrorLog(new Exception($"Rule is not added in iotconnect, Error: {addRuleResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
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
                    var updateRuleResult = _iotConnectClient.Rule.Update(request.Guid.ToString(), Mapper.Configuration.Mapper.Map<IOT.UpdateRuleModel>(request)).Result;
                    if (updateRuleResult != null && !updateRuleResult.status)
                    {
                        _logger.ErrorLog(new Exception($"Rule is not added in iotconnect, Error: {updateRuleResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }
                    else
                    {
                        actionStatus.Data = Get(request.Guid.Value);
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
        public Entity.ActionStatus ManageWebHook(string xml)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("data", xml);
            int result = _notificationsRepository.ExecuteStoredProcedureNonQuery("[IOTConnectAlert_Add]", param);
            return new Entity.ActionStatus() { Success = true };
        }
        public Entity.ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                var updateRuleStatusResult = _iotConnectClient.Rule.UpdateRuleStatus(id.ToString(), status).Result;
                if (updateRuleStatusResult != null && !updateRuleStatusResult.status)
                {
                    _logger.ErrorLog(new Exception($"Rule status is not updated in iotconnect, Error: {updateRuleStatusResult.message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    actionStatus.Success = false;
                    actionStatus.Message = new UtilityHelper().IOTResultMessage(updateRuleStatusResult.errorMessages);
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
        public Entity.ActionStatus Verify(Entity.VerifyRuleRequest request)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            IOT.DataResponse<IOT.VerifyRuleResult> ruleVerifyResult = _iotConnectClient.Rule.RuleVerify(new IOT.VerifyRuleModel()
            {
                deviceTemplateGuid = request.deviceTemplateGuid,
                expression = request.expression
            }).Result;

            if (!ruleVerifyResult.data.isValid)
            {
                actionStatus.Success = false;
                actionStatus.Message = ruleVerifyResult.message;
            }
            else
            {
                actionStatus.Data = new Entity.VerifyRuleResult()
                {
                    isValid = ruleVerifyResult.data.isValid,
                    attributes = ruleVerifyResult.data.attributes,
                    tags = ruleVerifyResult.data.tags
                };
            }
            return actionStatus;
        }
    }
}
