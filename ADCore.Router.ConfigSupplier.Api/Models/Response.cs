﻿using ADCore.Router.ConfigSupplier.Api.Models.enums;

namespace ADCore.Router.ConfigSupplier.Api.Models
{
    public class Response
    {
        public bool FindKey { get; set; }
        public bool UseProxy { get; set; }
        public bool UseDns { get; set; }
        public RateLimitPeriod RateLimitPeriod { get; set; }
        public int RateLimitCount { get; set; }

    }


}
