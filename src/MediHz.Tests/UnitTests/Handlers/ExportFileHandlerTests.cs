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

public class ExportFileHandlerTests : UnitTestBase
{
    private readonly Mock<IExportService> _exportServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly ExportFileHandler _handler;

    public ExportFileHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _currentUserMock.SetupGet(user => user.UserId).Returns(_userId);
        _handler = new(
            _exportServiceMock.Object,
            _currentUserMock.Object,
            Logger<ExportFileHandler>()
        );
    }

    [Fact]
    public async Task Handle_ShouldPassCurrentUserIdToExportService_AndReturnSuccess()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var request = new ExportFileRequest(fileId);
        var fileExportDto = Fixture.Create<FileExportDto>();

        _exportServiceMock
            .Setup(x => x.ExportFileAsync(fileId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileExportDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(fileExportDto);
        result.Problem.Should().BeNull();
        _exportServiceMock.Verify(
            x => x.ExportFileAsync(fileId, _userId, CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenExportServiceReturnsNull()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var request = new ExportFileRequest(fileId);

        _exportServiceMock
            .Setup(x => x.ExportFileAsync(fileId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FileExportDto?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Value.Should().BeNull();
        result.Problem.Should().NotBeNull();
        result.Problem!.Title.Should().Be("File Not Found");
        result
            .Problem.Detail.Should()
            .Be($"Audio file with ID '{fileId}' was not found or access was denied.");
        result.Problem.Status.Should().Be(StatusCodes.Status404NotFound);
        _exportServiceMock.Verify(
            x => x.ExportFileAsync(fileId, _userId, CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldPropagateCancellationToken_ToExportService()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var request = new ExportFileRequest(fileId);
        var fileExportDto = Fixture.Create<FileExportDto>();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _exportServiceMock
            .Setup(x => x.ExportFileAsync(fileId, _userId, token))
            .ReturnsAsync(fileExportDto);

        // Act
        var result = await _handler.Handle(request, token);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        _exportServiceMock.Verify(x => x.ExportFileAsync(fileId, _userId, token), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenExportServiceThrows()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var request = new ExportFileRequest(fileId);

        _exportServiceMock
            .Setup(x => x.ExportFileAsync(fileId, _userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        // Act
        var act = () => _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Service error");
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var request = new ExportFileRequest(fileId);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        _exportServiceMock
            .Setup(x => x.ExportFileAsync(fileId, _userId, cts.Token))
            .ThrowsAsync(new OperationCanceledException(cts.Token));

        // Act
        var act = () => _handler.Handle(request, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
