using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ADCore.ApiReader.Extensions;
using ADCore.Binance.CryptoPairWatcher.Entities;
using ADCore.Binance.CryptoPairWatcher.Models;
using ADCore.Mapper.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ADCore.Binance.CryptoPairWatcher.Services
{
    public interface IBinanceHandler
    {
        string JsonData { get; set; }
        Task HandleAddCoinsAsync();
        Task<RequestModel> HandleRequestToReadCoinsAsync();
    }


    public class BinanceHandler : IBinanceHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfigService _configService;
        private readonly AppSettings _appSettings;
        private readonly ILogger<BinanceHandler> _logger;
        public string JsonData { get; set; }

        public BinanceHandler(IServiceProvider serviceProvider, IConfigService configService,
            ILogger<BinanceHandler> logger,
                                IOptions<AppSettings> appSettingsOptions)
        {
            _serviceProvider = serviceProvider;
            _configService = configService;
            _logger = logger;
            _appSettings = appSettingsOptions.Value;
        }


        public async Task HandleAddCoinsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var binanService = scope.ServiceProvider.GetRequiredService<IBinanceService>();


            Config mapperConfig = await _configService.GetMapConfig(_appSettings.Key, _appSettings.MapperConfigSupplierUrl).ConfigureAwait(false);
            if (JsonData == null || mapperConfig == null)
            {
                _logger.LogError("Json Data Is Empty or Mapper Config is Null");
                return;
            }

            PairResponse res = _configService.MapToModel(JsonData, mapperConfig);
            binanService.AddOrUpdatePairs(res.PairExchange);
        }

        public async Task<RequestModel> HandleRequestToReadCoinsAsync()
        {
            var binanceConfigResponse =
                await _configService.GetApiConfig(_appSettings.Key, _appSettings.ApiReaderConfigSupplierUrl);

            var request = new RequestModel
            {
                TopicName = "RespondTopic",
                Url = binanceConfigResponse?.Url.AddQueryStringsToEndOfUrl(binanceConfigResponse.QueryStrings),
                HeaderStrings = binanceConfigResponse?.HeaderStrings
            };
            return request;
        }
    }
}
