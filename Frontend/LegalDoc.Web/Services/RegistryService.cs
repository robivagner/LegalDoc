using System.Net.Http.Json;
using LegalDoc.Web.Services;

using RegistryDto = LegalDoc.Web.Models.RegistryDto;

namespace LegalDoc.Web.Services;

public class RegistryService(HttpClient http)
{
    public async Task<List<RegistryDto>> GetRegistriesAsync() =>
        await http.GetFromJsonAsync<List<RegistryDto>>("api/v1/registries") ?? new();

    public async Task CreateRegistryAsync(RegistryDto registry) =>
        await http.PostAsJsonAsync("api/v1/registries", registry);
}