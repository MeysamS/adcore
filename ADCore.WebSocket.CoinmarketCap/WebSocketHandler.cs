using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ADCore.WebSocket.CoinmarketCap.Services;

namespace ADCore.WebSocket.CoinmarketCap
{

    public interface IWebSocketHandler
    {
        Task Run(string url);
    }

    public class WebSocketHandler : IWebSocketHandler
    {
        private readonly IWebSocketManager _webSocketManager;
        private readonly ICreateRequestMessage _createRequestMessage;


        public WebSocketHandler(IWebSocketManager webSocketManager, ICreateRequestMessage createRequestMessage)
        {
            _webSocketManager = webSocketManager;
            _createRequestMessage = createRequestMessage;
        }
        public async Task Run(string url)
        {
            string message = _createRequestMessage.Build();
            await _webSocketManager.ExecuteAsync(url, message);
        }
    }
}
