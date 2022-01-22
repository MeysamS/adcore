using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADCore.ApiReader;
using ADCore.ApiReader.Models;
using ADCore.ApiReader.Services;


namespace Api_1.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly IApiClient _apiClient;

        public HomeController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        [HttpGet]
        public async Task<string> Get()
        {
            var apiModel1 = new RequestModel()
            {
                Url = "https://reqres.in/api/products/1",
            };
            var result1 = await _apiClient.GetAsync(apiModel1);
            Thread.Sleep(5000);

            return result1.IsSuccess ? result1.Data : "Error";
        }
    }
}
