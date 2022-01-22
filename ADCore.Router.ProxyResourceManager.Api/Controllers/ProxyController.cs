using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADCore.Router.ProxyResourceManager.Api.Models;
using ADCore.Router.ProxyResourceManager.Api.Models.enums;
using ADCore.Router.ProxyResourceManager.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ADCore.Router.ProxyResourceManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProxyController : ControllerBase
    {

        private readonly IProxyManagerService _proxyManagerService;

        public ProxyController(IProxyManagerService proxyManagerService)
        {
            _proxyManagerService = proxyManagerService;
        }


        [HttpGet]
        public async Task<object> GetAsync(string domain = null, RateLimitPeriod RateLimitPeriod = RateLimitPeriod.None, int RateLimitCoun = 0)
        {
             return await _proxyManagerService.UseProxyAsync(domain, RateLimitPeriod, RateLimitCoun);
        }
    }
}
