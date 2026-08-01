using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos;

public class AppointmentModel
{
    public record Request(
        Guid DoctorId,
        Guid AvailabilitySlotId,
        PatientDto Patient,
        string Reason
    );
    public record PatientDto(
        long Dni
    );
    public record Response(
        Guid Id,
        Guid DcotorId,
        Guid AvailabilitySlotId,
        long PatientDni,
        string Reason,
        string Status
    );
    public record PatientAppointmentResponse(
        Guid Id,
        Guid DoctorId,
        Guid AvailabilitySlotId,
        string Reason,
        string Status
        );
}