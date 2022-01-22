using ADCore.Binance.CryptoListWatcher.Context;
using ADCore.Binance.CryptoListWatcher.Services;
using ADCore.Kafka.Extensions;
using ADCore.Mapper.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADCore.Binance.CryptoListWatcher.Extensions
{
    public static class BinanceServicesExtensions
    {
        public static void AddBinanceServicesExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<IBinanceService, BinanceService>();
            services.AddScoped<IBinanceHandler, BinanceHandler>();
            services.AddScoped<IMapperService, MapperService>();

            string connection = configuration.GetConnectionString("DefaultConnection");
            services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<BinanceDbContext>());
            services.AddDbContext<BinanceDbContext>(options =>
            {
                options.UseNpgsql(connection);
            });
            //services.AddNpgsqlPool<BinanceDbContext>(connection);
            //services.AddDbContextPool<BinanceDbContext>(options =>
            //{
            //    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            //});
        }
    }
}
