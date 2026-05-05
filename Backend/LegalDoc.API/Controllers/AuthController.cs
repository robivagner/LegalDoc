using LegalDoc.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LegalDoc.Application.Auth.Queries;

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
    
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { Message = "Token invalid sau corupt." });
        }
        
        var query = new GetCurrentUserQuery(userId);
        var result = await mediator.Send(query);

        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] string? username)
    {
        var query = new GetUsersQuery(username);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPatch("change-username")]
    public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
        var response = await mediator.Send(new ChangeUsernameCommand(userId, request.NewUsername));

        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
        await mediator.Send(new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword));
    
        return Ok(new { message = "Parola a fost schimbată cu succes." });
    }
}

public record ChangeUsernameRequest(string NewUsername);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);