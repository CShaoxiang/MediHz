using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.Projects;
using MediHz.Application.DTOs.Projects;
using MediHz.Application.Handlers;
using MediHz.Application.Handlers.Projects;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class GetProjectsHandlerTests : UnitTestBase
{
    private readonly Mock<IProjectService> _projectServiceMock = new();

    private GetProjectsHandler _handler;

    public GetProjectsHandlerTests(ITestOutputHelper output) : base(output)
    {
        _handler = new(_projectServiceMock.Object, Logger<GetProjectsHandler>());
    }

    [Fact]
    public async Task Handle_ShouldReturnProject_WhenOneExists()
    {
        // Arrange
        var request = new EmptyRequest();
        var project = Fixture.Create<ProjectDto>();
        _projectServiceMock
            .Setup(x => x.GetForUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([project]);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        var body = result.Body as IEnumerable<ProjectDto>;
        body!.Single().Should().BeEquivalentTo(project);
    }

    [Fact]
    public async Task Handle_ShouldReturnProjects_WhenMultipleExist()
    {
        // Arrange
        var request = new EmptyRequest();
        var projects = Fixture.CreateMany<ProjectDto>(2).ToArray();
        _projectServiceMock
            .Setup(x => x.GetForUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(projects);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyListSuccess_WhenNoneExist()
    {
        // Arrange
        var request = new EmptyRequest();
        _projectServiceMock.Setup(x => x.GetForUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEmpty();
    }
}

