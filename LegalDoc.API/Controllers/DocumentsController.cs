using LegalDoc.Application.Document.Commands;
using LegalDoc.Application.Document.Queries;
using LegalDoc.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalDoc.API.Controllers;

[ApiController]
[Route("api/v1/documents")]
[Authorize]
public class DocumentsController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Lawyer")]
    [HttpPost]
    public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request)
    {
        if (request.File.Length == 0)
            return BadRequest("Fișier lipsă.");
        
        using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        
        var command = new UploadDocumentCommand(
            request.Title,
            request.File.FileName,
            fileBytes,
            request.RegistryId
        );

        var id = await mediator.Send(command);
        
        return Created($"api/v1/documents/{id}", new { id });
    }

    [Authorize(Roles = "Admin,Lawyer,Viewer")]
    [HttpGet]
    public async Task<IActionResult> GetDocuments([FromQuery] Guid? documentId, [FromQuery] Guid? registryId, [FromQuery] DocumentStatus? status)
    {
        var query = new GetDocumentsQuery(documentId, registryId, status);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [Authorize(Roles = "Admin,Lawyer,Viewer")]
    [HttpGet("{documentId}/file")]
    public async Task<IActionResult> GetDocumentFile([FromRoute] Guid documentId)
    {
        var query = new GetDocumentFileQuery(documentId);
        var result = await mediator.Send(query);
        
        return File(result.Content, result.ContentType, result.FileName);
    }
    
    [Authorize(Roles = "Lawyer")]
    [HttpPatch("{documentId}/ai-analysis")]
    public async Task<IActionResult> UpdateAiAnalysis([FromRoute] Guid documentId)
    {
        var command = new UpdateDocumentAiAnalysisCommand(documentId);
        await mediator.Send(command);
        return NoContent();
    }
}

public class UploadDocumentRequest
{
    public string Title { get; set; } = string.Empty;
    public Guid RegistryId { get; set; }
    public IFormFile File { get; set; } = default!;
}