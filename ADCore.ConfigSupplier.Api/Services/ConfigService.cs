using ADCore.ConfigSupplier.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ADCore.ConfigSupplier.Services
{

    public class ConfigService : IConfigService
    {
        public ApiReaderConfig ReadApiReaderConfig(string key)
        {
            var det = GetConfig(key);
            if (det == null) return new ApiReaderConfig() {Url="Not Found." };

            return new ApiReaderConfig()
            {
                Url = det.Url,
                HeaderStrings = det.HeaderStrings,
                QueryStrings = det.QueryStrings
            };

        }
        public MapperConfig ReadMapperConfig(string key)
        {
            var det = GetConfig(key);
            if (det == null) 
                return new MapperConfig() {
               PropertySetsDic= new Dictionary<string, string>()
                {
                     { "Result", "Not Found..." },
                },
            };

            return new MapperConfig()
            {
                PropertySetsDic = det.PropertySetsDic,
            };

        }

        public void SaveDefaultBaseConfig()
        {
            BaseConfig baseConfig = new BaseConfig();
            baseConfig.ConfigDetails.Add(new ConfigDetail()
            {
                Key = "firstobject",
                Url = "FirstObject.Com",
                HeaderStrings = new Dictionary<string, string>()
                {
                     { "def", "c43b47000982" },
                     { "Accepts", "application/json" },
                },
                QueryStrings = new Dictionary<string, string>()
                {
                     { "start", "100" },
                     { "limit", "500" },
                     { "convert", "USD" }
                },
                PropertySetsDic = new Dictionary<string, string>()
                {
                     { "Property1", "Prop21" },
                     { "Property2", "Prop20" },
                     { "Property3", "Prop7" },
                },

            });

            baseConfig.ConfigDetails.Add(new ConfigDetail()
            {
                Key = "coinmarketcap.com",
                Url = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest",
                HeaderStrings = new Dictionary<string, string>()
                {
                     { "X-CMC_PRO_API_KEY", "9a518333-7cb0-4c5f-8525-c43b47000982" },
                     { "Accepts", "application/json" },
                },
                QueryStrings = new Dictionary<string, string>()
                {
                     { "start", "1" },
                     { "limit", "5000" },
                     { "convert", "USD" }
                },
                PropertySetsDic = new Dictionary<string, string>()
                {
                     { "Coins#Name", "data#name" },
                     { "Coins#Symbol", "data#symbol" },
                     { "Coins#Slug", "data#slug" },
                },
            });

            var ser = JsonConvert.SerializeObject(baseConfig);

            TextWriter writer;
            using (writer = new StreamWriter(@"ConfigData/DBaseConfig.json", append: false))
            {
                writer.WriteLine(ser);
            }

        }

        private BaseConfig ReadBaseConfig()
        {
            using (StreamReader sr = new StreamReader(@"ConfigData/BaseConfig.json"))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    BaseConfig result = serializer.Deserialize<BaseConfig>(reader);
                    return result;
                }
            }
        }

        private ConfigDetail GetConfig(string key)
        {
            var baseConf = ReadBaseConfig();
            return baseConf.ConfigDetails.FirstOrDefault(b => b.Key == key);
        }
         
    }
}
