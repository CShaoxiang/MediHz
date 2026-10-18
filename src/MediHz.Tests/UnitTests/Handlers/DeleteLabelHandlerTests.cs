using FluentAssertions;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class DeleteLabelHandlerTests : UnitTestBase
{
    private readonly Mock<ILabelService> _labelServiceMock = new();

    private DeleteLabelHandler _handler;

    public DeleteLabelHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _handler = new(_labelServiceMock.Object, Logger<DeleteLabelHandler>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNoContent_WhenLabelIsDeleted()
    {
        // Arrange
        var labelId = Guid.NewGuid();
        _labelServiceMock
            .Setup(s => s.DeleteAsync(labelId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(labelId, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenLabelDoesNotExist()
    {
        // Arrange
        var labelId = Guid.NewGuid();
        _labelServiceMock
            .Setup(s => s.DeleteAsync(labelId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(labelId, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Not Found");
    }

    [Fact]
    public async Task Handle_ShouldReturnForbidden_WhenLabelIsPrePackaged()
    {
        // Arrange
        var labelId = Guid.NewGuid();
        _labelServiceMock
            .Setup(s => s.DeleteAsync(labelId, CancellationToken.None))
            .ThrowsAsync(new InvalidOperationException("Cannot delete pre-packaged labels."));

        // Act
        var result = await _handler.Handle(labelId, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Forbidden");
    }
}
