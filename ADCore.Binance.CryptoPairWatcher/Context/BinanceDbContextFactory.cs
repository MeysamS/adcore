using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ADCore.Binance.CryptoPairWatcher.Context
{
    public class BinanceDbContextFactory: IDesignTimeDbContextFactory<BinanceDbContext>
    {
        public BinanceDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.dev.json")
                .Build();
            var builder = new DbContextOptionsBuilder<BinanceDbContext>();
            var cn = configuration.GetConnectionString("DefaultConnection");
            builder.UseNpgsql(cn);
            return new BinanceDbContext(builder.Options);
        }
    }
}
