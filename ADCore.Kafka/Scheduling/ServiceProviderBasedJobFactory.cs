using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace ADCore.Kafka.Scheduling
{
    public class ServiceProviderBasedJobFactory : IJobFactory
    {

        private readonly IServiceProvider _container;

        private static readonly ConcurrentDictionary<IJob, IServiceScope>
            JobScopes = new ConcurrentDictionary<IJob, IServiceScope>();

        public ServiceProviderBasedJobFactory(IServiceProvider container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var scope = _container.CreateScope();

            var job = (IJob)scope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType);
            JobScopes.TryAdd(job, scope);

            return job;
        }

        public void ReturnJob(IJob job)
        {
            if (!JobScopes.TryRemove(job, out var scope)) return;

            try
            {
                scope.Dispose();
            }
            catch (Exception ex)
            {
                // Logger.Default.LogError(ex);
            }
        }

    }
}
