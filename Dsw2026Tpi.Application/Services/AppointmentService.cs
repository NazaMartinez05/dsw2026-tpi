using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Data;
using Dsw2026Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2026Tpi.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly Dsw2026TpiDbContext _context; //se trae la base de datos 
    public AppointmentService(Dsw2026TpiDbContext context)
    {
        _context = context;
    }
    public async Task <AppointmentModel.Response> CreateAppointmentAsync(AppointmentModel.Request request)
    {
        bool isSlotTaken= await _context.Appointments 
           .AnyAsync(a => a.AvailabilitySlotId == request.AvailabilitySlotId );
        if (isSlotTaken)
        {
            throw new Exception("Horario seleccionado no disponible.");
        }
        var newAppointment = new Appointment(
           request.AvailabilitySlotId,
           request.PatientId,
           request.Reason
       );
        _context.Appointments.Add(newAppointment);
        await _context.SaveChangesAsync();
        
        return new AppointmentModel.Response(
            Guid.NewGuid(),
            "Reservado",
            "Turno en construccion"
            );
    }
}