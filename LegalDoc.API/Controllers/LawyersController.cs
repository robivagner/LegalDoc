using System.Security.Claims;
using LegalDoc.Application.Lawyer.Commands;
using LegalDoc.Application.Lawyer.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalDoc.API.Controllers;


[ApiController]
[Route("api/v1/lawyers")]
[Authorize]
public class LawyersController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Lawyer")]
    [HttpPost]
    public async Task<IActionResult> CreateLawyer([FromBody] CreateLawyerRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized("Token invalid.");
        }
        
        var command = new CreateLawyerCommand(userId, request.Name, request.BarNumber, request.Email);
        
        var id = await mediator.Send(command);
        
        return Created($"/api/v1/lawyers/{id}", new { id });
    }
    
    [Authorize(Roles = "Lawyer")]
    [HttpGet("me")]
    public async Task<IActionResult> GetLawyer()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetLawyerQuery(userId);
        var lawyer = await mediator.Send(query);
        
        if (lawyer == null)
        {
            return NotFound();
        }

        return Ok(lawyer);
    }

    [Authorize(Roles = "Admin,Lawyer,Viewer")]
    [HttpGet]
    public async Task<IActionResult> GetLawyers()
    {
        var query = new GetLawyersQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPatch("{lawyerId}/lawyer-activity")]
    public async Task<IActionResult> UpdateLawyerActivity(Guid lawyerId, [FromQuery] bool isActive)
    {
        await mediator.Send(new UpdateLawyerActivityCommand(lawyerId, isActive));
        return NoContent();
    }
}

public class CreateLawyerRequest
{
    public string Name { get; set; } = string.Empty;
    public string BarNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}