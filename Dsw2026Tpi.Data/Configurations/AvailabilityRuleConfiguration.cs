using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Data.Configurations
{
    public class AvailabilityRuleConfiguration : IEntityTypeConfiguration<AvailabilityRule>
    {
        public void Configure(EntityTypeBuilder<AvailabilityRule> builder)
        {
            builder.ToTable("AvailabilityRules");
            builder.HasQueryFilter(a => !a.Deleted);

            builder.Property(a => a.StartTime).HasColumnType("time");
            builder.Property(a => a.EndTime).HasColumnType("time");

            // UNIQUE(doctor_id, year, month, day_of_week, start_time, end_time)
            builder.HasIndex(a => new { a.DoctorId, a.Year, a.Month, a.DayOfWeek, a.StartTime, a.EndTime })
                .IsUnique();

            builder.HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
