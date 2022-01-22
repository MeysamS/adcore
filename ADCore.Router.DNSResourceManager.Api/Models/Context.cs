using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ADCore.Router.DNSResourceManager.Api.Models
{

    public static class MemoryContex
    {
        public static DB DB { get; set; }

        public static async Task SaveDBFileAsSampleAsync()
        {
            if (DB == null)
            {
                DB = new DB();

                var dns1 = new DNS()
                {
                    Id = Guid.NewGuid(),
                    CalExecuterUrl = "DNS1Url",
                    LastUsedTime = DateTime.Now
                };
                var dns2 = new DNS()
                {
                    Id = Guid.NewGuid(),
                    CalExecuterUrl = "DNS2Url",
                    LastUsedTime = DateTime.Now.AddMinutes(-100)
                };

                var dns3 = new DNS()
                {
                    Id = Guid.NewGuid(),
                    CalExecuterUrl = "DNS3Url",
                    LastUsedTime = DateTime.Now.AddMinutes(+100)
                };
                DB.DNSs.Add(dns1);
                DB.DNSs.Add(dns2);
                DB.DNSs.Add(dns3);
            }
            var db = DB.CreateDbForSnapShot(DB);

            var ser = JsonConvert.SerializeObject(db);

            StreamWriter sw = new StreamWriter(@"Data/SampleDBSnapShot.json");
            await sw.WriteAsync(ser);
            sw.Close();

        }

        internal static async Task LoadDBSnapShotAsync(bool force = false)
        {
            if (!force && DB != null) return;

            StreamReader sr = new StreamReader(@"Data/DBSnapShot.json");
            var data = await sr.ReadToEndAsync();
            sr.Close();
            DB result = JsonConvert.DeserializeObject<DB>(data);
            DB = result;
        }
        public static async Task CreateDBSnapShotAsync()
        {
            if (DB == null) await LoadDBSnapShotAsync(true);
            if (DB == null) DB = new DB();


            var db = DB.CreateDbForSnapShot(DB);

            var ser = JsonConvert.SerializeObject(db);

            StreamWriter sw = new StreamWriter(@"Data/DBSnapShot.json");
            await sw.WriteAsync(ser);
            sw.Close();

        }


    }


}
