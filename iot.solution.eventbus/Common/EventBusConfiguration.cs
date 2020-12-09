using System;
using System.Collections.Concurrent;

namespace component.eventbus.Common
{
    /// <summary>
    /// EventBusConfiguration
    /// </summary>
    public class EventBusConfiguration
    {
        /// <summary>
        /// The concurrent dictionary
        /// </summary>
        static readonly ConcurrentDictionary<string, string> _concurrentDictionary;

        /// <summary>
        /// Initializes the <see cref="EventBusConfiguration"/> class.
        /// </summary>
        static EventBusConfiguration()
        {
            if (_concurrentDictionary == null)
            {
                _concurrentDictionary = new ConcurrentDictionary<string, string>();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBusConfiguration"/> class.
        /// </summary>
        /// <param name="eventBusConfigurationList">The event bus configuration list.</param>
        public EventBusConfiguration(ConcurrentDictionary<string, string> eventBusConfigurationList)
        {
            foreach (System.Collections.Generic.KeyValuePair<string, string> Configuration in eventBusConfigurationList)
            {
                _concurrentDictionary.TryAdd(Configuration.Key, Configuration.Value);
            }
        }

        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="returnItem">The return item.</param>
        /// <returns></returns>
        public bool TryGet<T>(string cacheKey, out T returnItem)
        {
            Type t = typeof(T);

            if (_concurrentDictionary.ContainsKey(cacheKey))
            {
                object item = _concurrentDictionary[cacheKey];
                if (item != null)
                {
                    returnItem = (T)item;
                    return true;
                }
            }

            returnItem = default(T);
            return false;
        }
    }
}
