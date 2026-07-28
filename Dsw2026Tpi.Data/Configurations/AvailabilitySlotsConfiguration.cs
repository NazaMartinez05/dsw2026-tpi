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
        }
    }
}
