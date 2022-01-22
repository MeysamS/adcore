using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADCore.Router.ConfigSupplier.Api.Models;
using ADCore.Router.ConfigSupplier.Api.Services;
using Microsoft.AspNetCore.Mvc;
 

namespace ADCore.Router.ConfigSupplier.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RouterConfigController : ControllerBase
    {

        private IConfigService _configService;

        public RouterConfigController(IConfigService configService)
        {
            _configService = configService;
        }
     
        [HttpGet]
        public async Task<Response> GetAsync(string key)
        {
             return await _configService.ReadRouterConfigAsync(key);
        }



    }
}
