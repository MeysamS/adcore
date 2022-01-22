using ADCore.Router.CallManager.Models.enums;

namespace ADCore.Router.CallManager.Models
{
    public class RouterConfigSupplierResponse
    {
        public bool FindKey { get; set; }
        public bool UseProxy { get; set; }
        public bool UseDns { get; set; }
        public RateLimitPeriod RateLimitPeriod { get; set; }
        public int RateLimitCount { get; set; }
    }


}
