using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Data.Options;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Dsw2026Tpi.Application.Services
{
    // Carga una única vez (Singleton) el listado de feriados desde Sources/holidays.json
    // RN: "Para los días feriados o no laborales no se deben generar los turnos" (CU02 - flujo alternativo)
    public class Holiday : IHoliday
    {
        private readonly HashSet<DateOnly> _holidays;

        public Holiday()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Sources", "holidays.json");

            if (!File.Exists(path))
            {
                _holidays = [];
                return;
            }

            var json = File.ReadAllText(path);
            var dates = JsonSerializer.Deserialize<List<DateOnly>>(json, JsonOptions.JsonSerializerOptions);
            _holidays = dates is null ? [] : [.. dates];
        }

        public bool IsHoliday(DateOnly date) => _holidays.Contains(date);
    }
}
