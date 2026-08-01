using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Data;

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
        return new AppointmentModel.Response(
            Guid.NewGuid(),
            "Reservado",
            "Turno en construccion"
            );
    }
}