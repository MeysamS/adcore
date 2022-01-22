using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ADCore.Quartz.Jobs;
using ADCore.Quartz.Schedule;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Ad.Micro.Quartz
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var schedulerConfig = new ConfigurationBuilder()
                        .AddJsonFile($"scheduler.config.json")
                        .Build();
                    
                    services.Configure<SchedulerConfig>(t => schedulerConfig.GetSection("SchedulerConfig").Bind(t));
                    
                    services.AddHostedService<QuartzHostedService>();
                    services.AddSingleton<IJobFactory, SingletonJobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();



                   
                    

                     services.AddSingleton<MyJob>();
                    // services.AddSingleton(new JobSchedule(
                    //     jobType: typeof(MyJob),
                    //     cronExpression:"0/5 * * * * ? *"
                    // ));
                });
        
    }
    
    
    
}