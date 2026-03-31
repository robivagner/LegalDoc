using LegalDoc.Application.Abstractions;
using LegalDoc.Infrastructure.Persistence;
using LegalDoc.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Microsoft.OpenApi;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

// --- SERVICES ---
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddValidatorsFromAssembly(typeof(IDocumentsRepository).Assembly);
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); // Necesar pentru Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Legal Document Summarizer API",
        Version = "v1",
        Description = "API for legal document summarizer",
    });
});           // Activează generarea documentației Swagger
builder.Services.AddFluentValidationAutoValidation();

// Configurare MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(IDocumentsRepository).Assembly));

// Configurare Baza de Date
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IDocumentsRepository, DocumentsRepository>();
builder.Services.AddScoped<IRegistryRepository, RegistryRepository>();
builder.Services.AddScoped<ILawyerRepository, LawyerRepository>();
builder.Services.AddScoped<IReviewTaskRepository, ReviewTaskRepository>();

var app = builder.Build();

// --- MIDDLEWARE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();          // Genereaza fisierul JSON de documentatie
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Legal Document API V1");
        c.RoutePrefix = string.Empty;
    });         // Activeaza interfața vizuala (Pagina de test)
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();