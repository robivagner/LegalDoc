using LegalDoc.Application.Abstractions;
using LegalDoc.Application.ReviewTask.Queries;
using LegalDoc.Domain.Entities;
using Moq;
using FluentAssertions;

namespace LegalDoc.Tests.Unit;

public class GetReviewTasksTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllTasks_WhenNoFiltersApplied()
    {
        // Arrange
        var repoMock = new Mock<IReviewTaskRepository>();
        var docId = Guid.NewGuid();
        var lawyerId = Guid.NewGuid();
        
        var tasks = new List<ReviewTask> 
        { 
            ReviewTask.Create(docId, lawyerId, "Test Task") 
        }.AsQueryable();
        
        repoMock.Setup(x => x.Query()).Returns(tasks);
        var handler = new GetReviewTasksQueryHandler(repoMock.Object);
        var query = new GetReviewTasksQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Description.Should().Be("Test Task");
    }
    
    [Fact]
    public async Task Handle_Should_ApplyFilters_WhenProvided()
    {
        // Arrange
        var lawyerId = Guid.NewGuid();
        var tasks = new List<ReviewTask> { 
            ReviewTask.Create(Guid.NewGuid(), lawyerId, "Task") 
        }.AsQueryable();
    
        var repoMock = new Mock<IReviewTaskRepository>();
        repoMock.Setup(x => x.Query()).Returns(tasks);
        var handler = new GetReviewTasksQueryHandler(repoMock.Object);
    
        // Testam filtrarea dupa LawyerId
        var query = new GetReviewTasksQuery(lawyerId, null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}