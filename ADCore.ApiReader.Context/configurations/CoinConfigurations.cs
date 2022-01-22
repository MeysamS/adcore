using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADCore.ApiReader.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADCore.ApiReader.Context.configurations
{
    public class CoinConfigurations : IEntityTypeConfiguration<Coin>
    {
        public void Configure(EntityTypeBuilder<Coin> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(450).IsRequired();

        }
    }
}