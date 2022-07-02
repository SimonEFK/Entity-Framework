using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_HospitalDatabase.Data.Models
{
    public class PatientMedicamentConfiguration : IEntityTypeConfiguration<PatientMedicament>
    {
        public void Configure(EntityTypeBuilder<PatientMedicament> builder)
        {
            builder.HasKey(x=> new {x.PatientId,x.MedicamentId});

            builder
                .HasOne(x => x.Patient)
                .WithMany(x => x.Prescriptions)
                .HasForeignKey(x => x.PatientId);

            builder
                .HasOne(x => x.Medicament)
                .WithMany(x => x.Prescriptions)
                .HasForeignKey(x => x.MedicamentId);


        }
    }
}
