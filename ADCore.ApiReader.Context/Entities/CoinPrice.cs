using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADCore.ApiReader.Context.Entities
{
    public class CoinPrice
    {
        public Guid Id { get; set; }

        public int CoinId { get; set; }
        public Coin Coin { get; set; }

        public double Price { get; set; }
        public DateTime Created_At { get; set; }
    }
}