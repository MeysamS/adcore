using System;
using System.Collections.Generic;

namespace ADCore.Router.DNSResourceManager.Api.Models
{
    public class DB
    {
        public DB()
        {
            DNSs = new List<DNS>();
            Domains = new List<Domain>();
            DNSUseds = new List<DNSUsed>();
        }

        public static DB CreateDbForSnapShot(DB db)
        {
            return new DB
            {
                SnapShotDate = DateTime.Now,
                DNSs = db.DNSs,
                Domains = db.Domains,
                DNSUseds = db.DNSUseds
            };
        }


        public DateTime SnapShotDate { get; set; }

        public List<DNS> DNSs { get; set; }
        public List<Domain> Domains { get; set; }
        public List<DNSUsed> DNSUseds { get; set; }

    }


}
