using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Data;
using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Dsw2026Tpi.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly Dsw2026TpiDbContext _context; //se trae la base de datos 
    public AppointmentService(Dsw2026TpiDbContext context)
    {
        _context = context;
    }
    public async Task<AppointmentModel.Response> CreateAppointmentAsync(AppointmentModel.Request request)
    {
        var validation = new ValidationException();

        if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Length < 5)
            validation.WithDetail(nameof(request.Reason), "El motivo es obligatorio y debe contener al menos 5 caracteres");

        string dniString = request.Patient.Dni.ToString();
        if (dniString.Length < 7 || dniString.Length > 10)
            validation.WithDetail("patient.dni", "El DNI debe contener entre 7 y 10 dígitos");

        if (validation.Error.Details.Count > 0) throw validation;

        bool doctorExists = await _context.Doctors.AnyAsync(d => d.Id == request.DoctorId);
        if (!doctorExists)
            throw new ConflictException("ENTITY_NOTFOUND", "El doctor ID no existe")
                .WithDetail("availabilitySlotId", "slot_unavailable");

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Dni == dniString) ?? throw new ConflictException("ENTITY_NOTFOUND", "El paciente no se encuentra registrado")
                .WithDetail("availabilitySlotId", "slot_unavailable");

        bool isSlotTaken = await _context.Appointments
            .AnyAsync(a => a.AvailabilitySlotId == request.AvailabilitySlotId);
        if (isSlotTaken)
            throw new ConflictException("APPOINTMENT_CONFLICT", "El horario seleccionado ya se encuentra reservado")
                .WithDetail("availabilitySlotId", "slot_unavailable");

        var slot = await _context.AvailabilitySlots.FirstOrDefaultAsync(s => s.Id == request.AvailabilitySlotId);
        if (slot == null)
            throw new EntityNotFoundException(nameof(AvailabilitySlots));

        var appointment = new Appointment(slot.Id, patient.Id, request.Reason);

        slot.MarkAsDeleted();

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return new AppointmentModel.Response(
            appointment.Id,
            appointment.AvailabilitySlotId,
            appointment.Patient?.Dni!,
            appointment.Reason,
            appointment.Status.ToString());
    }
    public async Task<List<AppointmentModel.PatientAppointmentResponse>> GetPatientAppointmentsAsync(long dni)
    {
        string dniString = dni.ToString();
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Dni == dniString);
        if (patient == null)
            throw new EntityNotFoundException(nameof(Patient));

        var activeAppointments = await _context.Appointments
            .Where(a => a.PatientId == patient.Id && a.Status == AppointmentStatus.Booked)
            .ToListAsync();

        var responseList = new List<AppointmentModel.PatientAppointmentResponse>();
        foreach (var appointment in activeAppointments)
        {
            responseList.Add(new AppointmentModel.PatientAppointmentResponse(
                appointment.Id,
                Guid.Empty,
                appointment.AvailabilitySlotId,
                appointment.Reason,
                appointment.Status.ToString()
            ));
        }
        return responseList;
    }
    public async Task CancelAppointmentAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            throw new EntityNotFoundException(nameof(Appointment));

        if (appointment.Status != AppointmentStatus.Booked)
            throw new ConflictException("APPOINTMENT_STATUS_CONFLICT", "Solo se pueden cancelar turnos que estén en estado Reservado")
                .WithDetail("status", "invalid_status_for_cancellation");

        appointment.Status = AppointmentStatus.Cancelled;
        await _context.SaveChangesAsync();
    }

    public async Task<AppointmentModel.PaginatedSearchResponse> GetAppointmentsByDateAsync(DateTime date, int pageSize = 10, int pageIndex = 1)
    {
        var targetDate = DateOnly.FromDateTime(date);

        var query = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.AvailabilitySlots)
                .ThenInclude(slot => slot!.Doctor!)
                    .ThenInclude(doc => doc.Speciality)
            .Where(a => a.Status == AppointmentStatus.Booked);

        int total = await query.CountAsync();

        var appointments = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dataList = new List<AppointmentModel.SearchResponse>();

        List<AvailabilitySlots> availabilitySlots = [.. _context.AvailabilitySlots.Where(x => x.SlotDate == targetDate)];
        foreach (var a in appointments)
        {    
            dataList.Add(new AppointmentModel.SearchResponse(
                a.Id,
                a.Status.ToString(),
                new AppointmentModel.PatientSearchDto(
                    long.Parse(a!.Patient!.Dni),
                    a!.Patient!.FullName!
                ),
                availabilitySlots
           ));     
        }
        return new AppointmentModel.PaginatedSearchResponse(pageSize, pageIndex, total, dataList);   
    }
    public async Task<AppointmentModel.PaginatedSearchResponse> SearchAppointmentAsync(
        Guid? specialityId, Guid? doctorId, string? dni, DateTime date, int pageSize = 10, int pageIndex = 1)
    {
        var targetDate = DateOnly.FromDateTime(date);

        var query = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.AvailabilitySlots)
                .ThenInclude(slot => slot!.Doctor!)
                    .ThenInclude(doc => doc.Speciality)
            .Where(a => a.Status == AppointmentStatus.Booked);
        if (specialityId.HasValue)
        {
            query = query.Where(a => a.AvailabilitySlots!.Doctor!.SpecialityId == specialityId.Value);
        }
        if (doctorId.HasValue)
        {
            query = query.Where(a => a.AvailabilitySlots!.DoctorId == doctorId.Value);
        }

        query = query.Where(a => a.AvailabilitySlots!.SlotDate == targetDate);

        int total = await query.CountAsync();
        var appointments = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var dataList = new List<AppointmentModel.SearchResponse>();
        List<AvailabilitySlots> availabilitySlots = [.. _context.AvailabilitySlots.Where(x => x.SlotDate == targetDate)];
        foreach (var a in appointments)
        {
            dataList.Add(new AppointmentModel.SearchResponse(
                a.Id,
                a.Status.ToString(),
                new AppointmentModel.PatientSearchDto(
                    long.Parse(a!.Patient!.Dni),
                    a!.Patient!.FullName!
                ),
                availabilitySlots
           ));
        }
        return new AppointmentModel.PaginatedSearchResponse(pageSize, pageIndex, total, dataList);
    }
}