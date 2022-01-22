using System;
using ADCore.Kafka.Utilities;
using Newtonsoft.Json;

namespace ADCore.Kafka.Formatters.JsonNet
{

    public sealed class JsonNetUnixDateTimeFormatter : JsonConverter<DateTime>
    {
        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            var dto = new DateTimeOffset(value.ToUniversalTime(), TimeSpan.Zero);
            var uts = dto.ToUnixTimeMilliseconds();
            writer.WriteValue(uts);
        }

        public override DateTime ReadJson(
            JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var uts = Convert.ToInt64(reader.Value);
            var dt = DateTimeOffset.FromUnixTimeMilliseconds(uts).UtcDateTime;

            return dt;
        }

    }

    public sealed class JsonNetUnixDateTimeOffsetFormatter : JsonConverter<DateTimeOffset>
    {
        public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
        {
            var uts = value.ToUnixTimeMilliseconds();
            writer.WriteValue(uts);
        }

        public override DateTimeOffset ReadJson(
            JsonReader reader, Type objectType, DateTimeOffset existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var v = reader?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(v))
                return default;

            //return dt;

            //var v = reader?.Value?.ToString();
            //if (string.IsNullOrWhiteSpace(v))
            //    return default;

            var value = Normalizer.DigitsToLatin(v);
            if (long.TryParse(value, out long uts))
            {
                var dt = DateTimeOffset.FromUnixTimeMilliseconds(uts).UtcDateTime;
                return dt;
            }

            var dto = (DateTimeOffset)ConverterHelper.ConvertFromString(objectType, value);
            return dto;
        }

    }

    public sealed class JsonNetUnixNullableDateTimeFormatter : JsonConverter<DateTime?>
    {

        private readonly JsonNetUnixDateTimeFormatter _innerFormatter;

        public JsonNetUnixNullableDateTimeFormatter()
        {
            _innerFormatter = new JsonNetUnixDateTimeFormatter();
        }

        public override void WriteJson(JsonWriter writer, DateTime? value, JsonSerializer serializer)
        {

            if (value == null)
            {
                writer.WriteNull();

                return;
            }

            _innerFormatter.WriteJson(writer, value.Value, serializer);
        }

        public override DateTime? ReadJson(
            JsonReader reader, Type objectType, DateTime? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.Value == null) return null;

            return _innerFormatter.ReadJson(reader, objectType, existingValue.GetValueOrDefault(), hasExistingValue,
                                            serializer);
        }

    }

    public sealed class JsonNetUnixNullableDateTimeOffsetFormatter : JsonConverter<DateTimeOffset?>
    {

        private readonly JsonNetUnixDateTimeOffsetFormatter _innerFormatter;

        public JsonNetUnixNullableDateTimeOffsetFormatter()
        {
            _innerFormatter = new JsonNetUnixDateTimeOffsetFormatter();
        }

        public override void WriteJson(JsonWriter writer, DateTimeOffset? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();

                return;
            }

            _innerFormatter.WriteJson(writer, value.Value, serializer);
        }

        public override DateTimeOffset? ReadJson(
            JsonReader reader, Type objectType, DateTimeOffset? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.Value == null) return null;

            return _innerFormatter.ReadJson(reader, objectType, existingValue.GetValueOrDefault(), hasExistingValue,
                                            serializer);
        }

    }
}
