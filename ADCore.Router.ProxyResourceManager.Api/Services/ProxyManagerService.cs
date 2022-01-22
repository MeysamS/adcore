using ADCore.Router.ProxyResourceManager.Api.Models;
using ADCore.Router.ProxyResourceManager.Api.Models.enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ADCore.Router.ProxyResourceManager.Api.Services
{
    public interface IProxyManagerService
    {
        Task<Response> UseProxyAsync(string domain = null, RateLimitPeriod RateLimitPeriod = RateLimitPeriod.None, int RateLimitCount = 0);
        Task SaveDBFileAsSampleAsync();
        Task LoadDBSnapShotAsync();
    }
    public class ProxyManagerService : IProxyManagerService
    {

        private readonly IProxyService _proxyService;
        private readonly IDomainService _domainService;
        private readonly IProxyUsedService _proxyUsedService;

        public ProxyManagerService(IProxyService proxyService, IDomainService domainService,
            IProxyUsedService proxyUsedService)
        {
            _proxyService = proxyService;
            _domainService = domainService;
            _proxyUsedService = proxyUsedService;
        }

        private Response UseIdleProxy()
        {
            var item = _proxyService.FindIdleProxy();
            if (item != null)
            {
                item.LastUsedTime = DateTime.Now;

                return new Response
                {
                    IsSuccess = true,
                    ProxyUrl = item.Url,
                    ProxyUserName = item.UserName,
                    ProxyPassword = item.Password
                };
            }

            return new Response { IsSuccess = false };
        }
        private Response UseProxyWithDomain(Proxy proxy, Domain domain)
        {
            proxy.LastUsedTime = DateTime.Now;
            _proxyUsedService.Add(new ProxyUsed
            {
                DomainName = domain.Name,
                ProxyId = proxy.Id,
                UsedTime = DateTime.Now
            });

            return new Response
            {
                IsSuccess = true,
                ProxyUrl = proxy.Url,
                ProxyUserName = proxy.UserName,
                ProxyPassword = proxy.Password
            };

        }

        private Response UseRateLimiteProxy(string domainName, RateLimitPeriod rateLimitPeriod, int rateLimitCount)
        {
            //todo: can create cash for groupedProxyUsedItems
            //todo: can create managed delete expired data in UseProxy

            var domain = _domainService.Find(domainName);
            var proxy = _proxyService.FindIdleProxy();

            if (domain == null)
            {
                domain = _domainService.Add(new Domain
                {
                    Name = domainName,
                    RateLimitCount = rateLimitCount,
                    RateLimitPeriod = rateLimitPeriod
                });
                Console.WriteLine($">>>>>>>> hi {domain.Name}, wellcome");
                return UseProxyWithDomain(proxy, domain);

            }

            if (domain.RateLimitPeriod != rateLimitPeriod ||
                domain.RateLimitCount != rateLimitCount)
            {
                domain.RateLimitPeriod = rateLimitPeriod;
                domain.RateLimitCount = rateLimitCount;
                Console.WriteLine($">>>>>>>>{domain.Name} Rate Limit Setting Changed.");
            }

            var proxyUsedItems = _proxyUsedService.GetAllInPeriod(domain.Name, rateLimitPeriod);
            var groupedProxyUsedItems = proxyUsedItems.GroupBy(g => g.ProxyId);

            var allProxies = _proxyService.GetAll();
            var proxiPoint = new Dictionary<Guid, int>();
            foreach (var item in allProxies) proxiPoint[item.Id] = rateLimitCount;
            foreach (IGrouping<Guid, ProxyUsed> group in groupedProxyUsedItems)
            {
                proxiPoint[group.Key] -= group.Count();
            }

            // find min 
            var max = proxiPoint.Max(m => m.Value);
            if (max <= 0)
            {
                //reject no proxy can used
                Console.WriteLine($">>>>>>>> Reject Request : all proxy for {domain.Name} send {rateLimitCount} times ...");
                return new Response { IsSuccess = false };
            }

            var maxProxyPointKey = proxiPoint.FirstOrDefault(x => x.Value == max).Key;

            proxy = _proxyService.Find(maxProxyPointKey);
            Console.WriteLine($">>>>>>>> managed proxy: {proxy.Id} for {domain.Name} sended {max} times");

            return UseProxyWithDomain(proxy, domain);
        }

        public async Task<Response> UseProxyAsync(string domain = null, RateLimitPeriod rateLimitPeriod = RateLimitPeriod.None, int rateLimitCount = 0)
        {
            if (MemoryContex.DB == null)
            {
                await LoadDBSnapShotAsync();
                Console.WriteLine(">>>>>>>> SnapShot Loaded!!!");
            }

            var res = new Response { IsSuccess = false };

            if (domain == null || rateLimitPeriod == RateLimitPeriod.None || rateLimitCount < 1)
            {
                res = UseIdleProxy();
                await CalculateForRunSnapShotAsync();
                return res;
            }

            res = UseRateLimiteProxy(domain, rateLimitPeriod, rateLimitCount);
            await CalculateForRunSnapShotAsync();
            return res;


        }

        private async Task CalculateForRunSnapShotAsync()
        {
            //todo: algorithm for Create Snapshot
            await CreateDBSnapShotAsync();
        }


        public async Task SaveDBFileAsSampleAsync()
        {
            await MemoryContex.SaveDBFileAsSampleAsync();
        }

        public async Task LoadDBSnapShotAsync()
        {
            await MemoryContex.LoadDBSnapShotAsync();
        }
        private async Task CreateDBSnapShotAsync()
        {
            await MemoryContex.CreateDBSnapShotAsync();
        }


    }


}
