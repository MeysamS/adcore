using System;
using System.Collections.Generic;
using System.Text;
using ADCore.WebSocket.CoinmarketCap.Context;
using ADCore.WebSocket.CoinmarketCap.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADCore.WebSocket.CoinmarketCap.Extensions
{
    public static class AddWebSocketExtensions
    {
        public static void AddWebSocketServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<ICreateRequestMessage, CreateRequestMessage>();
            services.AddScoped<ICoinMarketCapPriceService, CoinMarketCapPriceService>();
            services.AddScoped<IWebSocketHandler, WebSocketHandler>();
            services.AddScoped<IWebSocketManager, WebSocketManager>();

            string connection = configuration.GetConnectionString("cnnString");
            services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<WebSocketDbContext>());
            services.AddDbContext<WebSocketDbContext>(options =>
            {
                options.UseNpgsql(connection);
            });
        }
    }
}
