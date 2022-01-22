using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.context;
using ADCore.ApiReader.Context.Entities;
using ADCore.ApiReader.Models;
using ADCore.ApiReader.Services;
using ADCore.Mapper.Models;
using ADCore.Mapper.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ADCore.ApiReader.Context.Services
{
    public class CoinService : ICoinService
    {

        private readonly IUnitOfWork _uow;
        private readonly DbSet<Coin> _coins;
        private readonly ILogger<CoinService> _logger;

        public CoinService(ILogger<CoinService> logger, IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(_uow));
            _logger = logger;
            _coins = _uow.Set<Coin>();
        }

        public void AddCoin(Coin coin)
        {
            _coins.Add(coin);
        }

        public IList<Coin> GetAllCoin()
        {
            return _coins.AsNoTracking().ToList();
        }

        public (bool, string) AddOrUpdateCoins(IList<Coin> coins)
        {
            try
            {
                coins.ToList().ForEach(x =>
                {
                    x.UpdateDate = DateTime.Now;
                    x.MarketName = "coinmarketcap.com";

                });
                if (!IsExistData)
                {
                    _coins.AddRange(coins);
                    _uow.SaveChanges();
                    return (true, "The first time all coins data is successfully stored in the database");
                }

                var coinsFromDb = _coins.ToList();

                //coins = coins.Skip(13).Take(7).ToList(); => for test
                var newCoins = coins.Where(c => !coinsFromDb.Any(x => x.Name == c.Name)).ToList();

                var updateCoins = coinsFromDb.Where(c => coins.Any(x => x.Name == c.Name)).ToList();
                updateCoins.ForEach(x => x.UpdateDate = DateTime.Now);

                var deletedState = coinsFromDb.Where(c => (!coins.Any(x => x.Name == c.Name))  && (!c.Deleted)).ToList();
                    //&& (c.UpdateDate.Day != DateTime.Now.Day)
                deletedState.ForEach(x => x.Deleted = true);

                _coins.AddRange(newCoins);
                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, "The add new coins and update old coins updatDate Filed successfully stored in the database");
        }

        private bool IsExistData => _coins.Any();




    }
}