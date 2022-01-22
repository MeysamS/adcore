using System;
using System.Linq;
using System.Reflection;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Scheduling;
using ADCore.Kafka.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace ADCore.Kafka.Extensions
{
    public static class WorkerServiceExtensions
    {
        public static IServiceCollection AddHostedWorker(
            this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddOptions();

            services.AddSingleton<IJobFactory, ServiceProviderBasedJobFactory>();


            services.AddSingleton<IWorkerScheduler, WorkerScheduler>();

            foreach (var type in assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => typeof(IJob).IsAssignableFrom(t))
                .Where(t => t.GetCustomAttribute<WorkerAttribute>() != null))
            {
                services.AddTransient(type);
                foreach (var ifs in type.GetInterfaces()) services.AddTransient(ifs, type);
            }

            services.AddSingleton<IScheduler>(x => {
                var config = x.GetRequiredService<IOptions<WorkerRootOptions>>().Value;

                var schedulerFactory = new StdSchedulerFactory();
                var props = config.ToProperties();
                if (props.Count > 0) schedulerFactory.Initialize(props);

                var scheduler = schedulerFactory.GetScheduler()
                    .ConfigureAwait(false)
                    .GetAwaiter().GetResult();

                scheduler.JobFactory = x.GetRequiredService<IJobFactory>();

                return scheduler;
            });

            services.AddHostedService<WorkerHostedService>();

            return services;
        }

    }
}
