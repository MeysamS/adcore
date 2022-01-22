using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ADCore.ApiReader.Extensions;
using ADCore.ApiReader.Models;
using Microsoft.Extensions.Logging;

namespace ADCore.ApiReader.Services
{
    public class ApiClient : IApiClient
    {
        private readonly ILogger<ApiClient> _logger;
        private readonly HttpClient _httpClient;
        public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        public async Task<ResponseModel> GetAsync(RequestModel model,
            Proxy proxySetting = null)
        {
            ResponseModel result = new ResponseModel();
          //  HttpClient.DefaultProxy = new WebProxy();

            


            if (proxySetting != null)
            {
                var proxy = new WebProxy
                {
                    Address = new Uri(proxySetting.ProxyUrl),
                    BypassProxyOnLocal = false,
                };
                if (!string.IsNullOrEmpty(proxySetting.UserName))
                {
                    proxy.UseDefaultCredentials = false;
                    proxy.Credentials =
                        new NetworkCredential(userName: proxySetting.UserName,
                        password: proxySetting.Password);
                }
              //  HttpClient.DefaultProxy = proxy;
            }


          

            try
            {

                //HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.binance.com/bapi/asset/v2/public/asset/asset/get-all-asset");
                //req.KeepAlive = false;
                //req.Method = "GET";
                //req.ContentType = "application/json";
                //req.MediaType = "application/json";
                //req.Accept = "application/json";
                //req.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                //HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                //using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                //{
                //    var responseFromServer = reader.ReadToEnd();
                //}


                _httpClient.DefaultRequestHeaders.Accept.Clear();
                AddRequestHeader(_httpClient.DefaultRequestHeaders, model.HeaderStrings);
                string url = model.Url.AddQueryStringsToEndOfUrl(model.QueryStrings);
                result.Data = await _httpClient.GetStringAsync(url)
                   .ConfigureAwait(false);
                _logger.LogInformation($"Api ({url}) Get completed successfully in Date: {DateTime.UtcNow}");
            }
            catch (Exception e)
            {
                result.Exception = e;
                _logger.LogError(e.Message);
            }
            return result;
        }

        private void AddRequestHeader(HttpRequestHeaders requestHeaders, Dictionary<string, string> headerString)
        {
            if (headerString != null)
            {
                foreach (var item in headerString)
                {
                    requestHeaders.Add(item.Key, item.Value);
                }
            }
        }

    }
}
