using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Formatters;
using ADCore.Kafka.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace ADCore.Kafka.Messaging.Handler
{
    /// <summary>
    /// Keep this per-scope.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class MessageHandlerManager<THandler, TMessage> : IMessageHandlerManager<THandler, TMessage>
        where THandler : IMessageHandler<TMessage>
    {
        private readonly IUnderlyingMessageConsumer<THandler, TMessage> _underlyingConsumer;
        private readonly IMessageHandler<TMessage> _messageHandler;

        public MessageHandlerManager(
            IUnderlyingMessageConsumer<THandler, TMessage> underlyingConsumer,
            IMessageHandler<TMessage> messageHandler)
        {
            _underlyingConsumer = underlyingConsumer;
            _messageHandler = messageHandler;
        }

        public async Task ManageAsync()
        {
            try
            {
                var result = await _underlyingConsumer.ConsumeAsync();
                if (string.IsNullOrWhiteSpace(result.Message.Value)) return;
                var message = JsonSerDes.Deserialize<TMessage>(result.Message.Value);
                var consumed = await _messageHandler.HandleAsync(message);
                if (consumed.Succeeded)
                    await _underlyingConsumer.CommitAsync();
            }
            catch (Exception e)
            {
                //todo: log
            }
        }

    }

    /// <summary>
    /// Keep this singleton-per-message
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="THandler"></typeparam>
    public class UnderlyingMessageConsumer<THandler, TMessage> : IUnderlyingMessageConsumer<THandler, TMessage>
    {
        private readonly IConsumer<Ignore, string> _consumer;
        public UnderlyingMessageConsumer(IOptions<MessagingOptions> options)
        {
            var handlerOptions = options.Value.HandlerOptions;
            var key = typeof(TMessage).FullName;

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var messageOptions = handlerOptions.Messages[key];
            var config = new ConsumerConfig
            {
                EnableAutoCommit = handlerOptions.EnableAutoCommit,
                AutoOffsetReset = handlerOptions.AutoOffsetReset
            };
            if (!string.IsNullOrWhiteSpace(handlerOptions.BootstrapServers))
                config.BootstrapServers = handlerOptions.BootstrapServers;

            var handlerAttr = typeof(THandler).GetCustomAttribute<SubscribableMessageHandlerAttribute>();
            var groupId = handlerAttr?.Group;
            if (string.IsNullOrWhiteSpace(groupId))
                throw new InvalidOperationException();
            config.GroupId = groupId;

            // if (handlerOptions.AutoOffsetReset != default) config.AutoOffsetReset = handlerOptions.AutoOffsetReset;

            if (handlerOptions.MaxPollInterval != default)
                config.MaxPollIntervalMs = (int)handlerOptions.MaxPollInterval.TotalMilliseconds;

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            var topic = !string.IsNullOrEmpty(handlerAttr.HandlerTopic) ? handlerAttr.HandlerTopic :
                typeof(TMessage).GetCustomAttribute<SubscribableMessageAttribute>()?.Topic;
            if (string.IsNullOrWhiteSpace(groupId))
                throw new InvalidOperationException();
            _consumer.Subscribe(topic);
        }
      
        public Task<ConsumeResult<Ignore, string>> ConsumeAsync()
        {
            var result = _consumer.Consume();
            return Task.FromResult(result);
        }

        public Task<List<TopicPartitionOffset>> CommitAsync()
        {
            var result = _consumer.Commit();
            return Task.FromResult(result);
        }

        public void Commit(ConsumeResult<Ignore, string> cr)
        {
            _consumer.Commit(cr); 
        }

        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }
}
