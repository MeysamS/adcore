using System;
using System.Collections.Generic;
using System.Text;

namespace ADCore.WebSocket.CoinmarketCap.Models
{
    public class Request
    {
        public string method { get; set; }
        public string id { get; set; }
        public Data data { get; set; }

    }

    public class Data
    {
        public List<int> cryptoIds { get; set; }
        public string index { get; set; }
    }

   
}

