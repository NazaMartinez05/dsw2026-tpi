using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Data.Configurations
{
    public class AvailabilitySlotConfiguration : IEntityTypeConfiguration<AvailabilitySlots>
    {
        public void Configure(EntityTypeBuilder<AvailabilitySlots> builder)
        {
            builder.ToTable("AvailabilitySlots");
            builder.HasQueryFilter(a => !a.Deleted);

            builder.Property(a => a.StartTime).HasColumnType("time");
            builder.Property(a => a.EndTime).HasColumnType("time");
            builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);

            // UNIQUE(doctor_id, slot_date, start_time) según modelo de datos evita superposición (RN03)
            builder.HasIndex(a => new { a.DoctorId, a.SlotDate, a.StartTime })
                .IsUnique();

            builder.HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Si se borra la regla que generó el slot, el slot no se borra (puede tener cita asociada), solo se desvincula.
            builder.HasOne(a => a.AvailabilityRule)
                .WithMany()
                .HasForeignKey(a => a.AvailabilityRuleId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
