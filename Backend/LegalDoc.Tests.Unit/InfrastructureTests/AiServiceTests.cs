using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LegalDoc.Application.Document.Queries;
using LegalDoc.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace LegalDoc.Tests.Unit.InfrastructureTests;

public class AiServiceTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<HttpMessageHandler> _handlerMock;

    public AiServiceTests()
    {
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(x => x["AiServiceSettings:BaseUrl"]).Returns("http://api-falsa.com");
        _handlerMock = new Mock<HttpMessageHandler>();
    }

    [Fact]
    public async Task AnalyzeDocumentAsync_Success_ReturnsResponse()
    {
        // Arrange
        var expectedResponse = new AiAnalysisResponse("Summary", "Clauses", "Risks");
        
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedResponse)
            });

        var httpClient = new HttpClient(_handlerMock.Object);
        var sut = new AiService(httpClient, _configMock.Object);

        // Act
        var result = await sut.AnalyzeDocumentAsync("conținut document");

        // Assert
        result.Should().NotBeNull();
        result!.Summary.Should().Be("Summary");
        result.Clauses.Should().Be("Clauses");
    }

    [Fact]
    public async Task AnalyzeDocumentAsync_ApiFails_ReturnsNull()
    {
        // Arrange
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError });

        var httpClient = new HttpClient(_handlerMock.Object);
        var sut = new AiService(httpClient, _configMock.Object);

        // Act
        var result = await sut.AnalyzeDocumentAsync("text");

        // Assert
        result.Should().BeNull();
    }
}