using System;

namespace ADCore.Router.ProxyResourceManager.Api.Models
{
    public class ProxyUsed
    {
        public Guid ProxyId { get; set; }
        public string DomainName { get; set; }
        public DateTime UsedTime { get; set; }
    }



}
