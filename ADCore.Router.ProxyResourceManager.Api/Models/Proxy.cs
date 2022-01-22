using System;

namespace ADCore.Router.ProxyResourceManager.Api.Models
{
    public class Proxy
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime LastUsedTime { get; set; }
    }



}
