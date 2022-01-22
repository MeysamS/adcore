using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ADCore.Kafka.Settings;

namespace ADCore.Kafka.Scheduling
{
    public interface IWorkerScheduler : IDisposable
    {
        Task StartAsync(CancellationToken ct);
        Task StopAsync(CancellationToken ct);
        Task ScheduleAsync(string name, Type jobType, WorkerOptions option,
            IDictionary<string, object> dataMap = null, CancellationToken ct = default);
    }
}
