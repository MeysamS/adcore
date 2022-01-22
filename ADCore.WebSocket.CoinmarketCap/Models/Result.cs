using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ADCore.WebSocket.CoinmarketCap.Models
{
    public class Cr
    {

        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("d")]
        public double d { get; set; }

        [JsonProperty("p1h")]
        public double p1h { get; set; }

        [JsonProperty("p24h")]
        public double p24h { get; set; }

        [JsonProperty("p7d")]
        public double p7d { get; set; }

        [JsonProperty("p30d")]
        public object p30d { get; set; }

        [JsonProperty("ts")]
        public object ts { get; set; }

        [JsonProperty("as")]
        public object @as { get; set; }

        [JsonProperty("fmc")]
        public object fmc { get; set; }

        [JsonProperty("mc")]
        public double mc { get; set; }

        [JsonProperty("mc24hpc")]
        public object mc24hpc { get; set; }

        [JsonProperty("vol24hpc")]
        public object vol24hpc { get; set; }

        [JsonProperty("fmc24hpc")]
        public object fmc24hpc { get; set; }

        [JsonProperty("p")]
        public double p { get; set; }

        [JsonProperty("v")]
        public double v { get; set; }
    }

    public class D
    {

        [JsonProperty("cr")]
        public Cr cr { get; set; }

        [JsonProperty("t")]
        public long t { get; set; }
    }

    public class Result
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("d")]
        public D d { get; set; }

        [JsonProperty("s")]
        public string s { get; set; }
    }

}
