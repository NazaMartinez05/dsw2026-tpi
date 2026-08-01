using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos
{
    public record AvailabilityModel
    {
        public record DayRequest(string Day, string StartTime, string EndTime);

        public record Request(Guid DoctorId, List<DayRequest> Days);

        // El enunciado no define un response para POST/PUT: devolvemos un resumen de lo que se genero

        public record GeneratedDay(string Day, string StartTime, string EndTime, int SlotsGenerated);

        public record Response(Guid DoctorId, int Month, int Year, List<GeneratedDay> Days);
    }
}
