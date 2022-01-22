using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADCore.WebSocket.CoinmarketCap;

namespace ADCore.ApiReader.Api.Controllers
{
    /// <summary>
    /// web socket coinmarketcap
    /// </summary>
    [Route("api/[controller]")]
    public class wsCMC : Controller
    {
        private readonly IWebSocketHandler _socketHandler;
        public wsCMC(IWebSocketHandler socketHandler)
        {
            _socketHandler = socketHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _socketHandler.Run("wss://stream.coinmarketcap.com/price/latest");

            return Content("");
        }
    }
}
