using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.CrossCutting.Helpers
{
    public static class DayHelper
    {        
        // Acepta con y sin tilde para no depender de cómo tipee el frontend
        private static readonly Dictionary<string, DayOfWeek> Map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["LUNES"] = DayOfWeek.Monday,
            ["MARTES"] = DayOfWeek.Tuesday,
            ["MIERCOLES"] = DayOfWeek.Wednesday,
            ["MIÉRCOLES"] = DayOfWeek.Wednesday,
            ["JUEVES"] = DayOfWeek.Thursday,
            ["VIERNES"] = DayOfWeek.Friday,
            ["SABADO"] = DayOfWeek.Saturday,
            ["SÁBADO"] = DayOfWeek.Saturday,
            ["DOMINGO"] = DayOfWeek.Sunday,
        };

        private static readonly Dictionary<DayOfWeek, string> ReverseMap = new()
        {
            [DayOfWeek.Monday] = "LUNES",
            [DayOfWeek.Tuesday] = "MARTES",
            [DayOfWeek.Wednesday] = "MIÉRCOLES",
            [DayOfWeek.Thursday] = "JUEVES",
            [DayOfWeek.Friday] = "VIERNES",
            [DayOfWeek.Saturday] = "SÁBADO",
            [DayOfWeek.Sunday] = "DOMINGO",
        };

        public static bool TryParse(string? day, out DayOfWeek dayOfWeek)
        {
            dayOfWeek = default;
            if (string.IsNullOrWhiteSpace(day)) return false;
            return Map.TryGetValue(day.Trim(), out dayOfWeek);
        }

        public static string ToSpanish(DayOfWeek dayOfWeek) => ReverseMap[dayOfWeek];
    }
}
