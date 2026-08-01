using Dsw2026Tpi.Application.Dtos;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentModel.Response> CreateAppointmentAsync(AppointmentModel.Request request);
    Task<List<AppointmentModel.PatientAppointmentResponse>> GetPatientAppointmentsAsync(long dni);
    Task CancelAppointmentAsync(Guid id);
    Task<AppointmentModel.PaginatedSearchResponse> GetAppointmentsByDateAsync(DateTime date, int pageSize= 10, int pageIndex=1);
    Task<AppointmentModel.PaginatedSearchResponse> SearchAppointmentAsync(Guid? specialityId, Guid? doctorId, string? dni, DateTime? date, int pageSize = 10, int pageIndex = 1);
}