using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Spi;

namespace ADCore.Quartz.Schedule
{
    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;

        private readonly SchedulerConfig _schedulerConfig;

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IOptions<SchedulerConfig> schedulerConfigOptions)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _schedulerConfig = schedulerConfigOptions.Value;
        }

        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;
            foreach (var jobSchedule in _schedulerConfig.AdJobs)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);
                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
                Console.WriteLine($"Job {jobSchedule.Name} going to start at {DateTimeOffset.UtcNow.Add(jobSchedule.StartAfter)}");
            }

            Console.WriteLine("----------------------------");
            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
        }

        private static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = Type.GetType(schedule.Name);
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.Name)
                .Build();
        }

        private static ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.Name}.trigger")
                .StartAt(DateTimeOffset.UtcNow.Add(schedule.StartAfter))
                .WithSimpleSchedule(x => x
                    .WithInterval(schedule.Interval)
                    .RepeatForever()
                )
                .Build();
        }


        // private IEnumerable<JobSchedule> GetJobs()
        // {
        //     var jobs = Assembly.Load(new AssemblyName("Ad.Micro.Quartz")).GetTypes()
        //         .Where(t => t.IsClass && t.CustomAttributes.Any(c=>c.AttributeType.FullName.Contains("AdJobConfigAttribute")))
        //         .ToList();
        //     return null;
        // }
    }
}