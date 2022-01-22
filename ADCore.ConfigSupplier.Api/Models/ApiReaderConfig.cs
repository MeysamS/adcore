using System;
using System.Collections.Generic;

namespace ADCore.ConfigSupplier.Models
{
    public class ApiReaderConfig
    {
        public string Url { get; set; }
        public Dictionary<string, string> QueryStrings { get; set; }
        public Dictionary<string, string> HeaderStrings { get; set; }
    }


}
