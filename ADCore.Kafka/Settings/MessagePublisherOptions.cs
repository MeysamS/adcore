using System;
using Confluent.Kafka;

namespace ADCore.Kafka.Settings
{
    public class MessagePublisherOptions
    {
        public string BootstrapServers { get; set; }
        public TimeSpan FlushWaitTime { get; set; }
        public bool EnableDeliveryReports { get; set; }
        // retry settings:
        // Receive acknowledgement from all sync replicas
        public Acks Acks { get; set; }
        // Number of times to retry before giving up
        public int MessageSendMaxRetries { get; set; }
        // Duration to retry before next attempt
        public int RetryBackoffMs { get; set; }
        // Set to true if you don't want to reorder messages on retry
        public bool EnableIdempotence { get; set; }
        public string FallBackUrl { get; set; }
        
    }
}
