using Microsoft.Azure.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace component.messaging.ServiceBus
{
    public interface ITopicSubscriber
    {
        void ReceiveMessage(Func<Message, CancellationToken, Task> callBackFunc);

        T DecodeMessage<T>(Message message);
    }
}
