using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LegalDoc.Web;
using MudBlazor.Services;
using Fluxor;
using LegalDoc.Web.Services;
using LegalDoc.Web.State;
using Microsoft.AspNetCore.Components.Authorization;
using LegalDoc.Web.Auth;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<JwtAuthorizationHandler>();

builder.Services.AddHttpClient("LegalDoc.API", client => 
    {
        client.BaseAddress = new Uri("http://localhost:5000");
    })
    .AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("LegalDoc.API"));
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<RegistryService>();
builder.Services.AddScoped<LawyerService>();
builder.Services.AddScoped<ReviewTaskService>();
builder.Services.AddScoped<AppState>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddMudServices();
builder.Services.AddFluxor(options => options.ScanAssemblies(typeof(Program).Assembly));

await builder.Build().RunAsync();