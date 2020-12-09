using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace component.eventbus.Model.Base
{
    /// <summary>
    /// BaseServiceBusModel
    /// </summary>
    public class BaseServiceBusModel
    {
        public BaseServiceBusModel()
        {
            _Notification = new NotificationBaseServiceBusModel();
        }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        [JsonIgnore]
        public string _Connection { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the topic.
        /// </summary>
        /// <value>
        /// The name of the topic.
        /// </value>
        [JsonIgnore]
        public string _TopicName { get; protected set; }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        [JsonIgnore]
        public Int16 _EventId { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        [JsonIgnore]
        public string _QueueName { get; protected set; }

        // below property used at receiver side
        /// <summary>
        /// Gets or sets the producer identifier.
        /// </summary>
        /// <value>
        /// The producer identifier.
        /// </value>
        //[JsonIgnore]
        public Int16 _ProducerId { get; set; }


        // below property used at receiver side
        /// <summary>
        /// Gets or sets the producer application identifier.
        /// </summary>
        /// <value>
        /// The producer application identifier.
        /// </value>
        //[JsonIgnore]
        public Int16 _ProducerApplicationId { get; set; }

        //[JsonIgnore]
        //public long _SendTimeStamp { get; set; }

        //[JsonIgnore]
        //public long _ReceivedTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the original message.
        /// </summary>
        /// <value>
        /// The original message.
        /// </value>
        [JsonIgnore]
        public string _OriginalMessage { get; set; }

        public NotificationBaseServiceBusModel _Notification { get; set; }

        public string _SenderEmail { get; set; }
    }

    public class NotificationBaseServiceBusModel
    {
        public NotificationBaseServiceBusModel()
        {
            _SpecialCaseReceiver = new List<SpecialCaseReceiver>();
        }

        public List<SpecialCaseReceiver> _SpecialCaseReceiver { get; set; }
    }

    public class SpecialCaseReceiver
    {
        public SpecialCaseReceiver()
        {
            _SpecialCaseReceiverEmails = new List<string>();
        }

        public int _SpecialCaseType { get; set; }

        public List<string> _SpecialCaseReceiverEmails { get; set; }
    }
}
