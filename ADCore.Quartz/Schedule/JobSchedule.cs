using System;
using System.Collections.Generic;

namespace ADCore.Quartz.Schedule
{

    public class SchedulerConfig
    {
        public List<JobSchedule> AdJobs { get; set; }
    }


    public class JobSchedule
    {
        // public JobSchedule(string name, TimeSpan startAfter, TimeSpan interval)
        // {
        //     Name = name;
        //     StartAfter = startAfter;
        //     Interval = interval;
        // }
    
        public string Name { get; set; }
        public TimeSpan StartAfter { get; set; }
        public TimeSpan Interval { get; set; }
        
    }
}