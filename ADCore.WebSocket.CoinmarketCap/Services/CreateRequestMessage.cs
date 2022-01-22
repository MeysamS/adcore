using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using ADCore.ApiReader.Context.Entities;
using ADCore.WebSocket.CoinmarketCap.Context;
using ADCore.WebSocket.CoinmarketCap.Models;
using Microsoft.EntityFrameworkCore;

namespace ADCore.WebSocket.CoinmarketCap.Services
{
    public class CreateRequestMessage : ICreateRequestMessage
    {
        private readonly WebSocketDbContext _dbContext;
        private readonly DbSet<Coin> _coins;

        public CreateRequestMessage(WebSocketDbContext dbContext)
        {
            _dbContext = dbContext;
            _coins = _dbContext.Set<Coin>();
        }
        public string Build()
        {
            var model = new Request
            {
                method = "subscribe",
                id = "price",
                data = new Data
                {
                    cryptoIds = _coins.Where(x => x.Deleted == false).Select(cid => cid.CoinMarketId).Take(200).ToList(),
                    index = null
                }
            };

            var message = JsonSerializer.Serialize(model);
            return message;
        }
    }
}
