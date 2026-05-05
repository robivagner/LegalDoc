using System.Net.Http.Headers;
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
    
    public async Task<(bool Succeeded, string? Error)> RegisterUserAsync(string username, string password)
    {
        var response = await http.PostAsJsonAsync("api/v1/auth/register", new 
        { 
            UserName = username, 
            Password = password 
        });

        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }

    public async Task<AuthResponse?> ChangeUsernameAsync(string newUsername)
    {
        var response = await http.PatchAsJsonAsync("api/v1/auth/change-username", new { newUsername });
    
        if (response.IsSuccessStatusCode)
        {
            var authData = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
            if (authData != null && !string.IsNullOrEmpty(authData.Token))
            {
                await js.InvokeVoidAsync("localStorage.setItem", "authToken", authData.Token);
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authData.Token);
            }
        
            return authData;
        }
    
        return null;
    }

    public async Task<(bool Succeeded, string? Error)> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        var response = await http.PostAsJsonAsync("api/v1/auth/change-password", new 
        { 
            CurrentPassword = currentPassword, 
            NewPassword = newPassword 
        });

        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await http.GetFromJsonAsync<List<UserDto>>("api/v1/auth/users");
        return users ?? new List<UserDto>();
    }
    
    public async Task<(bool Succeeded, string? Error)> UpdateUserRoleAsync(string userId, string newRole)
    {
        var response = await http.PostAsJsonAsync("api/v1/auth/assign-role", new 
        { 
            UserId = userId, 
            RoleName = newRole 
        });

        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }

    public async Task LogoutAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await js.InvokeVoidAsync("localStorage.removeItem", "userName");
    }
}