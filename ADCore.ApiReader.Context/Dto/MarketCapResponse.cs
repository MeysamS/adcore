using System;
using System.Collections.Generic;
using System.Text;
using ADCore.ApiReader.Context.Entities;

namespace ADCore.ApiReader.Context.Dto
{
    public class MarketCapResponse
    {
        public List<Coin> Coins { get; set; }
    }
}
