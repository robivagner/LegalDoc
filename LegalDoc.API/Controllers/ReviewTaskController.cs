using LegalDoc.Application.ReviewTask.Commands;
using LegalDoc.Application.ReviewTask.Queries;
using LegalDoc.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegalDoc.API.Controllers;

[ApiController]
[Route("api/v1/review-tasks")]
public class ReviewTasksController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AssignTask([FromBody] AssignReviewTaskCommand command)
    {
        var id = await mediator.Send(command);
        return Created($"/api/v1/review-tasks/{id}", new { id });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] Guid? lawyerId, [FromQuery] ReviewTaskStatus? status)
    {
        var query = new GetReviewTasksQuery(lawyerId, status);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPatch("{id}/review-task-status")]
    public async Task<IActionResult> UpdateReviewTaskStatus(Guid id, [FromBody] ReviewTaskStatus status)
    {
        await mediator.Send(new UpdateReviewTaskStatusCommand(id, status));
        return NoContent();
    }
}