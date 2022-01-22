using System;
using Utf8Json;

namespace ADCore.Kafka.Formatters.Utf8Json {

    public sealed class UnixDateTimeFormatter : IJsonFormatter<DateTime> {
        public void Serialize(ref JsonWriter writer, DateTime value, IJsonFormatterResolver formatterResolver) {
            var dto = new DateTimeOffset(value.ToUniversalTime(), TimeSpan.Zero);
            var uts = dto.ToUnixTimeMilliseconds();
            writer.WriteInt64(uts);
        }

        public DateTime Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) {
            var uts = reader.ReadInt64();
            var dt  = DateTimeOffset.FromUnixTimeMilliseconds(uts).UtcDateTime;
            return dt;
        }

    }
    public sealed class UnixDateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset>
    {

        public void Serialize(ref JsonWriter writer, DateTimeOffset value, IJsonFormatterResolver formatterResolver)
        {
            var uts = value.ToUnixTimeMilliseconds();
            writer.WriteInt64(uts);
        }

        public DateTimeOffset Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var uts = reader.ReadInt64();
            var dt = DateTimeOffset.FromUnixTimeMilliseconds(uts);

            return dt;
        }

    }

    public sealed class UnixNullableDateTimeFormatter : IJsonFormatter<DateTime?>
    {

        private readonly UnixDateTimeFormatter _innerFormatter;

        public UnixNullableDateTimeFormatter()
        {
            _innerFormatter = new UnixDateTimeFormatter();
        }

        public void Serialize(ref JsonWriter writer, DateTime? value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();

                return;
            }

            _innerFormatter.Serialize(ref writer, value.Value, formatterResolver);
        }

        public DateTime? Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) return null;

            return _innerFormatter.Deserialize(ref reader, formatterResolver);
        }

    }

    public sealed class UnixNullableDateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset?>
    {

        private readonly UnixDateTimeOffsetFormatter _innerFormatter;

        public UnixNullableDateTimeOffsetFormatter()
        {
            _innerFormatter = new UnixDateTimeOffsetFormatter();
        }

        public void Serialize(ref JsonWriter writer, DateTimeOffset? value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();

                return;
            }

            _innerFormatter.Serialize(ref writer, value.Value, formatterResolver);
        }

        public DateTimeOffset? Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) return null;

            return _innerFormatter.Deserialize(ref reader, formatterResolver);
        }

    }

}