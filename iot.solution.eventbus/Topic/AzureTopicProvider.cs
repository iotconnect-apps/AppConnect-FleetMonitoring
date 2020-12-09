using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace component.eventbus.Topic
{
    /// <summary>
    /// AzureTopicProvider
    /// </summary>
    public class AzureTopicProvider : ITopicProvider
    {
        string _connectionString = "";
        string _topicName = "";
        string _topicSubscription = "";

        /// <summary>Initializes a new instance of the <see cref="AzureTopicProvider"/> class.</summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="topicName">Name of the topic.</param>
        /// <param name="topicSubscription">The topic subscription.</param>
        public AzureTopicProvider(string connectionString, string topicName, string topicSubscription)
        {
            _connectionString = connectionString;
            _topicName = topicName;
            _topicSubscription = topicSubscription;
        }

        /// <summary>
        /// Adds the topic.
        /// </summary>
        public void AddTopic()
        {
            ManagementClient client = new ManagementClient(_connectionString);
            if (!client.TopicExistsAsync(_topicName).Result)
            {
                client.CreateTopicAsync(new TopicDescription(_topicName));
            }
        }

        /// <summary>Adds the subscriber.</summary>
        /// <param name="routingKey">The routing key.</param>
        public void AddSubscriber(string routingKey)
        {
            ManagementClient client = new ManagementClient(_connectionString);
            if (!client.SubscriptionExistsAsync(_topicName, _topicSubscription).Result)
            {
                SubscriptionDescription subscriptionDescription = new SubscriptionDescription(_topicName, _topicSubscription)
                {
                    LockDuration = TimeSpan.FromMinutes(5)
                };

                if (!string.IsNullOrWhiteSpace(routingKey))
                {
                    RuleDescription ruleDescription = new RuleDescription
                    {
                        Filter = new Microsoft.Azure.ServiceBus.SqlFilter(routingKey)
                    };

                    client.CreateSubscriptionAsync(subscriptionDescription, ruleDescription);
                }
                else
                {
                    client.CreateSubscriptionAsync(subscriptionDescription);
                }
            }
        }
    }
}
