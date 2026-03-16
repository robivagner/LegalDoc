using LegalDoc.Application.Registry.Commands;
using LegalDoc.Application.Registry.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegalDoc.API.Controllers;

[ApiController]
[Route("api/v1/departments")]
public class RegistryController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateRegistry([FromBody] CreateRegistryCommand command)
    {
        var id = await mediator.Send(command);
        return Created($"/api/v1/departments/{id}", new { id });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRegistries()
    {
        var result = await mediator.Send(new GetRegistriesQuery());
        return Ok(result);
    }
}