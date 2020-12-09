using System.Collections.Generic;
using System.Net.Http;

namespace component.eventbus.Queue
{
    /// <summary>
    /// AzureQueueProvider
    /// </summary>
    public class AzureQueueProvider : IQueueProvider
    {
        /// <summary>
        /// The connection string
        /// </summary>
        string ConnectionString = string.Empty;

        /// <summary>
        /// The queue name
        /// </summary>
        string QueueName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureQueueProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="queueName">Name of the queue.</param>
        public AzureQueueProvider(string connectionString, string queueName)
        {
            ConnectionString = connectionString;
            QueueName = queueName;
        }

        /// <summary>
        /// Adds the queue.
        /// </summary>
        /// <param name="busName">Name of the bus.</param>
        /// <param name="busKey">The bus key.</param>
        /// <param name="busValue">The bus value.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="utilityBaseUrl">The utility base URL.</param>
        /// <returns></returns>
        public bool AddQueue(string busName, string busKey, string busValue, string queueName, string utilityBaseUrl)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MessageBusName", busName);
            headers.Add("MessageBusKey", busKey);
            headers.Add("MessageBusValue", busValue);
            headers.Add("QueueName", QueueName);
            HttpResponseMessage result = ApiClient.GetApiResponse(utilityBaseUrl + "api/v1/utility/queue", new HttpMethod("POST"), headers, string.Empty);
            if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
                return true;
            return false;
        }
    }
}
