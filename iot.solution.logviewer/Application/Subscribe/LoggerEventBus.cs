using component.eventbus.Model.Topic.Logger;
using component.logger.platform.common.Enums;
using component.logger.platform.common.Log;
using component.services.logger.viewer.Application.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace component.services.logger.viewer.Application.Subscribe
{
    /// <summary>
    /// LoggerEventBus
    /// </summary>
    public static class LoggerEventBus
    {
        /// <summary>
        /// Subscribes the error log.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static async Task SubscribeErrorLog<T>(T message)
        {
            try
            {
                ErrorLoggerModel logmessage = message as ErrorLoggerModel;

                LogModel logModel = new LogModel
                {
                    Date = logmessage.Date,
                    Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), logmessage.Severity),
                    Logger = logmessage.Logger,
                    File = logmessage.File,
                    Method = logmessage.Method,
                    Identity = logmessage.Identity,
                    Message = logmessage.Message,
                    StackTrace = logmessage.StackTrace,
                    Exception = logmessage.Exception,
                    ApplicationCode = logmessage.ApplicationCode,
                    ErrorCode = logmessage.ErrorCode,
                    MessageData = logmessage.MessageData,
                    ComponentStatus = logmessage.ComponentStatus
                };

                LogFacade logFacade = new LogFacade();
                Models.BaseResponse<string> businessResponse = logFacade.AddEvent(logModel);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Subscribes the fatal log.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static async Task SubscribeFatalLog<T>(T message)
        {
            try
            {
                FatalLoggerModel logmessage = message as FatalLoggerModel;

                LogModel logModel = new LogModel
                {
                    Date = logmessage.Date,
                    Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), logmessage.Severity),
                    Logger = logmessage.Logger,
                    File = logmessage.File,
                    Method = logmessage.Method,
                    Identity = logmessage.Identity,
                    Message = logmessage.Message,
                    StackTrace = logmessage.StackTrace,
                    Exception = logmessage.Exception,
                    ApplicationCode = logmessage.ApplicationCode,
                    ErrorCode = logmessage.ErrorCode,
                    MessageData = logmessage.MessageData,
                    ComponentStatus = logmessage.ComponentStatus
                };

                LogFacade logFacade = new LogFacade();
                Models.BaseResponse<string> businessResponse = logFacade.AddEvent(logModel);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Subscribes the debug log.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static async Task SubscribeDebugLog<T>(T message)
        {
            try
            {
                DebugLoggerModel logmessage = message as DebugLoggerModel;

                LogModel logModel = new LogModel
                {
                    Date = logmessage.Date,
                    Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), logmessage.Severity),
                    Logger = logmessage.Logger,
                    File = logmessage.File,
                    Method = logmessage.Method,
                    Identity = logmessage.Identity,
                    Message = logmessage.Message,
                    StackTrace = logmessage.StackTrace,
                    Exception = logmessage.Exception,
                    ApplicationCode = logmessage.ApplicationCode,
                    ErrorCode = logmessage.ErrorCode,
                    MessageData = logmessage.MessageData,
                    ComponentStatus = logmessage.ComponentStatus
                };

                LogFacade logFacade = new LogFacade();
                Models.BaseResponse<string> businessResponse = logFacade.AddEvent(logModel);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Subscribes the warning log.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static async Task SubscribeWarningLog<T>(T message)
        {
            try
            {
                WarningLoggerModel logmessage = message as WarningLoggerModel;

                LogModel logModel = new LogModel
                {
                    Date = logmessage.Date,
                    Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), logmessage.Severity),
                    Logger = logmessage.Logger,
                    File = logmessage.File,
                    Method = logmessage.Method,
                    Identity = logmessage.Identity,
                    Message = logmessage.Message,
                    StackTrace = logmessage.StackTrace,
                    Exception = logmessage.Exception,
                    ApplicationCode = logmessage.ApplicationCode,
                    ErrorCode = logmessage.ErrorCode,
                    MessageData = logmessage.MessageData,
                    ComponentStatus = logmessage.ComponentStatus
                };

                LogFacade logFacade = new LogFacade();
                Models.BaseResponse<string> businessResponse = logFacade.AddEvent(logModel);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Subscribes the information log.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static async Task SubscribeInfoLog<T>(T message)
        {
            try
            {
                InfoLoggerModel logmessage = message as InfoLoggerModel;

                LogModel logModel = new LogModel
                {
                    Date = logmessage.Date,
                    Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), logmessage.Severity),
                    Logger = logmessage.Logger,
                    File = logmessage.File,
                    Method = logmessage.Method,
                    Identity = logmessage.Identity,
                    Message = logmessage.Message,
                    ApplicationCode = logmessage.ApplicationCode,
                    ErrorCode = logmessage.ErrorCode,
                    MessageData = logmessage.MessageData,
                    ComponentStatus = logmessage.ComponentStatus
                };

                LogFacade logFacade = new LogFacade();
                Models.BaseResponse<string> businessResponse = logFacade.AddEvent(logModel);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Losts the message notification.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static async Task LostMessageNotification<T>(T message)
        {
            try
            {
                LostMessageNotification lostMessage = message as LostMessageNotification;
                LogFacade logFacade = new LogFacade();
                Models.BaseResponse<string> businessResponse = logFacade.AddLostEventBudsMessage(lostMessage);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
