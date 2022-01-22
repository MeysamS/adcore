using System;
using ADCore.Kafka.Extensions;
using ADCore.Kafka.Formatters.JsonNet;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ADCore.Kafka.Hosting
{
    public abstract class AdHost
    {
        internal static bool IsConfigured { get; private set; }
        public static IHostBuilder Create<TStartup>(string[]? args,
            string[]? envDependSettings = null,
            Action<IHostEnvironment, IConfigurationBuilder>? preAddDefaultAppSettings = null,
            Action<IHostEnvironment, IConfigurationBuilder>? postAddDefaultAppSettings = null)
            where TStartup : class
            => Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration((hostingContext, config) => {
                    var env = hostingContext.HostingEnvironment;
                    preAddDefaultAppSettings?.Invoke(env, config);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);
                    if (envDependSettings != null)
                        foreach (var s in envDependSettings)
                        {
                            config.AddJsonFile($"{s}.json", optional: false, reloadOnChange: true)
                                .AddJsonFile($"{s}.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);
                        }
                    postAddDefaultAppSettings?.Invoke(env, config);
                    IsConfigured = true;
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<TStartup>(); });
        public static IHostBuilder CreateDaemon(
            string serviceName,
            string[]? args,
            string[]? envDependSettings = null,
            Action<IHostEnvironment, IConfigurationBuilder>? preAddDefaultAppSettings = null,
            Action<IHostEnvironment, IConfigurationBuilder>? postAddDefaultAppSettings = null)
        {
            return Host.CreateDefaultBuilder(args)
           .UseServiceProviderFactory(new AutofacServiceProviderFactory())
           .ConfigureAppConfiguration((hostContext, config) => {
               var env = hostContext.HostingEnvironment;
               preAddDefaultAppSettings?.Invoke(env, config);

               config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);
               if (envDependSettings != null)
                   foreach (var s in envDependSettings)
                   {
                       config.AddJsonFile($"{s}.json", optional: false, reloadOnChange: true)
                             .AddJsonFile($"{s}.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);
                   }

               postAddDefaultAppSettings?.Invoke(env, config);
               IsConfigured = true;

               hostContext.Configuration.UseIn(o => {
                   o.EnableApiExplorer = false;
                   o.EnableCleaners = false;
                   o.EnableDataAnnotations = false;
                   o.EnableFormatterMappings = false;
                   o.ServiceName = serviceName;

                   o.UseFormatter<JsonNetFormatterProvider>(f => {
                       f.AcceptInputLongString = true;
                       f.OutputLongAsString = true;
                   });
               });
           });
        }

    }
}
