using System;
using System.Collections.Generic;

namespace ADCore.ApiReader.Models
{
    public class RequestModel
    {
        public string Url { get; set; }
        public Dictionary<string, string> QueryStrings { get; set; }
        public Dictionary<string, string> HeaderStrings { get; set; }
        
    }
}
