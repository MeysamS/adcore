using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ADCore.Kafka.Extensions;
using ADCore.Kafka.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ADCore.Kafka.Scheduling
{
    public class WorkerHostedService : IHostedService
    {

        private readonly IServiceProvider _provider;
        private readonly IWorkerScheduler _scheduler;
        private readonly WorkerRootOptions _rootOptions;
        private readonly ApplicationOptions _applicationOptions;

        public WorkerHostedService(
            IServiceProvider provider, IWorkerScheduler scheduler,
            IOptions<WorkerRootOptions> options,
            IOptions<ApplicationOptions> appOptions)
        {
            _provider = provider;
            _scheduler = scheduler;
            _rootOptions = options.Value;
            _applicationOptions = appOptions.Value;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            await _scheduler.StartAsync(ct);

            StackTrace stackTrace = new StackTrace();           // get call stack
            StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)
            var asemblies = new List<Assembly>();

            asemblies.Add(stackFrames[1].GetMethod().DeclaringType.Assembly);
            asemblies.Add(stackFrames[stackFrames.Length - 1].GetMethod().DeclaringType.Assembly);

            await _scheduler.ScheduleAssemblyWorksAsync(_provider, _rootOptions, ct, asemblies.ToArray());//, GetType().Assembly
        }

        public async Task StopAsync(CancellationToken ct)
        {
            await _scheduler.StopAsync(ct);
        }

    }
}
