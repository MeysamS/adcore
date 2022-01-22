using System.Threading.Tasks;
using ADCore.Kafka.Messaging.Handler;
using Quartz;

namespace ADCore.Kafka.Jobs
{
    /// <summary>
    /// Keep this per-execution-context.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    [DisallowConcurrentExecution]
    public class MessageConsumerJob<THandler, TMessage> : IJob
        where THandler : IMessageHandler<TMessage>
    {
        private readonly IMessageHandlerManager<THandler, TMessage> _manager;
        public MessageConsumerJob(IMessageHandlerManager<THandler, TMessage> manager)
        {
            _manager = manager;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _manager.ManageAsync();
        }
    }
}
