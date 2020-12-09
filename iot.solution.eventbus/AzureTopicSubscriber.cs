using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace component.eventbus
{
    /// <summary>
    /// AzureTopicSubscriber
    /// </summary>
    /// <seealso cref="Microsoft.Azure.ServiceBus.SubscriptionClient" />
    /// <seealso cref="component.eventbus.IEventBus" />
    public class AzureTopicSubscriber : SubscriptionClient, IEventBus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTopicSubscriber"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="topicName">Name of the topic.</param>
        /// <param name="topicSubscription">The topic subscription.</param>
        public AzureTopicSubscriber(string connectionString, string topicName, string topicSubscription) : base(connectionString, topicName, topicSubscription, ReceiveMode.PeekLock, new RetryExponential(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(3), 3))
        {
        }

        /// <summary>
        /// Publishes the specified gereric event model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gerericEventModel">The gereric event model.</param>
        /// <returns></returns>
        /// <exception cref="MissingMethodException"></exception>
        public bool Publish<T>(T gerericEventModel)
        {
            throw new MissingMethodException();
        }

        /// <summary>
        /// Receives from queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetModel">The target model.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool ReceiveFromQueue<T>(T targetModel, Func<T, Task> handler)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends to queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gerericEventModel">The gereric event model.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool SendToQueue<T>(T gerericEventModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Subscribes the specified target model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetModel">The target model.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        /// <exception cref="MissingMethodException"></exception>
        public bool Subscribe<T>(T targetModel, Func<T, Task> handler)
        {
            throw new MissingMethodException();
        }
    }
}
