using System;
using System.Collections.Generic;

namespace ADCore.Binance.CryptoPairWatcher.Models
{
    public class ApiReaderConfigResponse
    {
        public string Url { get; set; }
        public Dictionary<string, string> QueryStrings { get; set; }
        public Dictionary<string, string> HeaderStrings { get; set; }
    }


}
