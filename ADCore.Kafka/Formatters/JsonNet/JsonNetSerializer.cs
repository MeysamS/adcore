using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.IO;
using Newtonsoft.Json;

namespace ADCore.Kafka.Formatters.JsonNet
{
    public class JsonNetSerializer : ISerializer
    {

        private readonly JsonSerializerSettings _settings;
        private readonly JsonSerializer _serializer;
        private readonly RecyclableMemoryStreamManager _streamManager;

        public JsonNetSerializer(JsonSerializerSettings settings)
        {
            _settings = settings;
            _serializer = JsonSerializer.Create(settings);
            _streamManager = new RecyclableMemoryStreamManager();
        }

        public string Serialize<T>(T value) => JsonConvert.SerializeObject(value, _settings);
        public Task<string> SerializeAsync<T>(T value)
            => Task.Factory.StartNew(() => JsonConvert.SerializeObject(value, _settings));

        public string Serialize(object value) => JsonConvert.SerializeObject(value, _settings);
        public Task<string> SerializeAsync(object value)
            => Task.Factory.StartNew(() => JsonConvert.SerializeObject(value, _settings));

        public string Serialize(object value, Type type) => JsonConvert.SerializeObject(value, type, _settings);

        public void Serialize<T>(T value, Stream stream)
            => SerializeAsync(value, stream, value?.GetType() ?? typeof(T))
              .ConfigureAwait(false).GetAwaiter().GetResult();

        public Task SerializeAsync<T>(T value, Stream stream)
            => SerializeAsync(value, stream, value?.GetType() ?? typeof(T));

        public void Serialize(object value, Stream stream)
            => SerializeAsync(value, stream, value?.GetType() ?? typeof(object))
              .ConfigureAwait(false).GetAwaiter().GetResult();

        public Task SerializeAsync(object value, Stream stream)
            => SerializeAsync(value, stream, value?.GetType() ?? typeof(object));

        public void Serialize(object value, Stream stream, Type type)
            => SerializeAsync(value, stream, type).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task SerializeAsync(object value, Stream stream, Type type)
        {
            await using (var memoryStream = _streamManager.GetStream())
            {
                await using var writer = new StreamWriter(memoryStream);
                _serializer.Serialize(writer, value, type);
                await writer.FlushAsync().ConfigureAwait(false);
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(stream).ConfigureAwait(false);
            }
            await stream.FlushAsync().ConfigureAwait(false);
        }

        public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _settings);
        public Task<T> DeserializeAsync<T>(string json)
            => Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json, _settings));

        public object Deserialize(string json, Type type) => JsonConvert.DeserializeObject(json, type, _settings);

        public Task<object> DeserializeAsync(string json, Type type)
            => Task.Factory.StartNew(() => JsonConvert.DeserializeObject(json, type, _settings));

        public T Deserialize<T>(Stream stream)
        {
            using var streamReader = new StreamReader(stream);

            using JsonReader reader = new JsonTextReader(streamReader);

            return _serializer.Deserialize<T>(reader);
        }

        public Task<T> DeserializeAsync<T>(Stream stream)
            => Task.Factory.StartNew(() => Deserialize<T>(stream));

        public object Deserialize(Stream stream, Type type)
        {
            using var reader = new StreamReader(stream);

            stream.Seek(0, SeekOrigin.Begin);

            return _serializer.Deserialize(reader, type);
        }

        public Task<object> DeserializeAsync(Stream stream, Type type)
            => Task.Factory.StartNew(() => Deserialize(stream, type));

    }
}
