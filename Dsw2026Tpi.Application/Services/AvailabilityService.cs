using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Helpers;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(30);
        private readonly IPersistence _persistence;
        private readonly IHoliday _holiday;

        public AvailabilityService(IPersistence persistence, IHoliday holiday)
        {
            _persistence = persistence;
            _holiday = holiday;
        }
        public async Task<AvailabilityModel.Response> CreateOrReplace(AvailabilityModel.Request request)
        {
            var doctor = await _persistence.GetById<Doctor>(request.DoctorId)
                ?? throw new EntityNotFoundException(nameof(Doctor));

            if (request.Days is null || request.Days.Count == 0)
                throw new ValidationException(ErrorCodes.AVAILABILITY_INVALID, nameof(ErrorCodes.AVAILABILITY_INVALID))
                    .WithDetail("days", "empty");

            var parsedDays = ValidateDays(request.Days);
            OrderBySlot(parsedDays);

            var today = DateOnly.FromDateTime(DateTime.Now);
            var month = today.Month;
            var year = today.Year;

            // "Resto del mes" -> desde hoy hasta el último día del mes en curso
            var lastDayOfMonth = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            var (rulesToDelete, slotsToDelete, bookedSlots) = await GetExistingMonthData(request.DoctorId, month, year);

            await _persistence.DeleteRange(slotsToDelete);
            await _persistence.DeleteRange(rulesToDelete);

            var bookedKeys = bookedSlots.Select(s => (s.SlotDate, s.StartTime)).ToHashSet();

            var newRules = new List<AvailabilityRule>();
            var newSlots = new List<AvailabilitySlots>();
            var responseDays = new List<AvailabilityModel.GeneratedDay>();

            foreach (var (dayOfWeek, start, end, originalDay) in parsedDays)
            {
                var rule = new AvailabilityRule(request.DoctorId, month, year, dayOfWeek, start, end);
                newRules.Add(rule);

                var slotsGenerated = 0;

                for (var date = today; date <= lastDayOfMonth; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != dayOfWeek) continue;
                    // CU02 - Flujo alternativo: para feriados/no laborales no se generan turnos
                    if (_holiday.IsHoliday(date)) continue;

                    for (var slotStart = start; slotStart < end; slotStart = slotStart.Add(SlotDuration))
                    {
                        // No pisa un turno que ya fue reservado por un paciente
                        if (bookedKeys.Contains((date, slotStart))) continue;

                        newSlots.Add(new AvailabilitySlots(request.DoctorId, date, slotStart, slotStart.Add(SlotDuration), rule.Id));
                        slotsGenerated++;
                    }
                }
                responseDays.Add(new AvailabilityModel.GeneratedDay(
                originalDay, start.ToString("HH:mm"), end.ToString("HH:mm"), slotsGenerated));
            }

            await _persistence.AddRange(newRules);
            await _persistence.AddRange(newSlots);

            return new AvailabilityModel.Response(request.DoctorId, month, year, responseDays);
        }

        //

        private static List<(DayOfWeek DayOfWeek, TimeOnly Start, TimeOnly End, string OriginalDay)> ValidateDays(
        List<AvailabilityModel.DayRequest> days)
        {
            var parsed = new List<(DayOfWeek, TimeOnly, TimeOnly, string)>();

            foreach (var d in days)
            {
                if (!DayHelper.TryParse(d.Day, out var dayOfWeek))
                    throw new ValidationException(ErrorCodes.AVAILABILITY_INVALID, nameof(ErrorCodes.AVAILABILITY_INVALID))
                        .WithDetail("day", "invalid_value");

                if (!TimeOnly.TryParse(d.StartTime, out var start) || !TimeOnly.TryParse(d.EndTime, out var end))
                    throw new ValidationException(ErrorCodes.AVAILABILITY_INVALID, nameof(ErrorCodes.AVAILABILITY_INVALID))
                        .WithDetail("time", "invalid_format");

                if (start >= end)
                    throw new ValidationException(ErrorCodes.AVAILABILITY_INVALID, nameof(ErrorCodes.AVAILABILITY_INVALID))
                        .WithDetail("time", "start_must_be_before_end");

                parsed.Add((dayOfWeek, start, end, d.Day));
            }

            return parsed;
        }

        private static void OrderBySlot(List<(DayOfWeek DayOfWeek, TimeOnly Start, TimeOnly End, string OriginalDay)> parsedDays)
        {
            foreach (var group in parsedDays.GroupBy(p => p.DayOfWeek))
            {
                var ordered = group.OrderBy(p => p.Start).ToList();

                for (var i = 1; i < ordered.Count; i++)
                {
                    if (ordered[i].Start < ordered[i - 1].End)
                        throw new ValidationException(ErrorCodes.AVAILABILITY_OVERLAP, nameof(ErrorCodes.AVAILABILITY_OVERLAP))
                            .WithDetail("day", ordered[i].OriginalDay);
                }
            }
        }

        private async Task<(List<AvailabilityRule> Rules, List<AvailabilitySlots> SlotsToDelete, List<AvailabilitySlots> Booked)>
        GetExistingMonthData(Guid doctorId, int month, int year)
        {
            var existingRules = (await _persistence.GetFiltered<AvailabilityRule>(
                r => r.DoctorId == doctorId && r.Month == month && r.Year == year))?.ToList() ?? [];

            var existingSlots = (await _persistence.GetFiltered<AvailabilitySlots>(
                s => s.DoctorId == doctorId && s.SlotDate.Month == month && s.SlotDate.Year == year))?.ToList() ?? [];

            var booked = existingSlots.Where(s => s.Status == SlotStatus.Booked).ToList();
            var toDelete = existingSlots.Where(s => s.Status != SlotStatus.Booked).ToList();

            return (existingRules, toDelete, booked);
        }
    }
}
