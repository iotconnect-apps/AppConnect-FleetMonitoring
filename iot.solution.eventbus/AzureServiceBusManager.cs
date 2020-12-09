using component.eventbus.Common;
using component.eventbus.CustomAttribute;
using component.eventbus.Model.Base;
using component.eventbus.Model.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using component.eventbus.Topic;
using component.eventbus.Queue;

namespace component.eventbus
{
    /// <summary>
    /// AzureServiceBusManager
    /// </summary>
    /// <seealso cref="component.eventbus.IEventBus" />
    public class AzureServiceBusManager : IEventBus    
    {
        /// <summary>
        /// The event bus configuration
        /// </summary>
        private static EventBusConfiguration _eventBusConfiguration;

        /// <summary>
        /// The domain manager
        /// </summary>
        private static DomainManager _domainManager;

        /// <summary>
        /// The topic manager
        /// </summary>
        private static readonly ConcurrentDictionary<string, AzureTopicPublisher> _topicManager;

        /// <summary>
        /// The queue manager
        /// </summary>
        private static readonly ConcurrentDictionary<string, AzureQueueSender> _queueManager;

        /// <summary>
        /// The subscription client
        /// </summary>
        private ISubscriptionClient subscriptionClient;

        /// <summary>
        /// The receiver client
        /// </summary>
        private IReceiverClient receiverClient;

        /// <summary>
        /// The topic client
        /// </summary>
        private ITopicClient topicClient;

        /// <summary>
        /// Initializes the <see cref="AzureServiceBusManager"/> class.
        /// </summary>
        static AzureServiceBusManager()
        {
            if (_topicManager == null)
            {
                _topicManager = new ConcurrentDictionary<string, AzureTopicPublisher>();
            }

            if (_queueManager == null)
            {
                _queueManager = new ConcurrentDictionary<string, AzureQueueSender>();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceBusManager"/> class.
        /// </summary>
        /// <param name="eventBusConfigurationList">The event bus configuration list.</param>
        /// <param name="domainManager">The domain manager.</param>
        public AzureServiceBusManager(EventBusConfiguration eventBusConfigurationList, IOptions<DomainManager> domainManager)
        {
            _eventBusConfiguration = eventBusConfigurationList;
            _domainManager = domainManager.Value;

            if (_topicManager != null && _topicManager.Count == 0)
                SetupAzureTopicProvider(domainManager.Value);

            if (_queueManager != null && _queueManager.Count == 0)
                SetupAzureQueueProvider(domainManager.Value);
        }

        #region Topic

        /// <summary>
        /// Publishes the specified gereric event model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gerericEventModel">The gereric event model.</param>
        /// <returns></returns>
        public bool Publish<T>(T gerericEventModel)
        {
            try
            {
                ServiceBusDetails serviceBusDetails = GetServiceBusDetailsFromT(gerericEventModel.GetType());

                if (!string.IsNullOrEmpty(serviceBusDetails.Connection))
                {
                    if (_topicManager.TryGetValue($"{serviceBusDetails.Endpoint}||{serviceBusDetails.TopicName}", out AzureTopicPublisher topicClient))
                    {
                        Dictionary<string, object> userPropertiesDict = new Dictionary<string, object>
                        {
                            //// version wise event bus: Consider ver = EventTypeVersion which is atleast 1 (which we have settled in GetServiceBusDetailsFromT() )
                            { "ver", serviceBusDetails.EventTypeVersion},
                            { "type", 1 },
                            { "mId", Guid.NewGuid().ToString() },
                            { "pId", serviceBusDetails.ProducerId },
                            { "eId",  Convert.ToInt16(serviceBusDetails.EventId) },
                            { "tsSend", DateTime.UtcNow.Ticks },

                            //// Producer application id;
                            { "paId", serviceBusDetails.ProducerApplicationId },
                        };

                        EventModel<object> eventModel = new EventModel<object>()
                        {
                            Value = JsonConvert.SerializeObject(gerericEventModel).ToBase64Encode(),
                        };

                        Message message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventModel).ToBase64Encode()));

                        foreach (KeyValuePair<string, object> item in userPropertiesDict)
                        {
                            message.UserProperties.Add(new KeyValuePair<string, object>(item.Key, item.Value));
                        }

                        topicClient.SendAsync(message).Wait();
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Subscribes the specified target model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetModel">The target model.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        public bool Subscribe<T>(T targetModel, Func<T, Task> handler)
        {
            try
            {
                ServiceBusDetails serviceBusDetails = GetServiceBusDetailsFromT(targetModel.GetType());

                if (!string.IsNullOrEmpty(serviceBusDetails.Connection))
                {
                    //Check for Subscription and topic, if not exists AzureTopicProvider will create both them.
                    //we are considering UserProperty event id for sql filter
                    AzureTopicProvider azureTopicProvider = new AzureTopicProvider(serviceBusDetails.Connection, serviceBusDetails.TopicName, serviceBusDetails.Subscription);

                    //// version wise event bus: Each version wise subscriber get only message of that versioned model/object
                    string routingKey = $"user.eId = {serviceBusDetails.EventId}  AND user.ver = {serviceBusDetails.EventTypeVersion}";

                    azureTopicProvider.AddSubscriber(routingKey);

                    subscriptionClient = new AzureTopicSubscriber(serviceBusDetails.Connection, serviceBusDetails.TopicName, serviceBusDetails.Subscription);
                    MessageHandlerOptions messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
                    {
                        MaxConcurrentCalls = 1,
                        AutoComplete = true
                    };

                    //subscriptionClient.RegisterMessageHandler(handler, messageHandlerOptions);
                    subscriptionClient.RegisterMessageHandler(async (msg, token) =>
                    {
                        try
                        {
                            // Get message  
                            string message = Encoding.UTF8.GetString(msg.Body).ToBase64Decode();

                            EventModel<object> objEvent1 = JsonConvert.DeserializeObject<EventModel<object>>(message);
                            T retrunModel = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(objEvent1.OriginalValue));

                            msg.UserProperties.Add(new KeyValuePair<string, object>("tsReceive", DateTime.UtcNow.Ticks));
                            msg.UserProperties.Add(new KeyValuePair<string, object>("rId", _domainManager.ServiceType));

                            PropertyInfo prop = null;

                            //// set producer id to the message model which will be received by subscriber
                            if (msg.UserProperties.TryGetValue("pId", out object producerId))
                            {
                                prop = retrunModel.GetType().GetProperty("_ProducerId", BindingFlags.Public | BindingFlags.Instance);
                                if (null != prop && prop.CanWrite)
                                {
                                    prop.SetValue(retrunModel, producerId, null);
                                }
                            }

                            //// set original message to the message model which will be received by subscriber
                            prop = retrunModel.GetType().GetProperty("_OriginalMessage", BindingFlags.Public | BindingFlags.Instance);
                            if (null != prop && prop.CanWrite)
                            {
                                prop.SetValue(retrunModel, JsonConvert.SerializeObject(msg), null);
                            }

                            //// set producer application id to the message model which will be received by subscriber 
                            if (msg.UserProperties.TryGetValue("paId", out object producerApplicationId))
                            {
                                prop = retrunModel.GetType().GetProperty("_ProducerApplicationId", BindingFlags.Public | BindingFlags.Instance);
                                if (null != prop && prop.CanWrite)
                                {
                                    prop.SetValue(retrunModel, producerApplicationId, null);
                                }
                            }

                            //// set event id to the message model which will be received by subscriber
                            if (msg.UserProperties.TryGetValue("eId", out object eventId))
                            {
                                prop = retrunModel.GetType().GetProperty("_EventId", BindingFlags.Public | BindingFlags.Instance);
                                if (null != prop && prop.CanWrite)
                                {
                                    prop.SetValue(retrunModel, eventId, null);
                                }
                            }

                            handler(retrunModel);
                        }
                        catch (Exception ex)
                        {
                        }
                    }, messageHandlerOptions);

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Exceptions the received handler.
        /// </summary>
        /// <param name="exceptionReceivedEventArgs">The <see cref="ExceptionReceivedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            ExceptionReceivedContext context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Setups the azure topic provider.
        /// </summary>
        /// <param name="domainManager">The domain manager.</param>
        static void SetupAzureTopicProvider(DomainManager domainManager)
        {
            if (domainManager != null && domainManager.DomainConfiguration != null)
            {
                foreach (Type data in domainManager.DomainConfiguration)
                {
                    ServiceBusDetails serviceBusDetails = GetServiceBusDetailsFromT(data);
                    if (!string.IsNullOrEmpty(serviceBusDetails.Connection))
                    {
                        if (!string.IsNullOrWhiteSpace(serviceBusDetails.TopicName))
                        {
                            ////Add topic client to _topicManager which will be used for publish
                            string topicKeyName = $"{serviceBusDetails.Endpoint}||{serviceBusDetails.TopicName}";
                            if (!_topicManager.ContainsKey(topicKeyName))
                            {
                                ////Check for topic and if not exists AzureTopicProvider will create it.
                                AzureTopicProvider azureTopicProvider = new AzureTopicProvider(serviceBusDetails.Connection, serviceBusDetails.TopicName, serviceBusDetails.Subscription);
                                azureTopicProvider.AddTopic();

                                AzureTopicPublisher topicClient = new AzureTopicPublisher(serviceBusDetails.Connection, serviceBusDetails.TopicName);
                                _topicManager.TryAdd(topicKeyName, topicClient);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Queue

        /// <summary>
        /// Sends to queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gerericEventModel">The gereric event model.</param>
        /// <returns></returns>
        public bool SendToQueue<T>(T gerericEventModel)
        {
            try
            {
                ServiceBusDetails serviceBusDetails = GetServiceBusDetailsFromT(gerericEventModel.GetType());

                if (!string.IsNullOrWhiteSpace(serviceBusDetails.Connection))
                {
                    if (_queueManager.TryGetValue($"{serviceBusDetails.Endpoint}||{serviceBusDetails.QueueName}", out AzureQueueSender queueClient))
                    {
                        Dictionary<string, object> userPropertiesDict = new Dictionary<string, object>
                        {
                            //// version wise event bus: Consider ver = EventTypeVersion which is atleast 1 (which we have settled in GetServiceBusDetailsFromT() )
                            { "ver", serviceBusDetails.EventTypeVersion},
                            { "type", 1 },
                            { "mId", Guid.NewGuid().ToString() },
                            { "pId", serviceBusDetails.ProducerId },
                            { "eId",  Convert.ToInt16(serviceBusDetails.EventId) },
                            { "ts", DateTime.UtcNow }
                        };

                        EventModel<object> eventModel = new EventModel<object>()
                        {
                            //OrignalValue = gerericEventModel,
                            Value = JsonConvert.SerializeObject(gerericEventModel).ToBase64Encode()
                        };

                        Message message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventModel).ToBase64Encode()));

                        foreach (KeyValuePair<string, object> item in userPropertiesDict)
                        {
                            message.UserProperties.Add(new KeyValuePair<string, object>(item.Key, item.Value));
                        }

                        queueClient.SendAsync(message).Wait();
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Receives from queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetModel">The target model.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        public bool ReceiveFromQueue<T>(T targetModel, Func<T, Task> handler)
        {
            try
            {
                ServiceBusDetails serviceBusDetails = GetServiceBusDetailsFromT(targetModel.GetType());

                if (!string.IsNullOrEmpty(serviceBusDetails.Connection))
                {
                    receiverClient = new AzureQueueReceiver(serviceBusDetails.Connection, serviceBusDetails.QueueName);
                    MessageHandlerOptions messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
                    {
                        MaxConcurrentCalls = 50,
                        AutoComplete = true
                    };

                    //subscriptionClient.RegisterMessageHandler(handler, messageHandlerOptions);
                    receiverClient.RegisterMessageHandler(async (msg, token) =>
                    {
                        // Get message  
                        string message = Encoding.UTF8.GetString(msg.Body).ToBase64Decode();
                        dynamic obj = JsonConvert.DeserializeObject(message);
                        dynamic retrunModel = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj.originalvalue));
                        handler(retrunModel);
                    }, messageHandlerOptions);

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Setups the azure queue provider.
        /// </summary>
        /// <param name="domainManager">The domain manager.</param>
        static void SetupAzureQueueProvider(DomainManager domainManager)
        {
            if (domainManager != null && domainManager.DomainConfiguration != null)
            {
                foreach (Type data in domainManager.DomainConfiguration)
                {
                    ServiceBusDetails serviceBusDetails = GetServiceBusDetailsFromT(data);
                    if (!string.IsNullOrEmpty(serviceBusDetails.Connection))
                    {
                        if (!string.IsNullOrWhiteSpace(serviceBusDetails.QueueName))
                        {
                            ////Add queue client to _queueManager which will be used for publish
                            string queueKeyName = $"{serviceBusDetails.Endpoint}||{serviceBusDetails.QueueName}";
                            if (!_queueManager.ContainsKey(queueKeyName))
                            {
                                ////Check for queue and if not exists AzureQueueProvider will create it.
                                AzureQueueProvider azureQueueProvider = new AzureQueueProvider(serviceBusDetails.Connection, serviceBusDetails.QueueName);
                                azureQueueProvider.AddQueue(serviceBusDetails.ServiceBusNamespace, serviceBusDetails.SharedAccessKeyName, serviceBusDetails.SharedAccessKey, serviceBusDetails.QueueName, string.Empty);

                                AzureQueueSender queueClient = new AzureQueueSender(serviceBusDetails.Connection, serviceBusDetails.QueueName);
                                _queueManager.TryAdd(queueKeyName, queueClient);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region General

        /// <summary>
        /// Gets the service bus details from t.
        /// </summary>
        /// <param name="domainType">Type of the domain.</param>
        /// <returns></returns>
        static ServiceBusDetails GetServiceBusDetailsFromT(Type domainType)
        {
            ServiceBusDetails serviceBusDetails = new ServiceBusDetails();

            ConfigureAttribute customAttribute = (ConfigureAttribute)Attribute.GetCustomAttribute(domainType, typeof(ConfigureAttribute));

            if (customAttribute != null && !string.IsNullOrEmpty(customAttribute.Connection))
            {
                if (_eventBusConfiguration != null)
                {
                    _eventBusConfiguration.TryGet(customAttribute.Connection, out string serviceBusConnection);
                    if (!string.IsNullOrEmpty(serviceBusConnection))
                    {
                        serviceBusDetails.Connection = serviceBusConnection;
                        serviceBusDetails.TopicName = customAttribute.TopicName;
                        serviceBusDetails.EventId = customAttribute.EventId;
                        //serviceBusDetails.ProducerId = (Int16)_domainManager.ServiceType;

                       //serviceBusDetails.ProducerApplicationId = (Int16)_domainManager.ApplicationType;

                        Dictionary<string, string> dict = serviceBusConnection.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(part => part.Split(new[] { '=' }, 2))
                                    .ToDictionary(split => split[0], split => split[1]);

                        serviceBusDetails.ServiceBusNamespace = dict["Endpoint"].Split("//")[1].Split('.')[0];
                        serviceBusDetails.SharedAccessKeyName = dict["SharedAccessKeyName"];
                        serviceBusDetails.SharedAccessKey = dict["SharedAccessKey"];
                        serviceBusDetails.Endpoint = dict["Endpoint"].Split("//")[1].Split('/')[0];

                        //// version wise event bus: if EventTypeVersion  == 0 then consider version = 1 else set subscription name based on version defined in ConfigureAttribute
                        serviceBusDetails.EventTypeVersion = customAttribute.EventTypeVersion;
                        if (serviceBusDetails.EventTypeVersion == 0)
                            serviceBusDetails.EventTypeVersion = 1;
                        //serviceBusDetails.Subscription = $"{_domainManager.ServiceType.ToString().ToLower()}_{customAttribute.EventId.ToString()}_v{customAttribute.EventTypeVersion.ToString()}";
                        serviceBusDetails.Subscription = "solution-logs-subs-qa";

                        serviceBusDetails.QueueName = customAttribute.QueueName;
                    }
                }
            }

            return serviceBusDetails;
        }

        #endregion       

    }
}
