using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public enum SlotStatus { Available, Booked, Blocked }

    public class AvailabilitySlots : EntityBase
    {
        public Guid DoctorId { get; init; }
        public Doctor? Doctor { get; private set; }
        public Guid? AvailabilityRuleId { get; init; }
        public AvailabilityRule? AvailabilityRule { get; private set; }
        public DateOnly SlotDate { get; init; }
        public TimeOnly StartTime { get; init; }
        public TimeOnly EndTime { get; init; }
        public SlotStatus Status { get; private set; }

        
        private AvailabilitySlots() { }

        public AvailabilitySlots(Guid doctorId, DateOnly slotDate, TimeOnly startTime, TimeOnly endTime,
            Guid? availabilityRuleId = null, Guid? id = null) : base(id)
        {
            DoctorId = doctorId;
            SlotDate = slotDate;
            StartTime = startTime;
            EndTime = endTime;
            AvailabilityRuleId = availabilityRuleId;
            Status = SlotStatus.Available;
        }
    }
}
