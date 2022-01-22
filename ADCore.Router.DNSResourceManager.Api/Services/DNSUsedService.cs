using ADCore.Router.DNSResourceManager.Api.Models;
using ADCore.Router.DNSResourceManager.Api.Models.enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ADCore.Router.DNSResourceManager.Api.Services
{
    public interface IDNSUsedService
    {
        DNSUsed Add(DNSUsed item);
        DNSUsed Find(Guid dnsId, string domainName);
        List<DNSUsed> GetAllInPeriod(string domainName, RateLimitPeriod rateLimitPeriod);
    }
    public class DNSUsedService : IDNSUsedService
    {

        public DNSUsed Add(DNSUsed item)
        {
            MemoryContex.DB.DNSUseds.Add(item);
            return item;
        }


        public DNSUsed Find(Guid dnsId, string domainName)
        {
            return MemoryContex.DB.DNSUseds.Find(s => s.DNSId == dnsId && s.DomainName == domainName);
        }
        public List<DNSUsed> GetAllInPeriod(string domainName, RateLimitPeriod rateLimitPeriod)
        {
            var res = new List<DNSUsed>();
            var query = MemoryContex.DB.DNSUseds.Where(pu => pu.DomainName == domainName).AsQueryable();
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
