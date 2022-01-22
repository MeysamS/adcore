using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.context;
using ADCore.ApiReader.Context.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADCore.ApiReader.Context.Extensions
{
    public static class AddApiReaderContextServicesExtensions
    {
        public static void AddApiReaderContextServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<ICoinService, CoinService>();
            services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<ApiReaderDbContext>());
            services.AddDbContext<ApiReaderDbContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString("cnnString"));
                });

        }
    }
}
