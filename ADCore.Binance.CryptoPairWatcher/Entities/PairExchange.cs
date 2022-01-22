using System;
using System.Collections.Generic;
using System.Text;

namespace ADCore.Binance.CryptoPairWatcher.Entities
{
    public class PairExchange
    {
        public int Id { get; set; }
        public string PairKey { get; set; }
        public string MarketName { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Deleted { get; set; }

     }
}
