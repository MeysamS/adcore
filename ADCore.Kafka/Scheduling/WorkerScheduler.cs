using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ADCore.Kafka.Extensions;
using ADCore.Kafka.Settings;
using Quartz;

namespace ADCore.Kafka.Scheduling
{
    public class WorkerScheduler : IWorkerScheduler
    {
        private readonly IScheduler _scheduler;
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1);
        private bool _started;

        public WorkerScheduler(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            try
            {
                await Semaphore.WaitAsync(ct);
                if (_started) return;
                _started = true;
                await _scheduler.Start(ct);
            }
            catch (Exception exception)
            {
                // TODO: log
                Trace.WriteLine(exception.Message);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        /// <summary>
        /// Stops all startup jobs. The method will wait until all current executing rounds get finished.
        /// </summary>
        public async Task StopAsync(CancellationToken ct)
        {
            try
            {
                await Semaphore.WaitAsync(ct);
                if (!_started) return;
                await _scheduler.Shutdown(true, ct);
                _started = false;
            }
            catch (Exception exception)
            {
                // TODO: log
                Trace.WriteLine(exception.Message);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task ScheduleAsync(string name, Type jobType, WorkerOptions option,
                                        IDictionary<string, object> dataMap = null, CancellationToken ct = default)
        {
            var jobDetail = JobBuilder.Create(jobType)
                                      .WithIdentity(name + "_Job")
                                      .Build();

            if (dataMap != null)
                jobDetail.JobDataMap.PutAll(dataMap);

            jobDetail.SetJobName(name);
            var triggerName = name + "_Trigger";
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerName)
                .WithSimpleSchedule(builder => {
                    builder.WithInterval(option.Interval);

                    if (!option.ExecuteCount.HasValue)
                    {
                        builder.RepeatForever();
                        return;
                    }

                    var repeatCount = option.ExecuteCount.Value - 1;

                    if (repeatCount < 0)
                        throw new ArgumentException("Invalid RepeatCount for "+ triggerName);

                    builder.WithRepeatCount(repeatCount);
                })
                .StartAt(DateTimeOffset.UtcNow.Add(option.StartAfter))
                .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger, ct);
        }

        public void Dispose()
        {
            StopAsync(CancellationToken.None).ConfigureAwait(false)
                .GetAwaiter().GetResult();
        }

    }
}
