using System;

namespace ADCore.Kafka.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SubscribableMessageHandlerAttribute : Attribute
    {

        public SubscribableMessageHandlerAttribute(string @group, string @handlerTopic = null)
        {
            Group = @group;
            HandlerTopic = @handlerTopic;
        }

        public string Group { get; }
        public string HandlerTopic { get; set; }

    }
}
