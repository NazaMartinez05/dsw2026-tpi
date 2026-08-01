using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService; 
    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }
    [HttpPost]
    public IActionResult CreateAppointment([FromBody] AppointmentModel.Request request)
    {
        return Ok(new { mesagge = "Endpoint funionando" });
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
}
