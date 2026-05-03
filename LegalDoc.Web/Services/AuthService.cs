using System.Net.Http.Json;
using LegalDoc.Web.Models;
using Microsoft.JSInterop;

namespace LegalDoc.Web.Services;

public class AuthService(HttpClient http, IJSRuntime js)
{
    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var response = await http.PostAsJsonAsync("api/v1/auth/login", request);
        
        if (!response.IsSuccessStatusCode) return false;
        
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result == null) return false;
        
        await js.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
        await js.InvokeVoidAsync("localStorage.setItem", "userName", result.UserName);
        
        return true;
    }

    public async Task LogoutAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await js.InvokeVoidAsync("localStorage.removeItem", "userName");
    }
}