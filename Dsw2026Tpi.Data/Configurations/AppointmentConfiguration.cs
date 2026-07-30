using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");
            builder.HasQueryFilter(a => !a.Deleted);// filtro de borrado logico 
            builder.HasIndex(a => a.AvailabilitySlotId).IsUnique(); //regla RN03, no se signe el mismo horario a dos turnos distintos
        }
    }
}
