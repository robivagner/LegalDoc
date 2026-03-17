using LegalDoc.Application.Abstractions;
using LegalDoc.Infrastructure.Persistence;
using LegalDoc.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// --- SERVICES ---
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddValidatorsFromAssembly(typeof(IDocumentsRepository).Assembly);
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); // Necesar pentru Swagger
builder.Services.AddSwaggerGen();           // Activează generarea documentației Swagger

// Configurare MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(IDocumentsRepository).Assembly));

// Configurare Baza de Date
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDocumentsRepository, DocumentsRepository>();
builder.Services.AddScoped<IRegistryRepository, RegistryRepository>();
builder.Services.AddScoped<ILawyerRepository, LawyerRepository>();
builder.Services.AddScoped<IReviewTaskRepository, ReviewTaskRepository>();

var app = builder.Build();

// --- MIDDLEWARE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();          // Genereaza fisierul JSON de documentatie
    app.UseSwaggerUI();         // Activeaza interfața vizuala (Pagina de test)
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();