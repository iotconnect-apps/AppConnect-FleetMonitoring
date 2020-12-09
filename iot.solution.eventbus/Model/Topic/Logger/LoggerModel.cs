using component.eventbus.Common;
using component.eventbus.Common.Enum;
using component.eventbus.CustomAttribute;
using component.eventbus.Model.Base;
using Newtonsoft.Json;
using System;

namespace component.eventbus.Model.Topic.Logger
{
    /// <summary>
    /// DebugLoggerModel
    /// </summary>
    [Configure(connection: "BrokerConnection", topicName: "solutions-logs-qa", eventType: (int)EventType.DebugLoggerModel, eventTypeVersion: (int)EventTypeVersion.v1)]
    public class DebugLoggerModel : BaseServiceBusModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLoggerModel"/> class.
        /// </summary>
        public DebugLoggerModel()
        {
            this._Connection = Methods.GetConnectionDetailFromAttribute(this).Connection;
            this._TopicName = Methods.GetConnectionDetailFromAttribute(this).TopicName;
            this._EventId = Convert.ToInt16(Methods.GetConnectionDetailFromAttribute(this).EventId);
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        [JsonProperty("severity")]
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        [JsonProperty("logger")]
        public string Logger { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        [JsonProperty("file")]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        [JsonProperty("identity")]
        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>
        /// The stack trace.
        /// </value>
        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        [JsonProperty("exception")]
        public string Exception { get; set; }

        /// <summary>
        /// Gets or sets the application code.
        /// </summary>
        /// <value>
        /// The application code.
        /// </value>
        [JsonProperty("applicationCode")]
        public string ApplicationCode { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the message data.
        /// </summary>
        /// <value>
        /// The message data.
        /// </value>
        [JsonProperty("messageData")]
        public string MessageData { get; set; }

        /// <summary>
        /// Gets or sets the component status.
        /// </summary>
        /// <value>
        /// The component status.
        /// </value>
        [JsonProperty("componentStatus")]
        public string ComponentStatus { get; set; }
    }


    /// <summary>
    /// ErrorLoggerModel
    /// </summary>
    [Configure(connection: "BrokerConnection", topicName: "solutions-logs-qa", eventType: (int)EventType.ErrorLoggerModel, eventTypeVersion: (int)EventTypeVersion.v1)]
    public class ErrorLoggerModel : BaseServiceBusModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLoggerModel"/> class.
        /// </summary>
        public ErrorLoggerModel()
        {
            this._Connection = Methods.GetConnectionDetailFromAttribute(this).Connection;
            this._TopicName = Methods.GetConnectionDetailFromAttribute(this).TopicName;
            this._EventId = Convert.ToInt16(Methods.GetConnectionDetailFromAttribute(this).EventId);
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        [JsonProperty("severity")]
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        [JsonProperty("logger")]
        public string Logger { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        [JsonProperty("file")]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        [JsonProperty("identity")]
        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>
        /// The stack trace.
        /// </value>
        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        [JsonProperty("exception")]
        public string Exception { get; set; }

        /// <summary>
        /// Gets or sets the application code.
        /// </summary>
        /// <value>
        /// The application code.
        /// </value>
        [JsonProperty("applicationCode")]
        public string ApplicationCode { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the message data.
        /// </summary>
        /// <value>
        /// The message data.
        /// </value>
        [JsonProperty("messageData")]
        public string MessageData { get; set; }

        /// <summary>
        /// Gets or sets the component status.
        /// </summary>
        /// <value>
        /// The component status.
        /// </value>
        [JsonProperty("componentStatus")]
        public string ComponentStatus { get; set; }
    }

    /// <summary>
    /// WarningLoggerModel
    /// </summary>
    [Configure(connection: "BrokerConnection", topicName: "solutions-logs-qa", eventType: (int)EventType.WarningLoggerModel, eventTypeVersion: (int)EventTypeVersion.v1)]
    public class WarningLoggerModel : BaseServiceBusModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarningLoggerModel"/> class.
        /// </summary>
        public WarningLoggerModel()
        {
            this._Connection = Methods.GetConnectionDetailFromAttribute(this).Connection;
            this._TopicName = Methods.GetConnectionDetailFromAttribute(this).TopicName;
            this._EventId = Convert.ToInt16(Methods.GetConnectionDetailFromAttribute(this).EventId);
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        [JsonProperty("severity")]
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        [JsonProperty("logger")]
        public string Logger { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        [JsonProperty("file")]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        [JsonProperty("identity")]
        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>
        /// The stack trace.
        /// </value>
        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        [JsonProperty("exception")]
        public string Exception { get; set; }

        /// <summary>
        /// Gets or sets the application code.
        /// </summary>
        /// <value>
        /// The application code.
        /// </value>
        [JsonProperty("applicationCode")]
        public string ApplicationCode { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the message data.
        /// </summary>
        /// <value>
        /// The message data.
        /// </value>
        [JsonProperty("messageData")]
        public string MessageData { get; set; }

        /// <summary>
        /// Gets or sets the component status.
        /// </summary>
        /// <value>
        /// The component status.
        /// </value>
        [JsonProperty("componentStatus")]
        public string ComponentStatus { get; set; }
    }

    /// <summary>
    /// InfoLoggerModel
    /// </summary>
    [Configure(connection: "BrokerConnection", topicName: "solutions-logs-qa", eventType: (int)EventType.InfoLoggerModel, eventTypeVersion: (int)EventTypeVersion.v1)]
    public class InfoLoggerModel : BaseServiceBusModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoLoggerModel"/> class.
        /// </summary>
        public InfoLoggerModel()
        {
            this._Connection = Methods.GetConnectionDetailFromAttribute(this).Connection;
            this._TopicName = Methods.GetConnectionDetailFromAttribute(this).TopicName;
            this._EventId = Convert.ToInt16(Methods.GetConnectionDetailFromAttribute(this).EventId);
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        [JsonProperty("severity")]
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        [JsonProperty("logger")]
        public string Logger { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        [JsonProperty("file")]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        [JsonProperty("identity")]
        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the application code.
        /// </summary>
        /// <value>
        /// The application code.
        /// </value>
        [JsonProperty("applicationCode")]
        public string ApplicationCode { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the message data.
        /// </summary>
        /// <value>
        /// The message data.
        /// </value>
        [JsonProperty("messageData")]
        public string MessageData { get; set; }

        /// <summary>
        /// Gets or sets the component status.
        /// </summary>
        /// <value>
        /// The component status.
        /// </value>
        [JsonProperty("componentStatus")]
        public string ComponentStatus { get; set; }
    }

    /// <summary>
    /// FatalLoggerModel
    /// </summary>
    [Configure(connection: "BrokerConnection", topicName: "solutions-logs-qa", eventType: (int)EventType.FatalLoggerModel, eventTypeVersion: (int)EventTypeVersion.v1)]
    public class FatalLoggerModel : BaseServiceBusModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FatalLoggerModel"/> class.
        /// </summary>
        public FatalLoggerModel()
        {
            this._Connection = Methods.GetConnectionDetailFromAttribute(this).Connection;
            this._TopicName = Methods.GetConnectionDetailFromAttribute(this).TopicName;
            this._EventId = Convert.ToInt16(Methods.GetConnectionDetailFromAttribute(this).EventId);
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        [JsonProperty("severity")]
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        [JsonProperty("logger")]
        public string Logger { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        [JsonProperty("file")]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        [JsonProperty("identity")]
        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>
        /// The stack trace.
        /// </value>
        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        [JsonProperty("exception")]
        public string Exception { get; set; }

        /// <summary>
        /// Gets or sets the application code.
        /// </summary>
        /// <value>
        /// The application code.
        /// </value>
        [JsonProperty("applicationCode")]
        public string ApplicationCode { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the message data.
        /// </summary>
        /// <value>
        /// The message data.
        /// </value>
        [JsonProperty("messageData")]
        public string MessageData { get; set; }

        /// <summary>
        /// Gets or sets the component status.
        /// </summary>
        /// <value>
        /// The component status.
        /// </value>
        [JsonProperty("componentStatus")]
        public string ComponentStatus { get; set; }
    }

    /// <summary>
    /// LostMessageNotification
    /// </summary>
    [Configure(connection: "BrokerConnection", topicName: "solutions-logs-qa", eventType: (int)EventType.LostMessageNotification, eventTypeVersion: (int)EventTypeVersion.v1)]
    public class LostMessageNotification : BaseServiceBusModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LostMessageNotification"/> class.
        /// </summary>
        public LostMessageNotification()
        {
            this._Connection = Methods.GetConnectionDetailFromAttribute(this).Connection;
            this._TopicName = Methods.GetConnectionDetailFromAttribute(this).TopicName;
            this._EventId = Convert.ToInt16(Methods.GetConnectionDetailFromAttribute(this).EventId);
        }

        /// <summary>
        /// Gets or sets the receiver identifier.
        /// </summary>
        /// <value>
        /// The receiver identifier.
        /// </value>
        [JsonProperty("receiverId")]
        public int ReceiverId { get; set; }

        /// <summary>
        /// Gets or sets the sender identifier.
        /// </summary>
        /// <value>
        /// The sender identifier.
        /// </value>
        [JsonProperty("senderId")]
        public int SenderId { get; set; }

        /// <summary>
        /// Gets or sets the topic.
        /// </summary>
        /// <value>
        /// The topic.
        /// </value>
        [JsonProperty("topic")]
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        [JsonProperty("eventId")]
        public string EventId { get; set; }

        //[JsonProperty("sendTimeStamp")]
        //public long SendTimeStamp { get; set; }

        //[JsonProperty("receivedTimeStamp")]
        //public long ReceivedTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the exception detail.
        /// </summary>
        /// <value>
        /// The exception detail.
        /// </value>
        [JsonProperty("exceptionDetail")]
        public string ExceptionDetail { get; set; }
    }
}
