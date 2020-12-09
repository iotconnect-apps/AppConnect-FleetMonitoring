using System;
using System.Threading.Tasks;

namespace component.eventbus
{
    /// <summary>
    /// IEventBus
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes the specified gereric event model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gerericEventModel">The gereric event model.</param>
        /// <returns></returns>
        bool Publish<T>(T gerericEventModel);

        /// <summary>
        /// Subscribes the specified target model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetModel">The target model.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        bool Subscribe<T>(T targetModel, Func<T, Task> handler);

        /// <summary>
        /// Sends to queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gerericEventModel">The gereric event model.</param>
        /// <returns></returns>
        bool SendToQueue<T>(T gerericEventModel);

        /// <summary>
        /// Receives from queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetModel">The target model.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        bool ReceiveFromQueue<T>(T targetModel, Func<T, Task> handler);
    }
}
