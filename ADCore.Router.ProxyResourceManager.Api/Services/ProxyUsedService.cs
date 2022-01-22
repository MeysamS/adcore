using ADCore.Router.ProxyResourceManager.Api.Models;
using ADCore.Router.ProxyResourceManager.Api.Models.enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ADCore.Router.ProxyResourceManager.Api.Services
{
    public interface IProxyUsedService
    {
        ProxyUsed Add(ProxyUsed item);
        ProxyUsed Find(Guid proxyId, string domainName);
        List<ProxyUsed> GetAllInPeriod(string domainName, RateLimitPeriod rateLimitPeriod);
    }
    public class ProxyUsedService : IProxyUsedService
    {

        public ProxyUsed Add(ProxyUsed item)
        {
            MemoryContex.DB.ProxyUseds.Add(item);
            return item;
        }


        public ProxyUsed Find(Guid proxyId, string domainName)
        {
            return MemoryContex.DB.ProxyUseds.Find(s => s.ProxyId == proxyId && s.DomainName == domainName);
        }
        public List<ProxyUsed> GetAllInPeriod(string domainName, RateLimitPeriod rateLimitPeriod)
        {
            var res = new List<ProxyUsed>();
            var query = MemoryContex.DB.ProxyUseds.Where(pu => pu.DomainName == domainName).AsQueryable();
            var time = DateTime.Now;
            switch (rateLimitPeriod)
            {
                case RateLimitPeriod.PerSecond:
                    time = DateTime.Now.AddSeconds(-1);
                    res = query.Where(pu => pu.UsedTime >= time).ToList();
                    break;

                case RateLimitPeriod.PerMinute:
                    time = DateTime.Now.AddMinutes(-1);
                    break;

                case RateLimitPeriod.PerHour:
                    time = DateTime.Now.AddHours(-1);
                    break;

                case RateLimitPeriod.PerDay:
                    time = DateTime.Now.AddDays(-1);
                    break;

                case RateLimitPeriod.PerWeek:
                    time = DateTime.Now.AddDays(-7);
                    break;

                case RateLimitPeriod.PerMonth:
                    time = DateTime.Now.AddMonths(-1);
                    break;

                case RateLimitPeriod.PerYear:
                    time = DateTime.Now.AddYears(-1);
                    break;

                default:
                    break;
            }
            res = query.Where(pu => pu.UsedTime >= time).ToList();

            return res;

        }




    }
}
