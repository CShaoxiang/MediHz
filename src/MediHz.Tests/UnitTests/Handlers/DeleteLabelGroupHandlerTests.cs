using FluentAssertions;
using MediHz.Application.Abstractions.Annotations;
using MediHz.Application.Handlers.Annotations;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;

namespace MediHz.Tests.UnitTests.Handlers;

public class DeleteLabelGroupHandlerTests : UnitTestBase
{
    private readonly Mock<ILabelGroupService> _labelGroupServiceMock = new();

    private DeleteLabelGroupHandler _handler;

    public DeleteLabelGroupHandlerTests(ITestOutputHelper output)
        : base(output)
    {
        _handler = new(_labelGroupServiceMock.Object, Logger<DeleteLabelGroupHandler>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNoContent_WhenLabelGroupIsDeleted()
    {
        // Arrange
        var labelGroupId = Guid.NewGuid();
        _labelGroupServiceMock
            .Setup(s => s.DeleteAsync(labelGroupId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(labelGroupId, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenLabelGroupDoesNotExist()
    {
        // Arrange
        var labelGroupId = Guid.NewGuid();
        _labelGroupServiceMock
            .Setup(s => s.DeleteAsync(labelGroupId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(labelGroupId, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Not Found");
    }

    [Fact]
    public async Task Handle_ShouldReturnForbidden_WhenLabelGroupIsPrePackaged()
    {
        // Arrange
        var labelGroupId = Guid.NewGuid();
        _labelGroupServiceMock
            .Setup(s => s.DeleteAsync(labelGroupId, CancellationToken.None))
            .ThrowsAsync(new InvalidOperationException("Cannot delete pre-packaged label groups."));

        // Act
        var result = await _handler.Handle(labelGroupId, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        result.Problem.Should().NotBeNull();
        result.Problem.Title.Should().Be("Forbidden");
    }
}
