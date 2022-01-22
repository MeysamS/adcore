using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADCore.Binance.CryptoPairWatcher.Context;
using ADCore.Binance.CryptoPairWatcher.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ADCore.Binance.CryptoPairWatcher.Services
{
    public interface IBinanceService
    {
        void AddPairExchange(PairExchange pairExchange);
        void AddOrUpdatePairs(IList<PairExchange> pairExchanges);
    }


    public class BinanceService : IBinanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<PairExchange> _pairExchanges;
        private readonly IConfigService _config;
        private readonly ILogger<BinanceService> _logger;

        public BinanceService(IUnitOfWork unitOfWork,ILogger<BinanceService> logger)
        {
            _pairExchanges = _unitOfWork.Set<PairExchange>();
            _logger = logger;
        }

        public void AddPairExchange(PairExchange pairExchange)
        {
            _pairExchanges.Add(pairExchange);
            _unitOfWork.SaveChanges();
        }
        private bool IsExistData => _pairExchanges.Any();

    
        public void AddOrUpdatePairs(IList<PairExchange> pairExchanges)
        {
            if (!pairExchanges.Any())
            {
                _logger.LogError("Exception : coins list is empty");
            }
            try
            {
                pairExchanges.ToList().ForEach(x =>
                {
                    x.UpdateDate = DateTime.Now;
                    x.MarketName = "binance.com";

                });
                if (!IsExistData)
                {
                    _pairExchanges.AddRange(pairExchanges);
                    _unitOfWork.SaveChanges();
                    _logger.LogInformation("The first time all coins data is successfully stored in the database");
                    
                }

                var coinsFromDb = _pairExchanges.ToList();
                //todo: we most check PairKey && Market Type for delete or.....
                //coins = coins.Skip(13).Take(7).ToList(); => for test
                var newCoins = pairExchanges.Where(c => coinsFromDb.All(x => x.PairKey != c.PairKey)).ToList();

                var updateCoins = coinsFromDb.Where(c => pairExchanges.Any(x => x.PairKey == c.PairKey)).ToList();
                updateCoins.ForEach(x => x.UpdateDate = DateTime.Now);

                var deletedState = coinsFromDb.Where(c => pairExchanges.All(x => x.PairKey != c.PairKey) && !c.Deleted).ToList();
                //&& (c.UpdateDate.Day != DateTime.Now.Day)
                deletedState.ForEach(x => x.Deleted = true);

                _pairExchanges.AddRange(newCoins);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
               _logger.LogError(ex.Message);
            }

            _logger.LogInformation("The add new coins from binance.com and update old coins updatDate Filed successfully stored in the database");
        }

    }
}
