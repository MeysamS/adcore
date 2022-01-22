using ADCore.Router.DNSResourceManager.Api.Models.enums;
using System;

namespace ADCore.Router.DNSResourceManager.Api.Models
{
    public class Domain
    {
        public string Name { get; set; }
        public int RateLimitCount { get; set; }
        public RateLimitPeriod RateLimitPeriod { get; set; }
    }


}
