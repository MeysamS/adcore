using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ADCore.WebSocket.CoinmarketCap.Models;

namespace ADCore.WebSocket.CoinmarketCap.Services
{
    public interface IWebSocketManager
    {
        Task ExecuteAsync(string url, string message);
    }
}
