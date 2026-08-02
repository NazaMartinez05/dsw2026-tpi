using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos
{
    public record AvailabilityModel
    {
        public record DayRequest(string Day, string StartTime, string EndTime);

        public record Request(Guid DoctorId, List<DayRequest> Days);

        // Instancia concreta y reservable de un turno. El "Id" es el que el módulo de
        // Citas necesita recibir como "availabilityId" en POST /api/appointments.
        public record SlotDto(Guid Id, DateOnly Date, string StartTime, string EndTime, string Status);

        // El enunciado no define un response para POST/PUT: devolvemos un resumen
        // de lo que efectivamente se generó, incluyendo los slots (con su Id) para
        // que se puedan usar de inmediato al reservar turnos.
        public record GeneratedDay(string Day, string StartTime, string EndTime, int SlotsGenerated, List<SlotDto> Slots);

        public record Response(Guid DoctorId, int Month, int Year, List<GeneratedDay> Days);
    }
}
