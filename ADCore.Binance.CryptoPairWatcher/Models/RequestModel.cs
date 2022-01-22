using ADCore.Kafka.Attributes;
using ADCore.Kafka.Messaging.Publisher;
using System.Collections.Generic;

namespace ADCore.Binance.CryptoPairWatcher.Models
{
    [SubscribableMessage("RequestTopic")]
    public class RequestModel : IPublishableMessage<RequestModel>
    {
        public string TopicName { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> HeaderStrings { get; set; }

        public RequestModel MapTo()
        {
            return this;
        }
    }

}
