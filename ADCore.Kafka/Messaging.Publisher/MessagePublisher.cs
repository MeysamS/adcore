using System;
using System.Net.Http;
using System.Threading.Tasks;
using AD.Micro.FallBack;
using ADCore.Kafka.Formatters;
using ADCore.Kafka.Settings;
using Confluent.Kafka;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;

namespace ADCore.Kafka.Messaging.Publisher
{
    public sealed class MessagePublisher : IMessagePublisher
    {
        private readonly IProducer<Null, string> _producer;
        private readonly MessagePublisherOptions _publisherOptions;
        private bool _isDisposed;

        private MessagePublisher(IOptions<MessagingOptions> options)
        {
            _publisherOptions = options.Value.PublisherOptions;
            _producer = new ProducerBuilder<Null, string>(new ProducerConfig
            {
                BootstrapServers = _publisherOptions.BootstrapServers,
            }).Build();
        }

        #region [Singleton-Factory]

        private static MessagePublisher? _instance;
        private static readonly object _lock = new object();
        public static MessagePublisher CreateInstance(IOptions<MessagingOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException($"Cannot resolve {typeof(IOptions<MessagingOptions>)} and it cannot be null.");
            if (_instance != null)
                return _instance;
            lock (_lock)
            {
                _instance ??= new MessagePublisher(options);
            }
            return _instance;
        }

        #endregion [Singleton-Factory]

        #region [Sending-Messages]

        public async Task PublishAsync<TPublishableMessage>(TPublishableMessage message)
            where TPublishableMessage : IPublishableMessage
        {
            var maps = PublishableMessageMapper<TPublishableMessage>.Map;
            foreach (var map in maps)
            {
                var targetObject = map.MapperFunc.Invoke(message);
                var targetJson = JsonSerDes.Serialize(targetObject);
                var topic = map.TopicFunc.Invoke(message);
                try
                {
                    // Produce to kafka
                    await _producer.ProduceAsync(topic, new Message<Null, string> { Value = targetJson });
                }
                catch (Exception ex)
                {
                    var httpClientHandler = new HttpClientHandler();
                    // Return `true` to allow certificates that are untrusted/invalid
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;

                    var httpClient = new HttpClient(httpClientHandler);

                    var channel = GrpcChannel.ForAddress(_publisherOptions.FallBackUrl, new GrpcChannelOptions { HttpClient = httpClient });
                    var client = new StreamerFallBack.StreamerFallBackClient(channel);
                    var request = new StreamerFallBackRequest
                    {
                        ErrorMessage = ex.Message,
                        Message = targetJson,
                        StackTrace = ex.StackTrace,
                        Topic = topic
                    };
                    var result = await client.SendFallbackAsync(request);
                }
            }
        }
        #endregion  [Sending-Messages]
       
        public void Dispose()
        {
            if (_isDisposed)
                return;
            // wait for up to _options.FlushWaitTime (for example 10 seconds) for any inflight messages to be delivered.
            _producer.Flush(_publisherOptions.FlushWaitTime);
            _producer.Dispose();
            _isDisposed = true;
        }
    }
}
