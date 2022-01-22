using System.Collections.Generic;

namespace ADCore.Kafka.Settings
{
    public class MessagingOptions
    {
        public IDictionary<string, WorkerOptions> WorkerOptions { get; set; }
        public MessageHandlerOptions HandlerOptions { get; set; }
        public MessagePublisherOptions PublisherOptions { get; set; }
    }
}
