using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADCore.Router.DNSResourceManager.Api.Models.enums;
using ADCore.Router.DNSResourceManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADCore.Router.DNSResourceManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DNSController : ControllerBase
    {

        private readonly IDNSManagerService _dnsManagerService;

        public DNSController(IDNSManagerService dnsManagerService)
        {
            _dnsManagerService = dnsManagerService;
        }


        [HttpGet]
        public async Task<object> GetAsync(string domain = null, RateLimitPeriod RateLimitPeriod = RateLimitPeriod.None, int RateLimitCoun = 0)
        {
            return await _dnsManagerService.UseDnsAsync(domain, RateLimitPeriod, RateLimitCoun);
        }
    }
}
