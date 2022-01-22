using ADCore.ConfigSupplier.Models;
using ADCore.ConfigSupplier.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADCore.ConfigSupplier.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiReaderConfigController : ControllerBase
    {
        
       private IConfigService _configService;

        public ApiReaderConfigController(IConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public ApiReaderConfig Get(string key)
        {
            return _configService.ReadApiReaderConfig(key); 
        }

    }
}
