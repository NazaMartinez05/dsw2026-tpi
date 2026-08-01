using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Dsw2026Tpi.CrossCutting.Helpers;

namespace Dsw2026Tpi.Application.Services;

public class DoctorService : IDoctorService
{
    // Inyección de dependencias por constructor
    private readonly IPersistence _persistence;

    public DoctorService(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public async Task<Pagination<DoctorModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null)
    {
        var doctors = await _persistence.Paginate<Doctor, string>(pageSize, pageIndex, d => string.IsNullOrWhiteSpace(name) ||
                                                   d.Name.Contains(name), x => x.Name, nameof(Doctor.Speciality));

        return doctors.Map(d => new DoctorModel.Response(d.Id, d.Name, d.LicenseNumber,
            new DoctorModel.SpecialityDto(d.Speciality?.Id, d.Speciality?.Name)));
    }

    // Crear medico 
    public async Task<DoctorModel.Response> Add(DoctorModel.Request request)
    {
        await Validate(request);

        var speciality = await _persistence.GetById<Speciality>(request.SpecialityId)
            ?? throw new EntityNotFoundException(nameof(Speciality));

        var doctor = new Doctor(request.Name, request.LicenseNumber, speciality);
        await _persistence.Add(doctor);

        return new DoctorModel.Response(doctor.Id, doctor.Name, doctor.LicenseNumber,
            new DoctorModel.SpecialityDto(speciality.Id, speciality.Name));
    }

    // Modificar un medico existente
    public async Task<DoctorModel.Response> Update(Guid id, DoctorModel.Request request)
    {
        await Validate(request);

        var doctor = await _persistence.GetById<Doctor>(id, nameof(Doctor.Speciality))
            ?? throw new EntityNotFoundException(nameof(Doctor));

        var speciality = await _persistence.GetById<Speciality>(request.SpecialityId)
            ?? throw new EntityNotFoundException(nameof(Speciality));

        doctor.Update(request.Name, request.LicenseNumber, request.SpecialityId);
        await _persistence.Update(doctor);

        return new DoctorModel.Response(doctor.Id, doctor.Name, doctor.LicenseNumber,
            new DoctorModel.SpecialityDto(speciality.Id, speciality.Name));
    }

    // Baja logica
    public async Task Delete(Guid id)
    {
        var doctor = await _persistence.GetById<Doctor>(id)
            ?? throw new EntityNotFoundException(nameof(Doctor));

        doctor.MarkAsDeleted();
        await _persistence.Update(doctor);
    }

    public async Task<IEnumerable<DoctorModel.AvailabilityResponse>> GetAvailabilities(Guid doctorId)
    {
        _ = await _persistence.GetById<Doctor>(doctorId)
            ?? throw new EntityNotFoundException(nameof(Doctor));

        var now = DateTime.UtcNow;
        var rules = await _persistence.GetFiltered<AvailabilityRule>(
            r => r.DoctorId == doctorId && r.Month == now.Month && r.Year == now.Year);

        return (rules ?? []).Select(r => new DoctorModel.AvailabilityResponse(
            DayHelper.ToSpanish(r.DayOfWeek),
            r.StartTime.ToString("HH:mm"),
            r.EndTime.ToString("HH:mm")));
    }
    

    // Validacion de formato
    private async Task Validate(DoctorModel.Request request)
    {
        var exception = new ValidationException();

        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length is < 3 or > 100)
            exception.WithDetail(nameof(request.Name), "El nombre es obligatorio y debe tener entre 3 y 100 caracteres");

        if (exception.Error.Details?.Count > 0) throw exception;

        await Task.CompletedTask;
    }
}