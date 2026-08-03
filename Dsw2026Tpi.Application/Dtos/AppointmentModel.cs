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
        Guid AvailabilitySlotId,
        string PatientDni,
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
    public record SearchResponse(
        Guid AppointmentsId,
        string AppointmentsStatus,
        PatientSearchDto Patient,
        List<AvailabilitySlots> AvailabilitySlots
    );
    public record PatientSearchDto(
        long Dni,
        string Fullname
    );
    public record DoctorSearchDto(
        Guid DcotorId,
        string Name,
        SpecialtySearchDto Speciality
    );
    public record SpecialtySearchDto(
        Guid SpecialityId,
        string Name
    );
    public record PaginatedSearchResponse(
        int PageSize,
        int PageIndex,
        int Total,
        List<SearchResponse> data
    );
}