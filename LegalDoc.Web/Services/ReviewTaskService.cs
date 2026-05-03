using System.Net.Http.Json;
using LegalDoc.Web.Models;

namespace LegalDoc.Web.Services;

public class ReviewTaskService(HttpClient http)
{
    public async Task<List<ReviewTaskDto>> GetTasksAsync()
    {
        var response = await http.GetFromJsonAsync<List<ReviewTaskDto>>("api/v1/review-tasks");
        return response ?? new List<ReviewTaskDto>();
    }
    
    public async Task<List<ReviewTaskDto>> GetTasksByStatusAsync(string status)
    {
        var response = await http.GetFromJsonAsync<List<ReviewTaskDto>>($"api/v1/review-tasks?status={status}");
        return response ?? new List<ReviewTaskDto>();
    }
    
    public async Task UpdateTaskStatusAsync(Guid taskId, string newStatus)
    {
        var response = await http.PatchAsync($"api/v1/review-tasks/{taskId}/review-task-status?status={newStatus}", null);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task AssignTaskAsync(Guid documentId, Guid lawyerId, string? description)
    {
        var payload = new 
        { 
            DocumentId = documentId, 
            LawyerId = lawyerId, 
            Description = description 
        };
        var response = await http.PostAsJsonAsync("api/v1/review-tasks", payload);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task ReassignTaskAsync(Guid taskId, Guid newLawyerId)
    {
        var response = await http.PatchAsync($"api/v1/review-tasks/{taskId}/review-task-lawyer?newLawyerId={newLawyerId}", null);
        response.EnsureSuccessStatusCode();
    }
}