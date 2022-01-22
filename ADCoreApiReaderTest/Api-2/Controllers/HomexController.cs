using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADCore.ApiReader;
using ADCore.ApiReader.Models;
using ADCore.ApiReader.Services;


namespace Api_2.Controllers
{
    [Route("api/[controller]")]
    public class HomexController : Controller
    {
        private readonly IApiClient _apiClient;

        public HomexController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        [HttpGet]
        public async Task<string> Get()
        {
            var apiModel1 = new RequestModel()
            {
                Url = "https://gorest.co.in/public/v1/comments",
            };
            var result1 = await _apiClient.GetAsync(apiModel1);

            return result1.IsSuccess ? result1.Data : "Error";
        }
    }
}
