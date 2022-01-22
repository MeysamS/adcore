using ADCore.Router.DNSResourceManager.Api.Models;
using ADCore.Router.DNSResourceManager.Api.Models.enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ADCore.Router.DNSResourceManager.Api.Services
{
    public interface IDNSManagerService
    {
        Task<Response> UseDnsAsync(string domain = null, RateLimitPeriod RateLimitPeriod = RateLimitPeriod.None, int RateLimitCount = 0);
        Task SaveDBFileAsSampleAsync();
        Task LoadDBSnapShotAsync();
    }
    public class DNSManagerService : IDNSManagerService
    {

        private readonly IDNSService _dnsService;
        private readonly IDomainService _domainService;
        private readonly IDNSUsedService _dnsUsedService;

        public DNSManagerService(IDNSService dnsService, IDomainService domainService,
            IDNSUsedService dnsUsedService)
        {
            _dnsService = dnsService;
            _domainService = domainService;
            _dnsUsedService = dnsUsedService;
        }

        private Response UseIdleDNS()
        {
            var item = _dnsService.FindIdleDns();
            if (item != null)
            {
                item.LastUsedTime = DateTime.Now;

                return new Response
                {
                    IsSuccess = true,
                    CalExecuterUrl = item.CalExecuterUrl,
                };
            }

            return new Response { IsSuccess = false };
        }
        private Response UseDNSWithDomain(DNS dns, Domain domain)
        {
            dns.LastUsedTime = DateTime.Now;
            _dnsUsedService.Add(new DNSUsed
            {
                DomainName = domain.Name,
                DNSId = dns.Id,
                UsedTime = DateTime.Now
            });

            return new Response
            {
                IsSuccess = true,
                CalExecuterUrl = dns.CalExecuterUrl,
            };

        }

        private Response UseRateLimiteDNS(string domainName, RateLimitPeriod rateLimitPeriod, int rateLimitCount)
        {
            //todo: can create cash for groupedDnsUsedItems
            //todo: can create managed delete expired data in UseDns

            var domain = _domainService.Find(domainName);
            var dns = _dnsService.FindIdleDns();

            if (domain == null)
            {
                domain = _domainService.Add(new Domain
                {
                    Name = domainName,
                    RateLimitCount = rateLimitCount,
                    RateLimitPeriod = rateLimitPeriod
                });
                Console.WriteLine($">>>>>>>> hi {domain.Name}, wellcome");
                return UseDNSWithDomain(dns, domain);

            }

            if (domain.RateLimitPeriod != rateLimitPeriod ||
                domain.RateLimitCount != rateLimitCount)
            {
                domain.RateLimitPeriod = rateLimitPeriod;
                domain.RateLimitCount = rateLimitCount;
                Console.WriteLine($">>>>>>>>{domain.Name} Rate Limit Setting Changed.");
            }

            var dnsUsedItems = _dnsUsedService.GetAllInPeriod(domain.Name, rateLimitPeriod);
            var groupedDnsUsedItems = dnsUsedItems.GroupBy(g => g.DNSId);

            var allDNSs = _dnsService.GetAll();
            var DnsPoint = new Dictionary<Guid, int>();
            foreach (var item in allDNSs) DnsPoint[item.Id] = rateLimitCount;
            foreach (IGrouping<Guid, DNSUsed> group in groupedDnsUsedItems)
            {
                DnsPoint[group.Key] -= group.Count();
            }

            // find min 
            var max = DnsPoint.Max(m => m.Value);
            if (max <= 0)
            {
                Console.WriteLine($">>>>>>>> Reject Request : all dns for {domain.Name} send {rateLimitCount} times ...");
                return new Response { IsSuccess = false };
            }

            var maxDnsPointKey = DnsPoint.FirstOrDefault(x => x.Value == max).Key;

            dns = _dnsService.Find(maxDnsPointKey);
            Console.WriteLine($">>>>>>>> managed dns: {dns.Id} for {domain.Name} sended {max} times");

            return UseDNSWithDomain(dns, domain);
        }

        public async Task<Response> UseDnsAsync(string domain = null, RateLimitPeriod rateLimitPeriod = RateLimitPeriod.None, int rateLimitCount = 0)
        {
            if (MemoryContex.DB == null)
            {
                await LoadDBSnapShotAsync();
                Console.WriteLine(">>>>>>>> SnapShot Loaded!!!");
            }

            var res = new Response { IsSuccess = false };

            if (domain == null || rateLimitPeriod == RateLimitPeriod.None || rateLimitCount < 1)
            {
                res = UseIdleDNS();
                await CalculateForRunSnapShotAsync();
                return res;
            }

            res = UseRateLimiteDNS(domain, rateLimitPeriod, rateLimitCount);
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
