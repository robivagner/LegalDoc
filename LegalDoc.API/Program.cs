using System.Text;
using LegalDoc.Application.Abstractions;
using LegalDoc.Infrastructure.Persistence;
using LegalDoc.Infrastructure.Repositories;
using LegalDoc.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// --- 1. BAZA DE DATE ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// --- 2. IDENTITY (Repară eroarea UserManager) ---
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// --- 3. AUTENTIFICARE JWT ---
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new Exception("CRITICAL ERROR: JWT Secret is missing from configuration!");
}
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = true,
        ValidateLifetime = true
    };
});

// --- 4. ALTE SERVICII ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddValidatorsFromAssembly(typeof(IDocumentsRepository).Assembly);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(IDocumentsRepository).Assembly));

// Am păstrat configurația ta originală de Swagger/OpenApi aici
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Legal Document Summarizer API",
        Version = "v1",
        Description = "API for legal document summarizer",
    });
});

builder.Services.AddScoped<IDocumentsRepository, DocumentsRepository>();
builder.Services.AddScoped<IRegistryRepository, RegistryRepository>();
builder.Services.AddScoped<ILawyerRepository, LawyerRepository>();
builder.Services.AddScoped<IReviewTaskRepository, ReviewTaskRepository>();
builder.Services.AddScoped<IDocumentTextExtractor, DocumentTextExtractor>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddHttpClient<IAiService, AiService>();

var app = builder.Build();

// --- 5. MIDDLEWARE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Legal Document API V1");
        c.RoutePrefix = string.Empty; 
    });
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Admin", "Lawyer", "Viewer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
    
    if (await userManager.FindByEmailAsync("admin@legaldoc.com") == null)
    {
        var admin = new IdentityUser { UserName = "admin", Email = "admin@legaldoc.com" };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

app.UseHttpsRedirection();
app.UseCors("BlazorPolicy");

// Ordinea contează aici:
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();