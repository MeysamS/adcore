extern alias ApiReaderContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ApiReaderContext::ADCore.ApiReader.Context.context;
using ApiReaderContext::ADCore.ApiReader.Context.Dto;
using ApiReaderContext::ADCore.ApiReader.Context.Services;
using ADCore.ApiReader.Models;
using ADCore.ApiReader.Services;
using ADCore.Mapper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ADCore.ApiReader.Context.Entities;
using ICoinService = ApiReaderContext::ADCore.ApiReader.Context.Services.ICoinService;
using IConfigService = ApiReaderContext::ADCore.ApiReader.Context.Services.IConfigService;
using MarketCapResponse = ApiReaderContext::ADCore.ApiReader.Context.Dto.MarketCapResponse;

namespace ADCore.ApiReader.Api.Controllers
{
    extern alias ApiReaderContext;

    [Route("api/[controller]")]
    public class CoinsController : Controller
    {
        private readonly ApiReaderContext::ADCore.ApiReader.Context.Services.ICoinService _coinservice;
        private readonly ApiReaderContext::ADCore.ApiReader.Context.Services.IConfigService _configService;
        private readonly ILogger<CoinsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CoinsController(ICoinService coinService, IConfigService configService, ILogger<CoinsController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _coinservice = coinService;
            _configService = configService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            RequestModel apiConfig = await _configService.GetApiConfig("coinmarketcap.com", "https://localhost:5004/ApiReaderConfig").ConfigureAwait(false);
            Config mapperConfig = await _configService.GetMapConfig("coinmarketcap.com", "https://localhost:5004/mapperConfig").ConfigureAwait(false);

            if (apiConfig == null || mapperConfig == null)
            {
                _logger.LogError("The return value of the GetApiConfig or GetMapperConfig ... is empty");
                return null;
            }

            var data = await _configService.CallApi(apiConfig);
            MarketCapResponse res = _configService.MapToModel(data, mapperConfig);
            var (isSuccess, message) = _coinservice.AddOrUpdateCoins(res.Coins);
            if (isSuccess)
                _logger.LogInformation(message);
            else
            {
                _logger.LogError(message);
            }

            return Content(message);
        }

    }
}
