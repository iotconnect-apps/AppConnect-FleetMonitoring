using component.messaging.Constants;
using component.messaging.Database;
using component.messaging.Model;
using component.messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace component.messaging.Messages
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly ITopicSubscriber _azureTopicSubscriber;

        public MessageHandler(IDatabaseManager databaseManager, ITopicSubscriber azureTopicSubscriber)
        {
            _databaseManager = databaseManager;
            _azureTopicSubscriber = azureTopicSubscriber;
        }

        public void InitializeSubscribers()
        {
            _azureTopicSubscriber.ReceiveMessage(MessageHanler);
        }

        private Task MessageHanler(Message message, CancellationToken cancellationToken)
        {
            var baseMessage = _azureTopicSubscriber.DecodeMessage<DeviceConnectMessageModel>(message);

            switch (baseMessage.MsgType)
            {
                case ServiceBusConstants.CompanyTopic:
                    _databaseManager.CompanyProcessMessage(baseMessage);                   
                    break;

                case ServiceBusConstants.UserTopic:
                    _databaseManager.UserProcessMessage(baseMessage);
                    break;

                case ServiceBusConstants.DeviceTopic:
                    _databaseManager.DeviceProcessMessage(baseMessage);
                    break;

                case ServiceBusConstants.EntityTopic:
                    _databaseManager.EntityProcessMessage(baseMessage);
                    break;

                case ServiceBusConstants.RoleTopic:
                    _databaseManager.RoleProcessMessage(baseMessage);
                    break;

                case ServiceBusConstants.Template:
                    _databaseManager.TemplateProcessMessage(baseMessage);
                    break;

                default:
                    throw new NotImplementedException(nameof(baseMessage.MsgType));
            }

            return Task.CompletedTask;
        }
    }
}
