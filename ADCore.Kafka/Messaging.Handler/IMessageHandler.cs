using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADCore.Kafka.Models;
using Confluent.Kafka;

namespace ADCore.Kafka.Messaging.Handler
{
    /// <summary>
    /// Keep this per-scope
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageHandler<in TMessage>
    {
        Task<HandlerResult> HandleAsync(TMessage message);
    }

    public interface IUnderlyingMessageConsumer<THandler, TMessage> : IDisposable
    {
        Task<ConsumeResult<Ignore, string>> ConsumeAsync();
        Task<List<TopicPartitionOffset>> CommitAsync();
        void Commit(ConsumeResult<Ignore, string> cr);
    }
}
