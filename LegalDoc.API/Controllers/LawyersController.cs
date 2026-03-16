using LegalDoc.Application.Lawyer.Commands;
using LegalDoc.Application.Lawyer.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegalDoc.API.Controllers;


[ApiController]
[Route("api/v1/lawyers")]
public class LawyersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateLawyer([FromBody] CreateLawyerCommand command)
    {
        var id = await mediator.Send(command);
        return Created($"/api/v1/lawyers/{id}", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetLawyers()
    {
        var query = new GetLawyersQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }
}