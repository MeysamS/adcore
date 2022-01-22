using System;
using System.Globalization;
using System.Linq;
using ADCore.Kafka.Exceptions;
using ADCore.Kafka.Settings;
using Newtonsoft.Json;

namespace ADCore.Kafka.Formatters.JsonNet
{
    public sealed class JsonNetNumberConverter : JsonConverter
    {

        private static readonly Type[] SupportedTypes = {
            typeof(decimal),
            typeof(float),
            typeof(double),
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
        };

        public string DecimalFormatString { get; set; } = FormatterOptions.DefaultDecimalFormatString;
        public string FloatFormatString { get; set; } = FormatterOptions.DefaultFloatFormatString;
        public string DoubleFormatString { get; set; } = FormatterOptions.DefaultDoubleFormatString;

        public override bool CanConvert(Type objectType) => SupportedTypes.Contains(objectType);

        public override object ReadJson(
            JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var value = reader.Value?.ToString();
            //if (!string.IsNullOrWhiteSpace(value))
            //    value = Normalizer.DigitsToLatin(value);

            if (objectType == typeof(decimal)) return string.IsNullOrWhiteSpace(value) ? default : Convert.ToDecimal(value);
            if (objectType == typeof(float)) return string.IsNullOrWhiteSpace(value) ? default : Convert.ToSingle(value);
            if (objectType == typeof(double)) return string.IsNullOrWhiteSpace(value) ? default : Convert.ToDouble(value);

            if (objectType == typeof(byte)) return string.IsNullOrWhiteSpace(value) ? default : Convert.ToByte(value);
            if (objectType == typeof(sbyte)) return string.IsNullOrWhiteSpace(value) ? default : Convert.ToSByte(value);
            if (objectType == typeof(short)) return string.IsNullOrWhiteSpace(value) ? default : Convert.ToInt16(value);
            if (objectType == typeof(ushort)) return string.IsNullOrWhiteSpace(value) ? default : Convert.ToUInt16(value);
            if (objectType == typeof(int)) return string.IsNullOrWhiteSpace(value) ? default : Convert.ToInt32(value);
            if (objectType == typeof(uint)) return string.IsNullOrWhiteSpace(value) ? default : Convert.ToUInt32(value);

            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) return;

            var type = value.GetType();

            if (type == typeof(decimal))
                writer.WriteRawValue(((decimal)value).ToString(DecimalFormatString, CultureInfo.InvariantCulture));
            else if (type == typeof(float))
                writer.WriteRawValue(((float)value).ToString(FloatFormatString, CultureInfo.InvariantCulture));
            else if (type == typeof(double))
                writer.WriteRawValue(((double)value).ToString(DoubleFormatString, CultureInfo.InvariantCulture));

            else if (type == typeof(byte))
                writer.WriteRawValue(((byte)value).ToString(CultureInfo.InvariantCulture));
            else if (type == typeof(sbyte))
                writer.WriteRawValue(((sbyte)value).ToString(CultureInfo.InvariantCulture));
            else if (type == typeof(short))
                writer.WriteRawValue(((short)value).ToString(CultureInfo.InvariantCulture));
            else if (type == typeof(ushort))
                writer.WriteRawValue(((ushort)value).ToString(CultureInfo.InvariantCulture));
            else if (type == typeof(int))
                writer.WriteRawValue(((int)value).ToString(CultureInfo.InvariantCulture));
            else if (type == typeof(uint))
                writer.WriteRawValue(((uint)value).ToString(CultureInfo.InvariantCulture));
        }

    }
    public class JsonNetLongStringConverter : JsonConverter<long>
    {

        public override void WriteJson(JsonWriter writer, long value, JsonSerializer serializer)
        {
            if (OutputLongAsString)
                writer.WriteValue(value.ToString(CultureInfo.InvariantCulture));
            else
                writer.WriteValue(value);
        }

        public override long ReadJson(
            JsonReader reader, Type objectType, long existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(long)) return (long)reader.Value;

            if (AcceptInputLongString && reader.ValueType == typeof(string))
            {
                var value = reader.Value?.ToString();
                if (string.IsNullOrWhiteSpace(value)) return default;
                value = Normalizer.DigitsToLatin(value);
                if (long.TryParse(value,
                               NumberStyles.Integer,
                               NumberFormatInfo.InvariantInfo,
                               out var l))
                    return l;
            }

            var message =
                reader is JsonTextReader r
                    ? $"Cannot convert `{r.ValueType}` to `{typeof(long)}`. Line number: `{r.LineNumber}`, Position: `{r.LinePosition}`, Path: `{r.Path}`, Value: `{r.Value}`"
                    : $"Cannot convert `{reader.ValueType}` to `{typeof(long)}`. Path: `{reader.Path}`, Value: `{reader.Value}`";

            throw new TypeConvertionException(message);
        }

        public bool OutputLongAsString { get; set; }
        public bool AcceptInputLongString { get; set; }

    }
}
