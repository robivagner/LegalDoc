using LegalDoc.Application.Abstractions;
using LegalDoc.Infrastructure.Persistence;
using LegalDoc.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ADAUGĂ ASTA LA SERVICES ---
builder.Services.AddControllers();
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

// --- 2. ADAUGĂ ASTA LA MIDDLEWARE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();          // Generează fișierul JSON de documentație
    app.UseSwaggerUI();         // Activează interfața vizuală (Pagina de test)
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();