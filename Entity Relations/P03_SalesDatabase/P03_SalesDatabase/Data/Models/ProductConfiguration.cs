using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_SalesDatabase.Data.Models
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.ProductId);

            builder.Property(x => x.Name)
                .IsUnicode()
                .HasMaxLength(50);
            builder
                .Property(x => x.Description)
                .HasMaxLength(250)
                .HasDefaultValue("No description");
        }
    }
}
