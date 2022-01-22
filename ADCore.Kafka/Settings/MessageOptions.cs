namespace ADCore.Kafka.Settings
{
    public class MessageOptions
    {
        public string WorkerKey { get; set; }
        public WorkerOptions Worker { get; set; }
    }
}
