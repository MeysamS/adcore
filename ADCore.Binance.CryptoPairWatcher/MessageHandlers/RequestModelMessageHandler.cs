using ADCore.Binance.CryptoPairWatcher.Jobs;
using ADCore.Binance.CryptoPairWatcher.Services;
using ADCore.Binance.CryptoPairWatcher.Models;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Messaging.Handler;
using ADCore.Kafka.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ADCore.Binance.CryptoPairWatcher.MessageHandlers
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
                DoActionAfterGetResponce(message);
            }

            return HandlerResult.Success();
        }

        private void DoActionAfterGetResponce(ResponseModel message)
        {
            using var scope = _serviceProvider.CreateScope();
            var binanceHandler = scope.ServiceProvider.GetRequiredService<IBinanceHandler>();

            binanceHandler.JsonData = message.Data;
            binanceHandler.HandleAddCoinsAsync();
        }
        private static void ShutDownListnerForResponce()
        {
            MessagingBootstrapperJob.Enabled = false;
        }
    }
}