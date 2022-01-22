using System;
using System.Collections.Generic;
using System.Text;
using ADCore.ApiReader.Context.Entities;

namespace ADCore.Binance.CryptoListWatcher.Models
{
   public class MarketCoinResponse
    {
        public List<Coin> Coins { get; set; }
    }
}
