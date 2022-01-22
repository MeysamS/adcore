using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.Entities;
using ADCore.WebSocket.CoinmarketCap.Context;
using ADCore.WebSocket.CoinmarketCap.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebSocketSharp.NetCore;

namespace ADCore.WebSocket.CoinmarketCap.Services
{
    public class WebSocketManager : IWebSocketManager
    {
        private readonly ILogger<WebSocketManager> _logger;
        private readonly ICoinMarketCapPriceService _coinMarketCapPrice;
        private readonly IUnitOfWork _unitOfWork;

        private int count = 0;
        public WebSocketManager(IUnitOfWork unitOfWork, ILogger<WebSocketManager> logger, ICoinMarketCapPriceService coinMarketCapPriceService)
        {
            _unitOfWork = unitOfWork;
            _coinMarketCapPrice = coinMarketCapPriceService;
            _logger = logger;
        }

        public async Task ExecuteAsync(string url, string message)
        {
            try
            {

                WebSocketSharp.NetCore.WebSocket socket = new WebSocketSharp.NetCore.WebSocket(url);
                socket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                var coins = _coinMarketCapPrice.GetCoins();
                await socket.ConnectAsync();
                socket.OnOpen += (sender, eventArgs) =>
                {
                    socket.SendAsync(message, async compelete =>
                    {
                        _logger.LogInformation(compelete.ToString());
                        Console.WriteLine(compelete.ToString());
                    });
                };

                socket.OnMessage += (sender, e) =>
                {

                    Result result = JsonSerializer.Deserialize<Result>(e.Data);
                    var coin = coins.FirstOrDefault(x => x.CoinMarketId == result.d.cr.id);

                    if (coin != null)
                    {
                        var price = new CoinPrice
                        {
                            Id = new Guid(),
                            Price = result.d.cr.p,
                            CoinId = coin.Id,
                            Created_At = DateTime.Now
                        };
                        _coinMarketCapPrice.Add(price);
                        count++;
                        if (count > 100)
                        {
                            _unitOfWork.SaveChanges();
                            count = 0;
                        }
                    }
                    _logger.LogInformation($"Recive Data : {e.Data}");
                };

                socket.OnClose += (sender, e) =>
                    socket.Connect();

                socket.OnError += (sender, e) =>
                    socket.Connect();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                _logger.LogError($"Recive Error : { e.Message }");
                //Console.WriteLine(e);
                //throw;
            }
        }

    }
}
