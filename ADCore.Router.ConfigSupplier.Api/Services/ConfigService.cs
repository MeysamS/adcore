using ADCore.Router.ConfigSupplier.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ADCore.Router.ConfigSupplier.Api.Services
{

    public class ConfigService : IConfigService
    {
        public async Task<Response> ReadRouterConfigAsync(string key)
        {
            var det =await GetConfigAsync(key);
            if (det == null) return new Response() { FindKey = false };

            return new Response
            {
                FindKey = true,
                UseProxy = det.UseProxy,
                UseDns = det.UseDns,
                RateLimitPeriod = det.RateLimitPeriod,
                RateLimitCount = det.RateLimitCount
            };

        }
         
        private async  Task<BaseConfig> ReadBaseConfigAsync()
        {
            StreamReader sr = new StreamReader(@"ConfigData/BaseConfig.json");
            var data =await sr.ReadToEndAsync();
            sr.Close();
            BaseConfig result = JsonConvert.DeserializeObject<BaseConfig>(data);
            return result;
        }



        private async Task<ConfigDetail> GetConfigAsync(string key)
        {
            var baseConf =await ReadBaseConfigAsync();
            return  baseConf.ConfigDetails.FirstOrDefault(b => b.Key == key);
        }

    }
}
