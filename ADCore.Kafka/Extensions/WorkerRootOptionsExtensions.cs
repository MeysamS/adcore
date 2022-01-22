using System.Collections.Specialized;
using ADCore.Kafka.Settings;

namespace ADCore.Kafka.Extensions
{
    public static class WorkerRootOptionsExtensions
    {

        public static NameValueCollection ToProperties(this WorkerRootOptions options)
        {
            var properties = new NameValueCollection();

            if (!string.IsNullOrWhiteSpace(options.Scheduler?.InstanceName))
                properties.Add("quartz.scheduler.instanceName", options.Scheduler?.InstanceName);

            if (!string.IsNullOrWhiteSpace(options.ThreadPool?.Type))
                properties.Add("quartz.threadPool.type", options.ThreadPool?.Type);

            if (!string.IsNullOrWhiteSpace(options.ThreadPool?.ThreadPriority))
                properties.Add("quartz.threadPool.threadPriority", options.ThreadPool?.ThreadPriority);

            if (options.ThreadPool?.ThreadCount != 0)
                properties.Add("quartz.threadPool.threadCount", options.ThreadPool?.ThreadCount.ToString());

            if (!string.IsNullOrWhiteSpace(options.Plugin?.JobInitializer?.Type))
                properties.Add("quartz.plugin.jobInitializer.type", options.Plugin?.JobInitializer?.Type);

            if (!string.IsNullOrWhiteSpace(options.Plugin?.JobInitializer?.FileNames))
                properties.Add("quartz.plugin.jobInitializer.fileNames", options.Plugin?.JobInitializer?.FileNames);

            return properties;
        }

    }
}
