using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.Entities;

namespace ADCore.WebSocket.CoinmarketCap.Services
{
    public interface ICoinMarketCapPriceService
    {
        Task Add(CoinPrice price);
        Coin GetCoinById(int coinMarketCapId);
        IList<Coin> GetCoins(bool asNoTracking=false);
        IList<CoinPrice> Get(int coinId);
    }
}
