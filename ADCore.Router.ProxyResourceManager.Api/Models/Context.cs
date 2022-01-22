using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ADCore.Router.ProxyResourceManager.Api.Models
{

    public class DB
    {
        public DB()
        {
            Proxies = new List<Proxy>();
            Domains = new List<Domain>();
            ProxyUseds = new List<ProxyUsed>();
        }

        public static DB CreateDbForSnapShot(DB db)
        {
            return new DB
            {
                SnapShotDate = DateTime.Now,
                Proxies = db.Proxies,
                Domains = db.Domains,
                ProxyUseds = db.ProxyUseds
            };
        }


        public DateTime SnapShotDate { get; set; }

        public List<Proxy> Proxies { get; set; }
        public List<Domain> Domains { get; set; }
        public List<ProxyUsed> ProxyUseds { get; set; }

    }

    public static class MemoryContex
    {
        //public static bool Loaded { get; set; }
        //public void LoadFromFile()
        //{

        //}
        public static DB DB { get; set; }

        public static async Task SaveDBFileAsSampleAsync()
        {
            if (MemoryContex.DB == null)
            {
                MemoryContex.DB = new DB();

                var proxy1 = new Proxy()
                {
                    Id = Guid.NewGuid(),
                    Url = "proxy1Url",
                    UserName = "proxy1UserName",
                    Password = "proxy1pass",
                    LastUsedTime = DateTime.Now
                };
                var proxy2 = new Proxy()
                {
                    Id = Guid.NewGuid(),
                    Url = "proxy2Url",
                    UserName = "proxy2UserName",
                    Password = "proxy2pass",
                    LastUsedTime = DateTime.Now.AddMinutes(-100)
                };

                var proxy3 = new Proxy()
                {
                    Id = Guid.NewGuid(),
                    Url = "proxy3Url",
                    UserName = "proxy3UserName",
                    Password = "proxy3pass",
                    LastUsedTime = DateTime.Now.AddMinutes(+100)
                };
                MemoryContex.DB.Proxies.Add(proxy1);
                MemoryContex.DB.Proxies.Add(proxy2);
                MemoryContex.DB.Proxies.Add(proxy3);
            }
            var db = DB.CreateDbForSnapShot(MemoryContex.DB);

            var ser = JsonConvert.SerializeObject(db);

            StreamWriter sw = new StreamWriter(@"Data/SampleDBSnapShot.json");
            await sw.WriteAsync(ser);
            sw.Close();
 
        }

        internal static async Task LoadDBSnapShotAsync(bool force = false)
        {
            if (!force && MemoryContex.DB != null) return;

            StreamReader sr = new StreamReader(@"Data/DBSnapShot.json");
            var data = await sr.ReadToEndAsync();
            sr.Close();
            DB result = JsonConvert.DeserializeObject<DB>(data);
            MemoryContex.DB = result;
        }
        public static async Task CreateDBSnapShotAsync()
        {
            if (MemoryContex.DB == null) await LoadDBSnapShotAsync(true);
            if (MemoryContex.DB == null) MemoryContex.DB = new DB();


            var db = DB.CreateDbForSnapShot(MemoryContex.DB);

            var ser = JsonConvert.SerializeObject(db);

            StreamWriter sw = new StreamWriter(@"Data/DBSnapShot.json");
            await sw.WriteAsync(ser);
            sw.Close();
             
        }


    }


}
