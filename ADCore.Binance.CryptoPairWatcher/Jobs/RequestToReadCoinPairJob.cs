using ADCore.ApiReader.Extensions;
using ADCore.ApiReader.Services;
using ADCore.Binance.CryptoPairWatcher.Services;
using ADCore.Binance.CryptoPairWatcher.Models;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Messaging.Publisher;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADCore.Binance.CryptoPairWatcher.Jobs
{
    
    [Worker, DisallowConcurrentExecution]
    public class RequestToReadCoinPairJob : IJob
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly AppSettings _appSettings;
        private readonly IConfigService _configService;

        public RequestToReadCoinPairJob(IConfigService configService, IOptions<AppSettings> appSettingsOptions, IMessagePublisher messagePublisher)
        {
            _configService = configService;
            _appSettings = appSettingsOptions.Value;
            _messagePublisher = messagePublisher;

        }
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($">>>>>>> RequestToReadCoinPairJob Execute --------------");

            //todo: Add binance.com-CoinPair to ApiReader
            //https://api.binance.com/api/v3/exchangeInfo


            var binanceConfigResponse =
                  await _configService.GetApiConfig(_appSettings.Key, _appSettings.ApiReaderConfigSupplierUrl);

            var request = new RequestModel
            {
                TopicName = "RespondTopic",
                Url = binanceConfigResponse.Url.AddQueryStringsToEndOfUrl(binanceConfigResponse.QueryStrings),
                HeaderStrings = binanceConfigResponse.HeaderStrings
            };

            SendToKafka(request);

            RunListnerForResponce();
        }

        private static void RunListnerForResponce()
        {
            MessagingBootstrapperJob.Enabled = true;
        }

        private void SendToKafka(RequestModel Request)
        {
            _messagePublisher.PublishAsync(Request);
        }




    }

}
