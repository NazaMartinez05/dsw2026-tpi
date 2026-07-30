using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos;

public class AppointmentModel
{
    //DTO de entrada (Request)
    public record Request( //se usa record porque es inmutable y su proposito es transportar datos de forma rapida y segura 
        [Required(ErrorMessage = "El paciente es obligatorio")] Guid PatientId,
        [Required(ErrorMessage= "El medico es obligatorio")] Guid DoctorId,
        [Required(ErrorMessage= "El horario es obligatorio")] Guid AvailabilitySlotId,
        [Required(ErrorMessage = "El motivo es obligario")][MinLength(5)] string Reason
    );
    //DTO de salida (Response)
    public record Response(
        Guid Id,
        string Status,
        string Message
    );
}