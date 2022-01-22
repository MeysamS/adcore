using System;

namespace ADCore.Kafka.Settings
{
    public class WorkerOptions
    {

        private int? _executeCount;

        public bool IsEnabled { get; set; }
        public TimeSpan StartAfter { get; set; }
        public TimeSpan Interval { get; set; }

        public int? ExecuteCount
        {
            get => _executeCount;
            set
            {
                if (value < 1)
                    throw new
                        ArgumentException($"The {nameof(ExecuteCount)} cannot be less than 1. If you want the job not execute at all, use the {nameof(IsEnabled)} property. If you want the job to execute forever, don't set this property at all.");

                _executeCount = value;
            }
        }

        public dynamic AdditionalData { get; set; }

    }
}
