using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Scheduling;
using ADCore.Kafka.Settings;
using Quartz;

namespace ADCore.Kafka.Extensions
{
    public static class WorkerSchedulerExtensions
    {

        public static async Task ScheduleAssemblyWorksAsync(this IWorkerScheduler scheduler,
            IServiceProvider serviceProvider,
            WorkerRootOptions rootOptions,
            CancellationToken ct,
            params Assembly[] assemblies)
        {

            if (assemblies.Length == 0) return;

            var typeAttrs = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(IJob).IsAssignableFrom(t))
                .Select(t => (type: t, attr: t.GetCustomAttribute<WorkerAttribute>()))
                .Where(t => t.attr != null)
                .ToList();

            foreach (var (type, attr) in typeAttrs)
            {

                var key = type.FullName;

                if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException();

                if (!rootOptions.Workers.ContainsKey(key))
                    throw new ArgumentException($"A job configuration with key {key} cannot be found.");

                var setting = rootOptions.Workers[key];

                if (!setting.IsEnabled) continue;

                IDictionary<string, object> dataMap = null;

                if (attr?.DataProvider != null)
                {
                    if (!(serviceProvider.GetService(attr.DataProvider) is IWorkerDataProvider dp))
                        throw new InvalidOperationException();

                    dataMap = dp.Provide(setting);
                }

                await scheduler.ScheduleAsync(key, type, setting, dataMap, ct);
            }
        }

    }
}
