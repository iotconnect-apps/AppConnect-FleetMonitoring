using System;

namespace component.eventbus.Model.Base
{
    /// <summary>
    /// ServiceBusConnectionData
    /// </summary>
    public class ServiceBusConnectionData
    {
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
        public string TopicName { get; set; }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        public Int16 EventId { get; set; }

        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        public string QueueName { get; set; }
    }
}
