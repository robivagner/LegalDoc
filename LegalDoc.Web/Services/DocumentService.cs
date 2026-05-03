using System.Net.Http.Json;
using LegalDoc.Web.Models;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace LegalDoc.Web.Services;

public class DocumentService(HttpClient http, NavigationManager nav)
{
    public async Task<List<DocumentDto>> GetDocumentsAsync() =>
        await http.GetFromJsonAsync<List<DocumentDto>>("api/v1/documents") ?? new();
    
    public async Task<List<DocumentDto>> GetDocumentsByStatusAsync(string status) =>
        await http.GetFromJsonAsync<List<DocumentDto>>($"api/v1/documents?status={status}") ?? new();

    public async Task UploadDocumentAsync(MultipartFormDataContent content)
    {
        var response = await http.PostAsync("api/v1/documents", content);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task TriggerAiAnalysisAsync(Guid documentId)
    {
        var response = await http.PatchAsync($"api/v1/documents/{documentId}/ai-analysis", null);
        response.EnsureSuccessStatusCode();
    }
    
    public void DownloadDocument(Guid documentId)
    {
        var baseUrl = http.BaseAddress?.ToString().TrimEnd('/');
        var downloadUrl = $"{baseUrl}/api/v1/documents/{documentId}/file";
        
        nav.NavigateTo(downloadUrl, forceLoad: true);
    }
}