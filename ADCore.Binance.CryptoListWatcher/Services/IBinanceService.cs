using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.Entities;
using ADCore.Binance.CryptoListWatcher.Context;
using ADCore.Binance.CryptoListWatcher.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ADCore.Binance.CryptoListWatcher.Services
{
    public interface IBinanceService
    {
        void AddOrUpdateCoins(IList<Coin> coins);
    }


    public class BinanceService : IBinanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<Coin> _coins;
        private readonly ILogger<BinanceService> _logger;

        public BinanceService(IUnitOfWork unitOfWork,ILogger<BinanceService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _coins = _unitOfWork.Set<Coin>();
            _logger = logger;
        }

        public void AddCoin(Coin coin)
        {
            _coins.Add(coin);
            _unitOfWork.SaveChanges();
        }
        private bool IsExistData => _coins.Any();
        public void AddOrUpdateCoins(IList<Coin> coins)
        {
            if (!coins.Any())
            {
               _logger.LogError("coins paramatere is empty");
            }
            try
            {
                coins.ToList().ForEach(x =>
                {
                    x.UpdateDate = DateTime.Now;
                    x.MarketName = "binance.com";

                });
                if (!IsExistData)
                {
                    _coins.AddRange(coins);
                    _unitOfWork.SaveChanges();
                    _logger.LogInformation("operation insert or update  coins is successfully");
                }

                var coinsFromDb = _coins.ToList();

                //coins = coins.Skip(13).Take(7).ToList(); => for test
                var newCoins = coins.Where(c => coinsFromDb.All(x => x.Name != c.Name)).ToList();
                // todo : check for add market type  to x => x.Name == c.Name

                var updateCoins = coinsFromDb.Where(c => coins.Any(x => x.Name == c.Name)).ToList();
                updateCoins.ForEach(x => x.UpdateDate = DateTime.Now);

                var deletedState = coinsFromDb.Where(c => (coins.All(x => x.Name != c.Name)) && (!c.Deleted)).ToList();
                //&& (c.UpdateDate.Day != DateTime.Now.Day)
                deletedState.ForEach(x => x.Deleted = true);

                _coins.AddRange(newCoins);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }

    }
}
