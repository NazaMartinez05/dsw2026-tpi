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
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentModel.Request request)
    {
        var response = await _appointmentService.CreateAppointmentAsync(request);

        return CreatedAtAction(nameof(CreateAppointment), new { }, response);
    }

    [HttpGet("patient")]
    public async Task<IActionResult> GetPatientAppointments([FromQuery] long dni)
    {
        try
        {
            var appointments = await _appointmentService.GetPatientAppointmentsAsync(dni);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message});
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelAppointment(Guid id)
    {
        try
        {
            await _appointmentService.CancelAppointmentAsync(id);
            return Ok("ok");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message});
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointmentsByDate(
        [FromQuery] DateTime date,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageIndex = 1)
    {
        try
        {
            var result = await _appointmentService.GetAppointmentsByDateAsync(date, pageSize, pageIndex);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }           

    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAppointmetns(
        [FromQuery] Guid? specialityId,
        [FromQuery] Guid? doctorId,
        [FromQuery] string? dni,
        [FromQuery] DateTime date,
        [FromQuery] int pageSize =10,
        [FromQuery] int pageIndex=1)
    {
        try
        {
            var result = await _appointmentService.SearchAppointmentAsync(
                specialityId, doctorId, dni, date, pageSize, pageIndex);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }
}
