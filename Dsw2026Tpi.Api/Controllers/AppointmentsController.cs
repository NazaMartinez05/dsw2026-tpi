using Dsw2026Tpi.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Tpi.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateAppointment([FromBody] AppointmentModel.Request request)
    {  
    return Ok(new { mesagge = "Endpoint funionando"});
    }
} 