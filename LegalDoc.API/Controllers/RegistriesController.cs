using LegalDoc.Application.Registry.Commands;
using LegalDoc.Application.Registry.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalDoc.API.Controllers;

[ApiController]
[Route("api/v1/registries")]
public class RegistriesController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Lawyer")]
    [HttpPost]
    public async Task<IActionResult> CreateRegistry([FromBody] CreateRegistryCommand command)
    {
        var id = await mediator.Send(command);
        return Created($"/api/v1/registries/{id}", new { id });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRegistries()
    {
        var result = await mediator.Send(new GetRegistriesQuery());
        return Ok(result);
    }
}