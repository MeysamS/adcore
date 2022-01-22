using Quartz;

namespace ADCore.Kafka.Extensions
{
    public static class JobDetailExtensions
    {

        private const string JobName = "___JobName___";

        public static void SetJobName(this IJobDetail detail, string name) => detail.JobDataMap.Put(JobName, name);

        public static string GetJobName(this IJobDetail detail) => detail.JobDataMap.GetString(JobName);

    }
}
