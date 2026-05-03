using LegalDoc.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalDoc.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        await mediator.Send(command);
        return Ok(new { Message = "Utilizator creat cu succes!" });
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleCommand command)
    {
        await mediator.Send(command);
        return Ok(new { Message = $"Role {command.RoleName} assigned successfully." });
    }
}