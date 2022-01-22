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
    public interface IProxyService
    {
        Proxy FindIdleProxy();
        List<Proxy> GetAll();
        Proxy Find(Guid Id);
    }
    public class ProxyService : IProxyService
    {
    

        //public Proxy Add(Proxy proxy)
        //{
        //    proxy.Id = Guid.NewGuid();
        //    MemoryContex.DB.Proxies.Add(proxy);
        //    return proxy;
        //}

        //public void Remove(Guid Id)
        //{
        //    var item = Find(Id);
        //    MemoryContex.DB.Proxies.Remove(item);
        //}

        public Proxy Find(Guid Id)
        {
            return MemoryContex.DB.Proxies.Find(s => s.Id == Id);
        }
        public List<Proxy> GetAll()
        {
            return MemoryContex.DB.Proxies;
        }


        public Proxy FindIdleProxy()
        {
            if (MemoryContex.DB.Proxies != null)
            {
                var datetime = MemoryContex.DB.Proxies.Min(b => b.LastUsedTime);
                var item = MemoryContex.DB.Proxies.Find(s => s.LastUsedTime == datetime);
                return item;
            }
            return null;
        }
 
 
    }
}
