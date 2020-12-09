using component.logger.data.log;
using component.logger.data.log.Helper;
using component.logger.data.log.Model;
using component.logger.data.log.Repositories;
using component.logger.platform.common.Enums;
using component.logger.platform.common.Log;
using component.services.logger.viewer.Application.AppSetting;
using NLog;
using System;
using System.Collections.Generic;
using component.logger.data.log.EntityModel;
using component.logger.data.log.Enum;
using component.logger.platform.common;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using component.services.logger.viewer.Models;

namespace component.services.logger.viewer.Application.Facade
{
    /// <summary>
    /// LogFacade
    /// </summary>
    public class LogFacade : BaseFacade
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The log management repository
        /// </summary>
        LogManagementRepository logManagementRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFacade"/> class.
        /// </summary>
        public LogFacade()
        {
        }

        /// <summary>
        /// Components the list.
        /// </summary>
        /// <returns></returns>
        public BaseResponse<List<ComponentModel>> ComponentList()
        {
            try
            {
                logManagementRepository = new LogManagementRepository();
                DataResponse<List<Component>> dataResponse = logManagementRepository.ComponentList();

                List<ComponentModel> componentList = new List<ComponentModel>();
                if (dataResponse != null && dataResponse.Data != null)
                {
                    foreach (Component item in dataResponse.Data)
                    {
                        componentList.Add(new ComponentModel()
                        {
                            ComponentId = item.ComponentId,
                            Name = item.Name
                        });
                    }
                }

                return new BaseResponse<List<ComponentModel>>()
                {
                    Data = componentList,
                    Message = new ResponseMessage() { Status = (ResponseStatus)dataResponse.Status },
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Components the list test.
        /// </summary>
        /// <returns></returns>
        public List<ComponentModel> ComponentListTest()
        {
            try
            {
                logManagementRepository = new LogManagementRepository();
                DataResponse<List<Component>> dataResponse = logManagementRepository.ComponentList();

                List<ComponentModel> componentList = new List<ComponentModel>();
                if (dataResponse != null && dataResponse.Data != null)
                {
                    foreach (Component item in dataResponse.Data)
                    {
                        componentList.Add(new ComponentModel()
                        {
                            ComponentId = item.ComponentId,
                            Name = item.Name
                        });
                    }
                }

                return componentList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds the application configuration.
        /// </summary>
        /// <param name="applicationCode">The application code.</param>
        /// <param name="enableOmsConfig">if set to <c>true</c> [enable oms configuration].</param>
        /// <returns></returns>
        public BaseResponse<string> AddApplicationConfiguration(string applicationCode, bool enableOmsConfig)
        {
            try
            {
                if (string.IsNullOrEmpty(applicationCode))
                    FireValidation(component.logger.platform.common.MessageCode.InvalidApplication, ResponseStatus.Business_InvalidData);

                logManagementRepository = new LogManagementRepository();
                DataResponse<string> dataResponse = logManagementRepository.AddApplicationConfiguration(applicationCode, enableOmsConfig);
                ValidateResponse(dataResponse);
                return new BaseResponse<string>()
                {
                    Data = dataResponse.Data,
                    Message = new ResponseMessage() { Status = (ResponseStatus)dataResponse.Status }
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds the event.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public BaseResponse<string> AddEvent(LogModel request)
        {
            try
            {
                if (request == null)
                    FireValidation(component.logger.platform.common.MessageCode.RequestCannotNull, ResponseStatus.Business_NullRequest);

                if (string.IsNullOrWhiteSpace(request.Logger))
                    FireValidation(component.logger.platform.common.MessageCode.LoggerCanNotBeEmpty, ResponseStatus.Business_EmptyData);

                //if (string.IsNullOrWhiteSpace(request.Message))
                //    FireValidation(MessageCode.MessageCanNotBeEmpty, ResponseStatus.Business_EmptyData);

                if (!Enum.IsDefined(typeof(LogSeverity), request.Severity))
                    FireValidation(component.logger.platform.common.MessageCode.InvalidSeverity, ResponseStatus.Business_EmptyData);

                //request.Date = DateTime.UtcNow;

                ComponentConfiguration appComponentConfig = LogConfigurations.Instance.GetConfiguration(request.ApplicationCode, request.Logger);
                if (appComponentConfig == null)
                {
                    appComponentConfig = LogConfigurations.Instance.AddComponentConfiguration(request.ApplicationCode, request.Logger);
                }

                if (appComponentConfig != null)
                {
                    NLog.Logger logger = LogManager.GetCurrentClassLogger();
                    //GlobalDiagnosticsContext.Set("LogId", Guid.NewGuid());
                    GlobalDiagnosticsContext.Set("ApplicationCode", request.ApplicationCode);
                    GlobalDiagnosticsContext.Set("ComponentStatus", request.ComponentStatus);
                    GlobalDiagnosticsContext.Set("LogDate", request.Date);
                    GlobalDiagnosticsContext.Set("ErrorCode", request.ErrorCode);
                    GlobalDiagnosticsContext.Set("Exception", request.Exception);
                    GlobalDiagnosticsContext.Set("LogFile", request.File);
                    GlobalDiagnosticsContext.Set("Identity", request.Identity);
                    GlobalDiagnosticsContext.Set("Logger", request.Logger);
                    GlobalDiagnosticsContext.Set("Message", request.Message);
                    GlobalDiagnosticsContext.Set("MessageData", request.MessageData);
                    GlobalDiagnosticsContext.Set("Method", request.Method);
                    GlobalDiagnosticsContext.Set("Severity", request.Severity);
                    GlobalDiagnosticsContext.Set("StackTrace", request.StackTrace);

                    LogConfiguration loggingConfig = LogConfigurations.Instance.GetLogSeverity(request.Severity);

                    switch (request.Severity)
                    {
                        case LogSeverity.Debug:
                            if (appComponentConfig.Debug)
                            {
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Debug<LogModel>(request);
                                }
                                if (loggingConfig.IsOms)
                                {
                                    AzureLogAnalytics.Instance.Debug<LogModel>(request);
                                }
                            }
                            break;

                        case LogSeverity.Info:
                            if (appComponentConfig.Info)
                            {
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Info<LogModel>(request);
                                }

                                if (loggingConfig.IsOms)
                                {
                                    AzureLogAnalytics.Instance.Info<LogModel>(request);
                                }
                            }
                            break;

                        case LogSeverity.Warn:
                            if (appComponentConfig.Warn)
                            {
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Warn<LogModel>(request);
                                }
                                if (loggingConfig.IsOms)
                                {
                                    AzureLogAnalytics.Instance.Warn<LogModel>(request);
                                }
                            }
                            break;

                        case LogSeverity.Error:
                            if (appComponentConfig.Error)
                            {
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Error<LogModel>(request);
                                }
                                if (loggingConfig.IsOms)
                                {
                                    AzureLogAnalytics.Instance.Error<LogModel>(request);
                                }
                            }
                            break;
                        case LogSeverity.Fatal:
                            if (appComponentConfig.Fatal)
                            {
                                Task.Run(() =>
                                {
                                    SendFatalEmail(request);
                                });
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Fatal<LogModel>(request);
                                }
                                if (loggingConfig.IsOms)
                                {
                                    AzureLogAnalytics.Instance.Fatal<LogModel>(request);
                                }
                            }
                            break;

                        case LogSeverity.SubscribeDebug:
                            if (appComponentConfig.SubscribeDebug)
                            {
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Debug<LogModel>(request);
                                }
                                if (loggingConfig.IsOms)
                                {
                                    AzureLogAnalytics.Instance.Debug<LogModel>(request);
                                }
                            }
                            break;

                        case LogSeverity.SubscribeError:
                            if (appComponentConfig.SubscribeError)
                            {
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Error<LogModel>(request);
                                }
                                if (loggingConfig.IsOms)
                                {
                                    AzureLogAnalytics.Instance.Error<LogModel>(request);
                                }
                            }
                            break;

                        case LogSeverity.CronJobDebug:
                            if (Convert.ToBoolean(appComponentConfig.CronJobDebug))
                            {
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Debug<LogModel>(request);
                                }
                                //if (loggingConfig.IsOms)
                                //{
                                //    AzureLogAnalytics.Instance.Debug<LogModel>(request);
                                //}
                            }
                            break;

                        case LogSeverity.CronJobError:
                            if (Convert.ToBoolean(appComponentConfig.CronJobError))
                            {
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Error<LogModel>(request);
                                }
                                //if (loggingConfig.IsOms)
                                //{
                                //    AzureLogAnalytics.Instance.Error<LogModel>(request);
                                //}
                            }
                            break;
                        case LogSeverity.CronJobFatal:
                            if (Convert.ToBoolean(appComponentConfig.CronJobFatal))
                            {
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Fatal<LogModel>(request);
                                }
                                //if (loggingConfig.IsOms)
                                //{
                                //    AzureLogAnalytics.Instance.Fatal<LogModel>(request);
                                //}
                            }
                            break;
                        case LogSeverity.CronJobInfo:
                            if (Convert.ToBoolean(appComponentConfig.CronJobInfo))
                            {
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Info<LogModel>(request);
                                }
                                //if (loggingConfig.IsOms)
                                //{
                                //    AzureLogAnalytics.Instance.Info<LogModel>(request);
                                //}
                            }
                            break;
                        case LogSeverity.CronJobWarn:
                            if (Convert.ToBoolean(appComponentConfig.CronJobWarn))
                            {
                                if (loggingConfig.IsDb)
                                {
                                    Logger.Warn<LogModel>(request);
                                }
                                //if (loggingConfig.IsOms)
                                //{
                                //    AzureLogAnalytics.Instance.Warn<LogModel>(request);
                                //}
                            }
                            break;
                        default:
                            break;
                    }
                }
                return new BaseResponse<string>()
                {

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds the lost event buds message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public BaseResponse<string> AddLostEventBudsMessage(object request)
        {
            try
            {
                LogConfiguration loggingConfig = LogConfigurations.Instance.GetLogSeverity(LogSeverity.LostMessage);
                if (loggingConfig!=null && loggingConfig.IsOms)
                {
                    AzureLogAnalytics.Instance.LostMesssage(request);
                }

                return new BaseResponse<string>()
                {

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Adds the lost event buds message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public void SendFatalEmail(LogModel request)
        {
            try
            {
                string BlobDomainUrl = ServiceAppSetting.Instance.GetRequiredAppSettingByKey(AppSettingKeyLogger.BlobDomainUrl.ToString()) ?? EmailConstants.BlobDomainUrl;
                string HtmlBody = string.Empty;
                string Subject = string.Empty;
                string fatalEmailTempalte = !string.IsNullOrWhiteSpace(ServiceAppSetting.Instance.GetRequiredAppSettingByKey(AppSettingKeyLogger.FatalEmailTemplate.ToString())) ? ServiceAppSetting.Instance.GetRequiredAppSettingByKey(AppSettingKeyLogger.FatalEmailTemplate.ToString()) : EmailConstants.FatalEmailTemplate;

                HtmlBody = BlobProvider.GetFileAsString(fatalEmailTempalte, BlobDomainUrl);
                HtmlBody = HtmlBody.Replace("{{blobDomainUrl}}", BlobDomainUrl)
                                   .Replace("{{Logger}}", Convert.ToString(request.Logger))
                                   .Replace("{{LogFile}}", Convert.ToString(request.File))
                                   .Replace("{{ApplicationCode}}", Convert.ToString(request.ApplicationCode))
                                   .Replace("{{Method}}", Convert.ToString(request.Method))
                                   .Replace("{{Severity}}", Convert.ToString(request.Severity))
                                   .Replace("{{LogDate}}", Convert.ToString(request.Date))
                                   .Replace("{{ErrorCode}}", Convert.ToString(request.ErrorCode))
                                   .Replace("{{Exception}}", Convert.ToString(request.Exception))
                                   .Replace("{{Message}}", Convert.ToString(request.Message))
                                   .Replace("{{MessageData}}", Convert.ToString(request.MessageData))
                                   .Replace("{{StackTrace}}", Convert.ToString(request.StackTrace));
                string subject = $"Fatal error - {request.ApplicationCode} - {request.Method}";

                if (!string.IsNullOrWhiteSpace(HtmlBody))
                {
                    EmailData EmailData = new EmailData()
                    {
                        FromEmailId = ServiceAppSetting.Instance.GetRequiredAppSettingByKey(AppSettingKeyLogger.FromEmailAddress.ToString()),
                        ToEmailId = ServiceAppSetting.Instance.GetRequiredAppSettingByKey(AppSettingKeyLogger.FatalEmailAddresses.ToString()),
                        SendGridAPIKey = ServiceAppSetting.Instance.GetRequiredAppSettingByKey(AppSettingKeyLogger.SendGridAPIKey.ToString()),
                        Subject = Subject,
                        Body = HtmlBody
                    };

                    var client = new SendGridClient(EmailData.SendGridAPIKey);
                    var msg = new SendGridMessage();

                    string emailBodyHTML = EmailData.Body;

                    msg = new SendGridMessage()
                    {
                        From = new EmailAddress(EmailData.FromEmailId, EmailConstants.EmailFromName),
                        Subject = EmailData.Subject,
                        HtmlContent = emailBodyHTML,

                    };

                    msg.AddTo(new SendGrid.Helpers.Mail.EmailAddress(EmailData.ToEmailId));
                    var response = client.SendEmailAsync(msg);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
