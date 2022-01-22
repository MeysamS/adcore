using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADCore.ApiReader.Daemons.Services;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Messaging.Handler;
using ADCore.Kafka.Settings;
using Microsoft.Extensions.Options;
using Quartz;

namespace ADCore.ApiReader.Daemons.Jobs
{
    [Worker, DisallowConcurrentExecution]
    public class CoinGeckoReaderJob : IJob
    {
        private readonly ICoinGeckoService _coinGeckoService;
        public CoinGeckoReaderJob(ICoinGeckoService coinGeckoService)
        {
            _coinGeckoService = coinGeckoService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _coinGeckoService.GetCoinList();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}