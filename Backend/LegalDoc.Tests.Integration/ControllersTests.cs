using System.Net.Http.Json;
using FluentAssertions;
using LegalDoc.API.Controllers;
using LegalDoc.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using LegalDoc.Application.Auth.Queries;
using LegalDoc.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Authentication;
using LegalDoc.Application.Lawyer.Queries;
using LegalDoc.Application.Document.Commands;
using LegalDoc.Application.ReviewTask.Commands;
using LegalDoc.Application.ReviewTask.Queries;
using LegalDoc.Application.Lawyer.Commands;
using LegalDoc.Application.Document.Queries;
using LegalDoc.Application.Registry.Commands;
using LegalDoc.Application.Registry.Queries;
using LegalDoc.Domain.Entities;

namespace LegalDoc.Tests.Integration;

public class ControllersTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IMediator> _mediatorMock = new();

    public ControllersTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("JwtSettings:Secret", "cheie_secreta_foarte_lunga_de_test_1234567890");
            builder.UseSetting("JWT_SECRET", "cheie_secreta_foarte_lunga_de_test_1234567890");

            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            });
        });
    }

    [Fact]
    public async Task AuthControllerTest()
    {
        var client = _factory.CreateClient();

        // Mocking to avoid 204 or errors
        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), default)).ReturnsAsync(new AuthResponse("t", "u"));
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCurrentUserQuery>(), default)).ReturnsAsync(new UserDto("id", "u", "Admin"));
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUsersQuery>(), default)).ReturnsAsync(new List<UserDto>());

        // 1. Register
        await client.PostAsJsonAsync("/api/v1/auth/register", new RegisterCommand("u", "p"));
        // 2. Login
        await client.PostAsJsonAsync("/api/v1/auth/login", new LoginCommand("u", "p"));
        // 3. Assign Role
        await client.PostAsJsonAsync("/api/v1/auth/assign-role", new AssignRoleCommand(Guid.NewGuid().ToString(), "Admin"));
        // 4. Me
        await client.GetAsync("/api/v1/auth/me");
        // 5. Users
        await client.GetAsync("/api/v1/auth/users");
        // 6. Change Password
        await client.PostAsJsonAsync("/api/v1/auth/change-password", new ChangePasswordRequest("old", "new"));
    }

    [Fact]
    public async Task DocumentsControllerTest()
    {
        var client = _factory.CreateClient();
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<UploadDocumentCommand>(), default)).ReturnsAsync(Guid.NewGuid());
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetDocumentsQuery>(), default)).ReturnsAsync(new List<DocumentDto>());
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetDocumentFileQuery>(), default))
            .ReturnsAsync(new DocumentFileDto(new byte[] { 1 }, "application/pdf", "file.pdf"));

        // 1. Upload
        var content = new MultipartFormDataContent { 
            { new StringContent("Title"), "Title" }, 
            { new StringContent(Guid.NewGuid().ToString()), "RegistryId" },
            { new ByteArrayContent([1]), "File", "f.pdf" } 
        };
        await client.PostAsync("/api/v1/documents", content);
        // 2. Get All
        await client.GetAsync("/api/v1/documents");
        // 3. Get File
        await client.GetAsync($"/api/v1/documents/{Guid.NewGuid()}/file");
        // 4. AI Analysis
        await client.PatchAsync($"/api/v1/documents/{Guid.NewGuid()}/ai-analysis", null);
    }

    [Fact]
    public async Task LawyersControllerTest()
    {
        var client = _factory.CreateClient();
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateLawyerCommand>(), default)).ReturnsAsync(Guid.NewGuid());
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetLawyersQuery>(), default)).ReturnsAsync(new List<LawyerDto>());

        // 1. Create
        await client.PostAsJsonAsync("/api/v1/lawyers", new CreateLawyerRequest { Name = "N", Email = "E", BarNumber = "B" });
        // 2. Get All
        await client.GetAsync("/api/v1/lawyers");
        // 3. Update Activity
        await client.PatchAsync($"/api/v1/lawyers/{Guid.NewGuid()}/lawyer-activity?isActive=true", null);
    }

    [Fact]
    public async Task ReviewTasksControllerTest()
    {
        var client = _factory.CreateClient();
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<AssignReviewTaskCommand>(), default)).ReturnsAsync(Guid.NewGuid());
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetReviewTasksQuery>(), default)).ReturnsAsync(new List<ReviewTaskDto>());

        // 1. Assign
        await client.PostAsJsonAsync("/api/v1/review-tasks", new AssignReviewTaskCommand(Guid.NewGuid(), Guid.NewGuid(), "Desc"));
        // 2. Get All
        await client.GetAsync("/api/v1/review-tasks");
        // 3. Update Status
        await client.PatchAsync($"/api/v1/review-tasks/{Guid.NewGuid()}/review-task-status?status=Completed", null);
        // 4. Update Lawyer
        await client.PatchAsync($"/api/v1/review-tasks/{Guid.NewGuid()}/review-task-lawyer?newLawyerId={Guid.NewGuid()}", null);
    }

    [Fact]
    public async Task RegistriesControllerTest()
    {
        var client = _factory.CreateClient();
        
        var newRegistryId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateRegistryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newRegistryId);
        
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetRegistriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RegistryDto> 
            { 
                new RegistryDto(newRegistryId, "Registru Test", "București", 100, 100) 
            });

        // --- ACT & ASSERT ---
        
        var createCommand = new CreateRegistryCommand("Registru Test", "București", 100);
        var postRes = await client.PostAsJsonAsync("/api/v1/registries", createCommand);
    
        postRes.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        postRes.Headers.Location?.ToString().Should().Contain(newRegistryId.ToString());

        // Test GET (Get All)
        var getRes = await client.GetAsync("/api/v1/registries");
    
        getRes.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    
        var registries = await getRes.Content.ReadFromJsonAsync<List<RegistryDto>>();
        registries.Should().NotBeEmpty();
        registries![0].Name.Should().Be("Registru Test");
    }
    
    [Fact]
    public async Task Remaining_Endpoints_Test()
    {
        var client = _factory.CreateClient();
        
        var fakeLawyer = Lawyer.Create(Guid.NewGuid(), "Robert Vagner", "12345", "robert@law.com");
    
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetLawyerQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeLawyer); // Acum tipurile se potrivesc

        await client.GetAsync("/api/v1/lawyers/me");
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<ChangeUsernameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthResponse("new-token", "robert"));

        await client.PatchAsJsonAsync("/api/v1/auth/change-username", new ChangeUsernameRequest("newrobert"));
    }
}