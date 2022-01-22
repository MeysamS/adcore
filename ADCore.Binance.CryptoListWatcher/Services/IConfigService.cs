using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADCore.ApiReader.Services;
using ADCore.Binance.CryptoListWatcher.Models;
using ADCore.Mapper.Models;
using ADCore.Mapper.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RequestModel = ADCore.ApiReader.Models.RequestModel;

namespace ADCore.Binance.CryptoListWatcher.Services
{
    public interface IConfigService
    {
        Task<RequestModel> GetApiConfig(string key, string url);
        Task<Config> GetMapConfig(string key, string url);
        Task<string> CallApi(RequestModel request);
        MarketCoinResponse MapToModel(string jsonData, Config config);
    }


    public class ConfigService : IConfigService
    {

        private readonly IApiClient _apiClient;
        private readonly ILogger<ConfigService> _logger;
        public IMapperService MapperService { get; set; }

        public ConfigService(IApiClient apiClient,
            IMapperService mapperService,
            ILogger<ConfigService> logger)
        {
            _apiClient = apiClient;
            MapperService = mapperService;
            _logger = logger;
        }
        public async Task<RequestModel> GetApiConfig(string key, string url)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(url))
            {
                _logger.LogWarning("argument null exception");
                throw new ArgumentNullException("key or url is null");
            }
            var result = await _apiClient.GetAsync(
                new RequestModel
                {
                    Url = url,
                    QueryStrings = new Dictionary<string, string>()
                    {
                        {"key", key}
                    }
                }).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                _logger.LogError(result.Exception.Message);
                return null;

            }

            RequestModel apiConfig = JsonConvert.DeserializeObject<RequestModel>(result.Data);
            return apiConfig;
        }

        public async Task<Config> GetMapConfig(string key, string url)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(url))
            {
                _logger.LogWarning("argument null exception");
                throw new ArgumentNullException("key or url is null");
            }
            var result = await _apiClient.GetAsync(
                new RequestModel
                {
                    Url = url,
                    QueryStrings = new Dictionary<string, string>()
                    {
                        {"key", key}
                    }
                }).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                _logger.LogError(result.Exception.Message);
            }

            Config mapConfig = JsonConvert.DeserializeObject<Config>(result.Data);
            return mapConfig;
        }

        public async Task<string> CallApi(RequestModel request)
        {
            var result = await _apiClient.GetAsync(
                new RequestModel
                {
                    Url = request.Url,
                    QueryStrings = request.QueryStrings,
                    HeaderStrings = request.HeaderStrings
                }).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                _logger.LogError(result.Exception.Message);
            }
            return result.Data;
        }

        public MarketCoinResponse MapToModel(string jsonData, Config config)
        {
            Resource res = new Resource() { Data = jsonData, Config = config };
            _ = MapperService.TryMap<MarketCoinResponse>(res, out object marketCapResponse);
            return (MarketCoinResponse)marketCapResponse;
        }
    }
}