using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace component.eventbus
{
    /// <summary>
    /// AzureQueueReceiver
    /// </summary>
    /// <seealso cref="Microsoft.Azure.ServiceBus.QueueClient" />
    /// <seealso cref="component.eventbus.IEventBus" />
    class AzureQueueReceiver : QueueClient, IEventBus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureQueueReceiver"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="queueName">Name of the queue.</param>
        public AzureQueueReceiver(string connectionString, string queueName) : base(connectionString, queueName, ReceiveMode.PeekLock, new RetryExponential(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(3), 3))
        {
        }

        /// <summary>
        /// Publishes the specified gereric event model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gerericEventModel">The gereric event model.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Publish<T>(T gerericEventModel)
        {
            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException"></exception>
        public bool Subscribe<T>(T targetModel, Func<T, Task> handler)
        {
            throw new NotImplementedException();
        }
    }
}
