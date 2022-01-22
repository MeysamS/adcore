using System;

namespace ADCore.Kafka.Messaging.Publisher
{
    public class PublishableMessageMap<TPublishableMessage> where TPublishableMessage : IPublishableMessage
    {
        public Func<TPublishableMessage, object> MapperFunc { get; set; }
        public Type TargetType { get; set; }
        public Func<dynamic, string> TopicFunc { get; set; }
    }
}
