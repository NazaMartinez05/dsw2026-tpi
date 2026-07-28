using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public enum AppointmentStatus { Booked, Cancelled, Attended, NoShow }

    public class Appointment : EntityBase
    {
        public Guid AvailabilitySlotId { get; init; }
        public AvailabilitySlots? AvailabilitySlots { get; private set; }
        public Guid PatientId { get; init; }
        public Patient? Patient { get; private set; }
        public string Reason { get; init; }
        public AppointmentStatus Status { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public DateTime? AttendedAt { get; private set; }


        #pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
        private Appointment() { }
        #pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.


        public Appointment(Guid availabilitySlotId, Guid patientId, string reason, Guid? id = null) : base(id)
        {
            AvailabilitySlotId = availabilitySlotId;
            PatientId = patientId;
            Reason = reason;
            Status = AppointmentStatus.Booked;
        }
    }
}
