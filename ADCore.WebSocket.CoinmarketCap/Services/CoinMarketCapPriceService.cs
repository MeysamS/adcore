using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.Entities;
using ADCore.WebSocket.CoinmarketCap.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ADCore.WebSocket.CoinmarketCap.Services
{
    public class CoinMarketCapPriceService : ICoinMarketCapPriceService
    {
        private readonly ILogger<CoinMarketCapPriceService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<CoinPrice> _coinPrices;
        private readonly DbSet<Coin> _coins;

        public CoinMarketCapPriceService(IUnitOfWork unitOfWork, ILogger<CoinMarketCapPriceService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(_unitOfWork)); ;
            _coins = _unitOfWork.Set<Coin>();
            _coinPrices = _unitOfWork.Set<CoinPrice>();
            _logger = logger;
        }
        public async Task Add(CoinPrice price)
        {
            if (price == null)
                return;
            await _coinPrices.AddAsync(price);
            //await _dbContext.SaveChangesAsync();
        }

        public Coin GetCoinById(int coinMarketCapId)
        {
            return _coins.SingleOrDefault(x => x.CoinMarketId == coinMarketCapId);
        }

        public IList<Coin> GetCoins(bool asNoTracking = false)
        {
            if (asNoTracking)
                return _coins.Where(x => x.Deleted == false).AsNoTracking().ToList();
            return _coins.Where(x => x.Deleted == false).ToList();
        }


        public IList<CoinPrice> Get(int coinId)
        {
            throw new NotImplementedException();
        }
    }
}
