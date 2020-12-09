namespace component.eventbus.Queue
{
    /// <summary>
    /// IQueueProvider
    /// </summary>
    public interface IQueueProvider
    {
        /// <summary>
        /// Adds the queue.
        /// </summary>
        /// <param name="busName">Name of the bus.</param>
        /// <param name="busKey">The bus key.</param>
        /// <param name="busValue">The bus value.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="utilityBaseUrl">The utility base URL.</param>
        /// <returns></returns>
        bool AddQueue(string busName, string busKey, string busValue, string queueName, string utilityBaseUrl);
    }
}
