using ADCore.ApiReader.Extensions;
using ADCore.Binance.CryptoListWatcher.Models;
using ADCore.Kafka.Extensions;
using ADCore.Kafka.Hosting;
using ADCore.Kafka.Messaging.Handler;
using ADCore.Kafka.Messaging.Publisher;
using ADCore.Kafka.Settings;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using ADCore.Binance.CryptoListWatcher.Extensions;
using ADCore.Binance.CryptoListWatcher.Services;

namespace ADCore.Binance.CryptoListWatcher
{
    class Program
    {
        private static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return AdHost.CreateDaemon("ADCore.Binance.CryptoListWatcher", args,
                       postAddDefaultAppSettings: (env, config) =>
                       {
                           config.AddJsonFile("appsettings.json", false, true);
                           config.AddJsonFile("appsettings.workers.json", false, true);
                           config.AddJsonFile("appsettings.messaging.json", false, true);
                       })
                       .ConfigureServices((hostContext, services) =>
                       {

                           services.AddOptions();

                           services.Configure<ApplicationOptions>(options =>
                           {
                               options.AppAssemblies = new[] { typeof(Program).Assembly };
                           });

                           services.Configure<AppSettings>(o
                                  => hostContext.Configuration.GetSection("appsettings").Bind(o));

                           services.Configure<WorkerRootOptions>(o
                                => hostContext.Configuration.GetSection("WorkerOptions").Bind(o));

                           services.Configure<MessagingOptions>(o
                                => hostContext.Configuration.GetSection("MessagingOptions").Bind(o));
                           
                           services.AddBinanceServicesExtensions(hostContext.Configuration);
                           services.AddMessaging(typeof(Program).Assembly);
                           services.AddHostedWorker(typeof(Program).Assembly);
                           services.AddScoped<IMessagingBootstrapper, MessagingBootstrapper>();
                           services.AddApiClient();

                       })
                       .ConfigureContainer<ContainerBuilder>(builder =>
                       {
                           builder.Register(builder =>
                           {
                               var options = builder.Resolve<IOptions<MessagingOptions>>();
                               return MessagePublisher.CreateInstance(options);
                           }).As<IMessagePublisher>().SingleInstance();
                       });
        }

    }
}
