using ADCore.Router.ProxyResourceManager.Api.Models.enums;
using System;

namespace ADCore.Router.ProxyResourceManager.Api.Models
{
    public class Domain
    {
        public string Name { get; set; }
        public int RateLimitCount { get; set; }
        public RateLimitPeriod RateLimitPeriod { get; set; }
    }


}
