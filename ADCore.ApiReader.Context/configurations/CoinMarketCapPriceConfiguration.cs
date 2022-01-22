using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADCore.ApiReader.Context.Configurations
{
    public class CoinMarketCapPriceConfiguration : IEntityTypeConfiguration<CoinPrice>
    {
        public void Configure(EntityTypeBuilder<CoinPrice> builder)
        {
            builder.HasOne(x => x.Coin)
                .WithMany(x => x.CoinPrices)
                .HasForeignKey(x => x.CoinId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}