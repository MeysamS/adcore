using System;
using System.Collections.Generic;

namespace ADCore.ConfigSupplier.Models
{
    public class BaseConfig
    {
        public BaseConfig()
        {
            ConfigDetails = new List<ConfigDetail>();
        }
        public List<ConfigDetail> ConfigDetails { get; set; }
    }

    public class ConfigDetail
    {
        public string Key { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> QueryStrings { get; set; }
        public Dictionary<string, string> HeaderStrings { get; set; }
        public Dictionary<string, string> PropertySetsDic { get; set; }
    }
}
