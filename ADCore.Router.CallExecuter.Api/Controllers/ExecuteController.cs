using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ADCore.ApiReader.Models;
using ADCore.ApiReader.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Proxy.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExecuteController : ControllerBase
    {
        public IApiClient _ApiClient { get; set; }
        public ExecuteController(IApiClient apiClient)
        {
            _ApiClient = apiClient;
        }


        [HttpGet]
        public async Task<object> GetAsync(string url)
        {
            if (string.IsNullOrEmpty(url)) return new {msg="url not found." };

            url = UrlModifier(url, HttpContext.Request.Query);

            var headerStrings = ReadHeaderAndSetToDic(HttpContext);

            var result = await _ApiClient.GetAsync(
             new RequestModel
             {
                 Url = url,
                 HeaderStrings = headerStrings
             });

            return result.Data;
        }

        private string UrlModifier(string url, IQueryCollection query)
        {
            foreach (var item in HttpContext.Request.Query)
            {
                if (item.Key != "url") url += $"&{item.Key}={item.Value}";
            }
            return url;
        }

        private Dictionary<string, string> ReadHeaderAndSetToDic(HttpContext context)
        {

            var headerStrings = new Dictionary<string, string>();

            foreach (var header in context.Request.Headers)
            {
                if (header.Key.ToLower() != "host")
                    headerStrings[header.Key] = header.Value;
            }
            return headerStrings;
        }

    }
}
