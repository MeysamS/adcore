using System;
using System.Threading.Tasks;
using ADCore.Quartz.Schedule;
using ADCore.Quartz.Schedule;
using Quartz;

namespace ADCore.Quartz.Jobs
{
    [AdQuartzJob]
    public class MyJob:IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"MyJob executed at {DateTimeOffset.UtcNow}");
        }
    }
}