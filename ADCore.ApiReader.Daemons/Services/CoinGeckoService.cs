using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.context;
using ADCore.ApiReader.Context.Services;
using ADCore.ApiReader.Daemons.Models;
using ADCore.ApiReader.Daemons.Models.CoinGeckoModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ADCore.ApiReader.Daemons.Services
{
    public class CoinGeckoService : ICoinGeckoService
    {
        private readonly ApiReaderDbContext _context;
        private readonly IConfigService _configService;
        private readonly ICoinService _coinService;
        private readonly ServiceOptions _options;
        public CoinGeckoService(ApiReaderDbContext context,
            IConfigService configService,
            ICoinService coinService,
            IOptions<ServiceOptions> options)
        {
            _context = context;
            _configService = configService;
            _coinService = coinService;
            _options = options.Value;
        }

        public async Task GetCoinList()
        {
            var apiConfig = await _configService.GetApiConfig("coingecko.com", _options.ApiReaderConfigUrl).ConfigureAwait(false);
            var mapperConfig = await _configService.GetMapConfig("coingecko.com", _options.MapperConfigUrl).ConfigureAwait(false);

            if (apiConfig == null || mapperConfig == null)
                return;

            var data = await _configService.CallApi(apiConfig); 
            // map data
            var response = _configService.MapToModel(data, mapperConfig);
            // save to database
            _coinService.AddOrUpdateCoins(response.Coins);
        }
    }

}
