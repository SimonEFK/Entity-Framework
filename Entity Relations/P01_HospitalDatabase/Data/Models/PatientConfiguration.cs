using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_HospitalDatabase.Data.Models
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(x=>x.PatientId);

            builder
                .HasMany(x=>x.Visitations)
                .WithOne(x=>x.Patient)
                .HasForeignKey(x=>x.PatientId);

            builder
                .HasMany(x => x.Diagnoses)
                .WithOne(x => x.Patient)
                .HasForeignKey(x => x.PatientId);


            builder
                .Property(x => x.FirstName)
                .IsUnicode()
                .HasMaxLength(50);

            builder
                .Property(x => x.LastName)
                .IsUnicode()
                .HasMaxLength(50);

            builder
                .Property(x => x.Address)
                .IsUnicode()
                .HasMaxLength(250);

            builder
                .Property(x => x.Email)
                .IsUnicode(false)
                .HasMaxLength(80);

        }
    }
}
