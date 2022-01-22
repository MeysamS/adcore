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
    public interface IDomainService
    {
        Domain Add(Domain item);
        Domain Find(string domain);
       
    }
    public class DomainService : IDomainService
    {
     
        public Domain Add(Domain item)
        {
            MemoryContex.DB.Domains.Add(item);
            return item;
        }

        //public void Remove(string  domain)
        //{
        //    var item = Find(domain);
        //    MemoryContex.DB.Domains.Remove(item);
        //}
 
        public Domain Find(string domain)
        {
            return MemoryContex.DB.Domains.Find(s => s.Name == domain);
        }
 

    }
}
