using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_SalesDatabase.Data.Models
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder
                .HasKey(x => x.CustomerId);

            builder
                .Property(x => x.Name)
                .IsUnicode()
                .HasMaxLength(100);

            builder
                .Property(x => x.Email)
                .HasMaxLength(80)
                .IsUnicode();


        }
    }
}
