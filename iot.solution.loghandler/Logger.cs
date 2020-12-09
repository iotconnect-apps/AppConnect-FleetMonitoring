using component.eventbus;
using component.eventbus.Common;
using component.eventbus.Model.Base;
using component.eventbus.Model.Topic.Logger;
using component.logger.platform.common.Enums;
using component.services.loghandler.Common;
using component.services.loghandler.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace component.services.loghandler
{
    public class Logger //: ILogHandler
    {
        private static IEventBus _eventBus;
        private static string _serviceType;
        private static bool _logging;
        private static string _applicationCode;

        public Logger(IEventBus eventBus, IOptions<DomainManager> domainManager)
        {
            _eventBus = eventBus;
            _serviceType = domainManager.Value.ServiceType;
            _logging = domainManager.Value.Logging;
            _applicationCode = domainManager.Value.ApplicationType.ToString();
        }

        public bool ErrorLog(Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.Error)
        {
            return ErrorLog(exception.Message, exception, fileName: fileName, methodName: methodName, componentStatus: componentStatus, logSeverity: logSeverity);
        }

        public bool ErrorLog(object message, Exception exception, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.Error)
        {
            if (!_logging)
                return false;

            try
            {
                ErrorLoggerModel errorLoggerModel = new ErrorLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = _serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = exception.StackTrace,
                    Exception = exception.ToString(),
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(errorLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ErrorLog(object message, Exception exception, ServiceType serviceType, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.Error)
        {
            if (!_logging)
                return false;

            try
            {
                ErrorLoggerModel errorLoggerModel = new ErrorLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = exception.StackTrace,
                    Exception = exception.ToString(),
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(errorLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool FatalLog(object message, Exception exception, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.Fatal)
        {
            if (!_logging)
                return false;

            try
            {
                FatalLoggerModel FatalLoggerModet = new FatalLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = _serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = exception != null ? exception.StackTrace : string.Empty,
                    Exception = exception != null ? exception.ToString() : string.Empty,
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(FatalLoggerModet);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool FatalLog(object message, Exception exception, ServiceType serviceType, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.Fatal)
        {
            if (!_logging)
                return false;

            try
            {
                FatalLoggerModel FatalLoggerModet = new FatalLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = exception != null ? exception.StackTrace : string.Empty,
                    Exception = exception != null ? exception.ToString() : string.Empty,
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(FatalLoggerModet);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DebugLog(object message, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.Debug)
        {
            if (!_logging)
                return false;

            try
            {
                DebugLoggerModel debugLoggerModel = new DebugLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = _serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = null,
                    Exception = null,
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(debugLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DebugLog(object message, ServiceType serviceType, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.Debug)
        {
            if (!_logging)
                return false;

            try
            {
                DebugLoggerModel debugLoggerModel = new DebugLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = null,
                    Exception = null,
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(debugLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool WarningLog(object message, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.Warn)
        {
            if (!_logging)
                return false;

            try
            {
                WarningLoggerModel warningLoggerModel = new WarningLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = _serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = null,
                    Exception = null,
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(warningLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool WarningLog(object message, ServiceType serviceType, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.Warn)
        {
            if (!_logging)
                return false;

            try
            {
                WarningLoggerModel warningLoggerModel = new WarningLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = serviceType.ToString(),
                    Logger = _serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = null,
                    Exception = null,
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(warningLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InfoLog(object message, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.Info)
        {
            if (!_logging)
                return false;

            try
            {
                InfoLoggerModel infoLoggerModel = new InfoLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = _serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(infoLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InfoLog(object message, ServiceType serviceType, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.Info)
        {
            if (!_logging)
                return false;

            try
            {
                InfoLoggerModel infoLoggerModel = new InfoLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(infoLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
       
        //public bool LostMessageLog(object message, Exception exception)
        //{
        //    if (!_logging)
        //        return false;

        //    try
        //    {
        //        LostMessageNotification notification = new LostMessageNotification
        //        {
        //            ReceiverId = (int)_serviceType,
        //            SenderId = ((BaseServiceBusModel)message)._ProducerId,
        //            Topic = Convert.ToString(((BaseServiceBusModel)message)._TopicName),
        //            EventId = Convert.ToString(((BaseServiceBusModel)message)._EventId),
        //            Message = message != null && ((BaseServiceBusModel)message)._OriginalMessage != null ? ((BaseServiceBusModel)message)._OriginalMessage : string.Empty,
        //            ExceptionDetail = exception != null ? JsonConvert.SerializeObject(exception) : string.Empty,
        //        };

        //        return _eventBus.Publish(notification);
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public bool CronJobErrorLog(Exception exception, [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.CronJobError)
        {
            return CronJobErrorLog(exception.Message, exception, fileName: fileName, methodName: methodName, componentStatus: componentStatus, logSeverity: logSeverity);
        }

        public bool CronJobErrorLog(object message, Exception exception, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.CronJobError)
        {
            if (!_logging)
                return false;

            try
            {
                ErrorLoggerModel errorLoggerModel = new ErrorLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = _serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = exception.StackTrace,
                    Exception = exception.ToString(),
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(errorLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CronJobErrorLog(object message, Exception exception, ServiceType serviceType, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.CronJobError)
        {
            if (!_logging)
                return false;

            try
            {
                ErrorLoggerModel errorLoggerModel = new ErrorLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = exception.StackTrace,
                    Exception = exception.ToString(),
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(errorLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CronJobFatalLog(object message, Exception exception, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.CronJobFatal)
        {
            if (!_logging)
                return false;

            try
            {
                FatalLoggerModel FatalLoggerModet = new FatalLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = _serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = exception != null ? exception.StackTrace : string.Empty,
                    Exception = exception != null ? exception.ToString() : string.Empty,
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(FatalLoggerModet);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CronJobFatalLog(object message, Exception exception, ServiceType serviceType, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.CronJobFatal)
        {
            if (!_logging)
                return false;

            try
            {
                FatalLoggerModel FatalLoggerModet = new FatalLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = exception != null ? exception.StackTrace : string.Empty,
                    Exception = exception != null ? exception.ToString() : string.Empty,
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(FatalLoggerModet);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CronJobDebugLog(object message, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.CronJobDebug)
        {
            if (!_logging)
                return false;

            try
            {
                DebugLoggerModel debugLoggerModel = new DebugLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = _serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = null,
                    Exception = null,
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(debugLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CronJobDebugLog(object message, ServiceType serviceType, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.CronJobDebug)
        {
            if (!_logging)
                return false;

            try
            {
                DebugLoggerModel debugLoggerModel = new DebugLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = null,
                    Exception = null,
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(debugLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CronJobWarningLog(object message, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.CronJobWarn)
        {
            if (!_logging)
                return false;

            try
            {
                WarningLoggerModel warningLoggerModel = new WarningLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = _serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    StackTrace = null,
                    Exception = null,
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(warningLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CronJobInfoLog(object message, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.CronJobInfo)
        {
            if (!_logging)
                return false;

            try
            {
                InfoLoggerModel infoLoggerModel = new InfoLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = _serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(infoLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CronJobInfoLog(object message, ServiceType serviceType, dynamic request = null, string errorCode = "", string identity = "", [CallerFilePath] string fileName = "", [CallerMemberName] string methodName = "", ComponentStatus componentStatus = ComponentStatus.None, LogSeverity logSeverity = LogSeverity.CronJobInfo)
        {
            if (!_logging)
                return false;

            try
            {
                InfoLoggerModel infoLoggerModel = new InfoLoggerModel
                {
                    Date = DateTime.UtcNow,
                    Severity = logSeverity.ToString(),
                    Logger = serviceType.ToString(),
                    File = fileName.ToFileName(),
                    Method = methodName,
                    Identity = !string.IsNullOrEmpty(identity) ? identity : Convert.ToString(CommonMethods.GetValueFromDynamic(request, "UserID")),

                    Message = Convert.ToString(message),
                    ApplicationCode = _applicationCode ?? string.Empty,
                    ErrorCode = errorCode,
                    MessageData = request != null ? JsonConvert.SerializeObject(request, Formatting.None) : string.Empty,
                    ComponentStatus = componentStatus != ComponentStatus.None ? componentStatus.ToString() : string.Empty
                };

                return _eventBus.Publish(infoLoggerModel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
