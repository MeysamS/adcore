using System;
using System.IO;
using System.Threading.Tasks;

namespace ADCore.Kafka.Formatters
{
    public interface ISerializer
    {

        string Serialize<T>(T value);
        Task<string> SerializeAsync<T>(T value);
        Task<string> SerializeAsync(object value);
        string Serialize(object value);
        string Serialize(object value, Type type);
        void Serialize<T>(T value, Stream stream);
        Task SerializeAsync<T>(T value, Stream stream);
        void Serialize(object value, Stream stream);
        Task SerializeAsync(object value, Stream stream);
        void Serialize(object value, Stream stream, Type type);
        Task SerializeAsync(object value, Stream stream, Type type);

        T Deserialize<T>(string json);
        Task<T> DeserializeAsync<T>(string json);
        object Deserialize(string json, Type type);
        Task<object> DeserializeAsync(string json, Type type);
        T Deserialize<T>(Stream stream);
        Task<T> DeserializeAsync<T>(Stream stream);
        object Deserialize(Stream stream, Type type);
        Task<object> DeserializeAsync(Stream stream, Type type);

    }
}
