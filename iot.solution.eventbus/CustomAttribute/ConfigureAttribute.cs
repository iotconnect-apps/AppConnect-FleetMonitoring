using System;

namespace component.eventbus.CustomAttribute
{
    /// <summary>
    /// ConfigureAttribute
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ConfigureAttribute : Attribute
    {
        /// <summary>
        /// The connection
        /// </summary>
        private string _Connection;

        /// <summary>
        /// The topic name
        /// </summary>
        private string _TopicName;

        /// <summary>
        /// The queue name
        /// </summary>
        private string _QueueName;

        /// <summary>
        /// The event type
        /// </summary>
        private Int16 _EventType;

        /// <summary>
        /// The event type version
        /// </summary>
        private Int16 _EventTypeVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureAttribute"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventTypeVersion">The event type version.</param>
        /// <param name="topicName">Name of the topic.</param>
        /// <param name="queueName">Name of the queue.</param>
        public ConfigureAttribute(string connection, Int16 eventType, Int16 eventTypeVersion, string topicName = "", string queueName = "")
        {
            this._TopicName = topicName;
            this._Connection = connection;
            this._EventType = eventType;
            this._EventTypeVersion = eventTypeVersion;
            this._QueueName = queueName;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public string Connection { get { return _Connection; } }

        /// <summary>
        /// Gets the name of the topic.
        /// </summary>
        /// <value>
        /// The name of the topic.
        /// </value>
        public string TopicName { get { return _TopicName; } }

        /// <summary>
        /// Gets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        public string QueueName { get { return _QueueName; } }

        /// <summary>
        /// Gets the event identifier.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        public Int16 EventId { get { return _EventType; } }

        /// <summary>
        /// Gets the event type version.
        /// </summary>
        /// <value>
        /// The event type version.
        /// </value>
        public Int16 EventTypeVersion { get { return _EventTypeVersion; } }
    }
}
