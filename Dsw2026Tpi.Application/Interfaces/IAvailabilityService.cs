using Dsw2026Tpi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces
{
    public interface IAvailabilityService
    {
        // Usado tanto por POST como por PUT /api/availabilities: en ambos casos
        // se sobreescribe la disponibilidad del mes en curso para el médico indicado.
        Task<AvailabilityModel.Response> CreateOrReplace(AvailabilityModel.Request request);
    }
}
