using System;
using System.Collections.Generic;
using System.Text;

using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;

namespace Dsw2026Tpi.Application.Services;

public class SpecialityService : ISpecialityService
{
    private readonly IPersistence _persistence;

    public SpecialityService(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public async Task<Pagination<SpecialityModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null)
    {
        var specialities = await _persistence.Paginate<Speciality, string>(
            pageSize, pageIndex,
            s => string.IsNullOrWhiteSpace(name) || s.Name.Contains(name),
            s => s.Name);

        return specialities.Map(s => new SpecialityModel.Response(s.Id, s.Name, s.Description));
    }

    public async Task<SpecialityModel.Response> Add(SpecialityModel.Request request)
    {
        Validate(request);

        var speciality = new Speciality(request.Name, request.Description);
        await _persistence.Add(speciality);

        return new SpecialityModel.Response(speciality.Id, speciality.Name, speciality.Description);
    }

    public async Task<SpecialityModel.Response> Update(Guid id, SpecialityModel.Request request)
    {
        Validate(request);

        var speciality = await _persistence.GetById<Speciality>(id)
            ?? throw new EntityNotFoundException(nameof(Speciality));

        speciality.Update(request.Name, request.Description);
        await _persistence.Update(speciality);

        return new SpecialityModel.Response(speciality.Id, speciality.Name, speciality.Description);
    }

    public async Task Delete(Guid id)
    {
        var speciality = await _persistence.GetById<Speciality>(id)
            ?? throw new EntityNotFoundException(nameof(Speciality));

        speciality.MarkAsDeleted();
        await _persistence.Update(speciality);
    }

    private static void Validate(SpecialityModel.Request request)
    {
        var exception = new ValidationException();

        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length is < 3 or > 100)
            exception.WithDetail(nameof(request.Name), "El nombre es obligatorio y debe tener entre 3 y 100 caracteres");

        if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Length is < 10 or > 100)
            exception.WithDetail(nameof(request.Description), "La descripción es obligatoria y debe tener entre 10 y 100 caracteres");

        if (exception.Error.Details?.Count > 0) throw exception;
    }
}
