using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_SalesDatabase.Data.Models
{
    public class SalesConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.HasKey(x => x.SaleId);

            builder
                .HasOne(x => x.Product)
                .WithMany(x => x.Sales);

            builder
                .HasOne(x => x.Customer)
                .WithMany(x => x.Sales);

            builder
                .HasOne(x => x.Store)
                .WithMany(x => x.Sales);

            builder
                .Property(x => x.Date)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
