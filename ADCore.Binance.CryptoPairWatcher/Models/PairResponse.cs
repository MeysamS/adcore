using System;
using System.Collections.Generic;
using System.Text;
using ADCore.Binance.CryptoPairWatcher.Entities;

namespace ADCore.Binance.CryptoPairWatcher.Models
{
    public class PairResponse
    {
        public IList<PairExchange> PairExchange { get; set; }
    }
}
