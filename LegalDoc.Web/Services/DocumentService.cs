using System.Net.Http.Json;
using LegalDoc.Web.Models;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LegalDoc.Web.Services;

public class DocumentService(HttpClient http, IJSRuntime js)
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
    
    public async Task DownloadDocumentAsync(Guid documentId, string fileName)
    {
        var response = await http.GetAsync($"api/v1/documents/{documentId}/file");

        if (response.IsSuccessStatusCode)
        {
            var fileStream = await response.Content.ReadAsStreamAsync();
            using var streamRef = new DotNetStreamReference(stream: fileStream);
            
            await js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
        else
        {
            throw new Exception("Nu s-a putut descărca fișierul. Serverul a răspuns cu eroare.");
        }
    }
}