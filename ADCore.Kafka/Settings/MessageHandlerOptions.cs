using System;
using System.Collections.Generic;
using Confluent.Kafka;

namespace ADCore.Kafka.Settings
{
    public class MessageHandlerOptions
    {
        public string BootstrapServers { get; set; }
        public bool EnableAutoCommit => false;
        // Read messages from start if no commit exists.
        public AutoOffsetReset AutoOffsetReset { get; set; }
        public bool EnableAutoOffsetStore => false;
        public TimeSpan MaxPollInterval { get; set; }
        public IDictionary<string, MessageOptions> Messages { get; set; }
    }
}
