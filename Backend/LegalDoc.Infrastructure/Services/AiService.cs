using System.Net.Http.Json;
using LegalDoc.Application.Document.Queries;
using LegalDoc.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace LegalDoc.Infrastructure.Services;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;

    public AiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var baseUrl = configuration["AiServiceSettings:BaseUrl"];
        _httpClient.BaseAddress = new Uri(baseUrl ?? "http://localhost:8000");
    }

    public async Task<AiAnalysisResponse?> AnalyzeDocumentAsync(string text)
    {
        var request = new AiAnalysisRequest(text);
        
        var response = await _httpClient.PostAsJsonAsync("/analyze", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AiAnalysisResponse>();
        }

        return null;
    }
}