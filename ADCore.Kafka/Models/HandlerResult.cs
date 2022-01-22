namespace ADCore.Kafka.Models
{
    public class HandlerResult
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
        public static HandlerResult Failed(string errorMessage)
            => new HandlerResult { ErrorMessage = errorMessage, Succeeded = false };
        public static HandlerResult Success() => new HandlerResult { Succeeded = true };
    }
}
