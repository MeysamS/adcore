using System;

namespace ADCore.Kafka.Exceptions
{
    public sealed class TypeConvertionException : Exception
    {
        public TypeConvertionException(string message) : base(message) { }

    }
}
