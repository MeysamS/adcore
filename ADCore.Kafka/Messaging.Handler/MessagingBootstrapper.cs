using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADCore.Kafka.Extensions;
using ADCore.Kafka.Scheduling;
using ADCore.Kafka.Settings;
using Quartz.Util;

namespace ADCore.Kafka.Messaging.Handler
{
    public class MessagingBootstrapper : IMessagingBootstrapper
    {
        private readonly IWorkerScheduler _scheduler;

        public MessagingBootstrapper(IWorkerScheduler scheduler)
        {
            _scheduler = scheduler;
        }
        public async Task BootstrapAsync(MessageHandlerOptions handlerOptions, IDictionary<string, WorkerOptions> workerOptions)
        {
            foreach (var item in JobTypeCache.IterateAndClear())
            {
                var key = item.HandlerType.FullName;

                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException();

                WorkerOptions workOption = null;

                var messageOption = handlerOptions.Messages.TryGetAndReturn(key);

                if (messageOption != null)
                {
                    if (!string.IsNullOrWhiteSpace(messageOption.WorkerKey))
                    {
                        workOption = messageOption.Worker;
                        var keyedWorkOption = workerOptions[messageOption.WorkerKey];
                        workOption.OverrideBy(keyedWorkOption);
                    }
                    else
                    {
                        workOption = messageOption.Worker;
                    }
                }

                workOption ??= workerOptions.Select(t => t.Value).FirstOrDefault()
                               ?? throw new InvalidOperationException();

                await _scheduler.ScheduleAsync(key, item.JobType, workOption);
            }
        }
    }
}
