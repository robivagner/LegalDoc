using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LegalDoc.Web;
using MudBlazor.Services;
using Fluxor;
using LegalDoc.Web.Services;
using LegalDoc.Web.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurare HttpClient pentru API-ul principal
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri("http://localhost:5000")
});
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<RegistryService>();
builder.Services.AddScoped<LawyerService>();
builder.Services.AddScoped<ReviewTaskService>();
builder.Services.AddScoped<AppState>();

builder.Services.AddMudServices();
builder.Services.AddFluxor(options => options.ScanAssemblies(typeof(Program).Assembly));

await builder.Build().RunAsync();