using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
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
        if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Length < 5)
        {
            throw new Exception("Motivo obligartorio, debe contener al menos 5 caracteres.");
        }
        string dniString = request.Patient.Dni.ToString();
        if (dniString.Length < 7 || dniString.Length > 10)
        {
            throw new Exception("El DNI debe contener entre 7 y 10 dígitos");
        }
        bool doctorExists = await _context.Doctors.AnyAsync(d => d.Id == request.DoctorId);
        if (!doctorExists)
        {
            throw new Exception("El medico seleccionado no existe");
        }
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Dni == dniString);
        if (patient == null)
        {
            throw new Exception("El paciente no esta registrado en el sistema");
        }
        bool isSlotTaken = await _context.Appointments
            .AnyAsync(a => a.AvailabilitySlotId == request.AvailabilitySlotId);
        if (isSlotTaken)
        {
            throw new Exception("El horario seleccionado no se encuentra disponible");
        }
        var newAppointment = new Appointment(
            request.AvailabilitySlotId,
            patient.Id,
            request.Reason
        );
        _context.Appointments.Add(newAppointment);
        await _context.SaveChangesAsync();

        return new AppointmentModel.Response(
            newAppointment.Id,
            request.DoctorId,
            newAppointment.AvailabilitySlotId,
            request.Patient.Dni,
            newAppointment.Reason,
            "BOOKED"
        );
    }
    public async Task<List<AppointmentModel.PatientAppointmentResponse>> GetPatientAppointmentsAsync(long dni)
    {
        string dniString = dni.ToString();
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Dni == dniString);
        if (patient == null)
        {
            throw new Exception("Paciente no registrado en el sistema");
        }
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
        {
            throw new Exception("El turno seleccionado no existe.");
        }
        if (appointment.Status != AppointmentStatus.Booked)
        {
            throw new Exception("Solo se puede cancelar turnos que esten en estado de reservado.");
        }
        appointment.Status = AppointmentStatus.Cancelled;
        await _context.SaveChangesAsync();
    }
    public async Task<AppointmentModel.PaginatedSearchResponse> GetAppointmentsByDateAsync(DateTime date, int pageSize=10, int pageIndex=1)
    {
        var targetDate=DateOnly.FromDateTime(date);
        var query = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.AvailabilitySlots)
                .ThenInclude(slot => slot.Doctor)
                    .ThenInclude(doc => doc.Speciality)
            .Where(a => a.Status == AppointmentStatus.Booked &&
                        a.AvailabilitySlots.SlotDate == targetDate);

        int total = await query.CountAsync();

        var appointments = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dataList= new List<AppointmentModel.SearchResponse>();
        foreach (var a in appointments)
        {
            dataList.Add(new AppointmentModel.SearchResponse(
                a.Id,
                a.Status.ToString(),
                new AppointmentModel.PatientSearchDto(
                    long.Parse(a.Patient.Dni),
                    a.Patient.FullName
                ),
                new AppointmentModel.DoctorSearchDto(
                    a.AvailabilitySlots.DoctorId,
                    a.AvailabilitySlots.Doctor.Name,
                    new AppointmentModel.SpecialtySearchDto(
                        a.AvailabilitySlots.Doctor.SpecialityId.GetValueOrDefault(),
                        a.AvailabilitySlots.Doctor.Speciality.Name
                    )
                 )
           ));     
        }
        return new AppointmentModel.PaginatedSearchResponse(pageSize, pageIndex, total, dataList);   
    }
    public async Task<AppointmentModel.PaginatedSearchResponse> SearchAppointmentAsync(
        Guid? specialityId, Guid? doctorId, string? dni, DateTime? date, int pageSize = 10, int pageIndex = 1)
    {
        var query = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.AvailabilitySlots)
                .ThenInclude(slot => slot.Doctor)
                    .ThenInclude(doc => doc.Speciality)
            .Where(a => a.Status == AppointmentStatus.Booked);
        if (specialityId.HasValue)
        {
            query = query.Where(a => a.AvailabilitySlots.Doctor.SpecialityId == specialityId.Value);
        }
        if (doctorId.HasValue)
        {
            query = query.Where(a => a.AvailabilitySlots.DoctorId == doctorId.Value);
        }
        if (date.HasValue)
        {
            var targetDate = DateOnly.FromDateTime(date.Value);
            query = query.Where(a => a.AvailabilitySlots.SlotDate == targetDate);
        }
        int total = await query.CountAsync();
        var appointments = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var dataList = new List<AppointmentModel.SearchResponse>();
        foreach (var a in appointments)
        {
            dataList.Add(new AppointmentModel.SearchResponse(
                a.Id,
                a.Status.ToString(),
                new AppointmentModel.PatientSearchDto(
                    long.Parse(a.Patient.Dni),
                    a.Patient.FullName
                ),
                new AppointmentModel.DoctorSearchDto(
                    a.AvailabilitySlots.DoctorId,
                    a.AvailabilitySlots.Doctor.Name,
                    new AppointmentModel.SpecialtySearchDto(
                        a.AvailabilitySlots.Doctor.SpecialityId.GetValueOrDefault(),
                        a.AvailabilitySlots.Doctor.Speciality.Name
                    )
                 )
           ));
        }
        return new AppointmentModel.PaginatedSearchResponse(pageSize, pageIndex, total, dataList);
    }
}