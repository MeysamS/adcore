using System;

namespace ADCore.Kafka.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SubscribableMessageAttribute : Attribute
    {
        public SubscribableMessageAttribute(string topic, string postFixProperty = null, string seperatorChar = null)
        {
            Topic = topic;
            PostFixProperty = postFixProperty;
            SeperatorChar = seperatorChar;
        }
        public string Topic { get; }
        public string PostFixProperty { get; set; }
        public string SeperatorChar { get; set; }
    }

}
