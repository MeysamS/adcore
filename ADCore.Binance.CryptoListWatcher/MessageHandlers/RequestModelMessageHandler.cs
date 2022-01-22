using ADCore.Binance.CryptoListWatcher.Jobs;
using ADCore.Binance.CryptoListWatcher.Models;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Messaging.Handler;
using ADCore.Kafka.Models;
using System;
using System.Threading.Tasks;
using ADCore.Binance.CryptoListWatcher.Services;
using ADCore.Mapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ADCore.Binance.CryptoListWatcher.MessageHandlers
{
    [SubscribableMessageHandler("Msghandler")]
    public class RequestModelMessageHandler : IMessageHandler<ResponseModel>
    {
        private readonly IServiceProvider _serviceProvider;
        public RequestModelMessageHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<HandlerResult> HandleAsync(ResponseModel message)
        {
            if (MessagingBootstrapperJob.Enabled)
            {
                ShutDownListnerForResponce();
                await DoActionAfterGetResponceAsync(message);
            }

            return HandlerResult.Success();
        }
        private static void ShutDownListnerForResponce()
        {
            MessagingBootstrapperJob.Enabled = false;
        }

        private async Task DoActionAfterGetResponceAsync(ResponseModel message)
        {
            
            using var scope = _serviceProvider.CreateScope();
            var binanceHandler = scope.ServiceProvider.GetRequiredService<IBinanceHandler>();

            await binanceHandler.HandleAddCoinsAsync(message.Data);
        }

    }
}