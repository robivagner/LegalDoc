using LegalDoc.Application.Documents.Commands;
using LegalDoc.Application.Documents.Queries;
using LegalDoc.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegalDoc.API.Controllers;

[ApiController]
[Route("api/v1/documents")]
public class DocumentsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> UploadDocument([FromBody] UploadDocumentCommand request)
    {
        var id = await mediator.Send(request);
        
        return Created($"api/v1/documents/{id}", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments([FromQuery] DocumentStatus? status, [FromQuery] Guid? registryId)
    {
        var query = new GetDocumentsQuery(status, registryId);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPatch("{id}/ai-analysis")]
    public async Task<IActionResult> UpdateAiAnalysis(Guid id, [FromBody] UpdateDocumentAiAnalysisCommand request)
    {
        var command = request with {DocumentId = id};
        await mediator.Send(command);
        return NoContent();
    }
}