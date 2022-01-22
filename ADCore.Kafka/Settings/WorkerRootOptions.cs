using System.Collections.Generic;

namespace ADCore.Kafka.Settings
{
    public class WorkerRootOptions
    {
        public IDictionary<string, WorkerOptions> Workers { get; set; }
        public WorkerSchedulerOptions Scheduler { get; set; }
        public WorkerThreadPoolOptions ThreadPool { get; set; }
        public WorkerPluginOptions Plugin { get; set; }
    }

    public class WorkerSchedulerOptions
    {
        public string InstanceName { get; set; }
    }

    public class WorkerThreadPoolOptions
    {
        public string Type { get; set; }
        public string ThreadPriority { get; set; }
        public int ThreadCount { get; set; }
    }

    public class WorkerPluginOptions
    {
        public WorkerJobInitializerOptions JobInitializer { get; set; }
    }

    public class WorkerJobInitializerOptions
    {
        public string Type { get; set; }
        public string FileNames { get; set; }
    }
}
