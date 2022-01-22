using System;

namespace ADCore.Kafka.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class WorkerAttribute : Attribute
    {
        public Type DataProvider { get; set; }
    }
}
