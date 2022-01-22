using System;
using System.Threading.Tasks;

namespace ADCore.Kafka.Messaging.Publisher
{
    public interface IPublishableMessage { }
    public interface IPublishableMessage<out TTargetMessage> : IPublishableMessage
    {
        TTargetMessage MapTo();
    }

    public interface IMessagePublisher : IDisposable
    {
        Task PublishAsync<TPublishableMessage>(TPublishableMessage message)
            where TPublishableMessage : IPublishableMessage;
    }
}
