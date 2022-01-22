using ADCore.ConfigSupplier.Models;
using ADCore.ConfigSupplier.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADCore.ConfigSupplier.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MapperConfigController : ControllerBase
	{
		private IConfigService _configService;

		public MapperConfigController(IConfigService configService)
		{
			_configService = configService;
		}

		[HttpGet]
		public MapperConfig Get(string key)
		{
			return _configService.ReadMapperConfig(key); ;
		}

	}
}
