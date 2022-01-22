using ADCore.Kafka.Attributes;
using ADCore.Kafka.Messaging.Handler;
using ADCore.Kafka.Settings;
using Microsoft.Extensions.Options;
using Quartz;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADCore.Binance.CryptoListWatcher.Jobs
{
    [Worker, DisallowConcurrentExecution]
    public class MessagingBootstrapperJob : IJob
    {
        private readonly IMessagingBootstrapper _bootstrapper;
        private readonly MessageHandlerOptions _handlerOptions;
        private readonly IDictionary<string, WorkerOptions> _workOptions;

        public static bool Enabled { get; set; }

        public MessagingBootstrapperJob(
            IMessagingBootstrapper bootstrapper,
            IOptions<MessagingOptions> options
            )
        {
            _bootstrapper = bootstrapper;
            _workOptions = options.Value.WorkerOptions;
            _handlerOptions = options.Value.HandlerOptions;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _bootstrapper.BootstrapAsync(_handlerOptions, _workOptions);
        }
    }
}