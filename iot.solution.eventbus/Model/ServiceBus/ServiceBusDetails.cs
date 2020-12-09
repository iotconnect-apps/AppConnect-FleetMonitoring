using System;

namespace component.eventbus.Model.ServiceBus
{
    /// <summary>
    /// ServiceBusDetails
    /// </summary>
    class ServiceBusDetails
    {
        /// <summary>
        /// The topic name
        /// </summary>
        private string _TopicName;

        /// <summary>
        /// The event identifier
        /// </summary>
        private Int16 _EventId;

        /// <summary>
        /// The event type version
        /// </summary>
        private Int16 _EventTypeVersion;

        /// <summary>
        /// The queue name
        /// </summary>
        private string _QueueName;

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public string Connection { get; set; }

        /// <summary>
        /// Gets or sets the name of the topic.
        /// </summary>
        /// <value>
        /// The name of the topic.
        /// </value>
        public string TopicName
        {
            get { return _TopicName; }
            set { _TopicName = value; }
        }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        public Int16 EventId
        {
            get { return _EventId; }
            set { _EventId = value; }
        }

        /// <summary>
        /// Gets or sets the subscription.
        /// </summary>
        /// <value>
        /// The subscription.
        /// </value>
        public string Subscription
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the event type version.
        /// </summary>
        /// <value>
        /// The event type version.
        /// </value>
        public Int16 EventTypeVersion
        {
            get { return _EventTypeVersion; }
            set { _EventTypeVersion = value; }
        }

        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        public string QueueName
        {
            get { return _QueueName; }
            set { _QueueName = value; }
        }

        /// <summary>
        /// Gets or sets the service bus namespace.
        /// </summary>
        /// <value>
        /// The service bus namespace.
        /// </value>
        public string ServiceBusNamespace { get; set; }

        /// <summary>
        /// Gets or sets the name of the shared access key.
        /// </summary>
        /// <value>
        /// The name of the shared access key.
        /// </value>
        public string SharedAccessKeyName { get; set; }

        /// <summary>
        /// Gets or sets the shared access key.
        /// </summary>
        /// <value>
        /// The shared access key.
        /// </value>
        public string SharedAccessKey { get; set; }

        /// <summary>
        /// Gets or sets the producer identifier.
        /// </summary>
        /// <value>
        /// The producer identifier.
        /// </value>
        public Int16 ProducerId { get; set; }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public string Endpoint { get; set; }

        public Int16 ProducerApplicationId { get; set; }
    }
}
