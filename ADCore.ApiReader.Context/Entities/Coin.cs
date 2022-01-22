using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ADCore.ApiReader.Context.Entities
{
    public class Coin
    {
        public int Id { get; set; }
        public string MarketName { get; set; }
        /// <summary>
        /// Coin Id from Market => (Binance , CoinGecko , CoinMarketCap, ... )
        /// </summary>
        public int CoinMarketId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Slug { get; set; }
        public bool Trading { get; set; }
        public bool Etf { get; set; }
        public bool Deleted { get; set; }
        public DateTime UpdateDate { get; set; }
        
        public virtual ICollection<CoinPrice> CoinPrices { get; set; }
    }
}