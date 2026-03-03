using LegalDoc.Application.Documents.Commands;
using LegalDoc.Application.Documents.Queries;
using LegalDoc.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegalDoc.API.Controllers;

[ApiController]
[Route("api/v1/documents")]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> UploadDocument([FromBody] UploadDocumentCommand request)
    {
        var id = await _mediator.Send(request);
        
        return Created($"api/v1/documents/{id}", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments([FromQuery] DocumentStatus? status)
    {
        var query = new GetDocumentQuery(status);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}