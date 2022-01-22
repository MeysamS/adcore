using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.Entities;
using ADCore.ApiReader.Models;
using ADCore.Mapper.Models;

namespace ADCore.ApiReader.Context.Services
{
    public interface ICoinService
    {
        void AddCoin(Coin coin);
        (bool, string) AddOrUpdateCoins(IList<Coin> coins);
        IList<Coin> GetAllCoin();
    }
}