using AutoFixture;
using FluentAssertions;
using MediHz.Application.Abstractions.Auth;
using MediHz.Application.Abstractions.Exports;
using MediHz.Application.DTOs.Exports;
using MediHz.Application.Handlers.Exports;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class ExportProjectHandlerTests : UnitTestBase
{
    private readonly Mock<IExportService> _exportServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly ExportProjectHandler _handler;

    public ExportProjectHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _currentUserMock.SetupGet(user => user.UserId).Returns(_userId);
        _handler = new(
            _exportServiceMock.Object,
            _currentUserMock.Object,
            Logger<ExportProjectHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldPassCurrentUserIdToExportService_AndReturnSuccess()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new ExportProjectRequest(projectId);
        var projectExportDto = Fixture.Create<ProjectExportDto>();

        _exportServiceMock
            .Setup(x => x.ExportProjectAsync(projectId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectExportDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(projectExportDto);
        result.Problem.Should().BeNull();
        _exportServiceMock.Verify(
            x => x.ExportProjectAsync(projectId, _userId, CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenExportServiceReturnsNull()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new ExportProjectRequest(projectId);

        _exportServiceMock
            .Setup(x => x.ExportProjectAsync(projectId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectExportDto?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Value.Should().BeNull();
        result.Problem.Should().NotBeNull();
        result.Problem!.Title.Should().Be("File Not Found");
        result
            .Problem.Detail.Should()
            .Be($"Project with ID '{projectId}' was not found or access was denied.");
        result.Problem.Status.Should().Be(StatusCodes.Status404NotFound);
        _exportServiceMock.Verify(
            x => x.ExportProjectAsync(projectId, _userId, CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldPropagateCancellationToken_ToExportService()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new ExportProjectRequest(projectId);
        var projectExportDto = Fixture.Create<ProjectExportDto>();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _exportServiceMock
            .Setup(x => x.ExportProjectAsync(projectId, _userId, token))
            .ReturnsAsync(projectExportDto);

        // Act
        var result = await _handler.Handle(request, token);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        _exportServiceMock.Verify(x => x.ExportProjectAsync(projectId, _userId, token), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenExportServiceThrows()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new ExportProjectRequest(projectId);

        _exportServiceMock
            .Setup(x => x.ExportProjectAsync(projectId, _userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Project export service failure"));

        // Act
        var act = () => _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Project export service failure");
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new ExportProjectRequest(projectId);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        _exportServiceMock
            .Setup(x => x.ExportProjectAsync(projectId, _userId, cts.Token))
            .ThrowsAsync(new OperationCanceledException(cts.Token));

        // Act
        var act = () => _handler.Handle(request, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
