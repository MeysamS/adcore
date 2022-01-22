using ADCore.Router.DNSResourceManager.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ADCore.Router.DNSResourceManager.Api.Services
{
    public interface IDNSService
    {
        DNS FindIdleDns();
        List<DNS> GetAll();
        DNS Find(Guid Id);
    }
    public class DNSService : IDNSService
    {
        public DNS Find(Guid Id)
        {
            return MemoryContex.DB.DNSs.Find(s => s.Id == Id);
        }
        public List<DNS> GetAll()
        {
            return MemoryContex.DB.DNSs;
        }


        public DNS FindIdleDns()
        {
            if (MemoryContex.DB.DNSs != null)
            {
                var datetime = MemoryContex.DB.DNSs.Min(b => b.LastUsedTime);
                var item = MemoryContex.DB.DNSs.Find(s => s.LastUsedTime == datetime);
                return item;
            }
            return null;
        }


    }
}
