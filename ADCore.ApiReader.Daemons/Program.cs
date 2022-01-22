using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.context;
using ADCore.ApiReader.Context.Entities;
using ADCore.ApiReader.Context.Extensions;
using ADCore.ApiReader.Context.Services;
using ADCore.ApiReader.Daemons.Models;
using ADCore.ApiReader.Daemons.Services;
using ADCore.ApiReader.Extensions;
using ADCore.ApiReader.Services;
using ADCore.Kafka.Extensions;
using ADCore.Kafka.Hosting;
using ADCore.Kafka.Messaging.Handler;
using ADCore.Kafka.Messaging.Publisher;
using ADCore.Kafka.Settings;
using ADCore.Mapper.Extensions;
using ADCore.Mapper.Services;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ADCore.ApiReader.Daemons
{
    public class Program
    {
        private static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
            => AdHost.CreateDaemon("ADCore.ApiReader.Daemons", args,
                    postAddDefaultAppSettings: (env, config) =>
                    {
                        config.AddJsonFile("appsettings.workers.json", false, true);
                    })
                .ConfigureServices((hostContext, services) =>
                {
                    var cs = hostContext.Configuration.GetConnectionString("DefaultConnection");
                    services.AddNpgsqlPool<ApiReaderDbContext>(cs);
                    services.AddOptions();

                    services.Configure<ApplicationOptions>(options =>
                    {
                        options.AppAssemblies = new[] { typeof(Program).Assembly };
                    });

                    services.Configure<WorkerRootOptions>(o
                        => hostContext.Configuration
                            .GetSection("WorkerOptions").Bind(o));

                    services.Configure<ServiceOptions>(o
                        => hostContext.Configuration
                            .GetSection("ServiceOptions").Bind(o));

                    services.AddMessaging(typeof(Program).Assembly);
                    services.AddHostedWorker(typeof(Program).Assembly);
                    services.AddScoped<IConfigService, ConfigService>();
                    services.AddScoped<ICoinGeckoService, CoinGeckoService>();
                    services.AddApiClient();
                    services.AddApiReaderContextServices(hostContext.Configuration);
                    services.AddADCoreMapperService<Coin>();
                });
    }
}
