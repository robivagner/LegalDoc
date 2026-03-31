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
    
    [HttpPatch("{reviewTaskId}/review-task-status")]
    public async Task<IActionResult> UpdateReviewTaskStatus(Guid reviewTaskId, [FromQuery] ReviewTaskStatus status)
    {
        await mediator.Send(new UpdateReviewTaskStatusCommand(reviewTaskId, status));
        return NoContent();
    }
    
    [HttpPatch("{reviewTaskId}/review-task-lawyer")]
    public async Task<IActionResult> UpdateReviewTaskLawyer(Guid reviewTaskId, [FromQuery] Guid newLawyerId)
    {
        await mediator.Send(new UpdateReviewTaskLawyerCommand(reviewTaskId, newLawyerId));
        return NoContent();
    }
}