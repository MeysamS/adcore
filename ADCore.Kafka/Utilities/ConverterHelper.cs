using System;
using System.ComponentModel;

namespace ADCore.Kafka.Utilities
{
    public class ConverterHelper
    {
        public static object ConvertFromString(Type targetType, string value)
        {
            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                return ConvertToDateTime(value);

            if (targetType == typeof(DateTimeOffset) || targetType == typeof(DateTimeOffset?))
                return ConvertToDateTimeOffset(value);

            var converter = TypeDescriptor.GetConverter(targetType);

            if (converter.CanConvertFrom(typeof(string)))
            {
                var propertyValue = converter.ConvertFromInvariantString(value);
                return propertyValue;
            }

            return null;
        }

        private static object ConvertToDateTime(string value) => throw new NotSupportedException();

        private static object ConvertToDateTimeOffset(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new NotSupportedException();
            return Pdate.GetUtcFromLocalString(value);
        }

    }
}
