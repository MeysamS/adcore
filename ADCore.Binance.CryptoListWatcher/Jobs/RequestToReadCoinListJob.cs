using ADCore.ApiReader.Extensions;
using ADCore.ApiReader.Services;
using ADCore.Binance.CryptoListWatcher.Models;
using ADCore.Binance.CryptoListWatcher.Services;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Messaging.Publisher;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.Entities;
using Microsoft.Extensions.DependencyInjection;


namespace ADCore.Binance.CryptoListWatcher.Jobs
{
    //RequestToReadCoinListJob
    [Worker, DisallowConcurrentExecution]
    public class RequestJob : IJob
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IServiceProvider _serviceProvider;


        public RequestJob(IMessagePublisher messagePublisher, IServiceProvider serviceProvider)
        {
            _messagePublisher = messagePublisher;
            _serviceProvider = serviceProvider;
        }


        public async Task Execute(IJobExecutionContext context)
        {

            Console.WriteLine($">>>>>>> Request To Read CoinList Job Execute --------------");
            // ****
            using var scope = _serviceProvider.CreateScope();
            var binanceHandler = scope.ServiceProvider.GetRequiredService<IBinanceHandler>();

            SendToKafka(await binanceHandler.HandleRequestToReadCoinsAsync());

            RunListnerForResponce();

        }
        private static void RunListnerForResponce()
        {
            MessagingBootstrapperJob.Enabled = true;
        }

        private void SendToKafka(RequestModel Request)
        {
            if (Request != null)
                _messagePublisher.PublishAsync(Request);
        }




    }

}
