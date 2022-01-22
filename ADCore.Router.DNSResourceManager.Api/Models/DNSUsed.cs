using System;

namespace ADCore.Router.DNSResourceManager.Api.Models
{
    public class DNSUsed
    {
        public Guid DNSId { get; set; }
        public string DomainName { get; set; }
        public DateTime UsedTime { get; set; }
    }



}
