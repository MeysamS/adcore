using ADCore.Binance.CryptoPairWatcher.Context;
using ADCore.Binance.CryptoPairWatcher.Services;
using ADCore.Kafka.Extensions;
using ADCore.Mapper.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADCore.Binance.CryptoPairWatcher.Extensions
{
    public static class BinanceServicesExtensions
    {
        public static void AddBinanceServicesExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<IBinanceService, BinanceService>();
            services.AddScoped<IBinanceHandler, BinanceHandler>();
            services.AddScoped<IMapperService, MapperService>();

            services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<BinanceDbContext>());

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddNpgsqlPool<BinanceDbContext>(connectionString);
            //services.AddDbContextPool<BinanceDbContext>(options =>
            //{
            //    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            //});
        }
    }
}
