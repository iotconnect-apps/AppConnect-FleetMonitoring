
namespace component.eventbus.Topic
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITopicProvider
    {
        /// <summary>
        /// Adds the topic.
        /// </summary>
        void AddTopic();

        /// <summary>
        /// Adds the subscriber.
        /// </summary>
        /// <param name="routingKey">The routing key.</param>
        void AddSubscriber(string routingKey);
    }
}
