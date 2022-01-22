using System;

namespace ADCore.Router.DNSResourceManager.Api.Models
{
    public class DNS
    {
        public Guid Id { get; set; }
        public string CalExecuterUrl { get; set; }
        public DateTime LastUsedTime { get; set; }
    }



}
