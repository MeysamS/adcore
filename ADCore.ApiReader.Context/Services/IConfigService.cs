using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.Dto;
using ADCore.ApiReader.Context.Entities;
using ADCore.ApiReader.Models;
using ADCore.Mapper.Models;

namespace ADCore.ApiReader.Context.Services
{
    public interface IConfigService
    {
        Task<RequestModel> GetApiConfig(string key, string url);
        Task<Config> GetMapConfig(string key, string url);
        Task<string> CallApi(RequestModel request);
        MarketCapResponse MapToModel(string jsonData, Config config);
    }
}