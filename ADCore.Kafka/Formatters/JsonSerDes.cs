using ADCore.Kafka.Formatters.Utf8Json;
using Utf8Json;
using Utf8Json.ImmutableCollection;
using Utf8Json.Resolvers;

namespace ADCore.Kafka.Formatters
{
    public static class JsonSerDes
    {
        public static readonly IJsonFormatterResolver FormatterResolver =
            CompositeResolver.Create(new IJsonFormatter[] {
                // add custome formatters, use other DateTime format.
                // if target type is struct, requires add nullable formatter too(use NullableXxxFormatter or StaticNullableFormatter(innerFormatter))
                new UnixDateTimeFormatter(),
                new UnixDateTimeOffsetFormatter(),
                new UnixNullableDateTimeFormatter(),
                new UnixNullableDateTimeOffsetFormatter(),
            }, new [] {
                // resolver custom types first
                ImmutableCollectionResolver.Instance, 
                EnumResolver.UnderlyingValue,
                // finaly choose standard resolver
                StandardResolver.ExcludeNull
        });

        public static T DeserializeByte<T>(byte[] buffer) => JsonSerializer.Deserialize<T>(buffer, FormatterResolver);

        public static byte[] SerializeByte<T>(T value) => JsonSerializer.Serialize(value, FormatterResolver);

        public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, FormatterResolver);

        public static string Serialize<T>(T value) => JsonSerializer.ToJsonString(value, FormatterResolver);

    }
}
