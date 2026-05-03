using System.Net.Http.Json;
using LegalDoc.Web.Models;

namespace LegalDoc.Web.Services;

public class LawyerService(HttpClient http)
{
    public async Task<List<LawyerDto>> GetLawyersAsync()
    {
        var response = await http.GetFromJsonAsync<List<LawyerDto>>("api/v1/lawyers");
        return response ?? new List<LawyerDto>();
    }
    
    public async Task UpdateLawyerActivityAsync(Guid lawyerId, bool isActive)
    {
        var response = await http.PatchAsync($"api/v1/lawyers/{lawyerId}/lawyer-activity?isActive={isActive}", null);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task CreateLawyerAsync(string name, string barNumber, string? email)
    {
        var payload = new { Name = name, BarNumber = barNumber, Email = email };
        
        var response = await http.PostAsJsonAsync("api/v1/lawyers", payload);
        response.EnsureSuccessStatusCode();
    }
}