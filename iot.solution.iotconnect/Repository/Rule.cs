using Flurl.Http;
using IoTConnect.Common;
using IoTConnect.Common.Constant;
using IoTConnect.Common.Interface;
using IoTConnect.Common.Repository;
using IoTConnect.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IoTConnect.RuleProvider
{
    /// <summary>
    /// Rule Template.
    /// </summary>
    public class Rule : IRule
    {
        #region Private properties
        private string _token = string.Empty;
        private IIoTConnectAPIDiscovery _ioTConnectAPIDiscovery;
        private string _envCode = string.Empty;
        private string _solutionKey = string.Empty;
        #endregion

        #region Ctor
        public Rule(string token, string environmentCode, string solutionKey)
        {
            _token = token;
            _envCode = environmentCode;
            _solutionKey = solutionKey;
            _ioTConnectAPIDiscovery = new IoTConnectAPIDiscovery();
            FlurlHttp.Configure(settings => settings.OnError = HandleFlurlErrorAsync);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles the flurl error asynchronous.
        /// </summary>
        /// <param name="call">The call.</param>
        /// <returns></returns>
        private void HandleFlurlErrorAsync(HttpCall call)
        {
            call.ExceptionHandled = true;
            IoTConnectException ioTConnectErrorResponse = JsonConvert.DeserializeObject<IoTConnectException>(call.Response.Content.ReadAsStringAsync().Result);
            throw ioTConnectErrorResponse;
        }
        #endregion

        #region Rule Crud 

        /// <summary>
        /// This API endpoint gives you the list of available rules in your device template.
        /// You can also add filters as given in the parameters.Though the filters are optional,
        /// to apply them, you need to send these filter parameters in a query string.
        /// </summary>
        /// <param name="pagingModel"></param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllRuleResult>>> All(PagingModel pagingModel)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, RuleApi.AllRules);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var result = await formattedUrl.WithOAuthBearerToken(_token)
                                         .SetQueryParams(new
                                         {
                                             pageSize = pagingModel.PageSize,
                                             pageNumber = pagingModel.PageNo,
                                             searchtext = pagingModel.SearchText,
                                             sortBy = pagingModel.SortBy
                                         })
                                         .GetJsonAsync<DataResponse<List<AllRuleResult>>>();

                return new DataResponse<List<AllRuleResult>>(null)
                {
                    data = result.data,
                    message = result.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllRuleResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Rule", "All()");
                throw ex;
            }
        }

        /// <summary>
        /// Rules enable you to monitor your devices in near real-time and automatically invoke actions, such as send emails, show UI notifications, etc.
        /// whenever the rule is matched. In a rule, you can define the condition for which you wish to monitor your device and configure the corresponding actions.
        /// Rule triggers when the selected device telemetry crosses a specified threshold mentioned in the condition.
        /// This API endpoint allows you to create rules.Here name, templateGuid, severityLevelGuid, 
        /// ruleType and applyTo are mandatory for any template rule.conditionText needs a condition on attribute values.If the condition is true,
        /// it is considered as the rule matched.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DataResponse<AddRuleResult>> Add(AddRuleModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);

                var errorList = Helper.ValidateObject(model);
                if (model?.deliveryMethod == null)
                {
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "DeliveryMethod is required",
                        Param = "deliveryMethod"
                    };
                    errorList.Add(errorItemModel);
                }
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddRuleResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }

                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, RuleApi.RuleAdd);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PostJsonAsync(model).ReceiveJson<DataResponse<List<AddRuleResult>>>();

                return new DataResponse<AddRuleResult>(null)
                {
                    data = result.data.FirstOrDefault(),
                    message = result.message,
                    status = true,
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddRuleResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Rule", "Add()");
                throw ex;
            }
        }

        /// <summary>
        /// This API endpoint gives you all the available details of a single rule. For that, you need to send ruleGuid in request url.
        /// </summary>
        /// <param name="ruleGuid"></param>
        /// <returns></returns>
        public async Task<DataResponse<SingleRuleResult>> Single(string ruleGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(ruleGuid))
                {
                    return new DataResponse<SingleRuleResult>(null)
                    {
                        message = "RuleGuid is required",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, RuleApi.RuleDetail);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, ruleGuid);
                var ruleDetail = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                .GetJsonAsync<DataResponse<List<SingleRuleResult>>>();

                return new DataResponse<SingleRuleResult>(null)
                {
                    data = ruleDetail.data.FirstOrDefault(),
                    message = ruleDetail.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<SingleRuleResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Rule", "Single()");
                throw ex;
            }
        }

        /// <summary>
        /// Once rules are created, anytime, you may need to change the severity level of a rule, threshold values, or where the rule should apply.
        /// You can make such changes in template rule using this API endpoint. For that,
        /// you need to send ruleGuid in request url and rule update details in the request body.
        /// </summary>
        /// <param name="ruleGuid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateRuleResult>> Update(string ruleGuid, UpdateRuleModel model)
        {
            try
            {
                var errorList = Helper.ValidateObject(model);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<UpdateRuleResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, RuleApi.RuleUpdate);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, ruleGuid);
                var updateUser = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(model).ReceiveJson<DataResponse<List<UpdateRuleResult>>>();

                return new DataResponse<UpdateRuleResult>(null)
                {
                    data = updateUser.data.FirstOrDefault(),
                    message = updateUser.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateRuleResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Rule", "Update()");
                throw ex;
            }
        }

        /// <summary>
        /// This API endpoint allows you to delete a rule. For that, you need to send ruleGuid in request url.
        /// </summary>
        /// <param name="ruleGuid"></param>
        /// <returns></returns>
        public async Task<DataResponse<DeleteRuleResult>> Delete(string ruleGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(ruleGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "RuleGuid is required",
                        Param = "RuleGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<DeleteRuleResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                DeleteRuleResult deleteTemplate = new DeleteRuleResult();
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, RuleApi.RuleDelete);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, ruleGuid);
                var ruleDelete = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SendJsonAsync(HttpMethod.Delete, deleteTemplate).ReceiveJson<DataResponse<List<DeleteRuleResult>>>();

                return new DataResponse<DeleteRuleResult>(null)
                {
                    data = ruleDelete.data.FirstOrDefault(),
                    message = ruleDelete.message,
                    status = ruleDelete.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<DeleteRuleResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };

            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Rule", "Delete()");
                throw ex;
            }
        }

        /// <summary>
        /// Once the rule is created or updated, using this API endpoint, you can check whether the rule condition on attributes is valid or not.
        /// If not, you will not be able to create or update a rule.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DataResponse<VerifyRuleResult>> RuleVerify(VerifyRuleModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<VerifyRuleResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, RuleApi.RuleVerify);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var verifyRule = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<VerifyRuleResult>>();

                return new DataResponse<VerifyRuleResult>(null)
                {
                    data = verifyRule.data,
                    message = verifyRule.message,
                    status = verifyRule.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<VerifyRuleResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Rule", "RuleVerify()");
                throw ex;
            }
        }

        /// <summary>
        /// This API endpoint allows you to update your rule status. For that, you need to send ruleGuid in request url and updated status in the request body.
        /// Send true to activate and false to deactivate a rule.
        /// </summary>
        /// <param name="ruleGuid"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateRuleStatusResult>> UpdateRuleStatus(string ruleGuid, bool Status)
        {
            try
            {
                if (string.IsNullOrEmpty(ruleGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "RuleGuid is required",
                        Param = "RuleGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<UpdateRuleStatusResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                UpdateRoleStatusModel request = new UpdateRoleStatusModel { isActive = Status };
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, RuleApi.RuleStatusUpdate);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, ruleGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(request).ReceiveJson<DataResponse<List<UpdateRuleStatusResult>>>();

                return new DataResponse<UpdateRuleStatusResult>(null)
                {
                    data = result.data.FirstOrDefault(),
                    message = result.message,
                    status = true
                };

            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateRuleStatusResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Rule", "UpdateRuleStatus()");
                throw ex;
            }
        }

        /// <summary>
        /// This API endpoint provides you the count of available rules.
        /// </summary>
        /// <returns></returns>
        public async Task<DataResponse<RuleCountResult>> RuleCount()
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, RuleApi.RuleCount);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var ruleDetail = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                .GetJsonAsync<DataResponse<List<RuleCountResult>>>();

                return new DataResponse<RuleCountResult>(null)
                {
                    data = ruleDetail.data.FirstOrDefault(),
                    message = ruleDetail.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<RuleCountResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Rule", "RuleCount()");
                throw ex;
            }
        }

        #endregion


        #region Event LookUp
        /// <summary>
        /// This API endpoint provides you the list of predefined event subscriptions. Using these details, 
        /// company users can subscribe to these events later.
        /// You can also add the filters as given in the parameters.
        /// Though the filters are optional, to apply them, you need to send these filter parameters in a query string.
        /// eventTopicGuid parameter is used to get the list of event subscriptions for a given event topic group.
        /// </summary>
        /// <returns></returns>
        public async Task<DataResponse<List<EventResult>>> Event(EventModel eventModel)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.EventBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, RuleApi.EventLookUp);
                string formattedUrl = String.Format(accessTokenUrl, Constants.eventVersion);
                var resultEvent = await formattedUrl.WithOAuthBearerToken(_token)
                                         .SetQueryParams(new
                                         {
                                             pageSize = eventModel.PageSize,
                                             pageNumber = eventModel.PageNo,
                                             searchtext = eventModel.SearchText,
                                             sortBy = eventModel.SortBy,
                                             eventTopicGuid = eventModel.eventTopicGuid
                                         })
                                         .GetJsonAsync<DataResponse<List<EventResult>>>();

                return new DataResponse<List<EventResult>>(null)
                {
                    data = resultEvent.data,
                    message = resultEvent.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<EventResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Rule", "Event()");
                throw ex;
            }
        }

        /// <summary>
        /// This API endpoint gives you the list of delivery methods for the event.
        /// For that, you need to send the event’s eventId in request url.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<DataResponse<List<EventDeliveryMethod>>> EventDeliveryMethod(string eventId)
        {
            try
            {
                if (string.IsNullOrEmpty(eventId))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "The value 'delivery-method' is not valid.",
                        Param = "eventId"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<EventDeliveryMethod>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.EventBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, RuleApi.EventDeliveryMethod);
                string formattedUrl = String.Format(accessTokenUrl, Constants.eventVersion,eventId);
                var deliveryMethods = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                               .GetJsonAsync<DataResponse<List<EventDeliveryMethod>>>();

                return new DataResponse<List<EventDeliveryMethod>>(null)
                {
                    data = deliveryMethods.data,
                    message = deliveryMethods.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<EventDeliveryMethod>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Rule", "EventDeliveryMethod()");
                throw ex;
            }
        }
        #endregion

        #region Severity-Level Lookup
        /// <summary>
        /// Users can define severity levels from critical, major, minor, information, 
        /// or warning based on the need for their use case. 
        /// This API endpoint helps you to get the lookup list of severity levels.
        /// </summary>
        /// <returns></returns>
        public async Task<DataResponse<List<SeverityLevelLookupResult>>> SeverityLevelLookup()
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.EventBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, RuleApi.SeverityLevelLookup);
                string formattedUrl = String.Format(accessTokenUrl, Constants.eventVersion);
                var resultEvent = await formattedUrl.WithOAuthBearerToken(_token)
                    .GetJsonAsync<DataResponse<List<SeverityLevelLookupResult>>>();

                return new DataResponse<List<SeverityLevelLookupResult>>(null)
                {
                    data = resultEvent.data,
                    message = resultEvent.message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<SeverityLevelLookupResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Rule", "SeverityLevelLookup()");
                throw ex;
            }
        } 
        #endregion



    }
}
