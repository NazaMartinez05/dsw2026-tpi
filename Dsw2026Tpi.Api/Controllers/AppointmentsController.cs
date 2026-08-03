using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : AppController
{
    private readonly IAppointmentService _appointmentService; 
    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentModel.Request request)
    {
        var response = await _appointmentService.CreateAppointmentAsync(request);

        return CreatedAtAction(nameof(CreateAppointment), new { }, response);
    }

    [HttpGet("patient")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatientAppointments([FromQuery] long dni)
    {
        var appointments = await _appointmentService.GetPatientAppointmentsAsync(dni);
        return Ok(appointments);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelAppointment(Guid id)
    {
       await _appointmentService.CancelAppointmentAsync(id);
       return Ok("ok");
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppointmentsByDate(
        [FromQuery] DateTime date,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageIndex = 1)
    {
        var result = await _appointmentService.GetAppointmentsByDateAsync(date, pageSize, pageIndex);
        return Ok(result);       
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAppointmetns(
        [FromQuery] Guid? specialityId,
        [FromQuery] Guid? doctorId,
        [FromQuery] string? dni,
        [FromQuery] DateTime date,
        [FromQuery] int pageSize =10,
        [FromQuery] int pageIndex=1)
    {
        var result = await _appointmentService.SearchAppointmentAsync(
                specialityId, doctorId, dni, date, pageSize, pageIndex);
        return Ok(result);
    }
}
